<p align="center">
  <a href="https://discord.gg/DXX4x5TQRq">
    <img src="https://img.shields.io/badge/dynamic/json?style=for-the-badge&logo=discord&logoColor=white&label=Discord&query=%24.approximate_member_count&suffix=%20members&url=https%3A%2F%2Fdiscord.com%2Fapi%2Fv10%2Finvites%2FDXX4x5TQRq%3Fwith_counts%3Dtrue&color=5865F2" alt="Discord members" />
  </a>
  <a href="https://discord.gg/DXX4x5TQRq">
    <img src="https://img.shields.io/badge/dynamic/json?style=for-the-badge&logo=discord&logoColor=white&label=Online&query=%24.approximate_presence_count&suffix=%20online&url=https%3A%2F%2Fdiscord.com%2Fapi%2Fv10%2Finvites%2FDXX4x5TQRq%3Fwith_counts%3Dtrue&color=22C55E" alt="Discord online" />
  </a>
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9" />
  <img src="https://img.shields.io/badge/WPF-Windows%20Desktop-0078D4?style=for-the-badge" alt="WPF" />
  <img src="https://img.shields.io/badge/Source-Public%20Audit-16A34A?style=for-the-badge" alt="Public Audit" />
</p>

## Overview

ValorantMature is a Windows desktop tool. This public repository contains the auditable local workflow only: detecting the VALORANT `Paks` folder, backing up VNG logo package files, removing those logo files while the game is running, and restoring the originals when the game closes.

The public source is intentionally limited. Private premium modules, backend code, license checks, cloud sync, red blood, corpse toggles, crosshair cloud editing, proprietary data files, and release binaries are not included.

## Documentation

English:

- [Guide](docs/en/guide.md)
- [Mechanism](docs/en/mechanism.md)
- [Risks](docs/en/risks.md)

Tiếng Việt:

- [Hướng dẫn](docs/vi/huong-dan.md)
- [Cơ chế](docs/vi/co-che.md)
- [Rủi ro](docs/vi/rui-ro.md)

## What Is Included

| Area | Included in public source |
| --- | --- |
| Desktop UI | WPF + WPF-UI shell, pages, tray behavior |
| Game folder detection | Local detection for VALORANT `Paks` path |
| File workflow | Backup, remove, restore VNG logo package files |
| Monitor | Process watcher for game open/close state |
| Docs | Build guide, mechanism notes, risk notes |

## What Is Not Included

| Area | Status |
| --- | --- |
| License/key system | Private |
| Backend/API keys | Private |
| Cloud preference sync | Private |
| Red blood/corpse features | Private |
| Crosshair cloud editing | Private |
| Proprietary game data/release binaries | Not published |

## Build From Source

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

## Support

Join the Discord server for announcements, support, and release information:

https://discord.gg/DXX4x5TQRq

## Disclaimer

This project is not affiliated with Riot Games, VALORANT, or VNG. The tool modifies local game package files while it is running. Use it at your own risk.
