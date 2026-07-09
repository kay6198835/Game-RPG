# ADR-0002: EnemyManager is a permitted singleton exception

## Status
Proposed

## Date
2026-07-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 2022.3.62f1 LTS |
| **Domain** | Core / Architecture |
| **Knowledge Risk** | LOW (pinned version is within training data) |
| **References Consulted** | docs/engine-reference/unity/VERSION.md |
| **Post-Cutoff APIs Used** | None |
| **Verification Required** | None — this is a code-organization/governance decision, not an engine-API decision |

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None |
| **Enables** | Enemy Spawn PlayMode lifecycle test harness (AC-L1…L6) can be authored against a decided architecture |
| **Blocks** | Epic `enemy-spawn` — REQ-SPAWN-LIFECYCLE stories were blocked pending this ADR |
| **Ordering Note** | Must be Accepted before the PlayMode lifecycle stories are implemented |

## Context

### Problem Statement
The Enemy Spawn & Per-Room Management GDD (`design/gdd/enemy-spawn-system.md`)
introduces `EnemyManager` — the runtime driver that listens for room-load,
resolves a `RoomData`, asks `EnemyDatabase` for a weight-budgeted enemy set,
spawns it, locks the doors, tracks the alive count, and emits `ON_CLEAR_ENEMY` /
`ON_ROOM_CLEAR` when the room is cleared. The GDD (Open Question #1) records that
the owner chose to implement `EnemyManager` as a **singleton** (2026-07-08).

This directly conflicts with a standard that is enforced project-wide:

- `.claude/docs/technical-preferences.md` → Forbidden Patterns: "New singletons
  beyond `MazeController`".
- `.claude/rules/manager-event-code.md`, `map-code.md`, `engine-code.md` →
  "`MazeController` is the only permitted singleton".

The conflict is a hard blocker: the PlayMode lifecycle test harness (AC-L1…L6)
cannot be authored against an undecided architecture. This ADR exists to either
ratify the exception or record the compliant alternative — so the decision is
explicit and traceable rather than an unreviewed rule violation.

### Constraints
- The "no new singletons" rule is enforced and referenced by four rule files —
  any exception must be explicit, named, and justified, not implicit.
- `EnemyManager` must be reachable from the event flow (`ON_LOAD_MAP`,
  `ON_ENEMY_DEATH`) and hold per-room combat state (alive count, room state).
- Only one dungeon/run is active at a time; there is never a need for two
  concurrent `EnemyManager` instances.
- The demo targets a single gameplay scene (`RandomMaze`); cross-scene
  persistence is not required.

### Requirements
- `EnemyManager` state (room lifecycle, alive count) must have a single,
  unambiguous owner during a run.
- The decision must not silently normalize "singletons are fine now" — the
  exception is scoped to `EnemyManager` only.
- Must be testable: the PlayMode harness needs a deterministic way to obtain and
  reset the manager between tests.

## Decision

**Grant `EnemyManager` a narrowly-scoped singleton exception**, joining
`MazeController` as the *only* two permitted singletons in the project. The
exception is explicit and bounded — it does not relax the "no new singletons"
rule for any other class.

Implementation constraints that bound the exception:

- `EnemyManager` exposes a single `public static EnemyManager Instance { get; private set; }`.
- `Awake()` enforces uniqueness and **returns after `Destroy(gameObject)`** on a
  duplicate — the exact guard `MazeController` is missing (CLAUDE.md Bug #14);
  do not repeat that bug here.
- It lives in the gameplay scene and is **not** `DontDestroyOnLoad` — it is a
  per-run/per-scene object, so `Reborn`/scene-reload naturally disposes it.
- All cross-system communication stays on the existing `EventManager` static bus
  (subscribe in `OnEnable`, unsubscribe in `OnDisable`) — the singleton is for
  *ownership of room-combat state*, not a back-channel for other systems to call
  into. Other systems must not reach into `EnemyManager.Instance` to mutate its
  state; they communicate via `EventID` events only.

### Architecture Diagram

```
  ON_LOAD_MAP ─┐
               ▼
        EnemyManager (singleton)
          ├─ resolve RoomData (RoomType → RoomData table)
          ├─ EnemyDatabase.GetHybridEnemySet(...)   ← pure, injectable-RNG, testable
          ├─ spawn at Tile_Spawn markers
          ├─ RoomCell.CloseDoor()                    ← Map system contract
          └─ aliveCount ← N
  ON_ENEMY_DEATH ──► aliveCount-- ──► 0 ──► Emit(ON_CLEAR_ENEMY) + Emit(ON_ROOM_CLEAR)
```

### Key Interfaces
- `public static EnemyManager Instance { get; private set; }`
- Subscribes: `EventID.ON_LOAD_MAP`, `EventID.ON_ENEMY_DEATH`.
- Emits: `EventID.ON_CLEAR_ENEMY` (existing), `EventID.ON_ROOM_CLEAR` (new).
- The selection algorithm (`EnemyDatabase.GetHybridEnemySet`) is deliberately
  **not** on the singleton — it is a pure method on the `EnemyDatabase` SO with an
  injected `System.Random`, so it is unit-testable without the singleton.

## Alternatives Considered

### Alternative 1: Singleton exception (CHOSEN)
- **Description**: `EnemyManager` is a `public static Instance` singleton, added to
  the permitted list alongside `MazeController`.
- **Pros**: Matches the owner decision (2026-07-08); simplest wiring; single owner
  of room-combat state; unblocks the lifecycle test harness immediately.
- **Cons**: A second permitted singleton — erodes the "one singleton only" line and
  sets a precedent that must be actively resisted for future managers; global
  mutable state is harder to isolate in tests (needs explicit reset between tests).
- **Rejection Reason**: N/A — chosen. Mitigations: exception is named and scoped;
  duplicate-guard mandated; state stays event-driven, not called into directly.

### Alternative 2: Inspector-wired scene component (rules-compliant)
- **Description**: `EnemyManager` is a plain `MonoBehaviour` placed in the gameplay
  scene and referenced via `[SerializeField]` by the few systems that need it
  (or discovered once at scene load and cached), with no `static Instance`.
- **Pros**: Fully complies with the enforced "no new singletons" rule; easier to
  instantiate a fresh isolated instance per PlayMode test; no global state.
- **Cons**: Requires Inspector wiring in every gameplay scene; the event-driven
  design already means almost nothing needs a *direct* reference, so the ergonomic
  gain of a singleton is small — but so is the cost of avoiding one.
- **Rejection Reason**: Owner chose the singleton for wiring simplicity. Recorded
  here as the compliant fallback if the singleton precedent becomes a problem — a
  future ADR could supersede this one to migrate to the Inspector-wired form.

### Alternative 3: Fold room-combat state into MazeController
- **Description**: Reuse the one existing permitted singleton instead of adding a
  new one — put alive-count/room-state on `MazeController`.
- **Pros**: No new singleton at all.
- **Cons**: Violates single-responsibility — `MazeController` owns maze generation,
  not per-room combat; couples two unrelated concerns; makes both harder to test.
- **Rejection Reason**: Worse separation of concerns than a scoped second singleton.

## Consequences

### Positive
- The lifecycle test harness (AC-L1…L6) is unblocked — architecture is now decided.
- Room-combat state has one unambiguous owner during a run.
- The exception is explicit and traceable; reviewers hitting `EnemyManager.Instance`
  can point here instead of flagging a rule violation.

### Negative
- Two permitted singletons instead of one — the rule's "only `MazeController`"
  wording in four files must be updated to read "`MazeController` and
  `EnemyManager`" or the exception will keep being re-flagged.
- Global mutable state: PlayMode tests must explicitly reset/dispose the instance
  between tests to avoid leakage (the project's TearDown discipline applies).

### Risks
- **Precedent creep** — future managers argue "EnemyManager got one too."
  *Mitigation*: this ADR scopes the exception to `EnemyManager` by name; any
  further singleton needs its own ADR and should be pushed toward Alternative 2.
- **Duplicate-instance bug** (the `MazeController` Bug #14 class of defect).
  *Mitigation*: mandated `return` after `Destroy(gameObject)` in `Awake()`.
- **Test leakage** from static state. *Mitigation*: tests null/dispose `Instance`
  in `TearDown`; the pure `GetHybridEnemySet` (no singleton) carries the bulk of
  the logic tests.

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|------------|-------------|--------------------------|
| design/gdd/enemy-spawn-system.md | Open Question #1 — `EnemyManager` singleton violates "no new singletons"; needs an ADR to ratify the exception or wrap as Inspector component | Ratifies the exception explicitly, scoped to `EnemyManager`, with duplicate-guard + event-driven constraints |
| design/gdd/enemy-spawn-system.md | REQ-SPAWN-LIFECYCLE (AC-L1…L6) — room lifecycle test harness | Decides the architecture the PlayMode harness is written against, unblocking those tests |

## Performance Implications
- **CPU**: Negligible — one static reference lookup; no per-frame cost added by the
  singleton pattern itself. Spawn work happens once per room load, not per frame.
- **Memory**: One long-lived manager object per run (already required regardless of
  the singleton decision).
- **Load Time**: None beyond the manager's own `Awake`.
- **Network**: N/A (single-player demo).

## Migration Plan
`EnemyManager` is not yet implemented. The current prototype (2026-07-09) drives
spawning through `LevelManager.SpawnRoomEnemies()` off an Editor button — a manual
stand-in with no lifecycle. Building `EnemyManager` replaces that editor-only path
with the event-driven runtime flow; the prototype's selection (`RoomModel.GetSpawnSet`)
can be reused behind it. If a future ADR supersedes this to adopt Alternative 2,
migration is: remove `static Instance`, add `[SerializeField]` references / a cached
scene lookup, and wire the manager in each gameplay scene.

## Validation Criteria
- Only `MazeController` and `EnemyManager` contain a `public static … Instance`
  pattern in the codebase (grep gate).
- `EnemyManager.Awake()` destroys and **returns** on a duplicate; a scene with two
  managers ends with exactly one live instance.
- PlayMode lifecycle tests (AC-L1…L6) obtain the instance deterministically and
  reset it in `TearDown` with no cross-test state leakage.

## Related Decisions
- ADR-0001 (StatSystem dual data structure) — boundary contract: `EnemyData` is
  spawn metadata and must not become a fourth stat store.
- `.claude/docs/technical-preferences.md` — "no new singletons" rule this ADR
  carves a named exception in.
- `design/gdd/enemy-spawn-system.md` — Open Question #1, Open Question #2 (runtime
  seed source, still open — separate decision).
