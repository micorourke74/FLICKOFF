# GTALPR port boundary

The donor is a GTA V mod. FLICK OFF is a standalone Unity game. The donor teaches observable camera behavior and implementation values, not engine dependencies.

## Pure or mostly portable logic

- Camera definition data and stable ids.
- Heading normalization and heading-to-forward conversion.
- Horizontal distance and cone/FOV tests.
- Recognition edge detection using previous visibility state.
- Cooldown deadline state.
- Destruction one-time guards.
- Destruction event records and fastest unique-camera windows.
- Salvage quantities as data.
- Stable destruction-state normalization and atomic-save intent.

## GTA API boundary

- GTA vectors become Unity `Vector3` values.
- GTA props and blips become Unity GameObjects, components, and later optional debug views.
- GTA spawning becomes prefab instantiation through a world adapter.
- GTA damage and weapon hashes become Unity damage-source records.
- GTA world raycasts become Unity `Physics.Raycast` or an injected physics-query interface.
- GTA game time becomes an injected clock or fixed tick source.
- GTA wanted-level reports become FLICK OFF events, not police native calls.
- GTA photo capture and reconstruction become a separate Unity capture pipeline.
- GTA local application-data storage becomes a FLICK OFF persistence service.

## Never cross this boundary

No `GTA.*` namespace, ScriptHook, Rockstar asset, GTA map coordinate catalog, native hash, or GTA-specific UI type may appear in FLICK OFF runtime assemblies.
