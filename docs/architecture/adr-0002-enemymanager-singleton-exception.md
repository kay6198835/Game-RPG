# ADR-0002: EnemyManager as a Time-Boxed Singleton Exception

## Status
Proposed

## Date
2026-07-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f1 LTS |
| Domain | Core / Architecture |
| Knowledge Risk | LOW (pinned version is within training data) |
| References Consulted | docs/engine-reference/unity/VERSION.md |
| Post-Cutoff APIs Used | None — static-Instance is a plain C# pattern, not an engine API |
| Verification Required | Confirm at implementation time (Sprint 6) that `EnemyManager`'s Awake() duplicate-instance guard includes `return` after `Destroy(gameObject)` — `MazeController`'s Awake() (`Assets/Script/Map/Maze/MazeController.cs:17-21`) omits it (TD-026 / Bug #14) and must not be copied verbatim. |

## ADR Dependencies

| Field | Value |
|-------|-------|
| Depends On | None |
| Enables | The PlayMode lifecycle test harness (AC-L1…L6) in `design/gdd/enemy-spawn-system.md`, which is explicitly gated on this decision |
| Blocks | None currently — no story/epic files exist yet (`production/stories/`, `production/epics/` not created; S4-D3 epic breakdown hasn't run) |
| Ordering Note | `EnemyManager`'s own build is scoped for Sprint 6 and is separately blocked on Bugs #7/#8 (enemy death chain), map-system Bug #16 (RoomType not read at runtime), and absent `Tile_Spawn` markers in all 13 room JSONs. This ADR resolves only the singleton-vs-alternative architecture question — it does not unblock those other prerequisites. |

## Context

### Problem Statement
`EnemyManager` is the sole runtime driver of the enemy-spawn system's per-room
combat lifecycle (`Idle → Populating → Fighting → Cleared`): it listens for
`ON_LOAD_MAP`, resolves `RoomData`, asks `EnemyDatabase` for a weight-budgeted
enemy set, spawns it, tracks `aliveCount` against `ON_ENEMY_DEATH`, and calls
`RoomCell.CloseDoor()` / sets `RoomCell.IsCleared`. Multiple independent call
sites — Enemy AI's death chain, Room Progression, and Event Bus subscribers —
need simple access to this single system's state without deep reference
wiring. The project's own preferences (`.claude/docs/technical-preferences.md`,
`.claude/rules/engine-code.md`, `.claude/rules/manager-event-code.md`)
currently forbid any new singleton beyond `MazeController`.
`design/gdd/enemy-spawn-system.md` Open Question #1 flags this conflict as
**blocking** — it must be resolved by ADR before the PlayMode lifecycle tests
(AC-L1…L6) can be authored, because those tests cannot be written against an
undecided architecture.

### Constraints
- Sprint 6 timeline — the owner explicitly does not want to spend additional
  engineering time this phase on a more elaborate DI/event-indirection
  solution.
- `EnemyManager` needs simple, ambient state access (`aliveCount`, lifecycle
  state) from Enemy AI's death-chain callback and from Room Progression, per
  the GDD's Interactions table.
- `.claude/rules/engine-code.md`: *"MazeController is the ONLY permitted
  global singleton — all other systems use GetComponent or Inspector refs"*
  and *"Core.cs and EntityCore.cs are the ONLY permitted component hubs — no
  new singleton hubs."*
- `.claude/rules/manager-event-code.md`: *"MazeController is the only
  permitted singleton"* and *"Managers that need cross-scene access use
  ScriptableObject events or DontDestroyOnLoad with a scene manager pattern —
  not Instance singletons."* This ADR formally excepts `EnemyManager` from
  both rules.
- Two structurally identical violations already exist in the codebase without
  any ADR: TD-023 (`LevelManager` singleton) and TD-031 (`ObjectPooling` fake
  singleton) — both still Backlog. This ADR is deliberately the first to
  ratify the pattern explicitly rather than leave it informal.

### Requirements
- Exactly one `EnemyManager` instance reachable without deep reference-passing,
  for the lifetime of a loaded dungeon/room scene.
- No stale per-room lifecycle state leaking across scene loads.
- Must not reproduce `MazeController`'s known Awake() duplicate-instance bug
  (TD-026 / Bug #14).
- The exception must be explicitly time-boxed with a concrete future review
  trigger, not an open-ended precedent.

## Decision
`EnemyManager` will use the same static-Instance singleton pattern as
`MazeController`: `public static EnemyManager Instance { get; private set; }`,
set in `Awake()`, **no** `DontDestroyOnLoad`.

**Correction from the original draft, found during engine-specialist
validation**: `Awake()` firing does NOT correspond to a per-room reset.
`RoomGridController`/`RoomGeneraterController` (per `CLAUDE.md`'s Map/Dungeon
Generation architecture) transition between rooms by clearing/repainting
`Tilemap`s within a single persistently-loaded dungeon scene — they never
call `SceneManager.LoadScene`. `EnemyManager.Awake()` therefore fires exactly
**once per dungeon run**, not once per room. The instance itself is correctly
scene-scoped (destroyed on dungeon-scene unload, no `DontDestroyOnLoad`
needed) — but its **per-room state** (`aliveCount`, lifecycle state
`Idle/Populating/Fighting/Cleared`) must be **explicitly reset by the
`ON_LOAD_MAP` handler**, not assumed to reset via `Awake()`. This must be
implemented as an explicit `ResetForRoom()`-style call at the top of the
`ON_LOAD_MAP` subscriber, not left implicit — see Migration Plan.

This is an explicit, **pragmatic, time-boxed exception** to the
"MazeController is the only permitted singleton" rule, not a reversal of that
rule. The project accepts a Single-Responsibility / Dependency-Inversion
trade-off here specifically to avoid spending Sprint 6 time on a more
decoupled solution, on the explicit condition that it is revisited later (see
Review Trigger below) rather than becoming silent, undocumented precedent —
the fate that already befell TD-023 and TD-031.

**Review Trigger (concrete, not vague):**
1. At the **Sprint 6 code-review gate**, when `EnemyManager` is actually
   implemented — confirm the Awake() duplicate-guard includes `return`
   (fixing, not repeating, TD-026/Bug #14) and re-assess whether the
   static-state testing friction predicted below has become a real
   EditMode/PlayMode test authoring cost.
2. If a **third** system independently requests this same singleton exception
   after `EnemyManager` (MazeController + EnemyManager already makes two
   ratified singletons, plus TD-023's `LevelManager` and TD-031's
   `ObjectPooling` as two more *unratified* violations of the same rule) —
   stop granting per-ADR exceptions and do a proper DI/service-locator pass
   covering all of them at once.
3. Whenever TD-023 or TD-031 are scheduled for tech-debt repayment, fold
   `EnemyManager`'s singleton into the same repayment pass rather than
   treating it separately.

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
                          └───────────┬──────────────┘
                    calls             │             emits
        ┌───────────────────┐        │        ┌───────────────────────┐
        │ EnemyDatabase      │◄───────┘        │ EventManager.Emit(    │
        │ .GetHybridEnemySet │                 │  ON_CLEAR_ENEMY,      │
        └────────┬────────────┘                │  ON_ROOM_CLEAR)       │
                  │ instantiate at              └───────────────────────┘
                  ▼ Tile_Spawn markers
        ┌───────────────────┐
        │ RoomCell            │
        │ .CloseDoor()          │
        │ .IsCleared = true      │
        └───────────────────┘
```

### Key Interfaces
- `public static EnemyManager Instance { get; private set; }` — scene-scoped
  accessor. Valid only while a dungeon scene is loaded; consumers must not
  cache it across scene loads.
- `Awake()` MUST call `return` immediately after `Destroy(gameObject)` in the
  duplicate-instance guard — this is the one concrete deviation required from
  `MazeController`'s current implementation.
- No `DontDestroyOnLoad(gameObject)` call anywhere in `EnemyManager`.
- Internal lifecycle state (`Idle`/`Populating`/`Fighting`/`Cleared`) may be
  exposed read-only later for HUD/debug use; not required by the current GDD.

## Alternatives Considered

### Alternative 1: Inspector-wired scene reference
- **Description**: A root scene controller (e.g. an extension of
  `RoomGeneraterController` or a new lightweight scene-root component) holds a
  `[SerializeField] private EnemyManager _enemyManager` and threads the
  reference to consumers (Enemy AI death-chain callback, Room Progression,
  Event Bus subscribers), per `manager-event-code.md`'s stated "GetComponent
  or Inspector refs" preference.
- **Pros**: No static/global state; fully unit-testable in isolation; no
  hidden coupling; SOLID-clean dependency injection instead of ambient static
  access.
- **Cons**: Every consumer needs the reference threaded through — either N
  Inspector wire-ups across scenes/prefabs, or building a small
  locator/registry anyway (which reintroduces the same problem in a different
  shape); more setup time this sprint, which is exactly what the owner wants
  to avoid; missed-wiring null-ref bugs are harder to catch than a
  compile-time singleton reference.
- **Rejection Reason**: Owner explicitly traded this SOLID-cleaner approach
  for implementation speed in Sprint 6. Revisit if wiring overhead in
  practice proves worse than the singleton's downsides (see Review Trigger
  #1/#2).

### Alternative 2: ScriptableObject-event-based indirection
- **Description**: Per `manager-event-code.md`'s second suggested pattern,
  decouple `EnemyManager` from its callers via SO event channels (e.g. an
  `OnEnemyDeathEventChannel` that Enemy AI raises and `EnemyManager` listens
  to), combined with a `GetComponent`/scene-root reference or a
  `DontDestroyOnLoad` scene-manager pattern for the non-event half of the
  interactions.
- **Pros**: Extends a pattern the project already uses and enforces
  (`EventManager` static bus is listed "enforced" in
  `technical-preferences.md`'s Architecture Decisions Log); no direct object
  reference for the event-shaped interactions; testable with mock event
  channels.
- **Cons**: Only solves half the access pattern. The GDD's Interactions table
  shows `EnemyManager` both as an event listener/emitter (event-shaped, fits
  this alternative) **and** as a direct method-caller on `RoomCell`
  (`CloseDoor()`, `IsCleared` setter — not event-shaped). This alternative
  still needs some reference-resolution mechanism for the direct-call half, so
  it does not fully replace the singleton; more design/implementation time to
  build and test the new SO event-channel types than a plain singleton.
- **Rejection Reason**: Solves less of the actual access problem than it
  appears to while costing more time than a plain singleton; owner
  prioritized speed over a partial decoupling gain.

## Consequences

### Positive
- Matches an existing, working precedent (`MazeController`) — no new mental
  model for contributors to learn.
- Fastest path to closing GDD Open Question #1 and unblocking AC-L1…L6 test
  authoring, preserving the Sprint 6 timeline.
- Simple ambient data access (`EnemyManager.Instance.aliveCount`, etc.) from
  Enemy AI and Room Progression callbacks without deep reference wiring.

### Negative
- **Explicit SOLID violation, accepted deliberately**: static `Instance`
  access is textbook Dependency-Inversion / Single-Responsibility erosion —
  any class can reach into `EnemyManager.Instance` directly instead of
  receiving it as an explicit dependency, which is exactly the coupling
  `engine-code.md`/`manager-event-code.md` were written to prevent. The owner
  explicitly accepted this trade-off in exchange for saving implementation
  time this phase.
- Harder to unit-test `EnemyManager` in isolation — static state must be reset
  between EditMode/PlayMode test runs.
- Sets a **second** ratified precedent alongside two already-existing,
  *unratified* violations of the same rule (TD-023 `LevelManager`, TD-031
  `ObjectPooling`). Without the explicit Review Trigger above, "just this
  once" singletons could proliferate silently, as they already have twice.

### Risks
- **Risk**: `EnemyManager` repeats `MazeController`'s exact Awake() bug
  (missing `return` after `Destroy(gameObject)` — TD-026/Bug #14).
  **Mitigation**: called out explicitly in Key Interfaces and Verification
  Required above; engine specialist validation checks this directly; make it
  a mandatory Sprint 6 code-review checklist item.
- **Risk (confirmed, not hypothetical — see Decision correction above)**:
  room transitions in this project are Tilemap swaps within one persistent
  dungeon scene, not scene reloads. `Awake()` fires once per dungeon run, so
  per-room state (`aliveCount`, lifecycle state) will silently carry over
  from the previous room unless explicitly reset.
  **Mitigation**: `EnemyManager`'s `ON_LOAD_MAP` handler MUST explicitly reset
  `aliveCount` and lifecycle state at the top of its handler, before resolving
  the new room's `RoomData` — do not rely on `Awake()`/instance lifecycle for
  this. Add as a mandatory Sprint 6 acceptance criterion (see Validation
  Criteria).
- **Risk**: if `Enter Play Mode Options` (`ProjectSettings/EditorSettings.asset`
  → `m_EnterPlayModeOptionsEnabled`) is ever turned on with domain reload
  disabled, static fields including `Instance` stop resetting between
  consecutive Editor Play sessions — masking or creating stale-Instance bugs
  that wouldn't reproduce in a build. Currently disabled (`0`) as of this ADR's
  writing, so not an active risk, but a settings-dependent one.
  **Mitigation**: none needed while the setting stays disabled; flag if anyone
  proposes enabling it for iteration speed.
- **Risk**: Precedent creep — a future contributor points to this ADR to
  justify an unrelated singleton.
  **Mitigation**: the registry entry (`docs/registry/architecture.yaml`)
  records this as a named, bounded exception scoped to `EnemyManager`, not a
  general license; Review Trigger #2 above defines the exact point where
  per-ADR exceptions stop being granted.

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|------------|-------------|--------------------------|
| design/gdd/enemy-spawn-system.md | Open Question #1 — "`EnemyManager` singleton violates 'no new singletons'"; gates AC-L1…L6 PlayMode test authoring | Ratifies the singleton pattern so Open Question #1 can close and the PlayMode lifecycle harness can be authored against a defined architecture |
| design/gdd/enemy-spawn-system.md | States/Transitions table already assumes "`EnemyManager` (singleton — see Open Questions)" | Confirms the singleton the GDD's own state machine already assumed is the ratified architecture, not a placeholder |

*Note: the GDD's own body header still reads `**Status**: In Design` while
`design/gdd/systems-index.md` lists it `Approved` (row 20) — a pre-existing
doc-sync gap unrelated to this ADR. This ADR cites the `Approved` status per
`systems-index.md` as authoritative; the stale header is a separate
housekeeping item already flagged in the Sprint 4 tracker.*

## Performance Implications
- **CPU**: Negligible — a static property read has no measurable per-frame
  cost beyond a normal field access; no different from existing
  `MazeController` usage.
- **Memory**: One additional persistent `MonoBehaviour` instance while a
  dungeon scene is loaded (same order of magnitude as `MazeController`);
  destroyed on scene unload since no `DontDestroyOnLoad` is used.
- **Load Time**: None beyond the normal `Awake()` cost already budgeted for a
  `MazeController`-equivalent component.
- **Network**: N/A — no networking in this project.

## Migration Plan
No existing code to migrate — `EnemyManager` does not exist yet; this is a
greenfield decision ahead of its Sprint 6 build. When it is implemented:
1. Its `Awake()` duplicate-guard **must** include `return` after
   `Destroy(gameObject)` — do not copy `MazeController.Awake()` verbatim; fix
   the known bug (TD-026/Bug #14) rather than propagate it into new code. Add
   this as an explicit Sprint 6 story acceptance criterion / code-review item.
2. Its `ON_LOAD_MAP` event handler **must** explicitly reset per-room state
   (`aliveCount`, lifecycle state) at the top of the handler, before resolving
   the new room's `RoomData` — room transitions are Tilemap swaps within one
   persistent scene (confirmed via `RoomGridController`/
   `RoomGeneraterController`), not scene reloads, so `Awake()` will not fire
   again between rooms. This is a mandatory Sprint 6 acceptance criterion, not
   optional cleanup.
3. Update `.claude/rules/engine-code.md` and
   `.claude/rules/manager-event-code.md` to name `EnemyManager` as a second
   permitted singleton exception, citing ADR-0002 — so the rule text and the
   codebase don't silently diverge the way they already have for TD-023 and
   TD-031.
4. `docs/tech-debt-register.md` TD-023 and TD-031 rows could be annotated to
   cross-reference ADR-0002 as the precedent that formalizes (without
   excusing) the same pattern they represent informally — left as a
   follow-up, not performed as part of authoring this ADR.

## Validation Criteria
- `EnemyManager.Instance` is non-null immediately after the dungeon scene's
  Awake phase completes (once per dungeon run).
- Duplicate-instance guard test: instantiate/trigger a second `EnemyManager`
  in the same scene and confirm the second is destroyed **and** does not
  overwrite `Instance` nor re-run initialization (the exact
  `MazeController`/TD-026 regression this ADR must not repeat).
- **Per-room reset test (new — from engine-specialist validation)**: load
  Room A, drive its lifecycle to `Cleared` with `aliveCount == 0`, then
  transition to Room B via the existing Tilemap-swap flow (`ON_LOAD_MAP`) —
  confirm `EnemyManager`'s `aliveCount` and lifecycle state are reset for
  Room B, not carried over from Room A's `Cleared` state.
- AC-L1…L6 (`enemy-spawn-system.md` PlayMode Acceptance Criteria) pass using
  `EnemyManager.Instance` as the harness's access point.
- No `DontDestroyOnLoad` call present on `EnemyManager` (code-review/grep
  check).
- This ADR's Status is explicitly revisited (not necessarily changed) at the
  Sprint 6 code-review gate for `EnemyManager`, and again if/when TD-023 or
  TD-031 are scheduled for repayment.

## Related Decisions
- ADR-0001 (StatSystem dual data structure) — unrelated domain, no direct
  dependency.
- design/gdd/enemy-spawn-system.md — Open Question #1; States/Transitions
  table.
- docs/tech-debt-register.md — TD-023 (`LevelManager` singleton, same
  violation, no ADR), TD-031 (`ObjectPooling` fake singleton, same violation,
  no ADR), TD-026 (`MazeController` Awake() duplicate-guard bug — cautionary
  precedent this ADR explicitly avoids repeating).
- Assets/Script/Map/Maze/MazeController.cs — reference implementation this
  decision extends to a second instance.
- .claude/rules/engine-code.md, .claude/rules/manager-event-code.md — rules
  this ADR formally excepts `EnemyManager` from.
