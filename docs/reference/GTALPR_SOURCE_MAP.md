# GTALPR source map

Reference repository: `wttdotm/gtalpr`

Pinned snapshot examined: `19a46bf8642242b35b6511903c3484035620c7d9`

Examined: 2026-09-09

The checkout used for inspection lives outside the FLICK OFF repository at `C:\Users\User\Documents\Codex\references\gtalpr-20260909`. It is not a shipping dependency.

## Core camera behavior

| Upstream file | Responsibility observed | Port note |
| --- | --- | --- |
| `SurveillanceScript.cs` | Main script lifecycle, camera catalog loading, nearby activation, FOV math, line of sight, sighting edge detection, cooldown, report flow, damage polling, destruction, loot, manual camera hooks, and stats/photo coordination | Split into focused Unity components and adapters. Do not reproduce this class as one runtime object. |
| `CameraDefinition.cs` | Camera catalog record with stable id, OSM metadata, X/Y position, heading, and mutable destruction flag | Maps to immutable definition data plus separate persistent state. |
| `ActiveCamera.cs` | Live camera state, prop and blip handles, FOV endpoints, previous sighting flags, and cooldown deadline | Maps to a scene instance and a small observation state object. |
| `ManualCameraPlacementMode.cs` | Interactive placement and preview of user-created cameras | Future editor or runtime placement tool, not foundation gameplay. |
| `ManualCameraStore.cs` | Save and load of manually placed camera definitions | Future persistence adapter with stable ids. |

## Destruction and salvage

| Upstream file | Responsibility observed | Port note |
| --- | --- | --- |
| `SurveillanceExplosiveWeapon.cs` | Identifies recent weapon damage and whether damage is explosive | Replace with Unity damage-source data, not weapon hashes. |
| `CameraJsonDestructionStateStore.cs` | Reads and atomically writes per-camera destroyed state outside install content | Replace with FLICK OFF persistence, retaining stable ids and atomic-save intent. |
| `FlockPileSpawner.cs` | Spawns a 200-prop physics pile after destruction | Replace with an authored or pooled breakable assembly and scrap policy. |
| `LootDrop.cs` | Holds salvage prop, position, and three component quantities | Map to a typed FLICK scrap result and pickup interaction. |

## Statistics and capture

| Upstream file | Responsibility observed | Port note |
| --- | --- | --- |
| `SurveillanceStats.cs` | Total destructions, sightings, police/false reports, destruction events, and fastest unique-camera windows for 3, 10, 50, and all cameras | Extract pure run-stat calculations and test them independently. |
| `SurveillanceCameraDestructionCapture.cs` | Schedules a destruction camera capture, ranks candidate views, and records a scene snapshot | Future capture-device and camera-view services. |
| `SurveillanceSceneRecorder.cs` | Records live scene entities and world state to a capture format | Not part of initial camera parity runtime. |
| `SurveillancePhotoLab.cs` | Loads recorded scenes, reconstructs them, renders photo outputs, and restores state | Future photo pipeline, outside foundation. |
| `SurveillancePhotoOutput.cs`, `SurveillancePhotoOverlay.cs`, `SurveillanceJpegCapture.cs` | Output formats, overlays, desktop capture, and JPEG rendering | Preserve as reference only until capture behavior is approved for FLICK OFF. |

## Data and tooling

| Upstream item | Responsibility observed | Port note |
| --- | --- | --- |
| `in_game_cameras.json`, `in_game_cameras_newer.json`, `in_game_cameras_old.json` | Camera catalog data | Do not copy GTA map data into shipping assets. Create FLICK camera definitions for the lab. |
| `SurveillanceSceneEntitySelection.cs`, `SurveillanceSceneReconstructor.cs`, `SurveillanceSceneRecorder.cs` | Entity selection, scene reconstruction, and snapshot persistence for photo views | Explicitly outside the first port slice. |
| `Check-SurveillanceScenes.ps1`, `Package-Beta.ps1`, `INSTRUCTIONS.md` | Donor packaging and diagnostics | Reference only. Do not execute as FLICK OFF tooling. |

## Observed concentration of behavior

The upstream README identifies `SurveillanceScript.cs` as the location of most camera loading, placement, FOV, and reporting logic. That concentration is useful for inventory but is not the architecture target for FLICK OFF.
