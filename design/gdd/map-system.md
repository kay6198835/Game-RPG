---
status: reverse-documented
source: Assets/Script/Map/
date: 2026-05-19
updated: 2026-06-04
verified-by: Kiet
---

# Map & Dungeon System Design

> **Note**: Reverse-engineered from existing implementation. Captures current behaviour
> and clarified design intent. Sections marked **[GAP]** describe intended design not yet
> implemented. Sections marked **[BUG]** identify known defects.

**Status**: In Design

*Covers two systems from the systems index: Dungeon Generation (system #7) and Room Progression (system #11).*

---

## Overview

The map system generates a procedural dungeon each run and manages the player's
progression through it. A DFS algorithm produces a perfect maze (no loops, no dead ends)
of interconnected rooms. Each room is a pre-authored tilemap layout loaded from JSON.
The player navigates by walking through doors; room-clear locks and unlocks doors as
enemies are defeated.

Two parallel grids run simultaneously: a **world grid** (`RoomGridController`) that places
playable rooms in 3D space, and a **minimap grid** (`MapGridController`) that tracks the
player's position in a small overlay.

---

## Player Fantasy

The dungeon should feel unknown but learnable. The player doesn't know what's in the
next room — but after a few runs, they start recognising the maze structure and planning
routes. Each room is a contained arena: the doors lock, you fight your way through, the
doors open, and you press forward.

There's a moment of tension as doors seal and enemies spawn. Clearing a room feels
rewarding: doors open, upgrade cards appear, the dungeon opens up a little more.

---

## Detailed Rules

### Dungeon Generation

On scene load, `MazeController.Awake()` runs the DFS generator and builds both grids.

**Algorithm: Depth-First Search (Recursive Backtracking)**
1. Initialise all cells unvisited, all walls present
2. Pick a **random** start cell (not (0,0)); mark visited; push to stack. Store as `MazeGenerator.Start`.
3. While stack not empty:
   - Peek current cell
   - Collect all unvisited adjacent neighbours
   - If none: pop stack (backtrack)
   - Else: pick one at random; carve passage between them; push neighbour; track as `MazeGenerator.End`
4. Output: `Cell[]` flat array — every cell has `Top/Bottom/Left/Right` door status

`MazeController.GetCellStart()` / `GetCellEnd()` expose Start and End to other systems.
`RoomGridController` uses these to fix the start room to template index 0 and the end room to the last template.

**Default grid size for demo: 4×4 (16 rooms)**
Set via `MazeController.Rows = 4`, `MazeController.Columns = 4`.

**Door status semantics:**
| Value | Status | Meaning | Collider |
|-------|--------|---------|----------|
| 0 | `DISABLE` | Hướng này không có door trong maze | Off |
| 1 | `ENEBLE` | Door tồn tại, đang bị khóa (combat) | Off |
| 2 | `BE_OPEN` | Receiver side — passage từ phía kia carve | Off |
| 3 | `OPEN` | Passable — player đi qua được | **On** |
| 4 | `CLOSE` | Tường kín runtime | Off |

`DoorController` collider chỉ enabled khi `Status == OPEN`. `ENEBLE` và `BE_OPEN` là door
tồn tại nhưng chưa mở; gọi `SetStatus(OPEN)` để kích hoạt. Khi enemy cleared,
`RoomCell.OpenDoors()` gọi `SetStatus(OPEN)` cho toàn bộ door trong room.

### Room Placement

Each generated cell maps to a `RoomCell` in the world:
```
worldPosition = (cell.Column, -cell.Row) × GAME_SCALE × LENGTH_ROOM
             = (Column, -Row) × 3.0 × 10.0 = (Column, -Row) × 30 units
```

Each `RoomCell` has `DoorController` children — one per passable direction — positioned and
named via `DoorController.SetDirection(name)`. Only directions with `STATUS_DOOR != DISABLE`
get a `DoorController` instantiated.

**Room JSON files:** `Assets/Data/Json/Room/NormalRoom_0.json … NormalRoom_12.json` (13 rooms).
Format: `LevelData { List<string> tiles (tile id), List<Vector3Int> poses, List<int> layerIndices }`.
Tiles are identified by name matching `TileSO.id` (e.g. `"Tile_Room"`, `"Tile_Door"`, `"Tile_Floor"`).

**RoomType enum:** `NormalRoom`, `StartRoom`, `BossRoom`, `CombatRoom`, `TreasureRoom`,
`ShopRoom`, `RestRoom`, `PuzzleRoom`, `SecretRoom`, `ExitRoom`. Stored in `RoomFile.roomType`.

### Random Room Assignment

On each run, `RoomGridController.Setting()` picks a unique random subset of rooms:

```
1. _fullDungeonRoomSO = LevelManager.GetDungeonRoomSO()     // = Maze_Storage.asset (full pool)
2. randomMazeRoomsIndex = Utility.PickUniqueIndex(total, mazeSize)
   — Fisher-Yates shuffle on indices [1 .. totalRooms-2] (exclude index 0 and last)
   — returns exactly mazeSize unique indices
3. _dungeonRoomSO.room[i] = _fullDungeonRoomSO.room[randomMazeRoomsIndex[i]]
4. Force: room[_startIndex] = room[0]    (StartRoom template)
          room[_endIndex]   = room[last] (BossRoom / ExitRoom template)
```

Result: maze layout differs each run (random DFS start) AND room content differs (random file pool).

### Door Tile Resolution (LoadRoom)

Each room JSON contains `Tile_Door` tiles at all four cardinal walls. `LoadRoom()` resolves which
doors to keep vs. wall off based on the cell's actual passages:

```
for each tile in LevelData:
  if tile.name == "Tile_Door" && !roomCell.IsCleared:
    direction = Utility.ToCardinalDirection(tile.position)
    if direction IN roomCell.ListDirectionDoors:
      → keep tile; save to DoorPoints + CurentDoorLevelData
    else:
      → swap tile to "Tile_Room" (wall); save to SwapLevelData
```

After tiling, `SwapTileMap()` corrects positions of swapped wall tiles.
`roomCell.SetDoorPoints(DoorPoints)` repositions `DoorController` transforms to the average
centre of each door tile cluster.

**Cleared room re-entry:** if `RoomCell.IsCleared == true`, `LoadRoom()` uses `roomCell.Data`
(cached `LevelData`) instead of reading the JSON file again — preserving the room state.

### Room Transition

```
DoorController.OnTriggerEnter2D():
  tag == "Player" && Status == OPEN
  → EventManager.Emit(ON_PLAYER_ON_DOOR, (Vector2)direction)

RoomGridController [ON_PLAYER_ON_DOOR] → ClearRoom(direction):
  1. Cache current room state vào RoomCell:
       roomCell.Data ← current LevelData
       roomCell.CurentDoorLevelData ← door tile layer data
       roomCell.DoorPoints ← door positions
       roomCell.IsCleared = true
  2. Gọi roomCell.CloseDoor() — set tất cả doors → CLOSE
  3. Clear tất cả tilemaps; reset SwapLevelData / DoorPoints / CurentDoorLevelData
  4. Gọi OnLoadMap(direction)

RoomGridController.OnLoadMap(direction):
  1. _next = GetNext(direction)              [BaseGrid: invert Y, CaculateIndex]
  2. index = CaculateIndex(_next.GetGridPosition())
  3. LoadRoom(index, _next)
  4. _next.GetStartDoorPosition(-direction)  [open entry door; calc StartDoorPosition inward offset]
  5. _current.UpdateStatusDoor(direction)    [open exit door của room cũ]
  6. fastMovement.position = _next.StartDoorPosition
  7. _current = _next; _next = null
  8. Emit(ON_LOAD_MAP, index)               [MapGridController — WIP, currently no-op]
```

**Entry door position formula:**
```
entryPosition = door.transform.position - direction × PADDING_DOOR_TELE_SCALE
PADDING_DOOR_TELE_SCALE = 2f × LENGTH_ROOM / 10 = 2.0 units inward
```

**⚠️ Note:** `DoorController` trigger fires only when `Status == OPEN`. Doors bắt đầu ở `ENEBLE`/`BE_OPEN` — chưa passable cho đến khi `ON_CLEAR_ENEMY` gọi `OpenDoors()`.

### Room-Clear Locking **[PARTIAL — event wired, lock-on-entry not yet implemented]**

**Implemented (2026-06-04):**
- `EventID.ON_CLEAR_ENEMY` đã có trong enum.
- `RoomGridController [ON_CLEAR_ENEMY] → DeleteDoorTileMap()`:
  - Xóa door tile layer (`CurentDoorLevelData`) khỏi tilemap — lộ cửa ra.
  - Gọi `RoomCell.OpenDoors()` → set tất cả `DoorController.Status = OPEN`.
- `RoomCell.CloseDoor()` / `OpenDoors()` implemented — có thể gọi trực tiếp.

**Còn thiếu [GAP]:**
1. Doors chưa bị lock khi player vào room — `ON_PLAYER_ON_DOOR` không gọi `CloseDoor()`.
2. Enemy count không được track — không biết khi nào emit `ON_CLEAR_ENEMY`.
3. `EntityDeathState` chưa emit event nào.

**Flow đầy đủ cần implement:**
```
ON_PLAYER_ON_DOOR → RoomCell.CloseDoor()    [lock doors]
EntityDeathState  → check roomEnemyCount == 0
                  → Emit(ON_CLEAR_ENEMY)
ON_CLEAR_ENEMY    → DeleteDoorTileMap() + OpenDoors()  [already implemented]
```

**Required EventID additions (still needed):**
- `ON_ENEMY_DEATH` (payload: none or Vector2 position) — per-enemy granular event
- `ON_ROOM_CLEAR` (payload: none) — fires when all enemies dead → triggers upgrade screen

### Minimap

`MapGridController` maintains a `MapCell` grid — one cell per room, each showing which
doors are open. `MapTracker` moves an `Avatar` GameObject to the current cell position
on every room transition, giving the player a live position indicator.

Map cell positioning:
```
cellPosition = mapGrid.transform.position + (Column, -Row) × CELL_SCALE × 2 units
```

Door visualization per cell: four child GameObjects active/inactive based on door status.

---

## Formulas

```
# World positioning  (GAME_SCALE=3, LENGTH_ROOM=10)
roomWorldPos = (Column, -Row) × GAME_SCALE × LENGTH_ROOM
             = (Column, -Row) × 3.0 × 10.0 = (Column, -Row) × 30 units

cellMinimapPos = mapGrid.origin + (Column, -Row) × GAME_SCALE × LENGTH_CELL × 2
              = (Column, -Row) × 3.0 × 1.0 × 2 = (Column, -Row) × 6 units

# Entry door teleport offset
PADDING_DOOR_TELE_SCALE = 2f × LENGTH_ROOM / 10 = 2.0 units inward
entryPosition = doorTransform.position - direction × 2.0

# Grid index (flat array)
index = row × Columns + column

# Next cell after transition
nextPos.y = currentPos.y - direction.y   [Y is negated in GetNext]
nextPos.x = currentPos.x + direction.x
nextIndex = nextPos.y × Columns + nextPos.x

# Random room pick (excluding start/end templates)
randomIndices = Utility.PickUniqueIndex(totalRooms, mazeSize)
             = Fisher-Yates shuffle on [1 .. totalRooms-2], take first mazeSize
```

---

## Edge Cases

| Scenario | Current Behaviour | Correct Behaviour |
|----------|------------------|-------------------|
| Player at maze edge walks into a `CLOSE` door | Door trigger disabled → no event fires | ✓ Correct — CLOSE doors are non-interactive |
| GetNext() for a cell at column 0 moving left | **[BUG]** No bounds check — calculates negative index → `IndexOutOfRangeException` | Guard: clamp position within `(0,0)` to `(Cols-1, Rows-1)` before index calculation |
| Room-clear doors lock on entry | **[GAP]** Doors always passable — player can leave without clearing room | Implement `RoomCell.LockRoom()` on `ON_PLAYER_ON_DOOR` |
| Enemy count reaches 0 but no event fires | **[GAP]** `ON_ENEMY_DEATH` not in `EventID` enum — death state has no emission | Add to enum; entity death state emits it |
| Previous room exit door stays open after transition | Open door remains open visually | Acceptable — player sees where they came from |
| Player re-enters a cleared room | Doors already open; no enemies → room-clear instant | ✓ Acceptable — no lock triggered if `enemyCount == 0` |
| `LevelData.tiles` serialization | **[BUG — potential]** `JsonUtility` cannot serialize `TileBase` references by value — JSON round-trip may fail or produce null tiles on load | Verify in editor; may need a tile-by-name lookup table instead |
| `fastMovement` null reference | `RoomNavigator` requires `FastMovement` field wired in Inspector — if unset, teleport silently fails | Add null check + warning log |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY` **[IMPLEMENTED]**, `ON_ENEMY_DEATH` **[GAP]**, `ON_ROOM_CLEAR` **[GAP]** | Map → EventManager |
| **Character system** | `DoorController` tags player via "Player" tag; `fastMovement` is the player transform for teleport | Map → Character |
| **Enemy AI** | `EntityDeathState` cần emit `ON_CLEAR_ENEMY` (hoặc `ON_ENEMY_DEATH`) khi tất cả enemy trong room chết | Enemy → Map |
| **Skill/Ability + Weapons** | No direct dependency | — |
| **LevelManager** | `RoomGridController` gọi `LevelManager.GetDungeonRoomSO()`, `GetTileSOs()`, `GetTilemaps()` trong `Setting()`; `LevelManager` phải có trong scene | Map → LevelEdit |
| **Per-Run Upgrades** | Upgrade card selection triggered by `ON_ROOM_CLEAR` | Map → Progression |

---

## Tuning Knobs

All values in `GameConstants.SettingStats` or `MazeController` Inspector fields.

| Parameter | Field | Default | Effect |
|-----------|-------|---------|--------|
| Maze rows | `MazeController.Rows` | 3 (target: **4**) | Height of dungeon in rooms |
| Maze columns | `MazeController.Columns` | 3 (target: **4**) | Width of dungeon in rooms |
| Room world size | `GameConstants.SettingStats.LENGTH_ROOM` | **10** units | Gap between room centres (before GAME_SCALE) |
| Cell minimap size | `GameConstants.SettingStats.LENGTH_CELL` | **1** unit | Scale of minimap cells |
| Global scale | `GameConstants.SettingStats.GAME_SCALE` | **3.0** | Multiplier on all positions — rooms are 30 units apart |
| Entry teleport offset | `GameConstants.SettingStats.PADDING_DOOR_TELE_SCALE` | **2.0** units | `= 2f × LENGTH_ROOM / 10` — khoảng cách spawn vào trong room |
| Room pool — Inspector | `RoomGridController._fullDungeonRoomSO` | `Maze_Storage.asset` | Full pool — tất cả room đã author |
| Room pool — runtime | `RoomGridController._dungeonRoomSO` | `Maze_Load_Room.asset` | Được clear và fill mỗi run — không edit trực tiếp |

---

## Acceptance Criteria

### Dungeon Generation
- [x] Maze generates without repeating the same layout every run — random DFS start cell
- [x] All rooms reachable — inherent to DFS spanning tree algorithm
- [x] Door states correctly reflect carving direction
- [x] Maze generates before rooms populated (MazeController.Awake → Start order)
- [x] Start room = index 0 template; End room = last template; middle rooms random

### Room Loading
- [x] `LoadRoom()` reads JSON by tile name, sets tiles on correct layer
- [x] Door tiles not matching cell's actual passages are swapped to wall tiles
- [x] `DoorController` transforms repositioned to average door tile cluster centre
- [x] Cleared rooms use cached `LevelData` instead of re-reading JSON
- [ ] No `IndexOutOfRangeException` when navigating any valid maze path — bounds check missing in `GetNext()`

### Room Transitions
- [x] Walking into an `OPEN` door triggers `ON_PLAYER_ON_DOOR`
- [x] Player teleports to entry door of next room (no visible cross-room travel)
- [x] Exit door of previous room visually opens after transition
- [ ] Minimap avatar updates — `MapGridController` WIP (Bug #11)
- [ ] Doors cannot be traversed before room is cleared — lock-on-entry not implemented

### Room-Clear Locking **[PARTIAL]**
- [x] `ON_CLEAR_ENEMY` event defined; `DeleteDoorTileMap()` + `OpenDoors()` implemented
- [ ] Doors lock when player enters room — `CloseDoor()` not called on `ON_PLAYER_ON_DOOR`
- [ ] Enemy death emits `ON_CLEAR_ENEMY` — EntityDeathState does not emit
- [ ] `ON_ROOM_CLEAR` fires when all enemies dead → upgrade screen
- [ ] Re-entering a cleared room does not re-lock doors (RoomCell.IsCleared check needed)

### Minimap **[WIP]**
- [ ] Avatar position matches player's current room on every transition
- [ ] Room grid layout visible after maze generation

### Dead Code Removal
- [ ] `Assets/Script/Map/Legacy/Door.cs` removed from project
- [ ] `Assets/Script/Map/Legacy/Room.cs` removed from project
