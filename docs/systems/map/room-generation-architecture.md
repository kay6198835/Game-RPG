# Room Generation and Control — Architecture

> Read from source on `sprint-23` (2026-11-18). Static analysis; not verified in Play Mode.
> **Updated 2026-11-18:** `EnemyManager` is now a child of `Room` and is called directly by
> `RoomGeneraterController` (no `ON_GET_SPAWN_POSITIONS`, no `EnemyManager.Instance` on the map side);
> boss rooms spawn `MapModel.EnemyModalBoss`.
> Companion to `docs/systems/map/README.md`.

The room system has three layers: a **maze generator** that builds a logical grid, two **view grids**
that turn that grid into GameObjects, and a **tilemap loader** that draws one room at a time from JSON.
After the initial load, everything is driven by `EventManager` events.

---

## 1. Layers and components

```
                 ┌──────────────────────────── MazeController (singleton) ─────────────────────────────┐
  Logic layer    │ MazeGenerator (plain C#) → Cell[rows*cols]  (Row, Column, Doors: dict<string, STATUS_DOOR>) │
                 └──────────────┬────────────────────────────────────────────────┬────────────────────┘
                                │ AddCell(cell) for each cell                     │
                 ┌──────────────▼──────────────┐                ┌─────────────────▼───────────────┐
  View layer     │ MapGridController           │                │ RoomGridController              │
  BaseGrid<T>    │  : BaseGrid<MapCell>        │                │  : BaseGrid<RoomCell>           │
                 │  minimap + avatar (DOTween) │                │  event hub, _current / _next    │
                 └─────────────────────────────┘                └──────────────┬──────────────────┘
                                                                               │ [RequireComponent]
                                                                ┌──────────────▼──────────────────┐
  Content layer                                                 │ RoomGeneraterController         │
                                                                │  picks rooms, reads JSON,       │
                                                                │  draws tiles, builds A* grid    │
                                                                └──┬───────────┬───────────┬──────┘
                                                                   │           │           │ [SerializeField] _enemyManager
                       Room/Grid/LevelManager (ref) ─┘  DungeonRoomSO   ┌────▼─────────────────────────────┐
                                                full room pool,       (Maze_Load_Room)│ Room/EnemyManager (child object) │
                                                TileSOs, Tilemaps                     │  EnemyManager · PathRequestManager│
                                                                                      │  · EnemySpawner (MapModel)        │
                                                                                      └──────────────────────────────────┘
  Per room: RoomCell (BaseCell) ── owns N × DoorController (BoxCollider2D trigger)
```

| Component | Role | File |
|---|---|---|
| `MazeGenerator` | DFS backtracker. Start = random cell; End = last cell carved | `Assets/Script/Map/Maze/MazeGenerator.cs:42-66` |
| `MazeController` | Entry point (singleton). Generates in `Awake()`, feeds both grids in `Start()`, emits `ON_LOAD_MAZE_DONE` | `Assets/Script/Map/Maze/MazeController.cs` |
| `Cell` | Logical cell: `Row`, `Column`, `Visited`, `Doors` (TOP/BOTTOM/LEFT/RIGHT → `STATUS_DOOR`) | `Assets/Script/Map/Cell/Cell.cs` |
| `BaseGrid<T>` | Shared grid: `AddCell` (instantiates one prefab per cell), `GetNext(dir)`, `CaculateIndex` = `y * Columns + x` | `Assets/Script/Map/BaseGrid.cs` |
| `BaseCell` | Shared cell view: holds `_cellData`, `GetGridPosition()`, abstract `Setting()` | `Assets/Script/Map/BaseCell.cs` |
| `MapGridController` / `MapCell` | Minimap: reveals visited cells, tweens the avatar | `Assets/Script/Map/Cell/` |
| `RoomGridController` | Room-flow controller. Listens to six events, owns `_current` | `Assets/Script/Map/Room/RoomGridController.cs` |
| `RoomGeneraterController` | Loads/draws room tiles, places doors, collects spawn points, builds the pathfinding grid | `Assets/Script/Map/Room/RoomGeneraterController.cs` |
| `RoomCell` | Per-room state: `IsCleared`, `EnemyCount`, door list, cached `LevelData` for re-drawing a cleared room | `Assets/Script/Map/Room/RoomCell.cs` |
| `DoorController` | Door trigger. Collider enabled only when `Status == OPEN`; player contact emits `ON_PLAYER_ON_DOOR(dir)` | `Assets/Script/Map/Room/Door/DoorController.cs` |
| `EnemyManager` | Child of `Room`. Room-side entry point: `SetPathfindingGrid()`, `SpawnRoom()`, `SpawnBoss()` (forwarded to `EnemySpawner`); still the A* singleton for entities (ADR-0002) | `Assets/Script/System/Enemy/EnemyManager.cs` |
| `EnemySpawner` | Same object as `EnemyManager`. `SpawnRoom()` = random `RoomModel` set from `MapModel`; `SpawnBoss()` = `MapModel.EnemyModalBoss`. Does not know the room type | `Assets/Script/System/Enemy/EnemySpawner.cs` |
| `LevelManager` | Runtime room data under `Room/Grid/Level Manager`: `TileSO` list + tilemaps; receives the room library from `RoomGeneraterController._fullDungeonRoomSO` (`SetDungeonRoomSO`). Not in DI | `Assets/Script/LevelEdit/LevelManager.cs` |
| `LevelEditorTool` | Room CRUD in the Editor — Save (create), Load (read), Edit (update), Delete (SO entry + JSON, confirm dialog), Import JSON — on the EditorOnly child `Level Editor Tool`. Fields: `_levelManager` (tiles + tilemaps) and its own `dungeonRoomSO`. Not referenced by the runtime flow | `Assets/Script/LevelEdit/LevelEditorTool.cs` |
| `DungeonRoomSO` / `RoomFile` | Room list SO; each entry = `roomName`, `filePath` (JSON), `roomType` | `Assets/SO/Dungeon/DungeonRoomSO.cs` |
| `LevelData` | Serialized room: parallel lists `tiles` (name), `poses` (cell), `layerIndices` | `Assets/Script/LevelEdit/LevelManager.cs` |

### Dependencies

| From | To | How |
|---|---|---|
| `RoomGridController`, `MapGridController` | `MazeController` | `MazeController.Instance` (singleton) |
| `RoomGridController` | `PlayerManager` | `[Inject] Construct(PlayerManager)` |
| `RoomGeneraterController` | `IPlayerService` | `[Inject] Construct(IPlayerService)` |
| `RoomGeneraterController` | `LevelManager` | `[SerializeField] _levelManager` (descendant `Room/Grid/Level Manager`; falls back to `GetComponentInChildren`) |
| `RoomGeneraterController` | `EnemyManager` | `[SerializeField] _enemyManager` (child object; falls back to `GetComponentInChildren`) — direct calls |
| `EnemyManager` | `EnemySpawner` | `[SerializeField] _spawner` (same GameObject; falls back to `GetComponent`) |
| `EntityMovement` | `EnemyManager` | `EnemyManager.Instance` (singleton, ADR-0002) — unchanged |

---

## 2. Initialisation flow

```
MazeController.Awake()
  → MazeGenerator.Generator(Rows, Columns)
      DFS carve: current side = ENEBLE, neighbour side = BE_OPEN
MazeController.Start() → SetCellData()
  ├─ for each Cell → MapGrid.AddCell(cell), RoomGrid.AddCell(cell)
  │     RoomCell.Setting(): position = (Column, -Row) × SCALE × LENGTH_ROOM;
  │                         instantiate a DoorController per non-DISABLE door
  ├─ MapGrid.Setting()  → _current = start cell
  ├─ RoomGrid.Setting() → _startIndex / _endIndex → RoomGeneraterController.Setting():
  │     _levelManager.SetDungeonRoomSO(_fullDungeonRoomSO); tiles, tilemaps ← LevelManager
  │     Utility.PickUniqueIndex(total, cellCount) → append to _dungeonRoomSO.room
  │     room[_startIndex] = full[0]; room[_endIndex] = full[last]      (by list position)
  │     new PathfindingGrid() → _enemyManager.SetPathfindingGrid()
  └─ Emit(ON_LOAD_MAZE_DONE)

ON_LOAD_MAZE_DONE
  ├─ RoomGridController.OnDoneLoadRoomGrid() → LoadRoom(_startIndex, _current)
  │                                          → PlayerManager.SetPlayerPosition(room position)
  └─ MapGridController.OnLoadMap()           → VisitRoom() + avatar scale-in tween
```

---

## 3. Loading a room — `RoomGeneraterController.LoadRoom(index, roomCell)`

1. **Read data.** Not cleared → `JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.dataPath + filePath))`.
   Cleared → copy the cached `RoomCell.Data`, `DoorPoints`, `IndexLevelDataDoor`.
2. Clear all tilemaps, then iterate `Data.tiles` / `poses` / `layerIndices`:
   - **DOOR tile** (room not cleared): direction = `Utility.ToCardinalDirection(pos)`.
     - Maze has a door that way → record a `DoorPoint` and the tile index.
     - Maze has no door that way → replace with a ROOM (wall) tile, queue in `_swapLevelData`.
   - **SPAWN tile** (room not cleared): append the world position to `spawnPositions`.
   - `SetTile(worldPos, TileSO)` on layer `layerIdx`.
3. `roomCell.SetDoorPoints()` averages door tiles per direction and moves each `DoorController` there.
4. Branch:

| Room state | Action |
|---|---|
| Not cleared, `roomType` ∈ {Shop, Rest, Treasure} | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `Emit(ON_OPEN_DOOR)` (doors open immediately) |
| Not cleared, `roomType == BossRoom` | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `_enemyManager.SpawnBoss(spawnPositions)` |
| Not cleared, other type | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `_enemyManager.SpawnRoom(spawnPositions)` |
| Cleared | `roomCell.OpenDoors()` |

---

## 4. Runtime loop (event-driven)

```
EnemyManager.SpawnRoom(pos) → EnemySpawner.SpawnRoom(pos)
                  → mapModel.GetRandomRoom() → RoomModel.GetSpawnSet() → pool.Spawn + ApplyData per enemy
                  → Emit(ON_DONE_SPAWN_ENEMY, n)
EnemyManager.SpawnBoss(pos) → EnemySpawner.SpawnBoss(pos)
                  → mapModel.EnemyModalBoss → SpawnRequest(RequestSpawnEnemy{prefab, random marker, data})
                  → Emit(ON_DONE_SPAWN_ENEMY, 1)          // no shuffle-bag draw
EnemySpawner      → ON_DONE_SPAWN_ENEMY(n)  → RoomCell.EnemyCount = n
                  → ON_SPAWN_EXTRA_ENEMY    → EnemyCount++
EntityDeathState  → ON_ENEMY_DEATH          → EnemyCount--
                                              at 0 → Emit(ON_ROOM_CLEARED), finally Emit(ON_OPEN_DOOR)

ON_OPEN_DOOR → RoomGridController.DeleteDoorTileMap()
             → RoomGeneraterController.DeleteDoorTileMap(): erase door tiles
             → RoomCell.OpenDoors(): every door Status = OPEN (collider on), IsCleared = true

DoorController.OnTriggerEnter2D(Player) → Emit(ON_PLAYER_ON_DOOR, direction)
  ├─ MapGridController.Move(dir)     → minimap avatar tween to the next cell
  └─ RoomGridController.ClearRoom(dir)
       → RoomGeneraterController.ClearRoom(_current)
            cache LevelData into RoomCell, erase tiles, RoomCell.CloseDoor(), reset buffers
       → OnLoadMap(dir)
            _current = GetNext(dir)
            LoadRoom(index, _current)
            _current.GetStartDoorPosition(-dir)
            SetPlayerPosition(StartDoorPosition)
            Emit(ON_LOAD_MAP, index)
```

### Events

| Event | Emitter | Subscribers |
|---|---|---|
| `ON_LOAD_MAZE_DONE` | `MazeController` | `RoomGridController`, `MapGridController` |
| `ON_GET_SPAWN_POSITIONS` | none since 2026-11-18 (replaced by direct `EnemyManager` calls) | none |
| `ON_DONE_SPAWN_ENEMY` | `EnemySpawner` | `RoomGridController` → `RoomCell` |
| `ON_SPAWN_EXTRA_ENEMY` (`RequestSpawnEnemy{prefab, positionSpawn, data}`) | none yet | `EnemySpawner`, `RoomGridController` → `RoomCell` |
| `ON_ENEMY_DEATH` | `EntityDeathState` | `RoomGridController` → `RoomCell` |
| `ON_ROOM_CLEARED` | `RoomCell` | none |
| `ON_OPEN_DOOR` | `RoomCell`, `RoomGeneraterController`, `PedestalChampionController` | `RoomGridController` |
| `ON_PLAYER_ON_DOOR` | `DoorController` | `RoomGridController`, `MapGridController` |
| `ON_LOAD_MAP` | `RoomGridController` | — |

### `STATUS_DOOR`

| Value | Name | Meaning | Collider |
|---|---|---|---|
| 0 | `DISABLE` | No door in this direction | Off |
| 1 | `ENEBLE` | Door exists, locked | Off |
| 2 | `BE_OPEN` | Receiver side of a carved passage | Off |
| 3 | `OPEN` | Passable | **On** |
| 4 | `CLOSE` | Runtime solid wall | Off |

---

## 5. Scene setup requirements

- `MazeController` with `RoomGridController` (+ `RoomGeneraterController`) and `MapGridController` as children.
- `RoomGridController.prefabObject` = `RoomCell` prefab (with `doorPrefab`); `RoomGeneraterController._dungeonRoomSO` = `Maze_Load_Room.asset`.
- `RoomGeneraterController._fullDungeonRoomSO = Maze_Storage.asset` and `_levelManager` = the `LevelManager` under
  `Room/Grid/Level Manager` (tile list + tilemaps). `Grid` must stay at the origin: tiles are placed in world cells.
- `Level Editor Tool` (EditorOnly child of `Level Manager`) holds the authoring `LevelEditorTool`; stripped from builds.
- `Room/EnemyManager` child object with `EnemyManager`, `PathRequestManager`, `EnemySpawner` (with `mapModel`;
  `MapModel.enemyModalBoss` set for boss rooms). A boss room JSON needs at least one `Tile_Spawn_Enemy`.
- `PlayerManager`, and a `GameLifetimeScope` registering
  `RoomGeneraterController`, `RoomGridController` (`LevelManager` is no longer registered since 2026-11-18).

---

## 6. Known structural weaknesses

| Issue | Location | Ref |
|---|---|---|
| `Setting()` appends to `_dungeonRoomSO.room` — a committed asset mutated at runtime; cleared only in `OnDisable()` | `RoomGeneraterController.cs:50-56` | — |
| `MazeController.Awake()` has no `return` after `Destroy(gameObject)` | `MazeController.cs:17-21` | Bug #14 |
| JSON read via `File.ReadAllText(Application.dataPath…)` — Editor only | `RoomGeneraterController.cs:69` | Bug #15 |
| Start / end rooms forced by list position (`full[0]`, `full[last]`) | `RoomGeneraterController.cs:55-56` | Bug #16, BUG-097 |
| Start and Boss rooms have no spawn marker and are not auto-open types | `RoomGeneraterController.cs:138-141` | BUG-096 |
| `RoomGeneraterController.OnDoneLoadRoomGrid()` is dead code; `DoorController.CheckCanBeOpened()` is a no-op | — | Bug #17 |
| `BaseGrid.GetNext()` has no bounds check | `BaseGrid.cs:31-38` | TD-027 |
| Two singletons (`MazeController`, `EnemyManager`) mixed with VContainer DI. The map side no longer uses `EnemyManager.Instance`; entities still do | — | ADR-0002, ADR-0004 |
| Boss detection is by `roomType == BossRoom`, but start/end placement is by list position: `BossRoom_ThroneArena` lands on the **start** cell today | `RoomGeneraterController.cs:61-62` | BUG-097 |
| One shared tilemap set: every transition clears and redraws all tiles | `RoomGeneraterController.cs:83` | — |
