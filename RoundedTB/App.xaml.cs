using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Serilog;
using WpfApplication = System.Windows.Application;

namespace RoundedTB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static readonly string MutexName = "RoundedTB_SingleInstance_Mutex";
        private Mutex _mutex;
        private bool _allowShutdown = false;
        private Window _hiddenWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Check if another instance is already running
            _mutex = new Mutex(true, MutexName, out bool isNewInstance);

            if (!isNewInstance)
            {
                Log.Information("RoundedTB is already running. Showing notification and exiting.");
                ShowInstanceAlreadyRunningNotification();
                _allowShutdown = true;
                Shutdown();
                return;
            }

            SetupLogging();
            SetupExceptionHandling();

            Log.Information("RoundedTB Started");

            base.OnStartup(e);

            // Prevent automatic shutdown when main window closes
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Create a hidden window to keep the application alive
            _hiddenWindow = new Window()
            {
                Title = "RoundedTB Hidden",
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Visibility = Visibility.Hidden
            };
            _hiddenWindow.Show();

            // Intercept any direct shutdown calls
            this.Dispatcher.ShutdownStarted += (sender, args) =>
            {
                Log.Information($"ShutdownStarted event triggered. _allowShutdown: {_allowShutdown}");
                Log.Information($"Call stack: {Environment.StackTrace}");
                if (!_allowShutdown)
                {
                    Log.Information("Attempting to prevent shutdown - this should not happen!");
                    // Handle the exception gracefully in our exception handler
                    throw new InvalidOperationException("Application shutdown not allowed - main window should stay hidden");
                }
            };

            WPFUI.Theme.Watcher.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (!_allowShutdown)
            {
                Log.Information("App.OnExit called but shutdown not allowed - this should not happen!");
                Log.Information($"OnExit call stack: {Environment.StackTrace}");
                Log.Information("CRITICAL: Something is calling shutdown despite ShutdownMode.OnExplicitShutdown!");

                // Try to prevent the shutdown by keeping the dispatcher alive
                this.Dispatcher.BeginInvoke(new Action(() => {
                    Log.Information("Attempting to keep application alive...");
                    // This might prevent the shutdown from completing
                }), System.Windows.Threading.DispatcherPriority.Send);

                return; // Don't call base.OnExit to try to prevent shutdown
            }

            Log.Information("App.OnExit called - Application is shutting down");
            Log.Information("RoundedTB Exiting");
            Log.CloseAndFlush();

            // Release the mutex
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();

            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            Log.Information("Session ending - allowing shutdown");
            base.OnSessionEnding(e);
        }

        public new void Shutdown()
        {
            Log.Information("App.Shutdown() called - allowing shutdown");
            _allowShutdown = true;
            base.Shutdown();
        }

        public new void Shutdown(int exitCode)
        {
            Log.Information($"App.Shutdown({exitCode}) called - allowing shutdown");
            _allowShutdown = true;
            base.Shutdown(exitCode);
        }

        private void ShowInstanceAlreadyRunningNotification()
        {
            try
            {
                // Try to show a toast notification with the app icon
                var notifyIcon = new NotifyIcon
                {
                    Icon = LoadAppIcon(),
                    Visible = true
                };

                notifyIcon.ShowBalloonTip(3000, "RoundedTB", "RoundedTB is already running!", ToolTipIcon.Info);
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to show notification for already running instance");

                // Fallback: Show a simple message box
                System.Windows.MessageBox.Show("RoundedTB is already running!", "RoundedTB", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private System.Drawing.Icon LoadAppIcon()
        {
            try
            {
                // Try to load the app icon from the executable itself
                try
                {
                    var exeIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Application.ResourceAssembly.Location);
                    if (exeIcon != null)
                    {
                        Log.Information("Successfully loaded icon from executable");
                        return exeIcon;
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug($"Failed to load icon from executable: {ex.Message}");
                }

                // Try to load the app icon from resources - try multiple possible paths
                string[] iconPaths = {
                    "pack://application:,,,/RoundedTB.ico",
                    "pack://application:,,,/RoundedTBCanary.ico",
                    "pack://application:,,,/res/TrayDark.ico"
                };

                foreach (string iconPath in iconPaths)
                {
                    try
                    {
                        var iconStream = System.Windows.Application.GetResourceStream(new Uri(iconPath));
                        if (iconStream != null)
                        {
                            Log.Information($"Successfully loaded icon from: {iconPath}");
                            return new System.Drawing.Icon(iconStream.Stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug($"Failed to load icon from {iconPath}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load app icon, using default");
            }

            Log.Information("Using fallback system information icon");
            // Fallback to system information icon
            return System.Drawing.SystemIcons.Information;
        }

        private void SetupLogging()
        {
            try
            {
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RoundedTB", "logs");
                Directory.CreateDirectory(logDir);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Debug()
                    .WriteTo.File(path: Path.Combine(logDir, "log-.txt"),
                                  rollingInterval: RollingInterval.Day,
                                  retainedFileCountLimit: 7,
                                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                // Fallback: If logging setup fails, we can't log it to file, but we can write to debug/console
                System.Diagnostics.Debug.WriteLine($"Failed to setup logging: {ex}");
            }
        }

        private void SetupExceptionHandling()
        {
            // UI Thread Exceptions
            this.DispatcherUnhandledException += (sender, args) =>
            {
                // Check if this is our shutdown prevention exception
                if (args.Exception is InvalidOperationException ex && ex.Message.Contains("Application shutdown not allowed"))
                {
                    Log.Information("Successfully prevented unwanted application shutdown");
                    args.Handled = true; // Prevent the exception from crashing the app
                    return;
                }

                Log.Fatal(args.Exception, "Unhandled UI Exception");
                // We are not setting Handled = true, treating it as fatal for now, but at least we log it.
            };

            // Background Task Exceptions
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Log.Error(args.Exception, "Unobserved Task Exception");
                args.SetObserved();
            };

            // Catch-all for AppDomain
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;

                // Check if this is our shutdown prevention exception
                if (ex is InvalidOperationException invalidEx && invalidEx.Message.Contains("Application shutdown not allowed"))
                {
                    Log.Information("Successfully prevented unwanted application shutdown at AppDomain level");
                    // Note: We can't prevent AppDomain termination in this framework version
                    return;
                }

                Log.Fatal(ex, "AppDomain Unhandled Exception. IsTerminating: {IsTerminating}", args.IsTerminating);
                Log.CloseAndFlush();
            };
        }
    }
}
