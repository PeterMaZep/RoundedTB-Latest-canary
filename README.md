![RoundedTB](https://web.archive.org/web/20230908070623/https://camo.githubusercontent.com/3732617601f341663216245851ca56a65f29fb736c4a2337eeb50deff63fb5f0/68747470733a2f2f63646e2e646973636f72646170702e636f6d2f6174746163686d656e74732f3237323530393837333437393232313234392f3839313535353531353739393331383536382f756e6b6e6f776e2e706e67)

# RoundedTB

#### Add margins, rounded corners and segments to your taskbars!

![image](https://user-images.githubusercontent.com/31840547/134795141-76349eaf-12da-40f8-b2a0-d7b7c268d152.png)

## Current status

We are examining RTB's code to try and fix it. We will gladly take any pull requests. This is a community effort, so RoundedTB might never become RoundedTB without YOU!

## Roadmap

### Now

We are examining RTB's code to try and fix it. Any pull requests will be gladly reviewed by us.

### The future

We will then try and fix some basic features.

## How do I get it?

Currently, this fix of rounded tb is in progress.
The easiest way to download RoundedTB is from the [Microsoft Store](https://www.microsoft.com/store/productId/9MTFTXSJ9M7F). You can also download the latest version from the Releases tab, unzip it and run `RoundedTB.exe`. If you're a madman, you can compile it yourself or check out the latest [Canary build](https://nightly.link/torchgm/RoundedTB/workflows/ci/master/rtb-artifacts.zip) (note these can be very unfinished, buggy and unstable).

## How do I build it?

### Prerequisites

- .NET 6.0 SDK

### Building via Command Line

1. Open a terminal in the project root.
2. Run

```bash
dotnet build RoundedTB/RoundedTB.csproj -c Release
```

3. The executable will be at `RoundedTB/bin/Release/net6.0-windows10.0.19041/RoundedTB.exe` or run

```bash
.\RoundedTB\bin\Release\net6.0-windows10.0.19041\RoundedTB.exe
```

### Building via Visual Studio Code

1. Open the project folder in VS Code.
2. Press `Ctrl+Shift+B` to build.
3. Press `F5` to start debugging.

## To use

### Basic options

The simplest way to use RoundedTB is by simply entering a margin and corner radius.

- **Margin** - controls how many pixels to remove from each side of the taskbar, creating a margin around it that you can see and click through.
- **Corner Radius** - adjusts how round the corners of the taskbar should be.

### Advanced options

The advanced options allow for further customisation, at the cost of some user-friendliness.

- **Independent Margins** - in the advanced settings, a <kbd>...</kbd> button appears on the margin box. Click it to enable independent margins, which allow you to specify the margin for each side of the taskbar. You can also use negative values to hide the rounded corners for some sides, allowing you to "attach" the taskbar to different sides of the monitor.
- **Dynamic Mode (Windows 11)** - dynamic mode automatically resizes the taskbars to accommodate the number of icons in it, making the taskbar behave similarly to macOS' Dock.
- **Split Mode (Windows 10)** - split mode is a simplified version of dynamic mode for Windows 10. Due to a more limited taskbar, dynamically resizing the taskbar isn't possible. However after some setup, split mode allows you to separate the taskbar from the system tray and resize it at will. I admit it's certainly not as cool as dynamic mode but for now it's better than nothing 🥺. For info on setting up, see the bottom of this readme.
- **Show System Tray** - this toggles whether or not the system tray, clock etc. is displayed in dynamic/split mode. It can be toggled at any time by pressing <kbd>Win</kbd>+<kbd>F2</kbd>.
- **TranslucentTB Compatibility** - due to a bug in Windows, apps that alter the composition of the taskbar don't allow RoundedTB's changes to show up automatically. Whilst I'm currently not aware of a fix, I've worked closely with [Sylveon](https://github.com/sylveon) to enable some level of compatibility between [TranslucentTB](https://github.com/TranslucentTB/TranslucentTB) and RoundedTB. This is experimental and _will_ flicker slightly. It requires TranslucentTB version 2021.5 to function.
- **About RoundedTB** - provides information about the current version of RoundedTB. The "Debug" section lets you open the config and log files.

## Known issues

- Auto-hiding is still incredibly experimental and may lead to a lot of flickering, especially with TranslucentTB compatibility or dynamic/split mode enabled. ([#36](https://github.com/torchgm/RoundedTB/issues/36))
- Rounded corners are not antialiased due to a Windows limitation. ([#4](https://github.com/torchgm/RoundedTB/issues/4))
- Dynamic mode won't hide the left side of the taskbar if the taskbar alignment has never been changed. This can be worked around by changing the alignment to Left and back to Center. ([#98](https://github.com/torchgm/RoundedTB/issues/98))
- Dynamic mode/split mode only work correctly when the taskbar is horizontal at the top/bottom of the screen.
- Split mode on Windows 10 only supports the main taskbar, secondary taskbars will not be split.
- When using dynamic mode, the taskbar may occasionally become too large, too small or not update. This can usually be fixed by moving a window to or from that monitor or briefly changing the taskbar alignment. These issues will be reduced in upcoming updates, don't worry! I just need to refactor a lot of code first.
- Compatibility with taskbar mods outside of TranslucentTB version 2021.5 is not currently guaranteed.

## Other info

RoundedTB is just a hobby of mine, and I'm certainly not an expert in this field, so I'm really sorry if you encounter a bug! If anything breaks catastrophically, press <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Esc</kbd> to open Task Manager, end RoundedTB and then restart Explorer. At worst, just reboot your PC. RoundedTB makes no permanent changes (though it will run on startup if you enable it from the tray icon), so restarting should clear any issues.

Feel free to let me know about any bugs by filing an issue so I can look into it. Alternatively if you want to discuss RoundedTB, get some insider sneak-peeks, need some assistance or just want to see what I'm up to, then feel free to join the [Discord server](https://discord.gg/wYQJd8VGSB).

### Configuring split mode on Windows 10

Split mode has a couple of limitations and requires a small amount of setup to get working properly.

#### Limitations

- Split mode doesn't resize itself automatically. This feature will be coming to RoundedTB for Windows 10 in the future.
- Toolbars are not compatible with split mode currently, and will need to be disabled apart from one. This is because toolbars are used to mark the "empty" space on the taskbar.
- Split mode only works when the taskbar is horizontal at the top or bottom of the screen, and on the primary monitor.

#### Setup

1. Right-click the taskbar and disable "Lock the taskbar".
2. Right-click it again and turn off any existing toolbars.
3. Right-click a third time, select Toolbars > Desktop.
4. Use the small <kbd>||</kbd> handle to resize the taskbar as you please.

Watch the following video for a guide on setting up split mode:

https://user-images.githubusercontent.com/31840547/134795022-1312d011-40f2-4641-8c8d-3d6c0e752747.mp4
