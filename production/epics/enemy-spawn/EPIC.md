# Epic: Enemy Spawn & Per-Room Management

> **Layer**: Feature
> **GDD**: design/gdd/enemy-spawn-system.md
> **Architecture Module**: Enemy Spawn & Room Combat Lifecycle (`Assets/Script/Enemy/`) — derived; no `architecture.md` yet
> **Status**: Ready (with untraced requirements — see below)
> **Stories**: Not yet created — run `/create-stories enemy-spawn`

## Overview

This epic implements the data layer and runtime driver that decide which enemies
appear in each room and manage the room's combat lifecycle. A data model of four
ScriptableObjects (`EnemyData`, `EnemyDatabase`, `MapEnemyDatabase`, `RoomData`)
defines the enemy pool and per-room difficulty as a single `weightBudget` dial;
`EnemyDatabase.GetHybridEnemySet` selects a budget-appropriate, seed-deterministic
enemy set via a two-phase (random + optimal-fill-with-overflow) algorithm; and
`EnemyManager` drives the per-room state machine — on room load it spawns the
selected set at `Tile_Spawn` markers, locks the doors, tracks the alive count, and
on clear emits `ON_CLEAR_ENEMY` + `ON_ROOM_CLEAR` to open doors and trigger the
upgrade screen. The player never touches this system directly; they feel it as the
per-run pacing and variety of each room.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| _None_ | No ADR governs this epic. **ADR-0001 (StatSystem)** is a boundary contract only: `EnemyData` is spawn metadata and must not become a fourth stat store; combat stats stay on the prefab's `EntityData`/`StatsSO`. | LOW |

> ⚠️ **Untraced.** No ADR covers the spawn algorithm, data model, lifecycle, or
> placement. The epic is created with placeholders; stories for these clusters are
> **Blocked** until ADRs exist. The most acute gap is **Open Question #1** below.

## GDD Requirements

No `tr-registry.yaml` exists in this project, so there are no formal TR-IDs. The
clusters below are derived from the GDD's acceptance-criteria groups
(`design/gdd/enemy-spawn-system.md`).

| Requirement Cluster | Requirement (GDD ACs) | ADR Coverage |
|---------------------|-----------------------|--------------|
| REQ-SPAWN-ALGO | `GetHybridEnemySet` weight-budget selection: budget bound, eligibility boundary, determinism, tie-break, variety, termination guard, degenerate inputs (AC-A1…A7) | ❌ No ADR |
| REQ-SPAWN-DATA | SO data model + `id` generation/validation: duplicate id, zero id, subset strip (AC-D1…D3) | ❌ No ADR |
| REQ-SPAWN-LIFECYCLE | `EnemyManager` room lifecycle + events: spawn/lock on entry, clear events, foreign/late event guard, zero-enemy room, cleared re-entry, null prefab (AC-L1…L6) | ❌ No ADR — **needs singleton-exception ADR (Open Q#1)** |
| REQ-SPAWN-PLACEMENT | Marker parse + round-robin balance + entry-safety (AC-P1…P3) | ❌ No ADR |

### Blocking Dependencies (inherited from the GDD)

| Dependency | Blocks | Source |
|------------|--------|--------|
| **Open Q#1** — `EnemyManager` violates "no new singletons"; needs an ADR to ratify the exception | PlayMode lifecycle test harness (AC-L1…L6) | GDD Open Questions #1 |
| **map-system Bug #16** — `RoomFile.roomType` not read at runtime | RoomType → `RoomData` routing | GDD Dependencies / CLAUDE.md Bug #16 |
| **Enemy AI Bugs #7/#8** — `EntityDeathState` wrong base class; empty `Health<=0` transition | real death chain emitting `ON_ENEMY_DEATH` (AC-L2b) | GDD Dependencies / CLAUDE.md Bugs #7/#8 |
| **`Tile_Spawn` markers** absent from all 13 room JSONs; parser branch does not exist | AC-P3 (marker placement); until fixed every room uses centre-fallback | GDD Dependencies |
| **Event Bus** — `ON_ENEMY_DEATH` + `ON_ROOM_CLEAR` not yet in `EventID` | lifecycle events | GDD Dependencies |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/enemy-spawn-system.md` (AC-A1…A7,
  AC-D1…D3, AC-L1…L6, AC-P1…P3) are verified
- All Logic and Integration stories have passing test files in `tests/`
  (EditMode for algorithm + data validation; PlayMode for room lifecycle)
- All Visual/Feel and UI stories (spawn telegraph VFX/SFX) have evidence docs with
  sign-off in `production/qa/evidence/`
- The untraced requirement clusters above have governing ADRs, **or** their stories
  are explicitly de-scoped for the demo

## Next Step

Run `/create-stories enemy-spawn` to break this epic into implementable stories.
Address **Open Question #1** (run `/architecture-decision` for the `EnemyManager`
singleton exception) before authoring the PlayMode lifecycle stories.
