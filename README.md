# ValorantMature

Public source build of the ValorantMature desktop app.

This repository is intentionally limited. It is published so users can inspect the local Windows application flow that detects the VALORANT `Paks` folder, backs up VNG logo package files, removes them while the game is running, and restores the original files after the game closes.

## Included

- WPF desktop UI using WPF-UI.
- Single-instance app startup and tray behavior.
- VALORANT `Paks` folder detection.
- Local backup, removal, and restore logic for VNG logo package files.
- Game process monitor.
- Public guidance, risk, and contact pages.

## Not Included

The private/premium code is deliberately not part of this source tree:

- License/key activation and verification.
- Private backend client code, API keys, serverless functions, and backend logic.
- Private cloud preference sync.
- Red blood / corpse toggles.
- Crosshair color/profile cloud editing.
- Proprietary game data files and release binaries.

This separation is intentional: the public repo can be audited for the local file operations, while sensitive server-side and premium logic stays private.

## Build

Requirements:

- Windows 10/11
- .NET 9 SDK

```powershell
dotnet restore .\src-wpf\ValorantUnlocker.Wpf.csproj
dotnet build .\src-wpf\ValorantUnlocker.Wpf.csproj
```

Publish a local self-contained build:

```powershell
dotnet publish .\src-wpf\ValorantUnlocker.Wpf.csproj -c Release -o .\publish
```

## Notes

This project is not affiliated with Riot Games, VALORANT, or VNG. Use at your own risk.
