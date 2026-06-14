# Guide

[Back to README](../../README.md) · [Discord support](https://discord.gg/DXX4x5TQRq)

## Purpose

This public build demonstrates the local VNG logo workflow. It does not include premium features, backend calls, license checks, cloud sync, red blood, corpse toggles, or crosshair cloud editing.

## Requirements

- Windows 10/11
- .NET 9 SDK if building from source
- VALORANT installed locally
- Access to the game `Paks` folder

## Build

```powershell
dotnet restore .\src-wpf\ValorantUnlocker.Wpf.csproj
dotnet build .\src-wpf\ValorantUnlocker.Wpf.csproj
```

To create a self-contained local build:

```powershell
dotnet publish .\src-wpf\ValorantUnlocker.Wpf.csproj -c Release -o .\publish
```

## Usage

1. Open the app before launching VALORANT.
2. Confirm the detected game folder in the main page.
3. If auto-detection fails, use `Game folder` and select the VALORANT install folder or the `Paks` folder.
4. Keep monitoring enabled.
5. Launch the game normally.
6. The app backs up and removes the VNG logo package files while the game is open.
7. Close the game normally.
8. The app restores the original VNG files from backup.

## Manual Restore

Use `Restore` in the main page or the tray menu if you want to restore the original files immediately.

Do this before:

- repairing the game,
- updating the game,
- deleting the tool folder,
- moving the backup folder.

## Public Build Limits

This repository is intentionally limited to local file operations. It does not contain private backend, license, cloud sync, red blood, corpse, crosshair, or proprietary data modules.
