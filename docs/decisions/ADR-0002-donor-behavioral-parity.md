# ADR-0002: Donor behavior is a reference contract

Status: Accepted

Date: 2026-09-09

## Decision

The pinned GTALPR source snapshot is treated as a behavioral reference contract for the camera-port phase. FLICK OFF will reproduce observable behavior first, then make changes only through explicit design decisions.

## Boundaries

- Engine-specific GTA glue stays outside FLICK OFF runtime code.
- The donor repository is never vendored into shipping source.
- The exact donor commit examined is recorded in `docs/reference/`.
- Pure camera math, state, and data are candidates for close translation.
- GTA API calls become Unity adapters or focused components.

## Consequence

Parity work is measured in testable behavior, not by copying file structure. A green parity matrix is the gate before substantial town or systems work.
