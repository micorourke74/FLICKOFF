# Planned FLICK camera architecture

This document sets boundaries for the donor-port phase. It is a target decomposition, not a claim that gameplay is implemented.

## Runtime composition

| Component | Responsibility | Does not own |
| --- | --- | --- |
| `FlickCamera` | Coordinates one placed camera instance and its lifecycle | Global catalog loading, UI, or save-file format |
| `FlickCameraDefinition` | Immutable identity, pose, and tuning data for a camera | Live damage, scene objects, or statistics |
| `FlickVisionSensor` | FOV, range, target filter, and line-of-sight queries | Wanted-level rules, photo rendering, or persistence |
| `FlickDamageable` | Receives damage and exposes destruction transition | Salvage policy or global statistics |
| `FlickBreakableAssembly` | Converts destruction into physical breakage and one-time event semantics | Catalog identity or save I/O |
| `FlickScrapEmitter` | Creates the salvage result for a destroyed camera | Camera recognition or run timing |
| `FlickCaptureDevice` | Records a camera observation for later photo processing | Camera destruction state or GTA-specific behavior |
| `FlickPersistentId` | Stable identity used by save and reload | Scene discovery or gameplay policy |

## Dependency direction

Stable data and pure math sit at the center. Unity physics, rendering, scene objects, persistence, and UI depend on those abstractions. The donor is a source of behavior and values, never a runtime assembly dependency.

## Translation rules

- GTA vectors map to Unity vectors.
- GTA props map to prefab instances with focused components.
- GTA spawning maps to prefab instantiation through a small world adapter.
- GTA damage maps to `FlickDamageable` and an explicit destruction event.
- GTA world raycasts map to Unity physics queries through a testable sensor adapter.
- GTA timing maps to an injected clock or deterministic tick source.
- GTA persistence maps to FLICK OFF run/world persistence.
- GTA notifications map to a future UI or event boundary.

Start with pure contracts and deterministic tests. Do not translate `SurveillanceScript` into one large `MonoBehaviour`.
