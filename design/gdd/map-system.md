---
status: reverse-documented
source: Assets/Script/Map/
date: 2026-05-19
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
2. Start at cell (0, 0); mark visited; push to stack
3. While stack not empty:
   - Peek current cell
   - Collect all unvisited adjacent neighbours
   - If none: pop stack (backtrack)
   - Else: pick one at random; carve passage between them; push neighbour to stack
4. Output: `Cell[]` flat array — every cell has `Top/Bottom/Left/Right` door status

**Default grid size for demo: 4×4 (16 rooms)**
Set via `MazeController.Rows = 4`, `MazeController.Columns = 4`.

**Door status semantics:**
| Status | Meaning | Visual |
|--------|---------|--------|
| `CLOSE` | Wall — no passage | Door object disabled |
| `OPEN` | Passage, initiator side | Red |
| `BE_OPEN` | Passage, receiver side | White |

Both `OPEN` and `BE_OPEN` are passable. The distinction records which cell carved the
passage during generation (used for visual differentiation only).

### Room Placement

Each generated cell maps to a `RoomCell` in the world:
```
worldPosition = (cell.Column, -cell.Row) × GAME_SCALE × LENGTH_ROOM
             = (Column, -Row) × 1.0 × 10.0 units
```

Each `RoomCell` has four `DoorController` children positioned at cardinal offsets:
- Top: `Vector3.up`
- Bottom: `Vector3.down`
- Left: `Vector3.left`
- Right: `Vector3.right`

Room interiors are loaded from pre-authored JSON tilemaps (`Assets/Data/Json/Room/room_N.json`)
via `LevelManager.LoadLevel(index)`. Each JSON file stores tile references and their
tilemap grid positions.

### Room Transition

When the player touches a door trigger:

```
Player collides with DoorController
  → DoorController.OnTriggerEnter2D(): tag == "Player" && status != CLOSE
  → EventManager.Emit(ON_PLAYER_ON_DOOR, (Vector2)direction)

Both listeners fire:
  RoomNavigator.Move(direction)
    → RoomGrid.GetNext(direction)   [inverts Y for coordinate conversion]
    → RoomGridController.OnAfterGetNext()
        → open exit door of current room
        → open entry door of next room
    → teleport fastMovement.position to next room's entry door position

  MapTracker.Move(direction)
    → MapGrid.GetNext(direction)
    → LoadRoom(): _current = _next; move avatar to current cell position
    → Emit(ON_LOAD_MAP) → MapGridController syncs _current
```

**Entry door position formula:**
```
entryPosition = nextRoom.GetDoor(-direction).transform.position
             + (direction × PADDING_DOOR_TELE_SCALE)
where PADDING_DOOR_TELE_SCALE = GAME_SCALE × LENGTH_ROOM / 10 = 1.1 units inward
```

### Room-Clear Locking **[GAP — not yet implemented]**

Doors lock when the player enters a room and unlock when all enemies are defeated.

**RoomCell tracks enemy count:**
1. On player entry (`DoorController` trigger fires `ON_PLAYER_ON_DOOR`):
   - `RoomCell.LockRoom()` — closes all non-entry doors; sets `enemyCount`
2. Each time an enemy dies: EventManager fires `ON_ENEMY_DEATH`
   - `RoomCell` listens: decrements `enemyCount`
3. When `enemyCount == 0`: `RoomCell.UnlockRoom()` opens all passable doors; emits `ON_ROOM_CLEAR`

**Required EventID additions:**
- `ON_ENEMY_DEATH` (payload: none or enemy position)
- `ON_ROOM_CLEAR` (payload: none)

**Required RoomCell fields (not yet present):**
```csharp
private int _enemyCount;
public void LockRoom(int enemyCount) { ... }
public void OnEnemyDeath() { ... }   // decrements, checks 0, calls UnlockRoom
public void UnlockRoom() { ... }
```

**Door locking rule:** Only `CLOSE` doors stay impassable. `OPEN`/`BE_OPEN` doors that
aren't the entry door become temporarily impassable during combat.

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
# World positioning
roomWorldPos = (Column, -Row) × GAME_SCALE × LENGTH_ROOM
             = (Column, -Row) × 1.0 × 10.0   [units]

cellMinimapPos = mapGrid.origin + (Column, -Row) × GAME_SCALE × LENGTH_CELL × 2
              = (Column, -Row) × 1.0 × 2.0   [units]

# Entry door teleport offset
entryOffset = GAME_SCALE × LENGTH_ROOM ÷ 10 × 1.1f = 1.1 units inward

# Grid index (flat array)
index = row × Columns + column

# Next cell after transition
nextPos   = currentPos + direction  [with Y negated]
nextIndex = nextPos.y × Columns + nextPos.x
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
| **Event Manager** | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_ENEMY_DEATH` **[GAP]**, `ON_ROOM_CLEAR` **[GAP]** | Map → EventManager |
| **Character system** | `DoorController` tags player via "Player" tag; `fastMovement` is the player transform for teleport | Map → Character |
| **Enemy AI** | Each enemy in a room increments `RoomCell.enemyCount` on spawn; emits `ON_ENEMY_DEATH` on death | Enemy → Map |
| **Skill/Ability + Weapons** | No direct dependency | — |
| **LevelManager** | Loads room tilesets from JSON; called by `RoomGridController` on map load | Map → LevelEdit |
| **Per-Run Upgrades** | Upgrade card selection triggered by `ON_ROOM_CLEAR` | Map → Progression |

---

## Tuning Knobs

All values in `GameConstants.SettingStats` or `MazeController` Inspector fields.

| Parameter | Field | Default | Effect |
|-----------|-------|---------|--------|
| Maze rows | `MazeController.Rows` | 3 (target: **4**) | Height of dungeon in rooms |
| Maze columns | `MazeController.Columns` | 3 (target: **4**) | Width of dungeon in rooms |
| Room world size | `GameConstants.LENGTH_ROOM` | 10 units | Gap between room centres |
| Cell minimap size | `GameConstants.LENGTH_CELL` | 1 unit | Scale of minimap cells |
| Global scale | `GameConstants.GAME_SCALE` | 1.0 | Multiplier on all positions |
| Entry offset | `PADDING_DOOR_TELE_SCALE × 1.1f` | 1.1 units | How far inside room the player spawns |

---

## Acceptance Criteria

### Dungeon Generation
- [ ] A 4×4 maze generates without repeating the same layout every run
- [ ] All rooms are reachable from room (0,0)
- [ ] No isolated rooms (perfect maze — spanning tree)
- [ ] Door states (OPEN/BE_OPEN/CLOSE) correctly reflect carving direction
- [ ] Maze generates before any room is populated (Awake order: MazeController first)

### Room Transitions
- [ ] Walking into an `OPEN` or `BE_OPEN` door triggers `ON_PLAYER_ON_DOOR`
- [ ] Player teleports to the entry door of the next room (no visible cross-room travel)
- [ ] Minimap avatar updates to the new cell on each transition
- [ ] `CLOSE` doors cannot be traversed
- [ ] No `IndexOutOfRangeException` when navigating any valid maze path

### Room-Clear Locking **[all GAPs — unimplemented]**
- [ ] On room entry: all non-entry doors become impassable
- [ ] Each enemy death decrements `RoomCell.enemyCount`
- [ ] When `enemyCount == 0`: doors reopen and `ON_ROOM_CLEAR` fires
- [ ] `ON_ROOM_CLEAR` event is received by Per-Run Upgrades system to present talent cards
- [ ] Re-entering a cleared room does not re-lock doors

### Minimap
- [ ] Minimap correctly shows room grid layout after maze generation
- [ ] Avatar position matches player's current room on every transition
- [ ] Open connections between rooms are visible on minimap; walls are not shown

### Dead Code Removal
- [ ] `Assets/Script/Map/Legacy/Door.cs` removed from project
- [ ] `Assets/Script/Map/Legacy/Room.cs` removed from project
