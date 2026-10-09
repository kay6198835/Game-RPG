# Map — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-10-09 — Correction: no-spawn rooms do not open; IsCleared moved into OpenDoors()
- **Commits:** `ac13ee4f` (reverted `40d2c793`), `5d1986db` (2026-10-06, "shop coding")
- **Changed:** `RoomGeneraterController.LoadRoom()`, `RoomGeneraterController.DeleteDoorTileMap()`, `RoomCell`
- **From → To:**
  - The 2026-10-05 entry below says marker-less rooms build their grid and delete door tiles on load → **false at HEAD.** `ac13ee4f` removed the zero-spawn branch, so those rooms wait forever for `ON_CLEAR_ENEMY` (BUG-096). README flow corrected.
  - `RoomCell.IsCleared` was set in `OnEnemyDeath()` at zero alive → it is now set in `OpenDoors()`. `DeleteDoorTileMap()` returns early when `IsCleared` is already true.
- **Why:** the `5d1986db` message ("shop coding") gives no reason, and the code carries no comment. The 2026-10-05 doc entry was written from the commit log without checking that the fix was still in the code (BUG-099).
- **Bugs:** BUG-096, BUG-097, BUG-099

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
