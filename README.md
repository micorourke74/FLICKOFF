# FLICK OFF

FLICK OFF is a standalone first-person Windows PC game built around physical interaction with a compact town, destructible FLICK cameras, salvage, and quota-driven runs. This repository is the technical foundation. It does not contain the finished game.

## Toolchain

- Unity 6000.3.23f1, Unity 6.3 LTS
- Universal Render Pipeline
- C# with the `FlickOff` root namespace
- Windows PC as the first target
- Visible Meta Files and Force Text asset serialization

Open the repository root as a Unity project with Unity 6000.3.23f1. Unity will regenerate `Library/`, `Temp/`, and other local caches. Those folders are ignored.

## Repository workflow

Use a causal branch for work. The foundation branch is `bootstrap/flickoff-foundation-20260909`. Work follows BUILD, PROVE, LAND: make the smallest coherent change, verify the actual result, then review the diff, status, and handoff before landing it.

Game-owned content lives under `Assets/FLICKOFF/`. Runtime code uses `FlickOff`. In-game surveillance devices are always called FLICK cameras. Donor or real-world names belong only in clearly marked reference and provenance documents.

## Verification

Run repository checks from the root:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\powershell\Test-RepositorySanity.ps1
```

Run Unity compile validation or tests with the wrappers under `tools/unity/`. The wrappers locate the installed editor, preserve logs, and return Unity's exit code.

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\unity\Run-UnityBatch.ps1 -ExecuteMethod FlickOff.Editor.FoundationProjectBuilder.ValidateFoundation
powershell -ExecutionPolicy Bypass -File .\tools\unity\Run-UnityTests.ps1 -TestPlatform EditMode
```

## Donor reference boundary

The public GTALPR project is a behavioral reference only. Its pinned source snapshot and translation boundary are documented in:

- [`docs/reference/GTALPR_SOURCE_MAP.md`](docs/reference/GTALPR_SOURCE_MAP.md)
- [`docs/reference/GTALPR_PORT_BOUNDARY.md`](docs/reference/GTALPR_PORT_BOUNDARY.md)
- [`docs/reference/GTALPR_PARITY_MATRIX.md`](docs/reference/GTALPR_PARITY_MATRIX.md)

No donor source, GTA assembly, ScriptHook dependency, Rockstar content, or GTA map data belongs in the shipping runtime.

## Current milestone

Foundation complete enough to hand off the next mission: GTALPR camera behavioral parity port. The acceptance gate is [`ACCEPTANCE.md`](ACCEPTANCE.md), and the latest handoff is [`docs/HANDOFF.md`](docs/HANDOFF.md).
