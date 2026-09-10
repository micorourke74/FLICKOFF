# FLICK OFF agent contract

## Identity

FLICK OFF is a standalone Unity 6.3 LTS project using Unity 6000.3.23f1, URP, C#, and Windows PC as the first platform. The primary namespace is `FlickOff`. Surveillance cameras inside the game are FLICK cameras.

## Terminology and boundaries

- Do not introduce real-world surveillance-company branding into shipping code, assets, or UI.
- Upstream names are allowed only in clearly marked donor, reference, or provenance documentation.
- GTA-specific concepts stay outside the runtime dependency graph. FLICK OFF must not depend on GTA assemblies, ScriptHook, Rockstar content, GTA native calls, or the GTA map.
- Do not begin multiplayer until explicitly authorized.

## Workflow

Inspect before editing. Follow BUILD, PROVE, LAND: make the smallest coherent change, verify the actual result, then spend the final portion reviewing `git diff`, `git status`, commits, and handoff documentation. Make causal commits. Never silently destroy owner work or force-push. Do not claim Unity tests ran unless Unity actually ran them.

## Architecture

Prefer composition over god classes. Keep dependencies pointed toward stable abstractions. Avoid singleton proliferation and generic `Manager` classes. Shipping systems should be independently testable where practical. Behavioral parity comes before embellishment during donor-port work.

## Unity rules

- Keep `.meta` files paired with their assets and folders.
- Use Visible Meta Files and Force Text serialization.
- Do not commit `Library/`, `Temp/`, `Obj/`, build output, IDE caches, or local logs.
- Keep game-owned content under `Assets/FLICKOFF/`.
- Do not hand-edit large Unity YAML files when editor tooling can safely produce them.
- Run Unity compile and test verification after meaningful code changes when the exact editor is available.

## Verification and scope

Compile errors block landing. Review warnings. Tests must be genuinely executed. Record blockers precisely. Do not build the whole town, final UI, final art, quotas, police systems, Steam integration, or multiplayer during foundation and donor-port work.
