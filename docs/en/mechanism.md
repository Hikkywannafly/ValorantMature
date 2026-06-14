# Mechanism

[Back to README](../../README.md) · [Discord support](https://discord.gg/DXX4x5TQRq)

## High-Level Flow

ValorantMature Public is a local Windows desktop workflow:

```text
Start app
  -> detect VALORANT Paks folder
  -> monitor game process
  -> game opens
  -> back up VNG logo files
  -> remove VNG logo files
  -> game closes
  -> restore original files
```

## Folder Detection

The app tries to find the VALORANT `Paks` directory through local install metadata and common install paths. If detection fails, the user can select the folder manually.

Accepted selections include:

- the VALORANT install root,
- `ShooterGame\Content\Paks`,
- a folder that directly contains `.pak` files.

## Backup Strategy

Before removing any VNG logo file, the app copies the original file into a local `backup` folder. Restore uses those backup files.

This means the backup folder should stay near the app or release package. If the backup folder is deleted, automatic restore cannot recover files that no longer exist.

## File Operations

The public source only handles the VNG logo package file names defined in the local engine. It does not publish proprietary replacement game data.

The file flow is intentionally simple:

- check if the target file exists,
- copy it to backup,
- delete the target file,
- restore from backup after the game closes.

## Process Monitor

The monitor checks whether the VALORANT process is running. It performs injection once per game session and restoration when the process is no longer detected.

## What Is Deliberately Absent

The following modules are private and not part of this source tree:

- backend/license system,
- premium feature gating,
- cloud preference sync,
- red blood and corpse feature logic,
- crosshair cloud profile editing,
- proprietary data packages.
