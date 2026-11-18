# Map — Maze, Rooms, Doors, Level Editor

> **Status:** live · **Last verified:** 2026-11-18, `sprint-23` `7e6e2156` (static read; not verified in Play Mode)
> History: [CHANGELOG.md](CHANGELOG.md) · Architecture: [room-generation-architecture.md](room-generation-architecture.md) ·
> GDD: `design/gdd/map-system.md` · Rules: `.claude/rules/map-code.md`

## Purpose

Procedural dungeon: a DFS maze decides which rooms connect; each maze cell loads an authored room
layout (JSON tilemap); clearing a room's enemies opens its doors; walking through a door loads
the next room. A minimap mirrors progress.

## Code

| Path | Contents |
|------|----------|
| `Map/BaseGrid.cs`, `BaseCell.cs`, `Interface/` | Generic grid |
| `Map/Maze/` | `MazeGenerator` (DFS), `MazeController` (singleton, permitted) |
| `Map/Cell/` | `Cell`, `MapCell`, `MapGridController` (minimap, DOTween avatar tween) |
| `Map/Room/` | `RoomCell` (room-clear counter), `RoomGridController` (event hub), `RoomGeneraterController` (room selection + tilemap loader), `Door/DoorController` |
| `Map/Legacy/` | `Door`, `Room` — superseded |
| `LevelEdit/LevelManager.cs` | Runtime room data: `TileSO` list + tilemaps, door helpers; declares `LevelData`. Not in DI |
| `LevelEdit/LevelEditorTool.cs` + `Editor/LevelEditorToolEditor.cs` | Editor-only room CRUD: Save / Load / Edit / Delete + Import JSON |
| `Assets/Data/Json/Room/` | 20 room layouts |
| `Assets/SO/Dungeon/` | `Maze_Storage.asset` (room library). `Maze_Load_Room.asset` deleted 2026-11-18 — the per-run list is `RoomGeneraterController._mazeRooms` |

## Room set (since 2026-10-04)

`StartRoom_Entrance`, `CombatRoom_{CentralIsland, Checkerboard, Cross, DiagonalWalls, EliteGuard,
FourPillars, GrandArena, HiddenCorner, NarrowBridges, PracticeYard, RoundArena, SmallMaze, Spiral,
TwinCorridors, TwoHalls}`, `BuffRoom_PowerShrine`, `RestRoom_Campfire`, `ShopRoom_Merchant`,
`BossRoom_ThroneArena`.

| Room | `roomType` | Content / how the doors open |
|---|---|---|
| `StartRoom_Entrance` | `StartRoom` (1) | 3 Champion monuments + Bonfire (moved from the boss room 2026-11-18); opens through the Champion pick |
| `CombatRoom_*` ×15 | `CombatRoom` (3) | `Tile_Spawn_Enemy` markers; open when every enemy is dead |
| `BossRoom_ThroneArena` | `BossRoom` (2) | One `Tile_Spawn_Enemy` at (0, 2) (added 2026-11-18); spawns `MapModel.EnemyModalBoss`, opens when it dies |
| `ShopRoom_Merchant`, `RestRoom_Campfire`, `BuffRoom_PowerShrine` | `ShopRoom` (5), `RestRoom` (6), `TreasureRoom` (4) | No marker; open on load |

Room selection per run (`BuildMazeRooms()`, since 2026-11-18): start cell = a `StartRoom`, end cell = a
`BossRoom` (by `roomType`, never by list position); the other cells get distinct random rooms with at least
one room per `_requiredRoomGroups` entry (default: Shop; Treasure / Rest / Puzzle / Secret).

## Scene (`LoadRandomMap.unity`)

```
Maze                         [MazeController]
├─ MainMap/Room              [RoomGridController, RoomGeneraterController]
│  ├─ EnemyManager           [EnemyManager, PathRequestManager, EnemySpawner]
│  └─ Grid                   [Grid]   (must stay at the origin)
│     └─ Level Manager       [LevelManager]  + 5 tilemaps
│        └─ Level Editor Tool [LevelEditorTool]  (EditorOnly)
└─ MiniMap                   [MapGridController]
GameLifetimeScope            [GameLifetimeScope]
```

`RoomGeneraterController` Inspector: `_fullDungeonRoomSO` = `Maze_Storage.asset`, `_levelManager`,
`_enemyManager`, `_requiredRoomGroups`. `EnemySpawner.mapModel` with `enemyModalBoss` set.

## Flow

```
MazeController.Awake → MazeGenerator.Generator(rows, cols) → cells
  → MapGrid / RoomGrid .Setting()
     RoomGeneraterController.Setting(start, end, cellCount):
       tiles + tilemaps ← _levelManager; BuildMazeRooms() → _mazeRooms
       new PathfindingGrid → _enemyManager.SetPathfindingGrid()
  → Emit(ON_LOAD_MAZE_DONE)

RoomGridController [ON_LOAD_MAZE_DONE] → OnDoneLoadRoomGrid()
  → LoadRoom(start) → PlayerManager.SetPlayerPosition(start room position)

RoomGeneraterController.LoadRoom(index, cell):
  read JSON of _mazeRooms[index] (File.ReadAllText(Application.dataPath…), Bug #15) → clear tilemaps
  per tile: door tiles kept only for maze directions; spawn tiles → spawnPositions
  if !cleared → SwapTileMap + build grid, then by roomType:
                 Shop / Rest / Treasure → Emit(ON_OPEN_DOOR)
                 BossRoom               → _enemyManager.SpawnBoss(spawnPositions)
                 other (Combat, Start)  → _enemyManager.SpawnRoom(spawnPositions)
  else → OpenDoors()

EnemySpawner.SpawnRoom → random RoomModel set, Entity.SetCharacterData(entry Data or prefab Data) → Emit(ON_DONE_SPAWN_ENEMY, n)
EnemySpawner.SpawnBoss → MapModel.EnemyModalBoss at one marker → Emit(ON_DONE_SPAWN_ENEMY, 1)
RoomCell: EnemyCount reaches 0 → Emit(ON_ROOM_CLEARED) (no subscriber), finally Emit(ON_OPEN_DOOR)

PedestalChampionController (start room, Champion confirmed) → Emit(ON_OPEN_DOOR)
ON_OPEN_DOOR → RoomGridController.DeleteDoorTileMap (returns early if IsCleared) → RoomCell.OpenDoors
  (OpenDoors() sets IsCleared = true since 5d1986db)
DoorController.OnTriggerEnter2D (Player, OPEN) → Emit(ON_PLAYER_ON_DOOR, dir)
  → RoomGridController.ClearRoom → OnLoadMap(dir) → LoadRoom(next) → SetNextRoom (player moved) → Emit(ON_LOAD_MAP)
  → MapGridController.Move (minimap)
```

Wiring: `RoomGeneraterController.Construct(IPlayerService)` and `RoomGridController.Construct(PlayerManager)` are
injected; `LevelManager` and `EnemyManager` are Inspector references (no DI, since 2026-11-18).
`ON_GET_SPAWN_POSITIONS` is no longer emitted or subscribed (kept in the enum).

`STATUS_DOOR`: 0 `DISABLE`, 1 `ENEBLE` (locked), 2 `BE_OPEN`, 3 `OPEN` (collider on), 4 `CLOSE`.

## Open issues

Statuses follow the bug files; the 2026-11-18 changes are waiting for the Saturday triage (inbox NOTE-20261118-3).

| Bug | Summary |
|-----|---------|
| #14 | `MazeController.Awake()` missing `return` after `Destroy` |
| #15 | Room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only |
| #16 / BUG-097 | Start / end by list position — fixed in code and data 2026-11-18 (`BuildMazeRooms()`, Champion moved, boss marker added); needs a Play Mode check |
| #17 | Dead door-gating code (`DoorController.OpenDoor()` etc.) |
| BUG-096 | S1 — marker-less rooms never open. Since 2026-11-18 the start room holds the Champion and the boss room has a marker, so every room type has a way to open; file still Open, needs a Play Mode check |
| TD-027 | `BaseGrid.GetNext()` no bounds check |
