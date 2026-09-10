# ADR-0001: Unity 6.3 LTS with URP

Status: Accepted

Date: 2026-09-09

## Decision

FLICK OFF uses Unity 6000.3.23f1, Unity 6.3 LTS, with the Universal Render Pipeline, C#, and Windows PC as the first platform.

## Reasons

- C# keeps the eventual donor behavior translation close to the reference implementation's language.
- Unity provides the physics, scene, prefab, serialization, and editor tooling needed for a standalone physical game.
- URP is a suitable starting renderer for an authored town and a PC-first prototype without committing to high-end rendering requirements.
- A pinned LTS patch makes local and agent verification repeatable.

## Consequences

The project must keep Unity-generated YAML and `.meta` files under version control. Future upgrades require an explicit decision and a fresh verification pass.
