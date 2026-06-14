# Playtest Report

## Session Info
- **Date**: 2026-06-12
- **Build**: 220c7cf
- **Duration**: 15 min (code-level verification — no live Unity Play Mode session this week)
- **Tester**: Kiet
- **Platform**: PC — Unity Editor
- **Input Method**: KB+M
- **Session Type**: Weekly wrap-up — verification pass (code review based, no Play Mode run logged)

## Test Focus
End-of-week verification of work landed this week: minimap (minimap) refactor (`MapGridController`, `MapCell`), DOTween (dotween) integration for minimap avatar movement, and `RoomGeneraterController`/`RoomCell` door-data restructure (`CurentDoorLevelData` → `IndexLevelDataDoor`). Sprint 1 combat/death/AI blockers (S1-01 → S1-04) were **not** touched this week.

## First Impressions (First 5 minutes)
- **Understood the goal?** Yes — minimap visited/avatar-tween work is a polish/UX (trải nghiệm người dùng) addition layered on top of last week's navigation skeleton.
- **Understood the controls?** N/A — no live session run
- **Emotional response**: Concerned — work this week stayed in Map/Minimap territory, which Sprint 1 risk register explicitly flagged as a "freeze" zone (rủi ro/risk: Map work destabilizing combat testing).
- **Notes**: No Play Mode walkthrough was recorded for this build. Findings below are from static code review (đọc code tĩnh) of the diff only.

## Gameplay Flow

### What worked well
- `MapGridController` now wires real `OnLoadMap`/`Move` handlers (previously fully commented out — Bug #11 superseded for the avatar-tween path) using DOTween (`DOScale`, `DOMove`) for minimap feedback (phản hồi trực quan).
- `MapCell.VisitRoom()` adds a clean visited-state toggle (`IsVisited`) with idempotent guard (`if (IsVisited) return`).
- `RoomGeneraterController` now skips `null` tilemap entries (`if (tilemap == null) continue`) — guards against a previous null-deref risk.
- `IndexLevelDataDoor` (List<int>) is a simpler data shape than the previous duplicated `CurentDoorLevelData` (LevelData) — less data duplication.

### Pain points
- **[High — carryover]** `WeaponMelee.Attack()` foreach body still empty (Bug #4 / S1-01) — combat still deals no damage. Unchanged for 4 consecutive weekly reviews.
- **[High — carryover]** `AnimationPlayerController` double `StartAnimation` registration still present (Bug #9 / S1-02) — `EndAnimation` never fires, combat states still get stuck.
- **[High — carryover]** Player death (Bug #6 / S1-03) and Enemy death chain (Bugs #5/#7/#8 / S1-04) still open — no commits this week touched `Core.TakeDamage`, `EntityDeathState`, or `EntityBasicState`.
- **[Medium — new]** `RoomGeneraterController.DeleteDoorTileMap()` now reads `this.Data` / `this.IndexLevelDataDoor` (instance fields) for the room being cleared, but `ClearRoom()` already calls `this.Data.Clear()` and `this.IndexLevelDataDoor.Clear()` at the end of its own execution. If `DeleteDoorTileMap` can run after `ClearRoom` for the same room transition, it would iterate over already-cleared lists (no-op) instead of the intended door tiles — needs a Play Mode check on the `ON_CLEAR_ENEMY` → door-open flow.
- **[Low — new]** `RoomGeneraterController.ClearRoom(ref RoomCell _current)` — `ref` on a reference type (`RoomCell` is a `class`) has no effect; likely leftover from a value-type assumption. Cosmetic, but worth cleaning up to avoid confusion.

### Confusion points
- Naming: `IndexLevelDataDoor` stores indices *into* `Data.tiles/poses/layerIndices`, but `DeleteDoorTileMap` also **mutates** `Data` entries at those indices (`Data.tiles[...] = null`, etc.) — the data-ownership boundary between "RoomGeneraterController.Data" (transient, per-load) and "RoomCell.Data" (cached, per-room) is harder to follow after this change than before.

### Moments of delight
- Minimap avatar now animates in (`DOScale` pop-in with `Ease.OutBack`) and tweens between rooms (`DOMove` with `Ease.InOutQuad`) — first visible polish (đánh bóng) pass on the minimap.

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | `WeaponMelee.Attack()` foreach body empty — no damage (carryover) | High | Always |
| 2 | `Core.TakeDamage()` no death check — player cannot die (carryover) | High | Always |
| 3 | Enemy death chain broken — `EntityDeathState` wrong base class, `EntityBasicState` death block empty (carryover) | High | Always |
| 4 | `EndAnimation` event never fires — `AnimationPlayerController` double registration (carryover) | Medium | Always |
| 5 | `DeleteDoorTileMap` may read already-cleared `Data`/`IndexLevelDataDoor` after `ClearRoom` (new — unverified) | Medium | Conditional |
| 6 | `ClearRoom(ref RoomCell _current)` — pointless `ref` on reference type (new — cosmetic) | Low | Always |

## Feature-Specific Feedback

### Minimap Avatar Tween (new this week)
- **Understood purpose?** Yes
- **Found engaging?** Yes — visible motion feedback on room transitions
- **Suggestions**: Verify DOTween sequences are killed/completed on rapid consecutive room transitions (no tween overlap leaks).

### Combat
- **Understood purpose?** Yes
- **Found engaging?** N/A — still no damage, unchanged from last week
- **Suggestions**: Sprint 1 Must-Haves (S1-01 → S1-04) remain entirely unstarted after the sprint's first week. Needs explicit reprioritization at Monday kickoff.

## Quantitative Data
- **Deaths**: 0 — player death not implemented
- **Combat encounters**: 0 — no Play Mode session this week
- **Features discovered vs missed**: Minimap visited-state ✅ | Minimap avatar tween ✅ | Combat ❌ | Death/restart ❌ | Room clear ❌

## Overall Assessment
- **Would play again?** N/A — no live session this week
- **Difficulty**: N/A
- **Pacing**: N/A
- **Session length preference**: N/A

## Top 3 Priorities from this session
1. **Sprint 1 Must-Haves (S1-01 → S1-04) have zero progress after week 1 of a planned 5-day sprint** — this repeats the exact pattern flagged as a risk in `sprint-01.md` ("3 straight weeks of effort went to Map refactors instead of these exact blockers"). Needs explicit decision at Monday kickoff: reprioritize or extend sprint.
2. **Verify `DeleteDoorTileMap` / `ClearRoom` data lifecycle in Play Mode** — confirm door tiles are correctly removed on `ON_CLEAR_ENEMY` after this week's `IndexLevelDataDoor` refactor; static review suggests a possible empty-list no-op.
3. **Run a live Play Mode pass next session** — this week's report is code-review only; no actual playtest data was collected.
