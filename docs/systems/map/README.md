# Map — Maze, Rooms, Doors, Level Editor

> **Status:** live · **Last verified:** 2026-11-05, `sprint-21` `028fdb90` (room-open flow only)
> History: [CHANGELOG.md](CHANGELOG.md) · GDD: `design/gdd/map-system.md` · Rules: `.claude/rules/map-code.md`

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
| `Map/Room/` | `RoomCell` (room-clear counter), `RoomGridController` (event hub), `RoomGeneraterController` (tilemap loader), `Door/DoorController` |
| `Map/Legacy/` | `Door`, `Room` — superseded |
| `LevelEdit/LevelManager.cs` | Editor save/import + runtime room/tile/tilemap queries |
| `Assets/Data/Json/Room/` | 20 room layouts |
| `Assets/SO/Dungeon/` | `Maze_Storage.asset` (full room pool), `Maze_Load_Room.asset` |

## Room set (since 2026-10-04)

`StartRoom_Entrance`, `CombatRoom_{CentralIsland, Checkerboard, Cross, DiagonalWalls, EliteGuard,
FourPillars, GrandArena, HiddenCorner, NarrowBridges, PracticeYard, RoundArena, SmallMaze, Spiral,
TwinCorridors, TwoHalls}`, `BuffRoom_PowerShrine`, `RestRoom_Campfire`, `ShopRoom_Merchant`,
`BossRoom_ThroneArena`. Start/Boss/Rest/Shop/Buff rooms carry no `Tile_Spawn_Enemy` marker. Since
`6094aa10` (2026-11-04) Shop (`roomType` 5), Rest (6) and Buff (authored as `TreasureRoom`, 4) open
their doors on load; Start opens through the Champion; **Boss still never opens** (BUG-096, file Open).
`Maze_Storage.asset` is alphabetical, so `room[0]` is the Boss room (BUG-097).

## Flow

```
MazeController.Awake → MazeGenerator.Generator(rows, cols) → cells
  → MapGrid / RoomGrid .Setting()
     RoomGeneraterController.Setting(): room pool from injected LevelManager;
       start = room[0], end = room[last] (by list position, Bug #16); new PathfindingGrid → EnemyManager
  → Emit(ON_LOAD_MAZE_DONE)

RoomGridController [ON_LOAD_MAZE_DONE] → OnDoneLoadRoomGrid()
  → LoadRoom(start) → PlayerManager.SetPlayerPosition(start room position)

RoomGeneraterController.LoadRoom(index, cell):
  read JSON (File.ReadAllText(Application.dataPath…), Bug #15) → clear tilemaps
  per tile: door tiles kept only for maze directions; spawn tiles → spawnPositions
  if !cleared → SwapTileMap + build grid
               + roomType in {ShopRoom, RestRoom, TreasureRoom} ? Emit(ON_OPEN_DOOR) : Emit(ON_GET_SPAWN_POSITIONS)
  else → OpenDoors()
  (no zero-spawn branch for other types: a Boss room with no marker never gets ON_OPEN_DOOR — BUG-096)

EnemySpawner spawns per entry, then Entity.SetCharacterData(entry Data or prefab Data) → Emit(ON_DONE_SPAWN_ENEMY)
RoomCell: EnemyCount reaches 0 → Emit(ON_ROOM_CLEARED) (no subscriber), finally Emit(ON_OPEN_DOOR)

PedestalChampionController (start room, Champion confirmed) → Emit(ON_OPEN_DOOR)
ON_OPEN_DOOR → RoomGridController.DeleteDoorTileMap (returns early if IsCleared) → RoomCell.OpenDoors
  (OpenDoors() sets IsCleared = true since 5d1986db)
DoorController.OnTriggerEnter2D (Player, OPEN) → Emit(ON_PLAYER_ON_DOOR, dir)
  → RoomGridController.ClearRoom → OnLoadMap(dir) → LoadRoom(next) → Emit(ON_LOAD_MAP)
  → MapGridController.Move (minimap; listener re-enabled 2026-09-30)
```

Dependencies are injected: `RoomGeneraterController.Construct(IPlayerService, LevelManager)`,
`RoomGridController.Construct(PlayerManager)`. Scene needs `LevelManager`, `EnemyManager`,
`EnemySpawner`, `GameLifetimeScope`.

`STATUS_DOOR`: 0 `DISABLE`, 1 `ENEBLE` (locked), 2 `BE_OPEN`, 3 `OPEN` (collider on), 4 `CLOSE`.

## Open issues

| Bug | Summary |
|-----|---------|
| #14 | `MazeController.Awake()` missing `return` after `Destroy` |
| #15 | Room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only |
| #16 | Start/end by list position; `RoomType` is read only for auto-open rooms (since `6094aa10`) |
| #17 | Dead door-gating code (`DoorController.OpenDoor()` etc.) |
| BUG-096 | S1 — marker-less rooms never open; fix `40d2c793` reverted by `ac13ee4f`. File still Open; since `6094aa10` only the Boss room is left in code |
| BUG-097 | S2 — alphabetical `Maze_Storage.asset`: start cell = Boss room, end cell = Start room |
| TD-027 | `BaseGrid.GetNext()` no bounds check |
