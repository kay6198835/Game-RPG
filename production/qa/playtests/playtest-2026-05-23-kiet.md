# Playtest Report

## Session Info
- **Date**: 2026-05-23
- **Build**: b538c7b
- **Duration**: Code analysis session (no playable build yet)
- **Tester**: Kiet
- **Platform**: PC — Unity Editor
- **Input Method**: KB+M
- **Session Type**: Targeted test — map generation & room loading

## Test Focus
Maze structure generation, room detail map loading, dungeon navigation flow. Combat and game loop not yet in scope.

## First Impressions (First 5 minutes)
- **Understood the goal?** N/A — pre-playable stage
- **Understood the controls?** N/A
- **Emotional response**: N/A
- **Notes**: Game is not yet playable end-to-end. Maze structure generates correctly but room tilemaps do not appear in the world.

## Gameplay Flow

### What worked well
- Maze structure generation (DFS) runs correctly — `MazeGenerator.Generator()` produces a valid `Cell[]` grid with correct `Top/Bottom/Left/Right` door flags.
- `RoomCell` grid positions are calculated and placed correctly in world space via `(column, -row) * SCALE * LENGTH_ROOM`.
- Door status semantics (`OPEN` / `BE_OPEN` / `CLOSE`) are applied correctly by `DoorController.Setting()`.
- Event bus wiring is correct: `RoomNavigator` and `MapTracker` both register `ON_PLAYER_ON_DOOR` and `ON_LOAD_MAZE_DONE` with matching `OnEnable/OnDisable` pairs.
- `MazeController` singleton pattern is safe — duplicate instance is destroyed on `Awake`.

### Pain points
- **[High] Room detail map (tilemap) does not load.** `LevelManager.LoadRoom(index, pos)` has an index mismatch bug: for `index == 0` it reads from `dungeonRoomSO.room[0]` (correct), but for `index > 0` it reads from `listRooms[index]` where `index` is the raw grid cell index (0–8 for a 3×3 maze), while `listRooms` only contains `amount` randomly shuffled entries. Any grid index ≥ `amount` throws `IndexOutOfRangeException` and the room fails to render.
- **[High] No game loop.** Player can enter a door but there is no win/lose condition, no enemy clear requirement, and no death-restart cycle.
- **[Medium] Player death is silent.** `Core.TakeDamage()` decrements `currentHealth` but never checks `<= 0` — no death event, no respawn, no game over.

### Confusion points
- `LevelManager.LoadRoom()` uses the word `index` for two different concepts: grid cell position index vs. shuffled room list index. These should be named differently to avoid future confusion.

### Moments of delight
- The DFS maze generation is clean and produces a guaranteed-connected maze — no orphaned rooms.

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | `LevelManager.LoadRoom(index > 0)` uses grid cell index to access `listRooms[]`, which only has `amount` entries — `IndexOutOfRangeException` on any room transition beyond the first | High | Yes — always |
| 2 | `WeaponMelee.Attack()` — `foreach` body is empty, no `TakeDamage()` call — player attacks deal zero damage | High | Yes — always |
| 3 | `EntityBasicState.LogicUpdate()` — `Health <= 0` block is empty, enemy never transitions to death state | High | Yes — always |
| 4 | `EntityDeathState` extends `MonoBehaviour` instead of `EntityState` — not integrated into state machine | High | Yes — always |
| 5 | `Core.TakeDamage()` — no death check after health decrement, no `ON_PLAYER_DEATH` event emitted | High | Yes — always |

## Feature-Specific Feedback

### Maze Generation (MazeGenerator / MazeController)
- **Understood purpose?** Yes
- **Found engaging?** N/A — internal system, not player-facing
- **Suggestions**: Working correctly. No changes needed.

### Room Detail Map Loading (LevelManager / RoomGridController)
- **Understood purpose?** Yes
- **Found engaging?** N/A — broken, not yet testable
- **Suggestions**: Fix the index mapping bug (Bug #1 above). `LoadRoom` should map grid cell index to a deterministic room assignment, not use raw grid index to access `listRooms[]`.

### Dungeon Navigation (RoomNavigator / DoorController)
- **Understood purpose?** Yes
- **Found engaging?** N/A — cannot evaluate without visible room content
- **Suggestions**: Door trigger and event chain look correct in code. Will be testable once room tilemaps render.

### Combat (WeaponMelee)
- **Understood purpose?** Yes
- **Found engaging?** N/A — no damage applied
- **Suggestions**: Add `INegativeReceiver.TakeDamage()` inside `WeaponMelee.Attack()` foreach body (Bug #2).

### Enemy AI (EntityBasicState / EntityDeathState)
- **Understood purpose?** Yes
- **Found engaging?** N/A — enemies cannot die
- **Suggestions**: Fix `EntityDeathState` base class (Bug #4) and fill the empty death block in `EntityBasicState` (Bug #3).

## Quantitative Data
- **Deaths**: N/A
- **Time per room**: N/A
- **Skills used**: N/A
- **Features discovered vs missed**: N/A

## Overall Assessment
- **Would play again?** N/A — not yet playable
- **Difficulty**: N/A
- **Pacing**: N/A
- **Session length preference**: N/A

## Top 3 Priorities from this session
1. **Fix `LevelManager.LoadRoom()` index bug** — rooms must render before any other system can be evaluated.
2. **Fix `WeaponMelee.Attack()` empty foreach** — combat is the core loop; it must deal damage.
3. **Fix enemy death chain** — `EntityDeathState` base class + `EntityBasicState` death block + player death check in `Core.TakeDamage()`.
