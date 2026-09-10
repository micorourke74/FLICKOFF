# FLICK OFF roadmap

## Current

- Unity 6000.3.23f1 URP project root established.
- Repository hygiene, LFS policy, agent contract, assembly boundaries, and repeatable validation tools established.
- GTALPR source inventory and behavioral parity matrix started from pinned commit `19a46bf8642242b35b6511903c3484035620c7d9`.
- Camera parity lab scene skeleton exists at `Assets/FLICKOFF/Scenes/Labs/CameraParityLab.unity`.

## Next mission

GTALPR CAMERA BEHAVIORAL PARITY PORT.

Start with pure, deterministic camera math and state transitions. Bring the camera definition, FOV, line-of-sight, recognition edge, cooldown, damage, one-time destruction, persistence, salvage, and statistics contracts into focused Unity systems. Prove them in the parity lab before adding town content.

## Later, only after parity is green

- Expand the physical camera assembly and scrap interactions.
- Add a small authored town slice around the lab.
- Add quota and run persistence after the camera behavior is stable.
- Evaluate multiplayer as a separate authorized architecture decision.
