# Event System — Changelog

Newest first. Entries reconstructed from the count history in `CLAUDE.md` and `git log`.

## 2026-10-10 — ON_CLEAR_ENEMY renamed ON_OPEN_DOOR (still 24 values)
- **Commits:** `221d54be` "logic open door at start room", merged into `sprint-18` by `42a81260` "update flow" (`c6cbc57a`)
- **Changed:** `EventManager.cs:46`
- **From → To:** `ON_CLEAR_ENEMY` → `ON_OPEN_DOOR`, same slot. Producers: `RoomCell` (0 alive), `ChampitionController` (Champion confirmed), `LevelManagerEditor` "Clear Enemy" button. Consumer: `RoomGridController.DeleteDoorTileMap`.
- **Why:** not recorded in the commit; the new name describes the effect (inferred).
- **Bugs:** none

## 2026-10-05 — ON_PLAYER_READY (23 → 24 values)
- **Commit:** `cb0de496`
- **Changed:** `EventID`
- **From → To:** 23 values → 24 (`ON_PLAYER_READY`, emitted by `VitalStatsComponent.Reborn()` with the player's `ICharacter` root)
- **Why:** the UIFlow HUD needs to find the runtime-spawned player without `FindObjectOfType` (commit message and source comment)
- **Bugs:** none

## 2026-09-29 — First ON_PLAYER_DEATH subscriber; parallel UI bus
- **Commit:** `0c38633d`
- **Changed:** no change to `EventManager.cs`; new consumers
- **From → To:** `ON_PLAYER_DEATH` 0 subscribers → 1 (`UIFlow.RealPlayerDataProvider`, de-duplicated by a flag); new static `UIFlow.UIEvents` bus outside `EventID`
- **Why:** UIFlow game-over screen.
- **Bugs:** BUG-087 → PARTIAL.

## 2026-08-22 … 09-07 — 20 → 23 values
- **Commits:** StatPointAllocator and Item system work (`853fe39b`, `9f1258cd`)
- **Changed:** `EventID`
- **From → To:** 20 values → 23 (`ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`)
- **Why:** stat allocation session reset; item drop / pickup.
- **Bugs:** none.

## 2026-08-22 — 18 → 20 values
- **Commit:** StatsScreen UI work (`5e5d1c5e` era)
- **Changed:** `EventID`
- **From → To:** 18 → 20 (`ON_REVERT_STATS_BY_UI`, `ON_RESTORE_STATS_BY_UI`)
- **Why:** stats screen revert / restore.
- **Bugs:** none. (The 2026-08-20 audit's "19" was a miscount of 18.)
