# Changelog — `map-system.md`

Change log for [`design/gdd/map-system.md`](../map-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-10 — Event rename banner (doc-sync --auto)
- **Commits:** `221d54be` "logic open door at start room", merged into `sprint-18` by `42a81260` "update flow" (`c6cbc57a`)
- **Changed:** Re-synced banner; front-matter `updated`
- **From → To:** body names `ON_CLEAR_ENEMY` → banner says read it as `ON_OPEN_DOOR` and lists the new producers (`RoomCell`, `ChampitionController`, editor button). Body left as written — most mentions are dated or struck-through history.
- **Why:** enum value renamed in code.
- **Bugs:** BUG-096

## 2026-10-09 — Doc-truth correction (doc-sync --auto)
- **Commit:** branch `pm/doc-sync-2026-10-09` (base `sprint-17` `cdf68555`)
- **Changed:** two claims from the 2026-10-05 re-sync, found false by the claim sweep `production/qa/doc-truth-2026-10-09.md`
  - **From → To:** banner `:22` "Rooms without spawn markers open at once (`40d2c793`)" → the fix was reverted by `ac13ee4f`; the rooms stay sealed (BUG-096)
  - **From → To:** edge-case row "✅ Doors open on load" → "❌ Doors stay sealed" (BUG-096)
  - **From → To:** acceptance criterion `[x] Rooms without spawn markers open their doors on load` → `[ ]` with a reason
  - **From → To:** `[x] Start room = index 0 template …` → rule still ticked, with a note that the alphabetical `Maze_Storage.asset` puts the Boss room at the start (BUG-097)
  - **From → To:** `RoomCell.IsCleared` was written only in `OnEnemyDeath()` → it is now written in `OpenDoors()`, and `DeleteDoorTileMap()` returns early when the room is cleared (`5d1986db`, 2026-10-06)
- **Why:** BUG-099. The 2026-10-05 sync took `40d2c793` from the commit log without checking that the change was still in the code.
- **Bugs:** BUG-096, BUG-097, BUG-099

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** `:113` lists `NormalRoom_0 … NormalRoom_12` (13 rooms) — replaced by 20 named rooms on 2026-10-04 (`d3400ee7`)
  - **From → To:** Bug #13 (`:208-210`, `:353`, `:414`) still described open — fixed in `9154763f` (2026-09-30): `OnDoneLoadRoomGrid()` → `PlayerManager.SetPlayerPosition()`
  - **From → To:** Rooms without spawn markers now open their doors immediately (`40d2c793`) — not described
  - **From → To:** Door transition step `fastMovement.position = …` → `RoomGeneraterController.SetNextRoom()` → `IPlayerService.SetPlayerPosition()`; `LevelManager` singleton → injected (Bug #12)
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+11 / −0 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** **Changed:** verification banners. `map-system.md` and `animation-system.md` had **no drift** — all
- **Sections touched:** “Map & Dungeon System Design”
- **From → To:** (nothing) → `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** No drift found — the maze generation, room ``

## 2026-08-22 — chore: park the Skill Enhance ability framework under prototypes/
- **Commit:** `eda31a1e` (+1 / −1 lines)
- **Sections touched:** “Dependencies”
- **From → To:** `` | **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY`, `ON_ENEMY_DEATH`, `ON_DONE_SPAWN_ENEMY`, … `` → `` | **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY`, `ON_ENEMY_DEATH`, `ON_DONE_SPAWN_ENEMY`, … ``
- **Why:** commit message: “chore: park the Skill Enhance ability framework under prototypes/”

## 2026-08-21 — docs: correct my own EventID count - the enum has 18 values, not 19
- **Commit:** `01459440` (+1 / −1 lines)
- **Sections touched:** “Dependencies”
- **From → To:** `` | **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY`, `ON_ENEMY_DEATH`, `ON_DONE_SPAWN_ENEMY`, … `` → `` | **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY`, `ON_ENEMY_DEATH`, `ON_DONE_SPAWN_ENEMY`, … ``
- **Why:** commit message: “docs: correct my own EventID count - the enum has 18 values, not 19”

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+33 / −19 lines)
- **Sections touched:** “Map & Dungeon System Design”, “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …”, “Edge Cases”, “Dependencies”, “Room-Clear Locking **[PARTIAL — spawn owned by …”
- **From → To:** `` **Status**: In Design `` → `` **Status**: In Design (implementation-status rows corrected 2026-08-20) ``
- **From → To:** `` 2. Enemy count is not tracked — nothing knows when to emit `ON_CLEAR_ENEMY`. `` → `` 2. ~~Enemy count is not tracked — nothing knows when to emit `ON_CLEAR_ENEMY`.~~ ✅ **RESOLVED 2026-08-20** — `RoomCell.EnemyCount` is the … ``
- **From → To:** `` 4. The ONLY producer of `ON_CLEAR_ENEMY` is the editor debug button `` → `` 4. ~~The ONLY producer of `ON_CLEAR_ENEMY` is the editor debug button~~ ✅ **RESOLVED** — the producer is now `RoomCell.OnEnemyDeath()` at … ``
- … 8 more hunks — `git show e1cd01f4 -- design/gdd/map-system.md`
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-07-11 — docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05
- **Commit:** `e600eb24` (+29 / −19 lines)
- **Sections touched:** “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …”
- **From → To:** `` > approved **Enemy Spawn & Per-Room Management** GDD (`design/gdd/enemy-spawn-system.md`), which `` → `` > **Enemy Spawn & Per-Room Management** GDD (`design/gdd/enemy-spawn-system.md`), which owns spawn ``
- **From → To:** (nothing) → `` > `RoomCell` has `CloseDoor()`/`OpenDoors()` but nothing in the spawn path calls them yet. ``
- **From → To:** `` > **As-built today (2026-07-09, prototype):** only the data model + selection exists — `` → `` > Map-side contract this system still owns: `RoomCell.CloseDoor()` / `OpenDoors()` / `IsCleared` ``
- **Why:** commit message: “docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05”

## 2026-07-09 — docs(enemy-spawn): reverse-sync GDD to prototype code + propagate
- **Commit:** `950ae319` (+6 / −0 lines)
- **Sections touched:** “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …”
- **From → To:** (nothing) → `` > **As-built today (2026-07-09, prototype):** only the data model + selection exists — ``
- **Why:** commit message: “docs(enemy-spawn): reverse-sync GDD to prototype code + propagate”

## 2026-07-09 — docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002
- **Commit:** `3dc2394d` (+28 / −20 lines)
- **Sections touched:** “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …”, “Dependencies”, “Room-Clear Locking **[PARTIAL — spawn owned by …”
- **From → To:** `` updated: 2026-07-02 `` → `` updated: 2026-07-09 ``
- **From → To:** `` **Agreed spawn architecture (2026-07-02) [PLANNED]:** `` → `` **Spawn architecture — [SUPERSEDED 2026-07-08 → see `design/gdd/enemy-spawn-system.md`]:** ``
- **From → To:** `` | **Enemy AI** | `EntityDeathState` must emit `ON_ENEMY_DEATH`; planned `RoomEnemySpawner` tracks the count and emits `ON_CLEAR_ENEMY` when … `` → `` | **Enemy AI** | `EntityDeathState` must emit `ON_ENEMY_DEATH`; `EnemyManager` (see `enemy-spawn-system.md`) tracks the count and emits … ``
- … 2 more hunks — `git show 3dc2394d -- design/gdd/map-system.md`
- **Why:** commit message: “docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002”

## 2026-07-02 — docs(gdd): update map-system.md to verified code state + design decisions
- **Commit:** `50d1a283` (+131 / −69 lines)
- **Sections touched:** “Overview”, “Dungeon Generation”, “Random Room Assignment”, “Door Tile Resolution (LoadRoom)”, “Room Transition”, “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …” …
- **From → To:** `` updated: 2026-06-04 `` → `` updated: 2026-07-02 ``
- **From → To:** `` Two parallel grids run simultaneously: a **world grid** (`RoomGridController`) that places `` → `` Two parallel grids run simultaneously: a **world grid** — `RoomGridController` (event hub, ``
- **From → To:** `` | 0 | `DISABLE` | Hướng này không có door trong maze | Off | `` → `` | 0 | `DISABLE` | No passage in this direction (maze wall) | Off | ``
- … 25 more hunks — `git show 50d1a283 -- design/gdd/map-system.md`
- **Why:** commit message: “docs(gdd): update map-system.md to verified code state + design decisions”

## 2026-06-04 — update document GDD map system
- **Commit:** `3c87f5af` (+156 / −90 lines)
- **Sections touched:** “Dungeon Generation”, “Room Placement”, “Random Room Assignment”, “Door Tile Resolution (LoadRoom)”, “Room Transition”, “Room-Clear Locking **[PARTIAL — event wired, lock-on-entry …” …
- **From → To:** (nothing) → `` updated: 2026-06-04 ``
- **From → To:** `` 2. Start at cell (0, 0); mark visited; push to stack `` → `` 2. Pick a **random** start cell (not (0,0)); mark visited; push to stack. Store as `MazeGenerator.Start`. ``
- **From → To:** `` - Else: pick one at random; carve passage between them; push neighbour to stack `` → `` - Else: pick one at random; carve passage between them; push neighbour; track as `MazeGenerator.End` ``
- … 30 more hunks — `git show 3c87f5af -- design/gdd/map-system.md`
- **Why:** commit message: “update document GDD map system”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+260 / −0 lines)
- **Changed (from → to):** document created (+260 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”
