# Playtest Report

## Session Info
- **Date**: 2026-06-05
- **Build**: e053593
- **Duration**: 20 min
- **Tester**: Kiet
- **Platform**: PC — Unity Editor
- **Input Method**: KB+M
- **Session Type**: Weekly wrap-up — verification pass

## Test Focus
End-of-week verification of the map system completed this week: random DFS start/end, `PickUniqueIndex` room pool, `RoomGridController.LoadRoom()` JSON loading, door tile swap/seal, player teleport on door entry. Combat and enemy systems not in scope.

## First Impressions (First 5 minutes)
- **Understood the goal?** Yes
- **Understood the controls?** Yes
- **Emotional response**: Satisfied — navigation skeleton is stable
- **Notes**: Dungeon generates and loads correctly each run. No compile errors. Player spawns at random start room.

## Gameplay Flow

### What worked well
- Random start/end selection produces varied dungeon layouts run-to-run.
- `PickUniqueIndex` correctly reserves index 0 and last for start/end rooms; no duplicate rooms in pool.
- Door seal logic: unused door directions are swapped to ROOM tile on load — no wall gaps.
- Player teleport on door entry positions correctly at the entry door of the next room.
- `ON_LOAD_MAZE_DONE` → `ON_PLAYER_ON_DOOR` → `ON_CLEAR_ENEMY` event chain fires in correct order.
- Previously visited rooms reload from cached `RoomCell` state (IsCleared path) without re-reading JSON.

### Pain points
- **[High] Combat non-functional.** `WeaponMelee.Attack()` foreach body is empty — no damage dealt. Dungeon navigation is verified but the core game loop cannot be tested.
- **[High] No enemy clear requirement.** Doors open immediately — room lock-on-enter and `ON_CLEAR_ENEMY` from actual enemy death are not connected.
- **[High] Player cannot die.** `Core.TakeDamage()` decrements health with no death state or restart.
- **[Medium] `RoomCell.GetDoor()` returns `new DoorController()` instead of `null` when direction not found.** Silent failure — the returned detached instance is operated on without error, masking missing-door bugs.

### Confusion points
- `_dungeonRoomSO.room` is mutated directly on the SO asset at runtime in `Setting()` and cleared in `OnDisable()`. An unexpected Editor exit between these two moments leaves a dirty SO asset with duplicate entries on disk.

### Moments of delight
- Distinct room layouts visible immediately after maze generation. The random variation is noticeable even in a 3×3 grid.

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | `WeaponMelee.Attack()` foreach body empty — no damage | High | Always |
| 2 | `Core.TakeDamage()` no death check — player cannot die | High | Always |
| 3 | Enemy death chain broken — `EntityDeathState` wrong base class, `EntityBasicState` death block empty | High | Always |
| 4 | `EndAnimation` event never fires — `AnimationPlayerController` registers `StartAnimation` twice | Medium | Always |
| 5 | `RoomCell.GetDoor()` returns `new DoorController()` instead of `null` — silent failure on missing direction | Medium | Conditional |
| 6 | `_dungeonRoomSO.room` mutated on SO asset at runtime — dirty asset on crash | Low | On crash |

## Feature-Specific Feedback

### Random Maze Generation
- **Understood purpose?** Yes
- **Found engaging?** Yes — layout variation is satisfying
- **Suggestions**: Expose `Rows`/`Columns` as Inspector-configurable range to enable tuning without code changes.

### Room Navigation
- **Understood purpose?** Yes
- **Found engaging?** Yes — transition feels responsive
- **Suggestions**: Add a brief screen flash or sound cue on transition so the teleport is legible, not jarring.

### Door Tile Swap (Seal Logic)
- **Understood purpose?** Yes
- **Found engaging?** N/A — internal system
- **Suggestions**: Correctly seals unused doors. No player-facing issues.

### Combat
- **Understood purpose?** Yes
- **Found engaging?** N/A — no damage, cannot evaluate
- **Suggestions**: Bug #1 must be fixed before any combat evaluation is possible.

## Quantitative Data
- **Room transitions completed**: ~6 (3×3 maze, full traversal)
- **Deaths**: 0 — player death not implemented
- **Combat encounters**: 0 — enemies present but no damage exchange
- **Features discovered vs missed**: Navigation ✅ | Room cache (revisit) ✅ | Combat ❌ | Death/restart ❌ | Room clear ❌

## Overall Assessment
- **Would play again?** Yes — dungeon skeleton is solid
- **Difficulty**: N/A — cannot die
- **Pacing**: Too slow — no threat, no urgency
- **Session length preference**: N/A — game loop incomplete

## Top 3 Priorities from this session
1. **Fix `WeaponMelee.Attack()` empty foreach** (Bug #4 in CLAUDE.md) — combat must deal damage before any game loop testing is possible.
2. **Deploy enemy death chain** (Bugs #7, #8 in CLAUDE.md) — fix `EntityDeathState` base class + fill `EntityBasicState` death block + emit `ON_CLEAR_ENEMY` on last enemy death.
3. **Player death + restart** (Bug #6 in CLAUDE.md) — `Core.TakeDamage()` death check → `ON_PLAYER_DEATH` → `PlayerData.Reborn()` → reload `StartScene`.
