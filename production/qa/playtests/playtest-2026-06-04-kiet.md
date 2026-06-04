# Playtest Report

## Session Info
- **Date**: 2026-06-04
- **Build**: 3c87f5a
- **Duration**: 30 min
- **Tester**: Kiet
- **Platform**: PC — Unity Editor
- **Input Method**: KB+M
- **Session Type**: Weekly wrap-up — targeted test (room navigation + random maze load)

## Test Focus
Random maze generation with DFS start/end selection, random room pool assignment via `PickUniqueIndex`, room-to-room navigation through door triggers, door tile swap (seal unused doors), and player teleportation on door entry. Combat and enemy systems not in scope this session.

## First Impressions (First 5 minutes)
- **Understood the goal?** Yes — dungeon navigation is legible
- **Understood the controls?** Yes
- **Emotional response**: Engaged — room-to-room flow feels connected
- **Notes**: Map loads visually at start. Player spawns at start room. Door transitions fire correctly when player walks through.

## Gameplay Flow

### What worked well
- **Random start/end selection**: `MazeGenerator` now picks a random start cell (not always top-left) and uses the last DFS-visited cell as end. Dungeon layouts feel varied.
- **Random room pool**: `Utility.PickUniqueIndex()` correctly excludes index 0 and last (reserved for start/end rooms) — no duplicates in pool.
- **Room tile loading**: `RoomGridController.LoadRoom()` reads JSON, offsets tile positions by `roomCell.transform.position`, and renders the tilemap correctly.
- **Door seal logic**: Unused door tiles are swapped to `ROOM` tile on load — no visual gaps in walls for sealed directions.
- **Player teleport**: `_fastMovement.SetPositionAndRotation()` places player at the correct entry door of the next room after transition.
- **Previous LevelManager.LoadRoom() index bug** (Bug from 2026-05-23): Superseded — `RoomGridController` now owns room loading logic with correct index mapping.

### Pain points
- **[High] Combat is still non-functional.** `WeaponMelee.Attack()` foreach body is empty — no damage is dealt. The dungeon navigation is working but the game loop cannot be tested without functional combat.
- **[High] No enemy clear requirement.** Doors open immediately — there is no room lock on enter and no `ON_CLEAR_ENEMY` trigger connected to actual enemy death. The full room-clear→door-open loop is not yet exercised.
- **[Medium] Player cannot die.** `Core.TakeDamage()` decrements health silently — no death state, no restart. Enemy hits land but the run never ends.
- **[Medium] `LevelManager.GetTilemaps()` returns `tilemap` (save list), not `genmap` (generation list).** `RoomGridController.Setting()` assigns `_genmap` from this call — if both lists are wired identically in Inspector this works by coincidence, but the naming discrepancy is a latent bug waiting to surface when the Inspector is rewired.

### Confusion points
- `_dungeonRoomSO.room` is mutated at runtime in `RoomGridController.Setting()` (adds rooms) and cleared in `OnDisable()`. If the Editor exits unexpectedly between these two moments, the SO asset on disk will be left with duplicate entries. This is fragile and should be replaced with a runtime-only list.

### Moments of delight
- Entering a door and arriving at a visually distinct room already feels like dungeon exploration. The random layout variation is noticeable even with 3×3 grids.

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | `WeaponMelee.Attack()` foreach body empty — no damage | High | Always |
| 2 | `Core.TakeDamage()` no death check — player cannot die | High | Always |
| 3 | Enemy death chain broken — `EntityDeathState` extends `MonoBehaviour`, `EntityBasicState` death block empty | High | Always |
| 4 | `EndAnimation` event never fires — `AnimationPlayerController` registers `StartAnimation` twice | Medium | Always |
| 5 | `LevelManager.GetTilemaps()` returns save tilemap list instead of generation tilemap list — latent wiring bug | Medium | Conditional |
| 6 | `_dungeonRoomSO.room` mutated on SO asset at runtime — dirty asset on crash | Low | On crash |

## Feature-Specific Feedback

### Random Maze Generation
- **Understood purpose?** Yes
- **Found engaging?** Yes — layout variation is immediately apparent
- **Suggestions**: Consider exposing min/max maze size in `MazeController` Inspector for tuning without code changes.

### Room Navigation (DoorController / RoomGridController)
- **Understood purpose?** Yes
- **Found engaging?** Yes — transition feels responsive
- **Suggestions**: Add a brief visual flash or sound cue on room transition so the teleport doesn't feel like a glitch.

### Door Tile Swap (Seal Logic)
- **Understood purpose?** Yes
- **Found engaging?** N/A — internal system
- **Suggestions**: Working correctly. No player-facing issues observed.

### Combat (WeaponMelee)
- **Understood purpose?** Yes
- **Found engaging?** N/A — no damage applied, cannot evaluate
- **Suggestions**: Bug #1 must be fixed before any combat feedback is meaningful.

## Quantitative Data
- **Room transitions completed**: ~6 (3×3 maze, traversed most rooms)
- **Deaths**: 0 (player death not implemented)
- **Combat encounters**: 0 (enemies present but no damage exchange works)
- **Features discovered vs missed**: Navigation ✅ | Combat ❌ | Death/restart ❌ | Room clear ❌

## Overall Assessment
- **Would play again?** Yes — navigation skeleton is satisfying
- **Difficulty**: N/A — cannot die
- **Pacing**: Too slow — no threat from enemies, no urgency
- **Session length preference**: N/A

## Top 3 Priorities from this session
1. **Fix `WeaponMelee.Attack()` empty foreach** (Bug #4 in CLAUDE.md) — combat must deal damage before any game loop testing is possible.
2. **Deploy enemy death chain** (Bugs #7, #8 in CLAUDE.md) — fix `EntityDeathState` base class + fill `EntityBasicState` death block + emit `ON_CLEAR_ENEMY` on last enemy death.
3. **Player death + restart** (Bug #6 in CLAUDE.md) — `Core.TakeDamage()` death check → `ON_PLAYER_DEATH` → `PlayerData.Reborn()` → reload `StartScene`.
