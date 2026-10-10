# Changelog — `enemy-spawn-system.md`

Change log for [`design/gdd/enemy-spawn-system.md`](../enemy-spawn-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-10 — Event rename banner (doc-sync --auto)
- **Commits:** `221d54be` "logic open door at start room", merged into `sprint-18` by `42a81260` "update flow" (`c6cbc57a`)
- **Changed:** Re-synced banner
- **From → To:** body names `ON_CLEAR_ENEMY` → banner says read it as `ON_OPEN_DOOR`. Body left as written (Approved GDD).
- **Why:** enum value renamed in code.
- **Still ⚠️ Out of date:** `RarityTier` → code `RarityTierEnemy` (BUG-098), carried from 2026-10-09.

## 2026-10-09 — ⚠️ Out of date against code (found by doc-sync, not yet fixed)
- **Commit:** branch `pm/doc-sync-2026-10-09` (base `sprint-17` `cdf68555`)
- **Stale term:** `RarityTier`. The code enum is `RarityTierEnemy` (`RoomModel.cs:110`, `853fe39`). The 2026-09-11 banner (`:34`) says the `RarityTier` rules "still match".
- **Why not fixed:** this is an Approved GDD and the drift is open bug **BUG-098**. It is left for the owner or the bug fix.

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+19 / −1 lines)
- **Sections touched:** “Enemy Spawn & Per-Room Management System”, “Current Implementation (2026-07-13) — HISTORICAL, …”
- **From → To:** (nothing) → `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** The selection algorithm, budget model and ``
- **From → To:** `` 1. **`EnemySpawner.cs`** (`Assets/Script/Enemy/`) — **no longer an empty stub.** `OnEnable`/`OnDisable` `` → `` 1. **`EnemySpawner.cs`** (`Assets/Script/System/Enemy/`) — **no longer an empty stub.** `OnEnable`/`OnDisable` ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-22 — chore: park the Skill Enhance ability framework under prototypes/
- **Commit:** `eda31a1e` (+1 / −1 lines)
- **Sections touched:** “What changed since 2026-07-13”
- **From → To:** `` | `EventID` has 6 values; still no `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` | **18 values.** Both exist, plus `ON_DONE_SPAWN_ENEMY`, … `` → `` | `EventID` has 6 values; still no `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` | **20 values.** Both exist, plus `ON_DONE_SPAWN_ENEMY`, … ``
- **Why:** commit message: “chore: park the Skill Enhance ability framework under prototypes/”

## 2026-08-21 — docs: correct my own EventID count - the enum has 18 values, not 19
- **Commit:** `01459440` (+1 / −1 lines)
- **Sections touched:** “What changed since 2026-07-13”
- **From → To:** `` | `EventID` has 6 values; still no `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` | **19 values.** Both exist, plus `ON_DONE_SPAWN_ENEMY`, … `` → `` | `EventID` has 6 values; still no `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` | **18 values.** Both exist, plus `ON_DONE_SPAWN_ENEMY`, … ``
- **Why:** commit message: “docs: correct my own EventID count - the enum has 18 values, not 19”

## 2026-08-21 — docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic
- **Commit:** `66b7604c` (+24 / −7 lines)
- **Sections touched:** “New findings from the 2026-08-20 audit”
- **From → To:** `` (`RoomModel.cs:55-58`) does `candidateEnemies.AddRange(enemiesOfRoom)` with **no weight `` → `` (`RoomModel.cs:55-58`) does `candidateEnemies.AddRange(enemiesOfRoom)` with **no weight filter ``
- **Why:** commit message: “docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic”

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+56 / −2 lines)
- **Sections touched:** “Enemy Spawn & Per-Room Management System”, “Current Implementation — re-synced 2026-08-20 …”
- **From → To:** (nothing) → `` revised: 2026-08-20 (documentation audit — third reverse-sync. The 2026-07-13 "Current ``
- **From → To:** `` not converged on it** (see Doc-sync note + Current Implementation) `` → `` not converged on it** (see Doc-sync note + Current Implementation). Implementation section ``
- **From → To:** `` ## Current Implementation (2026-07-13) — authoritative for "what is built" `` → `` ## Current Implementation — re-synced 2026-08-20 (authoritative for "what is built") ``
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-07-23 — fix(sprint-06): close S6-01/02/03/04 bugs, resolve S6-09 data-model decision
- **Commit:** `d3b29d9d` (+35 / −0 lines)
- **Sections touched:** “Data Model”, “Current Implementation (as of 2026-07-23 — accepted shape, …”
- **From → To:** (nothing) → `` > **[SUPERSEDED 2026-07-23 — S6-09/S6-D2 decision, see ADR-0003 "Amendment"]** The SO-extending-`EntityModel` ``
- **From → To:** (nothing) → `` #### Current Implementation (as of 2026-07-23 — accepted shape, ADR-0003 Accepted against this) ``
- **Why:** commit message: “fix(sprint-06): close S6-01/02/03/04 bugs, resolve S6-09 data-model decision”

## 2026-07-13 — docs(gdd): resolve remaining S5-A1 open questions (Q#2 RNG, Q#4 room mapping, RoomType role)
- **Commit:** `4fc04c54` (+19 / −7 lines)
- **Sections touched:** “Room → `RoomModel` Resolution (Open Q#4 — **RESOLVED …”, “Open Questions”
- **From → To:** (nothing) → `` revised: 2026-07-13 (same session, continued — resolved the remaining S5-A1 open-Qs: Q#4 room→preset ``
- **From → To:** `` ### Room → `RoomModel` Resolution (Open Q#4 — **REOPENED 2026-07-13**, was marked resolved in error) `` → `` ### Room → `RoomModel` Resolution (Open Q#4 — **RESOLVED 2026-07-13**) ``
- **From → To:** `` **Not evaluated yet:** whether the shuffle-bag is an intentional simplification worth keeping (it is `` → `` **Decision (2026-07-13):** the shuffle-bag is kept as an intentional simplification, not a regression ``
- … 2 more hunks — `git show 4fc04c54 -- design/gdd/enemy-spawn-system.md`
- **Why:** commit message: “docs(gdd): resolve remaining S5-A1 open questions (Q#2 RNG, Q#4 room mapping, RoomType role)”

## 2026-07-13 — docs(gdd): lock Option C spec for enemy-spawn-system (Room Budget + Candidate Pool + RarityTier)
- **Commit:** `0d7a5b7c` (+171 / −4 lines)
- **Sections touched:** “Open Questions”, “Option C — Room Budget + Candidate Pool + Spawn Chance …”
- **From → To:** (nothing) → `` revised: 2026-07-13 (Option C locked — owner session resolved Open Q#8: Room Budget + Candidate Pool ``
- **From → To:** `` | 8 | **[NEW 2026-07-13]** Which selection-algorithm direction do we commit to: harden the current uniform-random … `` → `` | 8 | Which selection-algorithm direction do we commit to: harden the current uniform-random `RoomModel.GetSpawnSet()`, revive the … ``
- **From → To:** `` B is, which lowers migration cost. This does not resolve Open Q#8 — it needs explicit owner sign-off `` → `` B is, which lowers migration cost. ``
- **Why:** commit message: “docs(gdd): lock Option C spec for enemy-spawn-system (Room Budget + Candidate Pool + RarityTier)”

## 2026-07-11 — docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05
- **Commit:** `e600eb24` (+513 / −352 lines)
- **Sections touched:** “Enemy Spawn & Per-Room Management System”, “Current Implementation (2026-07-13) — authoritative for …”, “Overview”, “Player Fantasy”, “Core Data Model”, “`RoomModel.GetSpawnSet()` — the actual selection method” …
- **From → To:** `` revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData; `` → `` revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData; ``
- **From → To:** (nothing) → `` revised: 2026-07-13 (second reverse-sync — code diverged further from the 2026-07-08 target rather ``
- **From → To:** `` **Status**: Approved (design) · Prototype partial (see Current Implementation Status) `` → `` **Status**: Approved (design) · Prototype partial — **code has diverged from the 2026-07-08 target, ``
- … 38 more hunks — `git show e600eb24 -- design/gdd/enemy-spawn-system.md`
- **Why:** commit message: “docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05”

## 2026-07-09 — fix doc
- **Commit:** `65c8228e` (+10 / −2 lines)
- **Sections touched:** “Current Implementation Status (2026-07-09) — authoritative …”
- **From → To:** `` ⚠️ There is **no `EnemyManager`, no room-combat lifecycle, no door lock, no alive-count, and no `` → `` ⚠️ Spawning is still a manual editor action, not the `ON_LOAD_MAP`-driven runtime flow. As of ``
- **Why:** commit message: “fix doc”

## 2026-07-09 — fix conflic doc
- **Commit:** `3b64fca0` (+3 / −7 lines)
- **Sections touched:** “Open Questions”
- **From → To:** `` <<<<<<< HEAD `` → `` revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData; ``
- **From → To:** `` >>>>>>> origin/claude/enemy-spawn-manager-review-7aq2wa `` → (removed)
- **From → To:** `` <<<<<<< HEAD `` → (removed)
- … 2 more hunks — `git show 3b64fca0 -- design/gdd/enemy-spawn-system.md`
- **Why:** commit message: “fix conflic doc”

## 2026-07-09 — docs(enemy-spawn): reverse-sync GDD to prototype code + propagate
- **Commit:** `950ae319` (+95 / −7 lines)
- **Sections touched:** “Enemy Spawn & Per-Room Management System”, “Core Data Model”, “`EnemyDatabase.GetHybridEnemySet(List<int> idEnemy, int …”, “States and Transitions (per-room combat lifecycle) — …”, “Acceptance Criteria”, “Future Enhancements (Should-Have / Post-Demo)”
- **From → To:** (nothing) → `` revised: 2026-07-09 (reverse-synced to prototype code — Assets/Script/Database-SO/Modal + LevelManager.SpawnRoomEnemies) ``
- **From → To:** `` **Status**: In Design `` → `` **Status**: Approved (design) · Prototype partial (see Current Implementation Status) ``
- **From → To:** `` > (4 ScriptableObjects + `GetHybridEnemySet` + `EnemyManager`) and **supersedes** the earlier `` → `` > (ScriptableObjects + `GetHybridEnemySet`/`GetSpawnSet` selection + a runtime driver) and ``
- … 6 more hunks — `git show 950ae319 -- design/gdd/enemy-spawn-system.md`
- **Why:** commit message: “docs(enemy-spawn): reverse-sync GDD to prototype code + propagate”

## 2026-07-09 — docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002
- **Commit:** `3dc2394d` (+1 / −1 lines)
- **Sections touched:** “Open Questions”
- **From → To:** `` | 1 | **`EnemyManager` singleton violates "no new singletons"** (only `MazeController` permitted). Owner chose singleton (2026-07-08). | ⬜ … `` → `` | 1 | **`EnemyManager` singleton violates "no new singletons"** (only `MazeController` permitted). Owner chose singleton (2026-07-08). | ✅ … ``
- **Why:** commit message: “docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002”

## 2026-07-09 — docs(architecture): ratify EnemyManager singleton, resolve enemy-spawn seed source
- **Commit:** `8a7b481a` (+35 / −24 lines)
- **Sections touched:** “`EnemyDatabase.GetHybridEnemySet(List<int> idEnemy, int …”, “Room → `RoomData` Resolution (Open Q#3/#4 — RESOLVED …”, “Dependencies”, “Open Questions”
- **From → To:** `` revised: 2026-07-08 (post /design-review — 4 specialist passes; owner decisions logged) `` → `` revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData; prior: 2026-07-08 post /design-review — 4 … ``
- **From → To:** `` > ⚠️ **Runtime seed source — OPEN, decide in Sprint 4** (see Open Questions #2). The overload's RNG `` → `` > ✅ **Runtime seed source — RESOLVED 2026-07-09** (see Open Questions #2). Per-room-from-run-seed, ``
- **From → To:** `` ### Room → `RoomData` Resolution (Open Q#3 — RESOLVED 2026-07-08) `` → `` ### Room → `RoomData` Resolution (Open Q#3/#4 — RESOLVED 2026-07-08, mechanism updated 2026-07-09) ``
- … 7 more hunks — `git show 8a7b481a -- design/gdd/enemy-spawn-system.md`
- **Why:** commit message: “docs(architecture): ratify EnemyManager singleton, resolve enemy-spawn seed source”

## 2026-07-08 — plan enemy spawn
- **Commit:** `ddbd54d7` (+470 / −0 lines)
- **Changed (from → to):** document created (+470 lines)
- **Why:** commit message: “plan enemy spawn”
