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

> **Prototype status (updated 2026-07-13, was last checked 2026-07-09).** A partial prototype exists
> (`Assets/Script/Database-SO/Modal/` — `RoomModel`/`MapModel`/`EnemyModal`, plus two parallel
> runtime drivers). It implements the data model + weight-budget selection, and **diverges from this
> epic's originally-planned model** (different class names, direct refs instead of `id`s,
> `UnityEngine.Random` instead of an injected seed, uniform-random Phase 2 instead of `argmin`) —
> and has kept diverging further since 2026-07-09, not converging.
>
> **Corrections to the 2026-07-09 snapshot above (re-verified 2026-07-13):**
> - `EnemySpawner.cs` is **no longer an empty stub** — it subscribes `EventID.ON_GET_SPAWN_POSITIONS`
>   (not `ON_LOAD_MAP` as previously assumed) and has a working `OnDoneLoadRoomGrid`/`SpawnRoomEnemies`
>   implementation. It compiles (confirmed by bug-triage-2026-07-10 — the suspected `.ConvertTo<T>()`
>   compile error was a false alarm; `Unity.VisualScripting` ships that extension method).
> - `RoomGeneraterController.LoadRoom()` now **parses `Tile_Spawn_Enemy` markers** and emits
>   `ON_GET_SPAWN_POSITIONS` with their positions — this is what `EnemySpawner` consumes. 1 of 13 room
>   JSONs (`NormalRoom_0.json`) has a marker authored; the other 12 have no fallback and would throw.
> - `RoomCell` does **not** have a `GetSpawnPosition()` method (the 2026-07-09 note claiming one exists
>   calling an undefined `GetRoomSpawnSet()` does not match current code) — spawn positions flow
>   directly from `RoomGeneraterController` to `EnemySpawner` via the event payload instead.
> - `EnemyManager.cs` is **unchanged** — still exactly `public static EnemyManager Instance { get; private set; }`,
>   no `Awake`, no lifecycle. Still has the harmless-for-now unused `System.Numerics` import.
> - `MapModel` no longer holds `idRooms`/`totalWeight` — it now holds `fullRoomList` (List\<RoomModel\>)
>   and draws a random preset per spawn event via `GetRandomRoom()` (shuffle-bag, no-replacement pool).
>   This means room→enemy-preset assignment is **decoupled from room identity** — the `RoomFile.roomData`
>   direct-reference mechanism the GDD once marked "RESOLVED" was **never implemented**.
> - **BUG-ES-1 confirmed live in both runtime drivers** (`EnemySpawner` and `LevelManager`) — neither
>   guards `RoomModel.GetSpawnSet()`'s `null` return on an empty enemy pool.
>
> Full detail in `design/gdd/enemy-spawn-system.md` → "Current Implementation" and "Prototype
> Deviations" (both revised 2026-07-13). Stories must decide per cluster: harden the prototype up to
> a design, or pick a different target design — see the GDD's "Future Architecture Direction" for an
> evaluated comparison of three options (harden current code / revive the original id-database target
> / adopt a newly proposed Room Budget + Candidate Pool + Spawn Chance model), none yet decided.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| **ADR-0002** (Proposed) | `EnemyManager` granted a scoped singleton exception (joins `MazeController`), with mandated duplicate-guard + event-driven state. Governs REQ-SPAWN-LIFECYCLE; unblocks the PlayMode harness (AC-L1…L6). | LOW |
| ADR-0001 (StatSystem) | Boundary contract only: `EnemyData` is spawn metadata and must not become a fourth stat store; combat stats stay on the prefab's `EntityData`/`StatsSO`. | LOW |

> ⚠️ **Partially traced.** ADR-0002 now covers the lifecycle/singleton decision.
> Still **untraced**: the spawn algorithm (REQ-SPAWN-ALGO), SO data model
> (REQ-SPAWN-DATA), and placement (REQ-SPAWN-PLACEMENT) — stories for those clusters
> remain **Blocked** until ADRs exist.

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
| ~~**Open Q#1**~~ ✅ RESOLVED — `EnemyManager` singleton exception ratified by **ADR-0002** (Proposed) | ~~PlayMode lifecycle test harness (AC-L1…L6)~~ unblocked (the manager itself still needs to be built) | GDD Open Questions #1 / ADR-0002 |
| **Q#4 REOPENED 2026-07-13** — no per-room→preset mapping exists; `MapModel.GetRandomRoom()` shuffle-bag is decoupled from room identity, and `RoomFile.roomData` was never added to `DungeonRoomSO.cs` | RoomType/room-identity-aware routing (AC-P placement work assumes markers are meaningful per-room) | GDD "Room → RoomModel Resolution" (revised 2026-07-13) |
| **Enemy AI Bugs #7/#8** — `EntityDeathState` wrong base class; empty `Health<=0` transition | real death chain emitting `ON_ENEMY_DEATH` (AC-L2b) | GDD Dependencies / CLAUDE.md Bugs #7/#8 |
| **`Tile_Spawn_Enemy` markers** — parser **is built** (`RoomGeneraterController.LoadRoom()`, confirmed 2026-07-13); only 1 of 13 room JSONs authored, and there is **no centre-fallback** for the other 12 (new gap — `EnemySpawner` throws on an empty marker list) | AC-P3 (parser done); marker authoring + fallback still needed | GDD Dependencies (revised 2026-07-13) |
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
Open Question #1 is now resolved (**ADR-0002**), so the PlayMode lifecycle stories
are unblocked. Still write ADRs for the spawn algorithm + SO data model before
implementing REQ-SPAWN-ALGO / REQ-SPAWN-DATA.
