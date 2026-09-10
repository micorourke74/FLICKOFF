# FLICK OFF roadmap

## Current

- Unity 6000.3.23f1 URP project root established.
- Repository hygiene, LFS policy, agent contract, assembly boundaries, and repeatable validation tools established.
- GTALPR source inventory and behavioral parity matrix started from pinned commit `19a46bf8642242b35b6511903c3484035620c7d9`.
- Camera parity lab scene skeleton exists at `Assets/FLICKOFF/Scenes/Labs/CameraParityLab.unity`.
- Original street test ground exists at `Assets/FLICKOFF/Scenes/Labs/StreetTestGround.unity` with three FLICK camera actors, a controllable vehicle proxy, occluder geometry, HUD, destruction, quota, and donor-matched scrap accounting.
- Fresh Unity verification is green: EditMode 19/19 and PlayMode 1/1.

## Next mission

PLAYABLE STREET SLICE.

Finish the first-person vehicle/on-foot transition, physical salvage pickup, camera activation lifecycle, and a small end-to-end objective in the original street ground. Keep the deterministic camera core as the contract and prove every new rule with focused tests.

## Later, after the street slice is fun and stable

- Add authored visual identity and modular town content.
- Add quota and run persistence after the camera behavior is stable.
- Add capture/stats/debug tooling from the donor behavior matrix where it serves the FLICK OFF loop.
- Evaluate multiplayer as a separate authorized architecture decision.
