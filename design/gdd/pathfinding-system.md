---
status: reverse-documented
source: Assets/Script/System/Pathfinding/
date: 2026-11-13
verified-by: not yet verified by the owner (code facts read from origin/sprint-22 a8725a77; evidence tier STATIC)
---

# Pathfinding System Design

> 📜 Change log: [changelog/pathfinding-system.CHANGELOG.md](changelog/pathfinding-system.CHANGELOG.md)
> Technical page (code map): [docs/systems/pathfinding/README.md](../../docs/systems/pathfinding/README.md)
> Architecture decision: [ADR-0006](../../docs/architecture/adr-0006-grid-astar-pathfinding.md) ·
> Service façade: [ADR-0002](../../docs/architecture/adr-0002-enemymanager-singleton-exception.md) (Amendment 2026-08-21)

> **What this document is.** The A* pathfinding system has been running since 2026-07 with no design
> document (BUG-052). This GDD is **reverse-documented from code** at `origin/sprint-22` `a8725a77`
> (2026-11-13). Nothing here was observed in Play Mode. It exists so QA can write test cases against
> a written rule instead of against the code.
>
> **Every rule carries one label:**
>
> | Label | Meaning |
> |---|---|
> | **CONFIRMED** | An owner decision exists and is cited |
> | **AS-BUILT** | The code does this today; nobody has confirmed it is the intended design |
> | **OPEN** | The owner must decide. The rule text describes what the code does today; the options and a recommendation are in [Open Questions](#open-questions) |
>
> QA: a test written against an **AS-BUILT** or **OPEN** rule pins current behaviour. Mark such a
> case "pins as-built" so a later owner decision can change it without the case reading as a
> regression.

## Summary

Enemies find their way around the current room with A* on a square grid: one grid cell per tile,
eight movement directions, straight step cost 10 and diagonal step cost 14. The grid is rebuilt from
the room's tile data each time an un-cleared room is loaded. Enemies ask for a path through a queue
that solves a few requests per frame, then walk the returned waypoints cell by cell.

**Rule count:** 41 — 3 CONFIRMED, 27 AS-BUILT, 11 OPEN.

## 1. Overview [IMPLEMENTED]

| Part | Class | Role |
|---|---|---|
| Grid data | `PathfindingGrid`, `GridBuilder`, `Node`, `SearchNode` | Walkability per cell, neighbour lists, world ↔ cell conversion |
| Search | `AStar`, `Heuristic`, `PriorityQueue` | One path from a start point to a target point |
| Result | `NavPath` | `Success` flag + `Waypoints` (world positions, start → target) |
| Request queue | `PathRequestManager`, `PathRequest` | FIFO queue, N requests solved per frame, result returned by callback |
| Façade | `EnemyManager` (`System/Enemy/`) | Singleton entry point: `SetPathfindingGrid`, `RequestPath`, `GetNodeByPositionWorld` |
| Producer | `RoomGeneraterController` | Creates the grid object once per run, rebuilds it on room load |
| Consumer | `EntityMovement` | The only caller: chase, flee and wander destinations for enemies |

The player does not use pathfinding. `Utility/GridUtility.cs` is an empty placeholder.

## 2. Player Fantasy [PLANNED]

Enemies feel like they know the room: they come around pillars and through gaps instead of pushing
against a wall, and they do not freeze when the player moves behind cover. The player never notices
"pathfinding" — only that enemies arrive.

No owner statement exists for this section; it is a proposal.

## 3. Detailed Design [IMPLEMENTED]

### 3.1 Grid

Source: `GridBuilder.BuildGrid()` (`GridBuilder.cs:11-55`), `PathfindingGrid.cs`,
`RoomGeneraterController.LoadRoom()` (`RoomGeneraterController.cs:134-137`).

| ID | Label | Rule |
|---|---|---|
| PF-G1 | AS-BUILT | The grid is built from the room's `LevelData` (`tiles`, `poses`, `layerIndices`), the same data that paints the Tilemaps. Its extent is the bounding box of every entry in `poses`: `cols = max.x - min.x + 1`, `rows = max.y - min.y + 1`. |
| PF-G2 | AS-BUILT | A tile name is walkable only when it is exactly `Tile_Floor` or `Tile_Spawn_Enemy` (`GameConstants.TileName.FLOOR` / `.SPAWN`). Every other name — `Tile_Room`, `Tile_Door`, `Tile_Spawn_Champion_Monument_*`, any future name, or `null` — is not walkable. |
| PF-G3 | AS-BUILT | When several entries share one `(x, y)`, the cell is walkable only if **every** entry is walkable (logical AND). `z` and `layerIndices` are ignored for walkability. |
| PF-G4 | AS-BUILT | A cell inside the bounding box with no entry at all is not walkable. |
| PF-G5 | AS-BUILT | One cell is one world unit (hard-coded; `Grid.cellSize` is not read). Cell `(0, 0)` is the tile at `(min.x, min.y)`. The world position of cell `(x, y)` is `roomCell.transform.position + (min.x + x, min.y + y)`. |
| PF-G6 | AS-BUILT | World → cell: `cell = RoundToInt(world - gridOrigin)` per axis, where `gridOrigin = roomCell.transform.position + min`. A point that rounds outside `[0, cols) × [0, rows)` has no node (`null`). |
| PF-G7 | OPEN | The point stored for a cell is the tile's integer coordinate, and a cell owns the square `±0.5` around that point. With a Tilemap tile anchor of `(0.5, 0.5)` the drawn tile covers `[p, p+1)`, so the pathfinding cell is shifted half a cell down-left from the drawn tile. See OQ-1. |
| PF-G8 | AS-BUILT | One `PathfindingGrid` object exists per dungeon run (created in `RoomGeneraterController.Setting()`), and it is rebuilt **in place** each time an un-cleared room is loaded. Consumers may keep the reference; the content changes under them. |
| PF-G9 | OPEN | Loading a room that is already cleared does not rebuild the grid. The grid keeps describing the last un-cleared room that was loaded, at that room's world position. See OQ-2. |
| PF-G10 | AS-BUILT | The grid is static after it is built. Nothing updates walkability at runtime: enemies, the player, dropped items, summons and opened doors do not change it. |
| PF-G11 | AS-BUILT | Before the grid is built, each door the maze does not use is replaced by `Tile_Room` (`SwapTileMap()`), at the door cell and at the cell one step outward. Unused doors are therefore walls for pathfinding. |

**What the room data looks like today** (all 20 files in `Assets/Data/Json/Room/`, read 2026-11-13):

| Fact | Value |
|---|---|
| Bounds | every room: x and y from −15 to 14 → **30 × 30 = 900 cells** |
| Layers | `Tile_Room` on layer 0, `Tile_Floor` on layer 1, `Tile_Spawn_Enemy` on layer 3, `Tile_Door` on layer 0 |
| Walkable cells per room | 539 (`CombatRoom_NarrowBridges`) to 675 (`CombatRoom_PracticeYard`) |
| Stacked cells | `Tile_Floor` + `Tile_Spawn_Enemy` → walkable (3–6 per combat room) |
| | `Tile_Floor` + `Tile_Door` → **not** walkable (8 per room: 4 doors × 2 cells) |
| | `Tile_Floor` + `Tile_Room` → not walkable (4 cells, `ShopRoom_Merchant` only) |
| | `Tile_Floor` + `Tile_Spawn_Champion_Monument_*` → not walkable (3 cells, `BossRoom_ThroneArena` only) |
| Door cells | left `(-14, 2..3)`, right `(13, -3..-2)`, top `(-9..-8, 13)`, bottom `(1..2, -14)` |

### 3.2 Movement rules

Source: `GridBuilder.SetNodeNeighbors()` (`GridBuilder.cs:59-91`), `AStar.cs:32-52`.

| ID | Label | Rule |
|---|---|---|
| PF-M1 | AS-BUILT | A path may step to any of the 8 neighbouring cells (4 straight, 4 diagonal) that is walkable and inside the grid. |
| PF-M2 | AS-BUILT | A straight step costs **10**, a diagonal step costs **14** (`Heuristic.STRAIGHT`, `Heuristic.DIAGONAL`). All costs are integers. |
| PF-M3 | OPEN | Corner cutting: a diagonal step is forbidden only when **both** cells it squeezes between are not walkable. When exactly one of them is a wall, the diagonal step is allowed, so a path may clip the corner of a wall cell. See OQ-3. |
| PF-M4 | AS-BUILT | The moving agent has no size. A path is valid for a point; the enemy's collider radius is not considered. |
| PF-M5 | AS-BUILT | Every walkable cell costs the same. There is no terrain cost and no cost for cells near other enemies. |

Corner-cutting examples (`#` wall, `.` floor, move from `A` to `B`):

```
Allowed today (one side blocked)     Forbidden (both sides blocked)
    # B                                  # B
    A .                                  A #
```

### 3.3 Heuristic and search order

Source: `Heuristic.cs`, `PriorityQueue.Compare()` (`PriorityQueue.cs:76-81`).

| ID | Label | Rule |
|---|---|---|
| PF-H1 | AS-BUILT | The heuristic is Octile distance in the same units as the step costs: `h = 14 × min(dx, dy) + 10 × (max(dx, dy) − min(dx, dy))`. |
| PF-H2 | OPEN | A tie-break factor of **1.001** is applied and the result is rounded to an integer: `h' = RoundToInt(h × 1.001)`. Because of the rounding, `h' = h` whenever `h ≤ 500`. See section 4 for the consequence and OQ-4. |
| PF-H3 | AS-BUILT | The open set returns the node with the lowest `F = G + H`; on equal `F`, the node with the lowest `H` (the one closer to the target) wins. |
| PF-H4 | AS-BUILT | The search has no random element. The same grid, start point and target point always give the same path. Neighbours are expanded in the order of `GameConstants.Direction.Vector.ALL`: top, right, left, bottom, top-right, right-down, down-left, left-top. |

### 3.4 Result

Source: `NavPath.cs`, `AStar.FindPath()` / `Retrace()`.

| ID | Label | Rule |
|---|---|---|
| PF-R1 | AS-BUILT | A request returns a `NavPath`: `Success` (bool) and `Waypoints` (world positions, ordered start → target). On success the list holds the point of the start cell, every cell stepped through, and the point of the target cell. The points are cell points (PF-G5), not the exact start and target positions that were requested. |
| PF-R2 | AS-BUILT | A request fails — `Success = false`, `Waypoints` empty, no exception, no log — when: (a) the start or target point is outside the grid; (b) the start or target cell is not walkable; (c) no chain of allowed steps joins the two cells; (d) the grid has never been built. |
| PF-R3 | AS-BUILT | Start and target in the same cell: `Success = true`, `Waypoints` holds exactly one point (that cell's point). |
| PF-R4 | OPEN | There is no partial result. When the target cannot be reached, the path is not shortened to the nearest reachable cell — the request simply fails. See OQ-5. |
| PF-R5 | OPEN | A start point inside a non-walkable cell fails the request, even when a walkable cell is next to it. An enemy standing in such a cell cannot get any path. See OQ-5. |
| PF-R6 | AS-BUILT | Waypoints are not smoothed or reduced: one waypoint per cell, including cells on a straight line. |

All failures return the same shared object, `NavPath.Failure`. A caller must not modify its list.

### 3.5 Request flow

Source: `PathRequestManager.cs`, `EnemyManager.cs`, `EntityMovement.cs`, `EntityMoveState.cs`.

```
EntityMoveState.LogicUpdate()                       every frame while the enemy is in Move
  ├─ target locked, too close  → EntityMovement.FleeTarget()
  ├─ target locked, otherwise  → EntityMovement.ChaseToTarget()
  ├─ no target                 → EntityMovement.ToRandomPosition()
  └─ EntityMovement.MoveToNodeTarget()              advance the waypoint index

EntityMovement.SendResquestPath()                   throttled by repathInterval
  → EnemyManager.Instance.RequestPath(PathRequest(start, target, callback))
    → PathRequestManager queue (FIFO)
      → PathRequestManager.Update(): up to maxRequestsPerFrame per frame
        → AStar.FindPath(grid, start, target) → callback(NavPath)   same frame, main thread
          → EntityMovement.GetPath(): replace the waypoint list

EntityMoveState.PhysicsUpdate() → EntityMovement.MoveForwardToTarget()   rb.MovePosition toward the current waypoint
```

| ID | Label | Rule |
|---|---|---|
| PF-Q1 | CONFIRMED | `EnemyManager` is the pathfinding service façade and a permitted singleton: `SetPathfindingGrid()`, `RequestPath()`, `GetNodeByPositionWorld()`. *Source: ADR-0002, Amendment 2026-08-21 (owner decision C2).* |
| PF-Q2 | AS-BUILT | Requests are queued first-in-first-out. Each frame `PathRequestManager.Update()` solves at most `maxRequestsPerFrame` of them, on the main thread, and calls each callback in the same frame it is solved. A request made in frame *N* is answered in frame *N+1* at the earliest. |
| PF-Q3 | OPEN | `maxRequestsPerFrame` is **2** in code and **5** on the `EnemyManager` object in `LoadRandomMap.unity`. Which value is intended is not recorded. See OQ-6. |
| PF-Q4 | OPEN | A queued request cannot be cancelled, duplicates from one enemy are not merged, and the queue is not emptied when the room changes or when the requesting enemy dies and returns to the pool. See OQ-7. |
| PF-Q5 | AS-BUILT | While no grid has been handed to `PathRequestManager` (`SetGrid`), requests stay in the queue and are not answered. |
| PF-Q6 | AS-BUILT | Enemy use of a result (`EntityMovement.GetPath()`): a failed or empty path is ignored and the previous waypoints are kept. A successful path replaces the list; the enemy starts at waypoint index 1 when the path has more than one point (index 0 is its own cell), else index 0. A waypoint counts as reached within `PADDING_NODE_VALUE` = 0.15 units. |
| PF-Q7 | AS-BUILT | When an enemy asks again: **chase** — when the player moves into a different grid cell, on the first chase frame, and when switching back from flee; **flee** — once per flee point, a new point only after the current one is reached; **wander** — when the waypoint list is empty or finished and the destination is not yet reached. Every request passes one throttle: at most one request per enemy per `repathInterval` (default **0.25 s**, Inspector range 0.05–1 s). |
| PF-Q8 | AS-BUILT | Destination choice is the caller's job, not the pathfinder's. Chase aims at the cell at `PreferredDistance` from the player on the enemy's side, or at the player's position when that cell is not walkable. Flee aims `fleeDistance` (default 3) away, or at a random walkable cell near that point. Wander picks a random walkable cell within `maxRadiusSpawnPoint` (5 on the enemy prefab) of the spawn point, up to 10 attempts. |
| PF-Q9 | CONFIRMED | A scene `EnemyManager` must exist before any enemy's `Start()`: `EntityMovement.Start()` reads `EnemyManager.Instance.Grid` with no null guard. *Source: ADR-0002, Amendment 2026-08-21 — "Accepted for now".* |

### 3.6 Doors

| ID | Label | Rule |
|---|---|---|
| PF-D1 | AS-BUILT | A door cell is not walkable while the door is closed: the cell holds `Tile_Floor` and `Tile_Door`, and PF-G3 requires every entry to be walkable. |
| PF-D2 | OPEN | A door cell stays not walkable after the door opens. `DeleteDoorTileMap()` removes the door tile from the Tilemap but the grid is not rebuilt (PF-G10). **No decision is recorded: this is neither confirmed as intended nor reported as a defect.** See OQ-8. |

Why PF-D2 has no visible effect today: doors open only when the room's enemy count reaches zero (or
on load in Shop / Rest / Treasure rooms, which spawn no enemies), so no enemy is alive to path
through an open door.

### 3.7 Integration points

| System | Direction | Contract |
|---|---|---|
| Map (`RoomGeneraterController`) | produces | `new PathfindingGrid()` + `EnemyManager.Instance.SetPathfindingGrid()` once per run; `pathfindingGrid.BuildGrid(Data, roomCell.transform.position)` on each un-cleared room load, after unused doors are swapped to walls |
| Level editor (`LevelManager`) | produces | writes the room JSON (`LevelData`); tile names are a serialized contract ("Do not change parameter name or spell") |
| Enemy AI (`EntityMovement`) | consumes | `RequestPath()`, `GetNodeByPositionWorld()`, and a cached `PathfindingGrid` reference for `GetNodeFromWorld()` |
| Enemy spawn (`EnemySpawner`) | indirect | spawns at `Tile_Spawn_Enemy` cells, which are walkable (PF-G2) |
| Combat test scene | produces | `CombatTestEnemySpawner` sets a `new PathfindingGrid()` that is never built → every request fails (PF-R2 d), enemies there do not path |

## 4. Formulas [IMPLEMENTED]

**Step cost.** `g(next) = g(current) + 10` for a straight step, `+ 14` for a diagonal step.

**Heuristic.**

```
dx = |ax − bx|,  dy = |ay − by|
h  = 14 × min(dx, dy) + 10 × (max(dx, dy) − min(dx, dy))
h' = RoundToInt(h × 1.001)
```

**Effect of the tie-break factor on path quality.**

- `h × 1.001` adds `h / 1000`. Rounding to an integer discards that addition while it is below 0.5,
  so `h' = h` for every `h ≤ 500`.
- The largest `h` in a 30 × 30 grid is corner to corner: `14 × 29 = 406`. **In every room that
  exists today the factor changes nothing**: the heuristic is the plain Octile distance, which never
  overestimates on this grid, so every returned path has the minimum total cost under the 10 / 14
  metric.
- The factor starts to act only above `h = 500` (more than 50 cells in a straight line, or more than
  35 cells diagonally). There the heuristic may overestimate by about 0.1 %, and a returned path may
  cost up to about 0.1 % more than the minimum. The search opens fewer nodes in exchange.
- The "prefer the node closer to the target" behaviour that exists today comes from PF-H3 (the
  open-set comparison), not from this factor.

**Path cost** of a returned path = `10 × straightSteps + 14 × diagonalSteps`. `NavPath` does not
expose it; a test computes it from consecutive waypoints.

**Queue latency.** With `Q` requests waiting and `N = maxRequestsPerFrame`, the last one is answered
after `ceil(Q / N)` frames.

## 5. Edge Cases [IMPLEMENTED]

| # | Case | Behaviour today |
|---|---|---|
| 1 | Target cell is a wall | `Success = false`, no exception (PF-R2 b). The enemy keeps its previous waypoints (PF-Q6). |
| 2 | Target point outside the grid | `Success = false`, no exception (PF-R2 a). |
| 3 | Start point outside the grid, or in a wall cell | `Success = false` (PF-R2, PF-R5). The enemy gets no path until it is moved onto a walkable cell by something else. |
| 4 | Target enclosed by walls | `Success = false` after the reachable area is searched (PF-R2 c). This is the most expensive request. |
| 5 | Start and target in one cell | `Success = true`, one waypoint (PF-R3). The enemy moves to its own cell point — at most about 0.71 units. |
| 6 | Grid object exists but was never built | `Success = false` for every request; `GetNode()` returns `null` (PF-R2 d). |
| 7 | `EnemyManager.Grid` was never set | `GetNodeByPositionWorld()` throws `NullReferenceException`; `RequestPath()` queues without limit (PF-Q5). |
| 8 | `LevelData` with zero entries | By reading the arithmetic (`int.MaxValue` / `int.MinValue` seeds, unchecked overflow): a 2 × 2 grid of non-walkable cells, every request fails. Not exercised by any room. |
| 9 | Player standing on a door cell or a wall-side half cell | Chase destination falls back to the player's position (PF-Q8), whose cell is not walkable → the request fails → the enemy keeps or finishes its old path, then retries every `repathInterval`. |
| 10 | Enemy returned to the pool while its request is queued | The callback still runs and writes waypoints into the disabled enemy (PF-Q4). |
| 11 | Room change while requests are queued | The old requests are solved against the new room's grid (PF-Q4). |
| 12 | Cleared room re-entered | Grid not rebuilt (PF-G9). No enemy spawns in a cleared room, so there is no consumer today. |
| 13 | Rebuilding the grid from a room's data after its doors opened | Not done today. If added: `DeleteDoorTileMap()` rewrites each opened door entry to position `(0, 0, 0)` with a `null` tile name, so by PF-G2 / PF-G3 a rebuild would mark the room's centre cell `(0, 0)` not walkable. Any OQ-8 option that rebuilds must handle this. |
| 14 | A path request to world position exactly `(0, 0)` | `EntityMovement` uses `Vector2.zero` as "no destination", so that destination is never requested. The room in maze cell (0, 0) is centred on the world origin. |

## 6. Dependencies [IMPLEMENTED]

- **Map system** (`design/gdd/map-system.md`) — room JSON, tile names, room cell world position, the
  rule that one room is live at a time.
- **Enemy AI** — the only consumer. No GDD covers `EntityMovement` yet.
- **Enemy spawn** (`design/gdd/enemy-spawn-system.md`) — enemy count per room drives request volume.
- **ADR-0002** — `EnemyManager` singleton façade. **ADR-0006** — the algorithm and grid decision.
- Unity: `Vector2`, `Mathf`, `MonoBehaviour.Update` only. The grid, search and result classes are
  plain C# and need no scene — they can be covered by EditMode tests (`Assets/Tests/EditMode/`).

## 7. Tuning Knobs [IMPLEMENTED]

| Knob | Where | Value today | Notes |
|---|---|---|---|
| `maxRequestsPerFrame` | `PathRequestManager` (Inspector) | code 2, `LoadRandomMap.unity` 5 | OQ-6 |
| `repathInterval` | `EntityMovement` (Inspector, 0.05–1) | 0.25 s (code default; not overridden on the enemy prefab) | per enemy |
| `fleeDistance` | `EntityMovement` (Inspector, 0.5–10) | 3 | |
| `maxRadiusSpawnPoint` | `EntityMovement` (Inspector) | 5 on the enemy prefab (3 when left at 0) | wander radius |
| `PADDING_NODE_VALUE` | `GameConstants.SettingStats` | 0.15 | waypoint arrival radius; code constant |
| `STRAIGHT` / `DIAGONAL` | `Heuristic` | 10 / 14 | code constants |
| `TIE_BREAK_FACTOR` / `USE_TIE_BREAKING` | `Heuristic` | 1.001 / true | code constants; OQ-4 |
| Walkable tile names | `GridBuilder` + `GameConstants.TileName` | `Tile_Floor`, `Tile_Spawn_Enemy` | code; not data-driven |
| Cell size | — | 1 world unit | implicit |

## 8. Performance [PARTIAL]

| ID | Label | Rule |
|---|---|---|
| PF-P1 | CONFIRMED | Target load is about 15 enemies in a typical room with a hard ceiling of 20. The current A* is accepted for that load; structural rewrites (flow field, generation stamps, request priority) are deliberately deferred until a roadmap asks for 30+ enemies per room. *Source: `docs/tech-debt-register.md` TD-034 and TD-035, accepted 2026-07-29.* |
| PF-P2 | OPEN | Maximum grid size. Every room today is 30 × 30 = 900 cells; no limit is written or enforced. See OQ-9. |
| PF-P3 | OPEN | Time budget per frame for pathfinding and the longest acceptable wait for a path. Nothing is written or measured. See OQ-10. |
| PF-P4 | AS-BUILT | Cost per request today: the search state of every cell is reset (900 cells), and one heap array of `cols × rows` entries, one `HashSet` and one waypoint `List` are allocated. Cost per grid build: `cols × rows` `Node` and `SearchNode` objects plus their neighbour lists are allocated new. |

Worked numbers for the confirmed load (20 enemies, all chasing, player crossing cells continuously,
`repathInterval` 0.25 s): at most 80 requests per second. At 60 fps that is 1.3 requests per frame
on average — under both 2 and 5 per frame — but 20 enemies asking in the same frame wait up to
10 frames at `N = 2` and 4 frames at `N = 5`.

## 9. Acceptance Criteria [IMPLEMENTED]

Each criterion is checkable. "Logic" = EditMode test on `PathfindingGrid` + `AStar` with a
hand-built `LevelData`; "Scene" = Play Mode. The rule each one pins is in brackets; a criterion on an
AS-BUILT or OPEN rule pins current behaviour.

**Grid**

1. Logic — `LevelData` with a 5 × 5 block of `Tile_Floor`: 25 cells, all walkable. [PF-G1, PF-G2]
2. Logic — a cell holding `Tile_Floor` and `Tile_Spawn_Enemy`: walkable. [PF-G3]
3. Logic — a cell holding `Tile_Floor` and `Tile_Door`: not walkable. [PF-G3, PF-D1]
4. Logic — a cell holding `Tile_Floor` and `Tile_Room`, in either order in the list: not walkable. [PF-G3]
5. Logic — a cell holding only an unknown tile name: not walkable. [PF-G2]
6. Logic — a cell inside the bounding box with no entry: not walkable. [PF-G4]
7. Logic — tiles from x = −15 to 14 and room position `(30, −30)`: `GetNode((15, −45))` returns the cell of tile `(−15, −15)`, and that node's `WorldPosition` is `(15, −45)`. [PF-G5]
8. Logic — `GetNode(p + (0.49, 0.49))` and `GetNode(p − (0.49, 0.49))` return the same node as `GetNode(p)` for a cell point `p`. [PF-G6]
9. Logic — a point more than 0.5 outside the bounding box: `GetNode` returns `null`, no exception. [PF-G6]
10. Logic — every one of the 20 room files builds a 30 × 30 grid with at least one walkable cell. [PF-G1]

**Movement and cost**

11. Logic — open floor, target 5 cells to the right: `Success = true`, 6 waypoints, cost 50. [PF-M2, PF-R1]
12. Logic — open floor, target 4 right and 4 up: 5 waypoints, cost 56. [PF-M1, PF-M2]
13. Logic — open floor, target 5 right and 2 up: cost 58 (2 diagonal + 3 straight). [PF-H1]
14. Logic — diagonal step with **both** side cells walls: the step is not taken; the path goes around or fails. [PF-M3]
15. Logic — diagonal step with **one** side cell a wall: the step is taken (pins as-built). [PF-M3]
16. Logic — every waypoint of every returned path is a walkable cell point, and consecutive waypoints differ by at most 1 cell on each axis. [PF-M1, PF-R1]
17. Logic — on each of the 20 rooms, for 100 fixed start/target pairs of walkable cells, the returned cost equals the cost from a reference Dijkstra search using the same neighbour rule. [PF-H2, section 4]
18. Logic — the same request twice on the same grid returns identical waypoint lists. [PF-H4]

**Result**

19. Logic — target cell is a wall: `Success = false`, `Waypoints.Count = 0`, no exception. [PF-R2]
20. Logic — target point outside the grid: `Success = false`, no exception. [PF-R2]
21. Logic — start cell is a wall, target walkable: `Success = false`, no exception (pins as-built). [PF-R5]
22. Logic — target on floor fully enclosed by walls: `Success = false`, no exception. [PF-R2, PF-R4]
23. Logic — start and target in the same cell: `Success = true`, exactly 1 waypoint equal to that cell's point. [PF-R3]
24. Logic — a `PathfindingGrid` that was never built: `FindPath` returns `Success = false`, `GetNode` returns `null`, no exception. [PF-R2]
25. Logic — the first waypoint is the start cell's point and the last is the target cell's point. [PF-R1]
26. Logic — two requests in a row on one grid: the second result does not depend on the first (search state is reset). [PF-P4]

**Request flow**

27. Scene — 10 requests queued in one frame with `maxRequestsPerFrame = 2`: exactly 2 callbacks per frame for 5 frames, in request order. [PF-Q2]
28. Scene — a request made before any grid is set gets no callback; it is answered after `SetGrid` and a built grid. [PF-Q5]
29. Scene — an enemy that receives a failed path keeps moving on its previous waypoints. [PF-Q6]
30. Scene — a chasing enemy with the player standing still in one cell sends no new request while it still has waypoints. [PF-Q7]
31. Scene — a chasing enemy with the player crossing a cell border every frame sends at most 1 request per `repathInterval`. [PF-Q7]
32. Scene — in `LoadRandomMap`, an enemy and the player on opposite sides of a pillar: the enemy reaches attack range without stopping against the pillar. [PF-M1, PF-Q6]

**Doors**

33. Logic — with a room's data as loaded, each cell holding `Tile_Door` is not walkable, and each unused door replaced by `Tile_Room` is not walkable. [PF-D1, PF-G11]
34. Scene — after the room is cleared and its doors open, `GetNodeByPositionWorld` on a door cell still returns a non-walkable node (pins as-built; changes with OQ-8). [PF-D2]

**Performance** — no criterion can be written until OQ-9 and OQ-10 are answered. Placeholder for the
measurement: with 20 enemies chasing in the largest room, record the pathfinding time per frame
and the wait from request to callback.

## Open Questions

### Questions needing owner decision

Every OPEN rule is listed here. Nothing below has been decided by this document.

| # | Rule | Question | Options | Recommendation |
|---|---|---|---|---|
| OQ-1 | PF-G7 | Where is the pathfinding point of a cell: the tile's corner (today) or the tile's centre? | **A.** Keep the corner (today). **B.** Use the tile centre (`+0.5, +0.5`) so the cell square matches the drawn tile. | **B**, after one Play Mode check that the room Tilemap anchor is `(0.5, 0.5)` (the anchor lives in a prefab this pass could not read conclusively). With A, half of each floor tile next to a wall belongs to the wall's cell — see inbox NOTE-20261113-2. |
| OQ-2 | PF-G9 | Should the grid be rebuilt when a cleared room is re-entered? | **A.** No (today) — a cleared room has no enemies. **B.** Always rebuild on room load. **C.** Clear the grid (all requests fail) on a cleared room. | **B** — one rule ("the grid always matches the room on screen") and it makes extra spawns in a cleared room safe. Needs the edge case 13 fix. |
| OQ-3 | PF-M3 | When may a path cut a corner? | **A.** Forbid only when both side cells are walls (today). **B.** Forbid when **either** side cell is a wall. **C.** Allow always. | **B** — enemies have a body; with A the waypoint line passes through the wall's corner and the enemy can catch on the wall collider. Cost: slightly longer paths around pillars. |
| OQ-4 | PF-H2 | Keep the 1.001 tie-break factor? | **A.** Keep as is (no effect in any current room). **B.** Remove it (guaranteed minimum-cost paths at any grid size). **C.** Make it effective (e.g. scale costs by 1000 or use a float). | **B** — it does nothing at 30 × 30 and PF-H3 already provides the tie-break; removing it deletes a rule QA would otherwise have to reason about. |
| OQ-5 | PF-R4, PF-R5 | What should happen when the target cannot be reached, or the start/target point is in a non-walkable cell? | **A.** Fail (today). **B.** Snap start and target to the nearest walkable cell, still fail when unreachable. **C.** B plus return the path to the reachable cell closest to the target. | **B** now — it removes "enemy frozen because it stands half a cell inside a wall" and "player on a door cell" in one rule. C only if enemies are seen giving up in Play Mode. |
| OQ-6 | PF-Q3 | Which `maxRequestsPerFrame` is intended: 2 (code) or 5 (scene)? | **A.** 2. **B.** 5. **C.** 3–4 as TD-034 item 2 proposed. | **B (5)** — it is what the game runs with; set the code default to the same value so a new scene behaves alike. |
| OQ-7 | PF-Q4 | Should queued requests be dropped when the room changes or the requester is despawned? | **A.** No (today). **B.** Empty the queue on room load. **C.** B plus skip a request whose enemy is inactive. | **C** — small change, removes edge cases 10 and 11. |
| OQ-8 | PF-D2 | Is "door cells are not walkable even after the door opens" intended? | **A.** Yes, intended: enemies never use doors; record it as a rule. **B.** No: door cells become walkable when the door opens (grid update or rebuild). | **A** for the demo — doors open only when no enemy is alive, and keeping enemies out of doorways avoids them blocking the exit. Revisit if enemies ever spawn after the doors open. |
| OQ-9 | PF-P2 | What is the largest room grid the system must support? | **A.** 30 × 30 (900 cells) — the current authoring size, as a hard limit. **B.** 40 × 40 (1 600). **C.** 64 × 64 (4 096). | **A**, written as a level-design limit, unless a bigger boss room is planned. The per-request reset and allocation (PF-P4) grow with the cell count. |
| OQ-10 | PF-P3 | What are the budgets? | Proposal: pathfinding ≤ **1 ms** per frame on the target PC; a request answered within **0.25 s** (one `repathInterval`) at 20 enemies. | Accept the proposal as a starting budget, then measure once in `LoadRandomMap` and adjust. |

Related, already decided elsewhere and **not** reopened here: the singleton façade (ADR-0002) and
the deferred horde rewrites (TD-035).

### Flagged follow-up work

- Suspected defects found while reading are in `production/qa/bug-inbox.md`: NOTE-20261113-2 … -5.
- `EntityMovement` (chase / flee / wander destination rules, PF-Q7 and PF-Q8) belongs in an Enemy AI
  GDD, which does not exist. This document records only what touches pathfinding.
- No EditMode test covers this system yet; criteria 1–26 are written to be automated.
- `docs/registry/architecture.yaml` and `docs/architecture/tr-registry.yaml` were not updated by
  this pass (the TR registry is written by `/architecture-review`).

## Version History

| Date | Change |
|---|---|
| 2026-11-13 | Created — reverse-documented from code at `origin/sprint-22` `a8725a77`; 41 rules (3 CONFIRMED, 27 AS-BUILT, 11 OPEN); 10 open questions |
