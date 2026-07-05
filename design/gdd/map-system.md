---
status: reverse-documented
source: Assets/Script/Map/
date: 2026-05-19
updated: 2026-07-02
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

Two parallel grids run simultaneously: a **world grid** — `RoomGridController` (event hub,
extends `BaseGrid<RoomCell>`) working with `RoomGeneraterController` (tilemap loader; same
GameObject, wired via `RequireComponent`) — and a **minimap grid** (`MapGridController`) that
tracks the player's position in a small overlay.

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
| 0 | `DISABLE` | No passage in this direction (maze wall) | Off |
| 1 | `ENEBLE` | Passage exists — carver side (earlier-visited cell in DFS) | Off |
| 2 | `BE_OPEN` | Passage exists — receiver side | Off |
| 3 | `OPEN` | Passable — player can walk through | **On** |
| 4 | `CLOSE` | Sealed at runtime | Off |

`DoorController` collider is enabled only when `Status == OPEN`. `ENEBLE` and `BE_OPEN` are
**maze-generation / minimap semantics only** (clarified 2026-07-02): the DFS carver marks the
earlier-visited side of each passage `ENEBLE` and the receiver side `BE_OPEN`; the minimap uses
this asymmetry to draw each connector exactly once. Room-door gating uses only `OPEN`/`CLOSE`,
always in bulk: `RoomCell.OpenDoors()` on room clear / re-entry, `RoomCell.CloseDoor()` on leave.
Per-door open methods exist in code but are dead — see **[BUG #17]** under Room Transition.

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

On each run, `RoomGridController.Setting()` → `RoomGeneraterController.Setting()` picks a unique random subset of rooms:

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

**[BUG #16]** `RoomFile.roomType` is never read at runtime — start/end rooms are forced purely
by list position (`room[0]` / `room[last]`, RoomGeneraterController.cs:43-44). Reordering
`Maze_Storage.asset` silently breaks start/boss room selection.

### Door Tile Resolution (LoadRoom)

Each room JSON contains `Tile_Door` tiles at all four cardinal walls.
`RoomGeneraterController.LoadRoom()` resolves which doors to keep vs. wall off based on the
cell's actual passages:

```
for each tile in LevelData:
  if tile.name == "Tile_Door" && !roomCell.IsCleared:
    direction = Utility.ToCardinalDirection(tile.position)
    if direction IN roomCell.ListDirectionDoors:
      → keep tile; record in DoorPoints + IndexLevelDataDoor
    else:
      → swap tile to "Tile_Room" (wall); save to SwapLevelData
```

After tiling, `SwapTileMap()` corrects positions of swapped wall tiles.
`roomCell.SetDoorPoints(DoorPoints)` repositions `DoorController` transforms to the average
centre of each door tile cluster.

**Cleared room re-entry:** if `RoomCell.IsCleared == true`, `LoadRoom()` uses `roomCell.Data`
(cached `LevelData`) instead of reading the JSON file again — preserving the room state — and
calls `OpenDoors()` so all doors are immediately passable.

**[BUG #15]** Room JSON is read with `File.ReadAllText(Application.dataPath + filePath)`
(RoomGeneraterController.cs:57; same pattern in LevelManager.cs) — Editor-only. `Assets/Data/Json/`
is not packaged into Player builds; must move to `TextAsset` references or StreamingAssets before
the first standalone build.

### Room Transition

```
DoorController.OnTriggerEnter2D():
  tag == "Player" && Status == OPEN
  → EventManager.Emit(ON_PLAYER_ON_DOOR, (Vector2)direction)

RoomGridController [ON_PLAYER_ON_DOOR] → ClearRoom(direction):
  1. Cache current room state into RoomCell:
       roomCell.Data ← current LevelData
       roomCell.DoorPoints / roomCell.IndexLevelDataDoor ← door tile bookkeeping
       roomCell.IsCleared = true             [temporary semantics — see Room-Clear Locking]
  2. roomCell.CloseDoor() — ALL doors → CLOSE
  3. Clear all tilemaps; reset SwapLevelData / DoorPoints / IndexLevelDataDoor
  4. OnLoadMap(direction)

RoomGridController.OnLoadMap(direction):
  1. _next = GetNext(direction)              [BaseGrid: invert Y, CaculateIndex]
  2. index = CaculateIndex(_next.GetGridPosition())
  3. RoomGeneraterController.LoadRoom(index, _next)
  4. _next.GetStartDoorPosition(-direction)  [computes StartDoorPosition only — its OpenDoor() call is a no-op, BUG #17]
  5. _current.UpdateStatusDoor(direction)    [no-op — dead code, BUG #17]
  6. fastMovement.position = _next.StartDoorPosition
  7. _current = _next; _next = null
  8. Emit(ON_LOAD_MAP, index)               [MapGridController moves the minimap avatar]
```

**Entry door position formula:**
```
entryPosition = door.transform.position - direction × PADDING_DOOR_TELE_SCALE
PADDING_DOOR_TELE_SCALE = 2f × LENGTH_ROOM / 10 = 2.0 units inward
```

**Note:** the door trigger fires only when `Status == OPEN` (collider disabled otherwise). Doors
start at `ENEBLE`/`BE_OPEN` and become passable only when `ON_CLEAR_ENEMY` triggers `OpenDoors()`.
After leaving a room, ALL of its doors are `CLOSE`; they reopen in bulk on re-entry
(`IsCleared` branch of `LoadRoom`). Backtracking through cleared rooms is **intended design**
(confirmed 2026-07-02).

**[BUG #13]** The player is never teleported into the start room: the teleport line in
`RoomGridController.OnDoneLoadRoomGrid()` is commented out (RoomGridController.cs:56) and
`RoomGeneraterController.OnDoneLoadRoomGrid()` (which performs the teleport) is never called.
The start room has no entry door, so `StartDoorPosition` needs its own computation.

**[BUG #14]** `MazeController.Awake()` is missing `return` after `Destroy(gameObject)`
(MazeController.cs:17-21) — a duplicate instance still overwrites `Instance` and re-runs the
generator.

**[BUG #17]** Dead code: `DoorController.OpenDoor()` / `CheckCanBeOpened()` and
`RoomCell.UpdateStatusDoor()` are no-ops — door instances are only created for non-DISABLE
directions, so the `Status == DISABLE` guard never passes; `OpenDoor()` also bypasses the
collider sync in `SetStatus()`. Real gating is exclusively `OpenDoors()`/`CloseDoor()`.
Remove these methods to stop misleading readers.

### Room-Clear Locking **[PARTIAL — event wired, lock-on-entry not yet implemented]**

**Implemented (2026-06-04):**
- `EventID.ON_CLEAR_ENEMY` exists in the enum.
- `RoomGridController [ON_CLEAR_ENEMY] → DeleteDoorTileMap()`:
  - Erases the door tiles (via `IndexLevelDataDoor`) from the tilemap — reveals the exits.
  - Calls `RoomCell.OpenDoors()` → sets every `DoorController.Status = OPEN`.
- `RoomCell.CloseDoor()` / `OpenDoors()` implemented — callable directly.

**Still missing [GAP]:**
1. Doors are not locked when the player enters a room — nothing calls `CloseDoor()` on entry.
2. Enemy count is not tracked — nothing knows when to emit `ON_CLEAR_ENEMY`.
3. `EntityDeathState` emits no events (and is not a usable state — wrong base class, Bug #7).
4. The ONLY producer of `ON_CLEAR_ENEMY` is the editor debug button
   (LevelManagerEditor.cs:23-26) — in a real run, doors never open and the player is stuck
   in the first room.

**Agreed spawn architecture (2026-07-02) [PLANNED]:**
- **Spawn points**: `Tile_Spawn` marker tiles authored in room tilemaps, serialized into room
  JSON by the existing save path (`GameConstants.TileName.SPAWN` constant exists, currently
  unused and absent from all 13 room JSONs).
- **Encounter data**: `EncounterSO` keyed by `RoomType` — enemy pool entries
  (prefab + `EntityData` + weight); single wave for the demo, wave list in the schema for later.
- **`RoomEnemySpawner`**: listens `ON_LOAD_MAP`; spawns at markers; locks doors (`CloseDoor()`);
  tracks alive count via new `ON_ENEMY_DEATH`; emits existing `ON_CLEAR_ENEMY` when the count
  reaches zero (immediately for enemy-less rooms).
- **`RoomCell.IsCleared` decision (2026-07-02)**: will mean "enemies defeated", set on
  `ON_CLEAR_ENEMY`. The current set-on-leave behavior (`ClearRoom` on door transition) is
  temporary and will move when the spawner lands.

**Required EventID additions (still needed):**
- `ON_ENEMY_DEATH` (payload: none or Vector2 position) — per-enemy granular event
- `ON_ROOM_CLEAR` (payload: none) — fires when all enemies dead → triggers upgrade screen

### Minimap

**[IMPLEMENTED 2026-07-02 — Bug #11 fixed]** `MapGridController` maintains a `MapCell` grid —
one cell per room. On `ON_LOAD_MAZE_DONE` it pops the `Avatar` in at the start cell (DOTween
scale-in); on `ON_PLAYER_ON_DOOR` (`Move`) it tweens the avatar to the next cell.
`MapCell.VisitRoom()` reveals a cell the first time the player enters it; unvisited cells
stay hidden.

Connector de-duplication: each passage's connector (child objects `_top/_left/_right/_bottom`)
is drawn only by the cell holding the `ENEBLE` side — the two sides would overlap at the same
world point (cells sit 6 units apart; each connector sits 3 units from its cell centre).
Because the `ENEBLE` side is assigned by DFS generation order, some connectors of the current
room appear only after the neighbouring room has been visited — **accepted behavior**
(confirmed 2026-07-02).

Map cell positioning:
```
cellPosition = mapGrid.transform.position + (Column, -Row) × CELL_SCALE × 2 units
```

Door visualization per cell: four child GameObjects, activated when that direction's maze
status is `ENEBLE` (see connector de-duplication above).

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
| GetNext() for a cell at column 0 moving left | **[BUG — latent]** No bounds check (re-verified 2026-07-02, BaseGrid.cs:34-41) — negative/wrapping index → `IndexOutOfRangeException`. Masked today: edge cells never carve outward passages, so only reachable via misuse | Guard: clamp position within `(0,0)` to `(Cols-1, Rows-1)` before index calculation |
| Room-clear doors lock on entry | **[GAP]** Doors always passable — player can leave without clearing room | Implement `RoomCell.LockRoom()` on `ON_PLAYER_ON_DOOR` |
| Enemy count reaches 0 but no event fires | **[GAP]** `ON_ENEMY_DEATH` not in `EventID` enum — death state has no emission | Add to enum; entity death state emits it |
| Doors of the previous room after transition | ALL set to `CLOSE` on leave; reopened in bulk on re-entry (`IsCleared` branch) | ✓ Acceptable — backtracking through cleared rooms allowed by design (2026-07-02) |
| Scene starts — player position in start room | **[BUG #13]** No teleport (line commented out); `StartDoorPosition` = (0,0,0) | Re-enable teleport; compute start position without an entry door |
| Two `MazeController` instances in scene | **[BUG #14]** Duplicate destroys itself but still overwrites `Instance` and re-runs the generator | Add `return` after `Destroy(gameObject)` |
| Standalone Player build | **[BUG #15]** Room JSON read from `Application.dataPath` — files absent in build → load failure | Move to `TextAsset` refs or StreamingAssets |
| Player re-enters a cleared room | Doors already open; no enemies → room-clear instant | ✓ Acceptable — no lock triggered if `enemyCount == 0` |
| `LevelData.tiles` serialization | **[BUG — potential]** `JsonUtility` cannot serialize `TileBase` references by value — JSON round-trip may fail or produce null tiles on load | Verify in editor; may need a tile-by-name lookup table instead |
| `fastMovement` null reference | `RoomNavigator` requires `FastMovement` field wired in Inspector — if unset, teleport silently fails | Add null check + warning log |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY` **[IMPLEMENTED]**, `ON_ENEMY_DEATH` **[GAP]**, `ON_ROOM_CLEAR` **[GAP]** | Map → EventManager |
| **Character system** | `DoorController` tags player via "Player" tag; `fastMovement` is the player transform for teleport | Map → Character |
| **Enemy AI** | `EntityDeathState` must emit `ON_ENEMY_DEATH`; planned `RoomEnemySpawner` tracks the count and emits `ON_CLEAR_ENEMY` when all enemies in the room are dead | Enemy → Map |
| **Skill/Ability + Weapons** | No direct dependency | — |
| **LevelManager** | `RoomGeneraterController.Setting()` calls `LevelManager.GetDungeonRoomSO()`, `GetTileSOs()`, `GetTilemaps()`; `LevelManager` must exist in the scene (singleton — Bug #12) | Map → LevelEdit |
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
| Entry teleport offset | `GameConstants.SettingStats.PADDING_DOOR_TELE_SCALE` | **2.0** units | `= 2f × LENGTH_ROOM / 10` — inward spawn distance from the entry door |
| Room pool — Inspector | `RoomGeneraterController._fullDungeonRoomSO` | `Maze_Storage.asset` | Full pool — all authored rooms |
| Room pool — runtime | `RoomGeneraterController._dungeonRoomSO` | `Maze_Load_Room.asset` | Cleared and refilled every run — do not edit directly |

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
- [ ] No `IndexOutOfRangeException` when navigating any valid maze path — bounds check missing in `GetNext()` (latent)
- [ ] Room JSON loads in a standalone Player build (Bug #15)
- [ ] `RoomType` drives start/boss room selection instead of list position (Bug #16)

### Room Transitions
- [x] Walking into an `OPEN` door triggers `ON_PLAYER_ON_DOOR`
- [x] Player teleports to entry door of next room (no visible cross-room travel)
- [ ] Player teleports into the START room on maze load (Bug #13 — currently commented out)
- [x] Doors of previous room close on leave and reopen on re-entry — backtrack by design
- [x] Minimap avatar updates on every transition (Bug #11 fixed 2026-07-02)
- [ ] Doors cannot be traversed before room is cleared — lock-on-entry not implemented
- [ ] `MazeController` duplicate-instance guard (`return` after `Destroy`, Bug #14)

### Room-Clear Locking **[PARTIAL — spawn architecture agreed 2026-07-02]**
- [x] `ON_CLEAR_ENEMY` event defined; `DeleteDoorTileMap()` + `OpenDoors()` implemented
- [ ] Doors lock when player enters an uncleared room — `RoomEnemySpawner` calls `CloseDoor()`
- [ ] `RoomEnemySpawner` spawns from `Tile_Spawn` markers + `EncounterSO` per `RoomType`
- [ ] Enemy death emits `ON_ENEMY_DEATH`; spawner emits `ON_CLEAR_ENEMY` at zero alive
- [ ] `ON_ROOM_CLEAR` fires when all enemies dead → upgrade screen
- [x] Re-entering a cleared room does not re-lock doors (`IsCleared` branch calls `OpenDoors()`)
- [ ] `IsCleared` set on `ON_CLEAR_ENEMY` (= enemies defeated) instead of on room-leave
- [ ] Dead door methods removed — `OpenDoor()`, `CheckCanBeOpened()`, `UpdateStatusDoor()` (Bug #17)

### Minimap **[IMPLEMENTED 2026-07-02]**
- [x] Avatar position matches player's current room on every transition (DOTween)
- [x] Visited cells revealed via `MapCell.VisitRoom()`; unvisited cells hidden
- [x] Connector reveal follows DFS-side ownership — accepted behavior

### Dead Code Removal
- [ ] `Assets/Script/Map/Legacy/Door.cs` removed from project
- [ ] `Assets/Script/Map/Legacy/Room.cs` removed from project
