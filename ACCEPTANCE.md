# Foundation acceptance

## Repository foundation

- Canonical repository is `micorourke74/FLICKOFF`.
- Git root is the repository root, not a nested folder.
- Unity project markers are present at the root.
- Generated Unity caches are ignored and untracked.
- `.meta` files remain visible and paired.
- LFS policy is explicit for large binary source and game assets.
- Sanity checks and Unity wrappers are repeatable from the root.

## CAMERA DONOR PORT — GREEN

This is the first future gameplay milestone. It is not green during foundation setup. A future implementation must prove, with deterministic tests and parity-lab inspection, that:

1. a FLICK camera spawns at a known pose;
2. view direction matches its definition;
3. targets outside FOV are ignored;
4. targets inside FOV are recognized;
5. occlusion blocks recognition correctly;
6. detection and cooldown timing match the pinned donor reference;
7. the camera can receive damage;
8. destruction fires exactly once;
9. correct salvage behavior occurs;
10. state survives and reloads as intended;
11. statistics update correctly;
12. the behavior is deterministic enough to regression-test.

Each row in `docs/reference/GTALPR_PARITY_MATRIX.md` needs a test or an explicit reason it remains unknown before this gate turns green.
