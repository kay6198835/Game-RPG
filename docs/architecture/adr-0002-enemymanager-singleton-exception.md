# ADR-0002: EnemyManager is a permitted singleton exception

## Status
Proposed

## Date
2026-07-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f3 LTS |
| Domain | Core / Architecture |
| Knowledge Risk | LOW (pinned version is within training data) |
| References Consulted | docs/engine-reference/unity/VERSION.md |
| Post-Cutoff APIs Used | None — static-Instance is a plain C# pattern, not an engine API |
| Verification Required | At implementation time (Sprint 6) confirm `EnemyManager.Awake()`'s duplicate-instance guard includes `return` after `Destroy(gameObject)` — `MazeController.Awake()` (`Assets/Script/Map/Maze/MazeController.cs:17-21`) omits it (TD-026 / Bug #14) and must not be copied verbatim. |

## ADR Dependencies

| Field | Value |
|-------|-------|
| Depends On | None |
| Enables | The PlayMode lifecycle test harness (AC-L1…L6) in `design/gdd/enemy-spawn-system.md`, which is explicitly gated on this decision |
| Blocks | Epic `enemy-spawn` REQ-SPAWN-LIFECYCLE stories were blocked pending this ADR (now unblocked; stories not yet created) |
| Ordering Note | Must be Accepted before the PlayMode lifecycle stories are implemented. `EnemyManager`'s own build is scoped for Sprint 6 and is separately blocked on Bugs #7/#8 (enemy death chain), map-system Bug #16 (RoomType not read at runtime), and absent `Tile_Spawn` markers in all 13 room JSONs. This ADR resolves only the singleton-vs-alternative architecture question — it does not unblock those other prerequisites. |

## Context

### Problem Statement
`EnemyManager` is the sole runtime driver of the enemy-spawn system's per-room
combat lifecycle (`Idle → Populating → Fighting → Cleared`): it listens for
`ON_LOAD_MAP`, resolves `RoomData`, asks the selection engine
(`GetHybridEnemySet`/`GetSpawnSet`) for a weight-budgeted enemy set, spawns it at
`Tile_Spawn` markers, locks the doors (`RoomCell.CloseDoor()`), tracks `aliveCount`
against `ON_ENEMY_DEATH`, and sets `RoomCell.IsCleared` while emitting
`ON_CLEAR_ENEMY` / `ON_ROOM_CLEAR` when the room clears. Multiple independent call
sites — Enemy AI's death chain, Room Progression, and Event Bus subscribers — need
simple access to this single system's state without deep reference wiring.

The project's own standards forbid any new singleton beyond `MazeController`:
`.claude/docs/technical-preferences.md`, `.claude/rules/engine-code.md`, and
`.claude/rules/manager-event-code.md` all state "MazeController is the only permitted
singleton." `design/gdd/enemy-spawn-system.md` Open Question #1 flags this conflict as
**blocking** — it must be resolved by ADR before the PlayMode lifecycle tests
(AC-L1…L6) can be authored, because those tests cannot be written against an
undecided architecture. The owner chose to implement `EnemyManager` as a singleton
(2026-07-08); this ADR exists to either ratify that exception or record the compliant
alternative, so the decision is explicit and traceable rather than an unreviewed rule
violation.

### Constraints
- Sprint 6 timeline — the owner explicitly does not want to spend additional
  engineering time this phase on a more elaborate DI / event-indirection solution.
- `EnemyManager` needs simple, ambient state access (`aliveCount`, lifecycle state)
  from Enemy AI's death-chain callback and from Room Progression, per the GDD's
  Interactions table.
- The "no new singletons" rule is enforced and referenced by four files — any
  exception must be explicit, named, and justified, not implicit.
- Only one dungeon/run is active at a time; there is never a need for two concurrent
  `EnemyManager` instances. The demo targets a single gameplay scene (`RandomMaze`);
  cross-scene persistence is not required.
- Two structurally identical violations already exist without any ADR: TD-023
  (`LevelManager` singleton) and TD-031 (`ObjectPooling` fake singleton) — both still
  Backlog. This ADR is deliberately the first to ratify the pattern explicitly rather
  than leave it informal.

### Requirements
- Exactly one `EnemyManager` instance reachable without deep reference-passing, for the
  lifetime of a loaded dungeon/room scene.
- No stale per-room lifecycle state leaking across room transitions.
- Must not reproduce `MazeController`'s known `Awake()` duplicate-instance bug
  (TD-026 / Bug #14).
- The exception must be explicitly time-boxed with a concrete future review trigger,
  not an open-ended precedent.
- Testable: the PlayMode harness needs a deterministic way to obtain and reset the
  manager between tests.

## Decision
Grant `EnemyManager` a **narrowly-scoped singleton exception**, joining `MazeController`
as the *only* two permitted singletons in the project. It uses the same static-Instance
pattern: `public static EnemyManager Instance { get; private set; }`, set in `Awake()`,
with **no** `DontDestroyOnLoad`. The exception is explicit and bounded — it does not
relax the "no new singletons" rule for any other class.

**Correction found during engine-specialist validation:** `Awake()` firing does NOT
correspond to a per-room reset. `RoomGridController` / `RoomGeneraterController`
transition between rooms by clearing/repainting `Tilemap`s within a single
persistently-loaded dungeon scene — they never call `SceneManager.LoadScene`.
`EnemyManager.Awake()` therefore fires exactly **once per dungeon run**, not once per
room. The instance is correctly scene-scoped (destroyed on dungeon-scene unload, no
`DontDestroyOnLoad` needed), but its **per-room state** (`aliveCount`, lifecycle state)
must be **explicitly reset by the `ON_LOAD_MAP` handler** (an explicit
`ResetForRoom()`-style call at the top of the subscriber), not assumed to reset via
`Awake()`.

This is a pragmatic, time-boxed exception to the "MazeController is the only permitted
singleton" rule, not a reversal of it. The project accepts a Single-Responsibility /
Dependency-Inversion trade-off here specifically to avoid spending Sprint 6 time on a
more decoupled solution, on the explicit condition that it is revisited later.

**Review Trigger (concrete, not vague):**
1. At the **Sprint 6 code-review gate**, when `EnemyManager` is actually implemented —
   confirm the `Awake()` duplicate-guard includes `return` (fixing, not repeating,
   TD-026 / Bug #14) and re-assess whether static-state testing friction has become a
   real EditMode/PlayMode test-authoring cost.
2. If a **third** system independently requests this same singleton exception after
   `EnemyManager` — stop granting per-ADR exceptions and do a proper
   DI/service-locator pass covering all of them at once (including the unratified
   TD-023 `LevelManager` and TD-031 `ObjectPooling`).
3. Whenever TD-023 or TD-031 are scheduled for tech-debt repayment, fold
   `EnemyManager`'s singleton into the same repayment pass.

### Architecture Diagram
```
                    EventManager (static bus)
                 ON_LOAD_MAP ──────────┐
                 ON_ENEMY_DEATH ───────┤
                                       ▼
                          ┌─────────────────────────┐
                          │  EnemyManager.Instance   │◄── scene-scoped, no
                          │  (singleton, Awake-set)  │    DontDestroyOnLoad
                          │  state: Idle/Populating/ │
                          │  Fighting/Cleared        │
                          │  aliveCount              │
                          └───────────┬─────────────┘
                    calls             │             emits
        ┌────────────────────┐        │        ┌───────────────────────┐
        │ selection engine   │◄───────┘        │ EventManager.Emit(     │
        │ GetHybridEnemySet  │                 │  ON_CLEAR_ENEMY,       │
        └────────┬───────────┘                 │  ON_ROOM_CLEAR)        │
                 │ instantiate at              └───────────────────────┘
                 ▼ Tile_Spawn markers
        ┌────────────────────┐
        │ RoomCell           │
        │ .CloseDoor()       │
        │ .IsCleared = true  │
        └────────────────────┘
```

### Key Interfaces
- `public static EnemyManager Instance { get; private set; }` — scene-scoped accessor.
  Valid only while a dungeon scene is loaded; consumers must not cache it across scene
  loads.
- `Awake()` MUST call `return` immediately after `Destroy(gameObject)` in the
  duplicate-instance guard — the one concrete deviation required from `MazeController`'s
  current implementation.
- No `DontDestroyOnLoad(gameObject)` call anywhere in `EnemyManager`.
- The `ON_LOAD_MAP` handler explicitly resets per-room state before resolving the new
  room's `RoomData`.
- The selection algorithm (`GetHybridEnemySet`) is deliberately **not** on the singleton
  — it is a pure method on the `RoomData`/`EnemyDatabase` SO with an injected
  `System.Random`, so it is unit-testable without the singleton.
- All cross-system communication stays on the existing `EventManager` static bus
  (subscribe in `OnEnable`, unsubscribe in `OnDisable`). Other systems must not reach
  into `EnemyManager.Instance` to mutate its state; they communicate via `EventID`
  events only. The singleton is for *ownership of room-combat state*, not a back-channel.

## Alternatives Considered

### Alternative 1: Singleton exception (CHOSEN)
- **Description**: `EnemyManager` is a `public static Instance` singleton, added to the
  permitted list alongside `MazeController`.
- **Pros**: Matches the owner decision (2026-07-08); simplest wiring; single owner of
  room-combat state; unblocks the lifecycle test harness immediately.
- **Cons**: A second permitted singleton — erodes the "one singleton only" line and sets
  a precedent that must be actively resisted; global mutable state is harder to isolate
  in tests (needs explicit reset between tests).
- **Rejection Reason**: N/A — chosen. Mitigations: exception is named and scoped;
  duplicate-guard mandated; state stays event-driven, not called into directly.

### Alternative 2: Inspector-wired scene reference (rules-compliant fallback)
- **Description**: A plain `MonoBehaviour` in the gameplay scene referenced via
  `[SerializeField]` by the few systems that need it (or discovered once at scene load
  and cached), with no `static Instance` — per `manager-event-code.md`'s "GetComponent
  or Inspector refs" preference.
- **Pros**: No static/global state; fully unit-testable in isolation; SOLID-clean DI;
  easier to instantiate a fresh isolated instance per PlayMode test.
- **Cons**: Every consumer needs the reference threaded — N Inspector wire-ups or a
  small locator/registry (which reintroduces the same problem in a different shape);
  more setup time this sprint; missed-wiring null-refs harder to catch than a
  compile-time singleton reference.
- **Rejection Reason**: Owner traded this SOLID-cleaner approach for implementation speed
  in Sprint 6. Recorded as the compliant fallback — a future ADR could supersede this one
  to migrate to the Inspector-wired form (see Review Trigger #1/#2).

### Alternative 3: ScriptableObject-event-based indirection
- **Description**: Decouple `EnemyManager` from callers via SO event channels (e.g. an
  `OnEnemyDeathEventChannel`), combined with a `GetComponent`/scene-root reference for the
  non-event half of the interactions.
- **Pros**: Extends a pattern the project already uses; no direct object reference for the
  event-shaped interactions; testable with mock event channels.
- **Cons**: Only solves half the access pattern — the GDD's Interactions table shows
  `EnemyManager` both as an event listener/emitter **and** as a direct method-caller on
  `RoomCell` (`CloseDoor()`, `IsCleared` setter, not event-shaped). Still needs a
  reference-resolution mechanism for the direct-call half; more design/implementation time
  than a plain singleton.
- **Rejection Reason**: Solves less of the actual access problem than it appears to while
  costing more time than a plain singleton.

### Alternative 4: Fold room-combat state into MazeController
- **Description**: Reuse the one existing permitted singleton — put alive-count/room-state
  on `MazeController`.
- **Pros**: No new singleton at all.
- **Cons**: Violates single-responsibility — `MazeController` owns maze generation, not
  per-room combat; couples two unrelated concerns; makes both harder to test.
- **Rejection Reason**: Worse separation of concerns than a scoped second singleton.

## Consequences

### Positive
- Matches an existing, working precedent (`MazeController`) — no new mental model for
  contributors.
- Fastest path to closing GDD Open Question #1 and unblocking AC-L1…L6 test authoring,
  preserving the Sprint 6 timeline.
- Simple ambient data access (`EnemyManager.Instance.aliveCount`, etc.) from Enemy AI and
  Room Progression callbacks without deep reference wiring.
- Room-combat state has one unambiguous owner during a run; reviewers hitting
  `EnemyManager.Instance` can point here instead of flagging a rule violation.

### Negative
- **Explicit SOLID violation, accepted deliberately**: static `Instance` access is
  textbook Dependency-Inversion / Single-Responsibility erosion — any class can reach into
  `EnemyManager.Instance` directly. The owner accepted this trade-off to save
  implementation time this phase.
- Harder to unit-test `EnemyManager` in isolation — static state must be reset between
  EditMode/PlayMode test runs.
- Sets a **second** ratified precedent alongside two already-existing *unratified*
  violations (TD-023, TD-031). Without the Review Trigger above, "just this once"
  singletons could proliferate silently, as they already have twice.
- The rule's "only `MazeController`" wording in four files must be updated to read
  "`MazeController` and `EnemyManager`" or the exception will keep being re-flagged.
  (`.claude/rules/manager-event-code.md` already updated; `engine-code.md` and
  `map-code.md` still read "only `MazeController`" in places — Manual Review.)

### Risks
- **Risk**: `EnemyManager` repeats `MazeController`'s exact `Awake()` bug (missing
  `return` after `Destroy(gameObject)` — TD-026 / Bug #14).
  **Mitigation**: called out in Key Interfaces and Verification Required; mandatory
  Sprint 6 code-review checklist item.
- **Risk (confirmed, not hypothetical)**: room transitions are Tilemap swaps within one
  persistent dungeon scene, not scene reloads. `Awake()` fires once per dungeon run, so
  per-room state carries over unless explicitly reset.
  **Mitigation**: `ON_LOAD_MAP` handler MUST explicitly reset `aliveCount` and lifecycle
  state at the top, before resolving the new room's `RoomData`. Mandatory Sprint 6
  acceptance criterion.
- **Risk**: if `Enter Play Mode Options` is ever enabled with domain reload disabled,
  static fields including `Instance` stop resetting between consecutive Editor Play
  sessions. Currently disabled (`0`), so not active.
  **Mitigation**: none needed while the setting stays disabled; flag if anyone proposes
  enabling it.
- **Risk**: precedent creep — a future contributor points to this ADR to justify an
  unrelated singleton.
  **Mitigation**: registry entry (`docs/registry/architecture.yaml`) records this as a
  named, bounded exception scoped to `EnemyManager`; Review Trigger #2 defines where
  per-ADR exceptions stop.
- **Risk**: test leakage from static state.
  **Mitigation**: tests null/dispose `Instance` in `TearDown`; the pure
  `GetHybridEnemySet` (no singleton) carries the bulk of the logic tests.

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|------------|-------------|--------------------------|
| design/gdd/enemy-spawn-system.md | Open Question #1 — "`EnemyManager` singleton violates 'no new singletons'"; gates AC-L1…L6 PlayMode test authoring | Ratifies the singleton pattern so Open Question #1 can close and the PlayMode lifecycle harness can be authored against a defined architecture |
| design/gdd/enemy-spawn-system.md | States/Transitions table assumes "`EnemyManager` (singleton — see Open Questions)" | Confirms the singleton the GDD's own state machine already assumed is the ratified architecture, not a placeholder |
| design/gdd/enemy-spawn-system.md | REQ-SPAWN-LIFECYCLE (AC-L1…L6) — room lifecycle test harness | Decides the architecture the PlayMode harness is written against, unblocking those tests |

*Note: the GDD header now reads `**Status**: Approved (design) · Prototype partial`; an
earlier `In Design` header was flagged as a doc-sync gap and has since been reconciled.*

## Performance Implications
- **CPU**: Negligible — a static property read has no measurable per-frame cost beyond a
  normal field access; spawn work happens once per room load, not per frame.
- **Memory**: One additional persistent `MonoBehaviour` while a dungeon scene is loaded
  (same order of magnitude as `MazeController`); destroyed on scene unload (no
  `DontDestroyOnLoad`).
- **Load Time**: None beyond the normal `Awake()` cost.
- **Network**: N/A — single-player demo.

## Migration Plan
`EnemyManager` is not yet functionally implemented. As of 2026-07-09 `Assets/Script/Enemy/`
holds scaffold **stubs only** — `EnemyManager.cs` declares just `public static Instance`
(no `Awake`, no lifecycle) and `EnemySpawner.cs` subscribes `ON_LOAD_MAP` with an empty
`Spawn()` (both suspected non-compiling: undefined `OnDoneLoadRoomGrid`; `System.Numerics`
+ `UnityEngine` `Vector3` ambiguity). The current working spawn path is
`LevelManager.SpawnRoomEnemies()` off an Editor button — a manual stand-in with no
lifecycle. Building `EnemyManager` for real replaces that editor-only path with the
event-driven runtime flow; the prototype's selection (`RoomModel.GetSpawnSet`) can be
reused behind it. When implemented:
1. `Awake()` duplicate-guard **must** include `return` after `Destroy(gameObject)` — do
   not copy `MazeController.Awake()` verbatim; fix TD-026 / Bug #14. Explicit Sprint 6
   story acceptance criterion / code-review item.
2. The `ON_LOAD_MAP` handler **must** explicitly reset per-room state at the top, before
   resolving the new room's `RoomData`. Mandatory Sprint 6 acceptance criterion.
3. Update `.claude/rules/engine-code.md` and `.claude/rules/map-code.md` to name
   `EnemyManager` as a second permitted singleton citing ADR-0002 (as
   `manager-event-code.md` already does).
4. `docs/tech-debt-register.md` TD-023 and TD-031 rows may be annotated to
   cross-reference ADR-0002 as the precedent that formalizes (without excusing) the same
   pattern — follow-up, not part of authoring this ADR.
If a future ADR supersedes this to adopt Alternative 2: remove `static Instance`, add
`[SerializeField]` references / a cached scene lookup, and wire the manager in each
gameplay scene.

## Validation Criteria
- `EnemyManager.Instance` is non-null immediately after the dungeon scene's Awake phase
  completes (once per dungeon run).
- Duplicate-instance guard test: instantiate/trigger a second `EnemyManager` in the same
  scene and confirm the second is destroyed **and** does not overwrite `Instance` nor
  re-run initialization (the exact `MazeController` / TD-026 regression this ADR must not
  repeat).
- **Per-room reset test**: load Room A, drive its lifecycle to `Cleared` with
  `aliveCount == 0`, then transition to Room B via the existing Tilemap-swap flow
  (`ON_LOAD_MAP`) — confirm `aliveCount` and lifecycle state are reset for Room B, not
  carried over.
- Only `MazeController` and `EnemyManager` contain a `public static … Instance` pattern in
  the codebase (grep gate).
- No `DontDestroyOnLoad` call present on `EnemyManager` (code-review/grep check).
- AC-L1…L6 (`enemy-spawn-system.md` PlayMode Acceptance Criteria) pass using
  `EnemyManager.Instance` as the harness's access point, with `TearDown` reset and no
  cross-test state leakage.
- This ADR's Status is explicitly revisited (not necessarily changed) at the Sprint 6
  code-review gate, and again if/when TD-023 or TD-031 are scheduled for repayment.

## Related Decisions
- ADR-0001 (StatSystem dual data structure) — boundary contract: `EnemyData` is spawn
  metadata and must not become a fourth stat store; combat stats stay on the prefab's
  `EntityData`/`StatsSO`.
- design/gdd/enemy-spawn-system.md — Open Question #1; Open Question #2 (runtime seed
  source); States/Transitions table.
- docs/tech-debt-register.md — TD-023 (`LevelManager` singleton, same violation, no ADR),
  TD-031 (`ObjectPooling` fake singleton, same violation, no ADR), TD-026 (`MazeController`
  `Awake()` duplicate-guard bug — cautionary precedent this ADR explicitly avoids
  repeating).
- Assets/Script/Map/Maze/MazeController.cs — reference implementation this decision
  extends to a second instance.
- .claude/rules/engine-code.md, .claude/rules/manager-event-code.md, .claude/rules/map-code.md
  — rules this ADR formally excepts `EnemyManager` from.
