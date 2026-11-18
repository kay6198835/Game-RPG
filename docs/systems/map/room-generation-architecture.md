# Room Generation and Control — Architecture

> Read from source on `sprint-23` `3015e9cf` (2026-11-18). Static analysis; not verified in Play Mode.
> Companion to `docs/systems/map/README.md`. History of each change: `docs/systems/map/CHANGELOG.md`.
>
> **Changes on 2026-11-18 reflected here:** `EnemyManager` and `Grid` are children of `Room`;
> `RoomGeneraterController` calls `EnemyManager` directly and holds `LevelManager` by reference (no DI);
> start / end rooms are chosen by `roomType`; every run has the rooms of `_requiredRoomGroups`; the per-run
> room list is runtime only (`_mazeRooms`, `Maze_Load_Room.asset` deleted); boss rooms spawn
> `MapModel.EnemyModalBoss`; room authoring is `LevelEditorTool`; the Champion / Bonfire moved to the
> start room and the boss room has a spawn marker.

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
                                                                └──┬──────────────┬───────────┬───┘
                                   [SerializeField]                │              │           │ [SerializeField]
                                   _fullDungeonRoomSO              │              │           │ _enemyManager
                          ┌──────────────────────────────┐         │              │   ┌───────▼───────────────────────────┐
                          │ Maze_Storage.asset           │◄────────┘              │   │ Room/EnemyManager (child object)  │
                          │  room library (DungeonRoomSO)│                        │   │  EnemyManager · PathRequestManager│
                          └──────────────────────────────┘                        │   │  · EnemySpawner (MapModel)        │
                                                           [SerializeField]       │   └───────────────────────────────────┘
                                                           _levelManager          │
                                          ┌───────────────────────────────────────▼───┐
                                          │ Room/Grid/Level Manager (LevelManager)    │
                                          │  TileSO list + 5 tilemaps                 │
                                          │  └─ Level Editor Tool (EditorOnly)        │
                                          └───────────────────────────────────────────┘
  Per room: RoomCell (BaseCell) ── owns N × DoorController (BoxCollider2D trigger)
```

| Component | Role | File |
|---|---|---|
| `MazeGenerator` | DFS backtracker. Start = random cell; End = last cell carved | `Assets/Script/Map/Maze/MazeGenerator.cs` |
| `MazeController` | Entry point (singleton). Generates in `Awake()`, feeds both grids in `Start()`, emits `ON_LOAD_MAZE_DONE` | `Assets/Script/Map/Maze/MazeController.cs` |
| `Cell` | Logical cell: `Row`, `Column`, `Visited`, `Doors` (TOP/BOTTOM/LEFT/RIGHT → `STATUS_DOOR`) | `Assets/Script/Map/Cell/Cell.cs` |
| `BaseGrid<T>` | Shared grid: `AddCell` (instantiates one prefab per cell), `GetNext(dir)`, `CaculateIndex` = `y * Columns + x` | `Assets/Script/Map/BaseGrid.cs` |
| `BaseCell` | Shared cell view: holds `_cellData`, `GetGridPosition()`, abstract `Setting()` | `Assets/Script/Map/BaseCell.cs` |
| `MapGridController` / `MapCell` | Minimap: reveals visited cells, tweens the avatar | `Assets/Script/Map/Cell/` |
| `RoomGridController` | Room-flow controller. Listens to six events, owns `_current`, `_startIndex`, `_endIndex` | `Assets/Script/Map/Room/RoomGridController.cs` |
| `RoomGeneraterController` | Builds the per-run room list (`BuildMazeRooms()`), loads/draws room tiles, places doors, collects spawn points, builds the pathfinding grid, starts the room (open / spawn / boss) | `Assets/Script/Map/Room/RoomGeneraterController.cs` |
| `RoomCell` | Per-room state: `IsCleared`, `EnemyCount`, door list, cached `LevelData` for re-drawing a cleared room | `Assets/Script/Map/Room/RoomCell.cs` |
| `DoorController` | Door trigger. Collider enabled only when `Status == OPEN`; player contact emits `ON_PLAYER_ON_DOOR(dir)` | `Assets/Script/Map/Room/Door/DoorController.cs` |
| `EnemyManager` | Child of `Room`. Room-side entry point: `SetPathfindingGrid()`, `SpawnRoom()`, `SpawnBoss()` (forwarded to `EnemySpawner`); still the A* singleton for entities (ADR-0002) | `Assets/Script/System/Enemy/EnemyManager.cs` |
| `EnemySpawner` | Same object as `EnemyManager`. `SpawnRoom()` = random `RoomModel` set from `MapModel`; `SpawnBoss()` = `MapModel.EnemyModalBoss`. Does not know the room type | `Assets/Script/System/Enemy/EnemySpawner.cs` |
| `LevelManager` | Runtime room data at `Room/Grid/Level Manager`: `TileSO` list + tilemaps (`GetTileSOs()`, `GetTilemaps()`), door helpers. Not in DI | `Assets/Script/LevelEdit/LevelManager.cs` |
| `LevelEditorTool` | Editor-only room CRUD on the EditorOnly child `Level Editor Tool`: Save (create), Load (read), Edit (update), Delete (SO entry + JSON, confirm dialog), Import JSON. Fields: `_levelManager` (tiles + tilemaps) and its own `dungeonRoomSO`. Not referenced by the runtime flow | `Assets/Script/LevelEdit/LevelEditorTool.cs` |
| `DungeonRoomSO` / `RoomFile` | Room list SO; each entry = `roomName`, `filePath` (JSON), `roomType` | `Assets/SO/Dungeon/DungeonRoomSO.cs` |
| `LevelData` | Serialized room: parallel lists `tiles` (name), `poses` (room-local cell), `layerIndices` | `Assets/Script/LevelEdit/LevelManager.cs` |

### `RoomGeneraterController` fields

| Field | Kind | Purpose |
|---|---|---|
| `_fullDungeonRoomSO` | `[SerializeField]` | Room library (`Maze_Storage.asset`). Read-only at runtime |
| `_levelManager` | `[SerializeField]` | `Room/Grid/Level Manager`; `GetComponentInChildren` fallback |
| `_enemyManager` | `[SerializeField]` | `Room/EnemyManager`; `GetComponentInChildren` fallback |
| `_requiredRoomGroups` | `[SerializeField]` | Room-type groups every run must contain (default: `Shop`; `Treasure / Rest / Puzzle / Secret`) |
| `spawnPositions` | `[SerializeField]` (debug view) | Spawn markers of the room being shown |
| `pathfindingGrid` | `[SerializeField]` | A* grid rebuilt per room |
| `_mazeRooms` | private, runtime | One `RoomFile` per maze cell, rebuilt in `Setting()` |
| `_listTiles`, `_genmap` | private cache | Copied from `_levelManager` in `Setting()` |
| `Data`, `DoorPoints`, `IndexLevelDataDoor`, `_swapLevelData` | private, runtime | State of the room being shown |
| `_playerService` | `[Inject]` | `SetNextRoom()` moves the player |

### Dependencies

| From | To | How |
|---|---|---|
| `RoomGridController`, `MapGridController` | `MazeController` | `MazeController.Instance` (singleton) |
| `RoomGridController` | `PlayerManager` | `[Inject] Construct(PlayerManager)` |
| `RoomGridController` | `RoomGeneraterController` | `GetComponent` (same GameObject, `[RequireComponent]`) |
| `RoomGeneraterController` | `IPlayerService` | `[Inject] Construct(IPlayerService)` |
| `RoomGeneraterController` | `LevelManager` | `[SerializeField] _levelManager` — reference, not DI |
| `RoomGeneraterController` | `EnemyManager` | `[SerializeField] _enemyManager` — direct calls |
| `EnemyManager` | `EnemySpawner` | `[SerializeField] _spawner` (same GameObject; falls back to `GetComponent`) |
| `LevelEditorTool` | `LevelManager` | `[SerializeField] _levelManager` (`GetComponentInParent` fallback) |
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
  ├─ RoomGrid.Setting() → _startIndex / _endIndex → RoomGeneraterController.Setting(start, end, cellCount):
  │     resolve _enemyManager / _levelManager (LogError if missing)
  │     _listTiles, _genmap ← _levelManager
  │     BuildMazeRooms(cellCount, start, end)      → _mazeRooms (see below); false → Setting() stops
  │     new PathfindingGrid() → _enemyManager.SetPathfindingGrid()
  └─ Emit(ON_LOAD_MAZE_DONE)

ON_LOAD_MAZE_DONE
  ├─ RoomGridController.OnDoneLoadRoomGrid() → _current = start cell
  │                                          → RoomGeneraterController.LoadRoom(_startIndex, _current)
  │                                          → PlayerManager.SetPlayerPosition(room position)
  └─ MapGridController.OnLoadMap()           → VisitRoom() + avatar scale-in tween
```

### Room selection — `BuildMazeRooms(cellCount, startIndex, endIndex)`

Chosen by `roomType`, never by position in the library, so adding, deleting or re-sorting rooms in
`Maze_Storage.asset` does not move the start or boss room.

1. Split the library: `StartRoom` rooms, `BossRoom` rooms, and every other room (the middle pool).
2. Guard: no `StartRoom`, no `BossRoom`, or fewer than `cellCount − 2` middle rooms → `LogError`, return `false`.
3. Shuffle the middle pool.
4. `PickRequiredRooms()`: for each `_requiredRoomGroups` entry, take one middle room whose `roomType` is in the
   group (skip if an earlier pick already satisfies it). No matching room → warning; more groups than free
   cells → warning; that group is skipped.
5. Fill the remaining free cells with distinct rooms from the shuffled pool, then shuffle the picks again so
   required rooms land on random cells.
6. Walk the cells `0 … cellCount−1`: `startIndex` → a random `StartRoom`, `endIndex` → a random `BossRoom`,
   every other cell → the next pick.

Example (3 × 3 maze, current library of 20 rooms): start cell = `StartRoom_Entrance`, end cell =
`BossRoom_ThroneArena`, 7 other cells = `ShopRoom_Merchant` + at least one of `BuffRoom_PowerShrine` /
`RestRoom_Campfire` + random distinct rooms from the rest.

---

## 3. Loading a room — `RoomGeneraterController.LoadRoom(index, roomCell)`

1. **Read data.** `filePath = _mazeRooms[index].filePath`.
   Not cleared → `JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.dataPath + filePath))`.
   Cleared → copy the cached `RoomCell.Data`, `DoorPoints`, `IndexLevelDataDoor`.
2. Clear all tilemaps, then iterate `Data.tiles` / `poses` / `layerIndices`.
   World cell = room-local pose + `RoomCell` position (both data sources are room-local):
   - **DOOR tile** (room not cleared): direction = `Utility.ToCardinalDirection(pos)`.
     - Maze has a door that way → record a `DoorPoint` and the tile index.
     - Maze has no door that way → replace with a ROOM (wall) tile, queue in `_swapLevelData`.
   - **SPAWN tile** (`Tile_Spawn_Enemy`, room not cleared): append the world cell to `spawnPositions`.
   - `SetTile(worldCell, TileSO)` on layer `layerIdx`.
3. `roomCell.SetDoorPoints()` averages door tiles per direction and moves each `DoorController` there.
4. Branch on `_mazeRooms[index].roomType`:

| Room state | Action |
|---|---|
| Not cleared, `roomType` ∈ {Shop, Rest, Treasure} | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `Emit(ON_OPEN_DOOR)` (doors open immediately) |
| Not cleared, `roomType == BossRoom` | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `_enemyManager.SpawnBoss(spawnPositions)` |
| Not cleared, other type (incl. `StartRoom`) | `SwapTileMap` + `pathfindingGrid.BuildGrid` → `_enemyManager.SpawnRoom(spawnPositions)`. The start room has no spawn marker, so nothing spawns; its doors open through the Champion (`PedestalChampionController` → `ON_OPEN_DOOR`) |
| Cleared | `roomCell.OpenDoors()` |

### Room data (`Assets/Data/Json/Room/`, 20 rooms)

| Layer | Content |
|---|---|
| 0 | `Tile_Room` (walls), `Tile_Door` |
| 1 | `Tile_Floor` |
| 3 | `Tile_Spawn_Enemy` (combat rooms, boss room), `Tile_Spawn_Champion_Monument_{Archer,Mage,Paladin}` (start room) |
| 4 | `Tile_Spawn_Bonfire` (start room) |

- `StartRoom_Entrance`: three Champion monuments at (-7, 6) / (0, 6) / (6, 6), Bonfire at (0, 2) (moved from the
  boss room on 2026-11-18).
- `BossRoom_ThroneArena`: one `Tile_Spawn_Enemy` at (0, 2) — the boss spawns there.
- Shop / Rest / Buff (`TreasureRoom`): no marker; open on load.

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
            RoomGeneraterController.SetNextRoom(StartDoorPosition) → player moved
            Emit(ON_LOAD_MAP, index)
```

### Events

| Event | Emitter | Subscribers |
|---|---|---|
| `ON_LOAD_MAZE_DONE` | `MazeController` | `RoomGridController`, `MapGridController` |
| `ON_GET_SPAWN_POSITIONS` | none since 2026-11-18 (replaced by direct `EnemyManager` calls; kept in the enum) | none |
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

## 5. Scene setup requirements (`LoadRandomMap.unity`)

```
Maze                         [MazeController]
├─ MainMap/Room              [RoomGridController, RoomGeneraterController]
│  ├─ EnemyManager           [EnemyManager, PathRequestManager, EnemySpawner]
│  └─ Grid                   [Grid]
│     └─ Level Manager       [LevelManager]  (prefab Assets/Scenes/Main/Test/Prefab/Level Manager.prefab)
│        ├─ 5 tilemaps (Room Map, Floor Layer, Spawn Layer, Item Layer, Spawn Under Floor Layer)
│        └─ Level Editor Tool  [LevelEditorTool]  (tag EditorOnly)
└─ MiniMap                   [MapGridController]
GameLifetimeScope            [GameLifetimeScope]
```

- `RoomGridController.prefabObject` = `RoomCell` prefab (with `doorPrefab`).
- `RoomGeneraterController`: `_fullDungeonRoomSO = Maze_Storage.asset`, `_levelManager`, `_enemyManager` wired;
  `_requiredRoomGroups` uses the code defaults unless edited. The per-run room list needs no asset.
- `Maze`, `MainMap`, `Room` and `Grid` must stay at the origin: tiles are placed in world cells.
- `EnemySpawner.mapModel` assigned; `MapModel.enemyModalBoss` set (currently `EnemyPrefab` + `BossEntityData`).
- `Maze_Storage.asset` needs at least one `StartRoom`, one `BossRoom` and `cells − 2` other rooms; the boss
  room JSON needs at least one `Tile_Spawn_Enemy`.
- `PlayerManager`, and a `GameLifetimeScope` registering `RoomGeneraterController`, `RoomGridController`
  (`LevelManager` is not registered since 2026-11-18).

---

## 6. Known structural weaknesses

| Issue | Location | Ref |
|---|---|---|
| `MazeController.Awake()` has no `return` after `Destroy(gameObject)` | `MazeController.cs:15-20` | Bug #14 |
| JSON read via `File.ReadAllText(Application.dataPath…)` — Editor only | `RoomGeneraterController.LoadRoom()` | Bug #15 |
| Start / end rooms were forced by list position — **fixed in code and data 2026-11-18** (`BuildMazeRooms()` by `roomType`; Champion moved to the start room, boss marker added). Not verified in Play Mode; status left to the Saturday triage (inbox NOTE-20261118-3) | `RoomGeneraterController.BuildMazeRooms()` | Bug #16, BUG-097 |
| Start and Boss rooms never opened — **looks addressed 2026-11-18**: the start room now holds the Champion (opens through `PedestalChampionController`), the boss room has a spawn marker (opens when the boss dies). Status per the bug file | `Assets/Data/Json/Room/` | BUG-096 |
| `DoorController.OpenDoor()` / `CheckCanBeOpened()` and `RoomCell.UpdateStatusDoor()` are no-ops | `DoorController.cs`, `RoomCell.cs` | Bug #17 |
| `BaseGrid.GetNext()` has no bounds check | `BaseGrid.cs:34` | TD-027 |
| Two singletons (`MazeController`, `EnemyManager`) mixed with VContainer DI. The map side no longer uses `EnemyManager.Instance`; entities still do | — | ADR-0002, ADR-0004 |
| One shared tilemap set: every transition clears and redraws all tiles | `RoomGeneraterController.LoadRoom()` | — |
| `LevelEditorTool` draws on the same tilemaps the game uses; saving the scene after an Edit-Mode Load keeps those tiles in the scene (harmless at runtime: `LoadRoom()` clears first) | `LevelEditorTool.LoadLevel()` | — |
