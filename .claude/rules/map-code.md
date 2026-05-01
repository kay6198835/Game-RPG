---
description: Dungeon/map generation code standards — MazeGenerator, RoomController, DoorController, MainMapController
globs: ["Assets/Script/Map/**/*.cs"]
---

# Map / Dungeon Code Standards

## Singleton Discipline
- `MazeController` is the ONLY permitted singleton in map code
- `RoomMapController`, `CellMapController`, `MainMapController` must use Inspector refs or `GetComponent`
- Never add a new singleton — use event system or component wiring

## Known Compile Bugs (fix before any PR)
- `RoomMapController.GetNextRoom()`: `this.Columns` property missing; `_roomMapController` self-reference is wrong
- `MainMapController.LoadRoom()`: typo `cuonefsakdjfhnasdklfhjasdrrent` must be `current`
- `MainMapController.Start()`: call `.RoomMapController.GetStartRoom()` not `.GetStartRoom()` directly on MazeController

## Room Clear Condition
- `RoomController` must track enemy count; lock doors on room enter, unlock when count reaches 0
- Use `EventManager.Emit` for door state changes — never call `DoorController.OpenDoor()` directly from enemy death

## Event Bus Usage
- Room transitions go through `EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, direction)`
- Map load goes through `EventManager.Emit(EventID.ON_LOAD_MAP)`
- Do NOT add new `static` event fields — extend `EventID` enum in `EventManager.cs`

## Procedural Generation
- `MazeGenerator.Generator(rows, cols)` uses DFS — do not change the algorithm without design approval
- Cell door flags (`OPEN`, `BE_OPEN`, `CLOSE`) are the contract between generator and room spawner — do not add new flag types without updating both systems
