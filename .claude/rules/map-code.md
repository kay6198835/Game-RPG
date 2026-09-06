---
description: Dungeon/map generation code standards — MazeGenerator, RoomGridController, RoomGeneraterController, RoomCell, DoorController, MapGridController
globs: ["Assets/Script/Map/**/*.cs"]
---

# Map / Dungeon Code Standards

## Singleton Discipline
- `MazeController` is the ONLY permitted singleton in map code (`EnemyManager` is also permitted project-wide per ADR-0002, but is not a map-code class)
- `RoomGridController`, `MapGridController`, `RoomGeneraterController`, `RoomCell`, `MapCell` and `DoorController` must use Inspector refs or `GetComponent`
- `LevelManager` is a standing, unratified singleton violation (TD-023) — do not add reach-throughs to it; `RoomGeneraterController.Setting()` already has one and its own code comment asks for it to be removed
- Never add a new singleton — use event system or component wiring

## Known Bugs (fix before any PR that touches map code)

> The three compile bugs previously listed here were in `RoomMapController` and
> `MainMapController`, both deleted on 2026-06-04 and superseded by `RoomGridController`.
> They were blocking every PR on fixes that had become impossible. Replaced 2026-08-20 with
> the map-code bugs that are actually open — verified against source.

- `MazeController.Awake()` is missing `return` after `Destroy(gameObject)`, so a duplicate
  instance still overwrites `Instance` and re-runs the generator (`MazeController.cs:17-20`,
  Bug #14 / TD-026). `EnemyManager.Awake()` has the correct shape to copy.
- Room JSON loads through `File.ReadAllText(Application.dataPath + filePath)` — Editor-only;
  `Assets/Data/Json/` is not packaged into a Player build
  (`RoomGeneraterController.cs:63`, Bug #15 / TD-022).
- `RoomType` is never read at runtime; start and end rooms are forced by list position
  `room[0]` / `room[last]`, so reordering `Maze_Storage.asset` breaks selection silently
  (`RoomGeneraterController.cs:47-48`, Bug #16 / TD-025).
- The start-room teleport is commented out and
  `RoomGeneraterController.OnDoneLoadRoomGrid()` has no caller
  (`RoomGridController.cs:82`, Bug #13).
- Dead code that reads as live gating: `DoorController.OpenDoor()`,
  `DoorController.CheckCanBeOpened()` and `RoomCell.UpdateStatusDoor()` are all no-ops.
  The real mechanism is `RoomCell.OpenDoors()` / `CloseDoor()` (Bug #17 / TD-024).
- `BaseGrid.GetNext()` has no bounds check (`BaseGrid.cs:34-41`, TD-027).

## Room Clear Condition
- **Implemented as of 2026-08-20** — the owner of the alive count is `RoomCell.EnemyCount`,
  not a `RoomController` (no such class ever existed). `RoomGridController` forwards
  `ON_DONE_SPAWN_ENEMY` / `ON_SPAWN_EXTRA_ENEMY` / `ON_ENEMY_DEATH` to the current cell;
  `RoomCell.OnEnemyDeath()` emits `ON_CLEAR_ENEMY` when the count reaches zero.
- Doors are closed by `RoomCell.ClearRoom()` on room exit and opened by `OpenDoors()` when
  `ON_CLEAR_ENEMY` fires. Extend that path — do not add a second counter.
- Use `EventManager.Emit` for door state changes — never call `DoorController.OpenDoor()`
  directly from enemy death (it is a no-op anyway, see Known Bugs)

## Event Bus Usage
- Room transitions go through `EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, direction)`
- Map load goes through `EventManager.Emit(EventID.ON_LOAD_MAP)`
- Do NOT add new `static` event fields — extend `EventID` enum in `EventManager.cs`

## Procedural Generation
- `MazeGenerator.Generator(rows, cols)` uses DFS — do not change the algorithm without design approval
- Cell door flags (`OPEN`, `BE_OPEN`, `CLOSE`) are the contract between generator and room spawner — do not add new flag types without updating both systems
