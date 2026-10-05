# Map — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-10-04 / 10-05 — New room set; no-spawn rooms open at once
- **Commits:** `213fa5a6`, `8b778fd5`, `f9262495`, `d09671fd`, `d3400ee7`, `40d2c793`, `ac13ee4f`, `a8820666`
- **Changed:** `Assets/Data/Json/Room/`, `Maze_Storage.asset`, `RoomGeneraterController.LoadRoom()`
- **From → To:**
  - 13 rooms `NormalRoom_0` … `NormalRoom_12` → 20 named rooms (Start, 15 Combat, Buff, Rest, Shop, Boss)
  - An uncleared room with no spawn markers waited forever for `ON_CLEAR_ENEMY` → builds the grid and deletes door tiles immediately
- **Why:** a full start-to-boss run; "nothing will ever emit ON_CLEAR_ENEMY" for start/rest/shop/buff rooms (code comment).
- **Bugs:** none closed; Bug #16 (start/end by list position) now matters more — `Maze_Storage` order decides Start and Boss.

## 2026-10-02 — LevelManager no longer a singleton
- **Commit:** `0bc36406`
- **Changed:** `LevelManager`, `RoomGeneraterController`, `GameLifetimeScope`
- **From → To:** `LevelManager.Instance` (bare static) → registered in `GameLifetimeScope`, injected via `Construct(IPlayerService, LevelManager)`
- **Why:** "no new singletons" rule (TD-023).
- **Bugs:** closes Bug #12 / TD-023.

## 2026-09-30 … 10-05 — Start-room teleport; minimap fixes
- **Commits:** `9154763f` (teleport), `0bc36406` (minimap listener), `a8820666` (minimap start cell)
- **Changed:** `RoomGridController`, `MapGridController`
- **From → To:**
  - Teleport line commented out → `OnDoneLoadRoomGrid()` calls `PlayerManager.SetPlayerPosition(start room)`
  - `MapGridController` `ON_PLAYER_ON_DOOR` listener commented out → registered (`0bc36406`)
  - Minimap start cell `(Row, Column)` → `(Column, Row)` (`a8820666`)
- **Why:** player now spawned at runtime and must be placed; minimap avatar did not follow door moves (inferred).
- **Bugs:** closes Bug #13.

## 2026-06-04 — RoomGridController replaces Room/MainMapController
- **Commits:** around `06953aec`, `8fdda9be`
- **Changed:** map controllers
- **From → To:** `RoomMapController` / `MainMapController` → `RoomGridController` / `MapGridController`
- **Why:** grid-based rewrite.
- **Bugs:** Bugs #1-3 superseded.
