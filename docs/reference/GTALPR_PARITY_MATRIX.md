# GTALPR parity matrix

Reference repository: `wttdotm/gtalpr`

Pinned commit: `19a46bf8642242b35b6511903c3484035620c7d9`

`SOURCE INVENTORY ONLY` means the behavior was located in the pinned snapshot. `UNKNOWN` means the source needs a tighter read or runtime observation. `NOT PORTED` means no FLICK OFF implementation exists yet. `PORTED — CORE` means the standalone Unity-safe behavior is implemented and covered by EditMode tests; scene wiring or game-specific adapters may still remain.

| Behavior | Upstream source | Reference value or logic | Unity target component | Planned verification | Status |
| --- | --- | --- | --- | --- | --- |
| Camera definition identity | `CameraDefinition.cs` | Stable `FlockCameraId` is the key; catalog also carries OSM type/id | `FlickCameraDefinition`, `FlickCameraCatalog` | Load duplicate and blank ids; assert normalization | PORTED — CORE |
| Camera definition pose | `CameraDefinition.cs`, `SurveillanceScript.cs` | X/Y plus heading; Z is resolved against ground when activated | `FlickCameraDefinition`, world placement adapter | Spawn at a known lab pose and compare transform | PORTED — CORE |
| Heading conversion | `SurveillanceScript.cs` | Compass heading normalized, then converted to GTA heading | `FlickCameraMath` | Table-driven heading conversion tests | PORTED — CORE |
| Nearby activation | `SurveillanceScript.cs` | Activation distance is 150 m horizontal; update check is every 1000 ms | `FlickCamera` lifecycle service | Move test player across threshold; assert activation/deactivation | NOT PORTED |
| Camera view definition | `SurveillanceScript.cs` | FOV is 120 degrees, range is 44.86 m, eye height is 3.49 m | `FlickCameraTuning`, `FlickVisionSensor` | Deterministic cone boundary tests | PORTED — CORE |
| FOV inclusion | `SurveillanceScript.cs` | Horizontal distance squared check followed by dot-product half-FOV check | `FlickVisionSensor` | Targets just inside and outside 60-degree half-angle | PORTED — CORE |
| FOV zero-distance case | `SurveillanceScript.cs` | A target within 0.0001 squared distance is accepted | `FlickVisionSensor` | Zero-distance unit test | PORTED — CORE |
| Line of sight | `SurveillanceScript.cs` | Ray from camera eye to vehicle target; map, objects, and vehicles can block; target vehicle is accepted | `IFlickLineOfSightQuery` boundary | Place an occluder in the lab and move it clear | PORTED — CORE |
| Target filter | `SurveillanceScript.cs` | Recognition is evaluated for a player vehicle, not an on-foot player | `FlickVisionSensor` policy | Vehicle and on-foot cases in lab tests | PORTED — CORE |
| Recognition edge | `ActiveCamera.cs`, `SurveillanceScript.cs` | New sighting requires visible now, not visible last tick, and elapsed cooldown | `FlickObservationState`, `FlickCamera` | Re-enter FOV and assert one edge event | PORTED — CORE |
| Observation timing | `SurveillanceScript.cs` | Sightings are processed in the tick loop | `FlickCamera.Tick` with injected time | Fixed-step replay test | PORTED — CORE |
| Cooldown | `SurveillanceScript.cs`, `ActiveCamera.cs` | Sighting deadline is current game time plus 2000 ms | `FlickObservationState` | Trigger, remain visible, re-enter before and after 2 s | PORTED — CORE |
| Manual capture | `SurveillanceScript.cs` | Manual photo can be requested while camera can see the vehicle; it does not require a new sighting edge | `FlickCaptureDevice` | Manual capture test with stable visibility | SOURCE INVENTORY ONLY |
| Report behavior | `SurveillanceScript.cs` | First false-positive chance on a new sighting, then wanted-state reports can trigger on a later reportable edge | Future report event service | Event sequence test without GTA wanted-level APIs | UNKNOWN |
| Capture behavior | `SurveillanceSceneRecorder.cs`, `SurveillanceScript.cs` | Every new sighting queues a scene capture when enabled | `FlickCaptureDevice` | Capture request record matches camera pose and FOV | SOURCE INVENTORY ONLY |
| Camera damage | `SurveillanceScript.cs`, `SurveillanceExplosiveWeapon.cs` | Physical prop is damageable; recent weapon damage is inspected | `FlickDamageable` | Apply test damage and assert destruction request | SOURCE INVENTORY ONLY |
| Vehicle impact | `SurveillanceScript.cs` | A touching player vehicle destroys the camera; prop is unfrozen as impact approaches | `FlickDamageable`, `FlickBreakableAssembly` | Lab impact with controlled velocity | SOURCE INVENTORY ONLY |
| Destruction one-time semantics | `SurveillanceScript.cs` | Guard returns when camera or definition is already destroyed | `FlickBreakableAssembly` | Call destruction twice; assert one event and one salvage | PORTED — CORE |
| Destruction persistence | `CameraJsonDestructionStateStore.cs` | Per-id booleans are normalized and atomically written outside install content | `FlickPersistentId`, persistence service | Save, reload, and corrupt-file tests | SOURCE INVENTORY ONLY |
| Destruction impulse | `SurveillanceScript.cs` | Camera is unfrozen and receives a player-relative breakup force | `FlickBreakableAssembly` | Physics scene inspection with fixed force inputs | UNKNOWN |
| Salvage spawn | `SurveillanceScript.cs`, `LootDrop.cs` | One drop is created at camera position with 3 copper, 2 electronic, 1 gold-plated contact | `FlickScrapEmitter` | Assert one deterministic salvage result | SOURCE INVENTORY ONLY |
| Salvage pickup | `SurveillanceScript.cs` | On-foot player auto-picks up within 1.25 m; vehicle pickup is rejected | `FlickScrapEmitter`, pickup interaction | Boundary and vehicle tests | SOURCE INVENTORY ONLY |
| Camera salvage value | `SurveillanceScript.cs` | Pickup adds $600 in donor | FLICK economy decision, not yet approved | Do not port until economy decision exists | SOURCE INVENTORY ONLY |
| Statistics | `SurveillanceStats.cs` | Sightings, total destruction, police reports, false reports, and event records are persisted | `FlickRunStatistics` | Pure record and persistence tests | SOURCE INVENTORY ONLY |
| Speedrun windows | `SurveillanceStats.cs` | Fastest unique-camera windows for 3, 10, 50, and all cameras | `FlickRunStatistics` | Table-driven sliding-window tests | SOURCE INVENTORY ONLY |
| Debug placement tooling | `ManualCameraPlacementMode.cs`, `ManualCameraStore.cs` | F7/control-panel placement and saved manual cameras | Editor or future debug tool | Tool smoke test after runtime parity | NOT PORTED |
| Reset behavior | `SurveillanceScript.cs`, `SurveillanceStats.cs` | Respawn and all-time-stat reset actions exist in UI flow | Future reset service | Save/reload/reset integration test | UNKNOWN |
| Photo destruction view | `SurveillanceCameraDestructionCapture.cs` | Destruction capture schedules a view near the event and writes a scene record | `FlickCaptureDevice` | Separate capture slice after camera parity | NOT PORTED |
