# Cross-GDD Review Report — Enemy Spawn Focus

> Date: 2026-07-09
> Scope: Enemy Spawn & Per-Room Management and its related docs, after the
> architecture-review sync (map-system propagation + ADR-0002) and the
> doc-follows-code reverse-sync of the spawn prototype.
> Mode: consistency (docs↔docs) + docs↔code, enemy-spawn-focused.

## Documents Reviewed

- `design/gdd/enemy-spawn-system.md` (revised 2026-07-09 — reverse-synced to code)
- `design/gdd/map-system.md` (propagated 2026-07-09)
- `design/gdd/systems-index.md`
- `docs/architecture/adr-0001-statsystem-dual-data-structure.md`
- `docs/architecture/adr-0002-enemymanager-singleton-exception.md` (new)
- `production/epics/enemy-spawn/EPIC.md`
- Code: `Assets/Script/Database-SO/Modal/{EntityModel,EnemyModal,MapModel,RoomModel}.cs`,
  `Assets/Script/LevelEdit/LevelManager.cs` (`SpawnRoomEnemies`), `Assets/Editor/LevelManagerEditor.cs`,
  `Assets/Script/Manager/EventManager.cs` (EventID enum), `Assets/Script/Utility/GameConstants.cs`

---

## Part 1 — Decisions Synced This Session (resolved)

| Conflict | Decision (owner) | Sync action taken |
|----------|------------------|-------------------|
| C1 — `EnemyManager` singleton vs "no new singletons" | Keep singleton | Added **ADR-0002** (Proposed); GDD Open Q#1 → RESOLVED; epic Governing ADRs / Blocking Deps / Next Step updated; rule wording (`manager-event-code.md`, `CLAUDE.md`) now lists `MazeController` **and** `EnemyManager` |
| C2 — `map-system.md` still described superseded `EncounterSO` + `RoomEnemySpawner` | Propagate | `map-system.md` spawn block marked SUPERSEDED and repointed to `enemy-spawn-system.md`; Dependencies + Room-Clear checklist updated |
| A — GDD/prototype naming & behaviour divergence | **Doc follows code** | GDD reverse-documented: new "Current Implementation Status (2026-07-09)" section; Core Data Model / algorithm / lifecycle / ACs tagged `[PLANNED]` where they exceed the code; "Prototype Deviations" table (D1–D8) added |

Residual stale references cleaned: `memory/project_state.md`, `docs/tech-debt-register.md`
(TD-030 reworded, TD-031 added), `CLAUDE.md` (item #15 + singleton rule), `sprint-04-daily-plan.md`
(reconcile item + stale-header item), GDD header `In Design` → `Approved`.

---

## Part 2 — Docs↔Code Divergence (the substantive finding)

A prototype (commit `a420d5e`) implements only the **data model + weight-budget selection**, and
diverges from the approved GDD/ADR. It is now documented as-built; the gaps below remain open for
owner decision (harden code to design, or amend design). Tracked as **TD-031** and GDD "Prototype
Deviations".

| # | Deviation | As-built | Planned (GDD/ADR) | Acceptance criteria at risk |
|---|-----------|----------|-------------------|-----------------------------|
| D1 | RNG source | `UnityEngine.Random` (static) | injected `System.Random` | AC-A3, AC-A4 (untestable determinism) |
| D2 | Phase-2 fill | random pick among eligible | deterministic `argmin \|weight−remaining\|` + tie-break | AC-A4; "optimal fill" design intent |
| D3 | Enemy reference model | `RoomModel.enemiesOfRoom` direct `EnemyModal` refs | `id`-based via `EnemyDatabase.GetByID` | AC-D1…D3 (no central store / no dup/zero-id validation) |
| D4 | `weight ≥ 1` enforcement | runtime `> 0` guard only | `[Range(1,99)]` + `OnValidate` clamp | AC-A6 (author-time invariant) |
| D5 | `MapModel` role | `mapName` + `idRooms` + `totalWeight` | `mapName` + `idEnemy` (map enemy set) | map-scoped room pool contract |
| D6 | `id` generation | `Math.Abs(Guid.GetHashCode())`, no reroll, no public `EnsureId()` | reroll-until-nonzero + public test hook | AC-D2 + in-memory-SO test path |
| D7 | Runtime driver | `LevelManager.SpawnRoomEnemies()` Editor button, random positions | `EnemyManager` event-driven lifecycle (ADR-0002) | AC-L1…L6, AC-P1…P3 |
| D8 | Class naming / location | `EntityModel`/`EnemyModal`/`MapModel`/`RoomModel` in `Database-SO/Modal/` (typo "Modal"); in production `Script/`, not `prototypes/` | `EnemyData`/`EnemyDatabase`/`MapEnemyDatabase`/`RoomData` | naming drift; `prototype-code.md` isolation rule |

### Docs↔code items that MATCH (not issues)
- `EventID` enum = `ON_PLAYER_ON_DOOR, ON_LOAD_MAZE_DONE, ON_LOAD_MAP, ON_CLEAR_ENEMY, ON_TEST` —
  `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` correctly documented as new/needed.
- `GameConstants.TileName.SPAWN = "Tile_Spawn"` exists, unused — matches GDD.
- `EnemyManager` and the lifecycle correctly documented as PLANNED; Bugs #7/#8/#16 accurately open.

---

## Part 3 — Other Consistency Issues

| Issue | Location | Status |
|-------|----------|--------|
| Orphaned SO asset — `RoomData.asset` binds a script GUID (`0c92c27b…`) with no matching `.cs`; carries a `nameEnity` typo field; leftover from a pre-rename `RoomData` class | `Assets/SO/Room/RoomData.asset` | Open — recommend delete or rebind to `RoomModel` |
| ADR-0001 is `Proposed`, used by the epic as a boundary contract | `docs/architecture/adr-0001-*.md` | Minor — ratify when convenient |
| Engine version string drift `2022.3.62f1` (VERSION.md/ADRs) vs `2022.3.62f3` (CLAUDE.md/technical-preferences) | project-wide | Pre-existing — reconcile once |

---

## Verdict: CONCERNS

The two decided conflicts are synced and the GDD now truthfully reflects the code. No blocking
game-design contradictions remain. The open items are engineering-quality gaps in the prototype
(D1–D8) and one broken asset — none block documentation, but D1/D2/D3/D7 must be resolved (code
hardened or design amended) before the enemy-spawn epic's Logic/Integration acceptance criteria
can pass.

### Recommended next actions
1. Owner decision on each of D1–D8: harden code to the GDD, or amend the GDD to accept the
   prototype's simpler approach. (D1/D2/D3 are the load-bearing ones for testability.)
2. Isolate or promote the prototype: move `Assets/Script/Database-SO/**` under `prototypes/` with
   a README, or accept it as an early Sprint-5 start and schedule the hardening.
3. Delete/rebind the orphaned `RoomData.asset`.
4. Run `/architecture-review` in a fresh session to re-validate coverage now that ADR-0002 exists.
