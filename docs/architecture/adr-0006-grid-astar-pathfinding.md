# ADR-0006: Enemy pathfinding is A* on a per-room cell grid, 8 directions

> 📜 Change log: [changelog/adr-0006-grid-astar-pathfinding.CHANGELOG.md](changelog/adr-0006-grid-astar-pathfinding.CHANGELOG.md)

## Status
Proposed — **retroactive**. Awaiting owner review.

> ⚠️ **This ADR was written after the system was implemented.** The pathfinding code has been in the
> project since 2026-07 (moved to `Assets/Script/System/Pathfinding/` in `1c0742e0`, 2026-09-03) with
> no recorded decision (BUG-052). This document records the decision the code embodies, at
> `origin/sprint-22` `a8725a77`, and the alternatives as they can be weighed **today**. No record
> exists of which alternatives were weighed at the time; the "Alternatives Considered" section is a
> present-day analysis, not a reconstruction of a past discussion. No `.cs` file was changed.

## Date
2026-11-13 (decision implemented 2026-07; evidence tier STATIC)

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f3 LTS |
| Domain | Gameplay / AI navigation |
| Knowledge Risk | LOW (pinned engine version is within training data) |
| References Consulted | `docs/engine-reference/unity/VERSION.md`; `.claude/rules/engine-code.md`; `design/gdd/pathfinding-system.md` |
| Post-Cutoff APIs Used | None — the grid, search and result types are plain C#; the only engine touch points are `Vector2`, `Mathf` and `MonoBehaviour.Update` |
| Verification Required | Play Mode: the Tilemap tile anchor of the room Tilemaps versus the cell point used by the grid (GDD OQ-1) |

## ADR Dependencies

| Field | Value |
|-------|-------|
| Depends On | ADR-0002 (Amendment 2026-08-21) — `EnemyManager` is the singleton façade through which the grid and the request queue are reached. This ADR does not restate or change that decision |
| Related | ADR-0004 (VContainer) — Open Question #2 there defers moving the façade off the singleton; unaffected by this ADR |
| Enables | QA test cases and EditMode tests for pathfinding (`design/gdd/pathfinding-system.md` section 9) |
| Blocks | Nothing |

## GDD Requirements Addressed

| GDD | Requirement | How this ADR addresses it |
|---|---|---|
| `design/gdd/pathfinding-system.md` | PF-G1…G11 (grid from room tile data, rebuilt per room) | Decision 1 and 2 |
| `design/gdd/pathfinding-system.md` | PF-M1…M5, PF-H1…H4 (8 directions, 10 / 14, Octile) | Decision 3 |
| `design/gdd/pathfinding-system.md` | PF-R1…R6 (`NavPath` result) | Decision 4 |
| `design/gdd/pathfinding-system.md` | PF-Q2…Q5 (queued requests, N per frame) | Decision 5 |
| `design/gdd/pathfinding-system.md` | PF-P1 (≈15 typical, 20 ceiling per room) | Performance Implications |

## Context

### Problem Statement
Enemies must reach the player, back away from the player, and wander near their spawn point inside
a room that contains walls, pillars, corridors and islands (see the 15 combat rooms in
`Assets/Data/Json/Room/`). Moving straight at the target fails in most of those layouts.

### Constraints
- **Rooms are data, not scene geometry.** A room is a JSON `LevelData` (`tiles`, `poses`,
  `layerIndices`) painted into shared Tilemaps at runtime by `RoomGeneraterController.LoadRoom()`.
  Only one room is live at a time; changing room clears and repaints the same Tilemaps inside one
  persistent scene.
- **Rooms are small and uniform.** Every room is 30 × 30 tiles (900 cells), one world unit per tile.
- **Enemy counts are low.** About 15 per room typical, 20 as a hard ceiling (TD-034 / TD-035).
- **Enemies are pooled** and spawned at runtime, so any navigation data must be reachable without
  scene wiring (hence the ADR-0002 façade).
- Solo developer, demo scope: no navigation package is installed (`Packages/manifest.json` has no
  AI Navigation / NavMesh package), and adding third-party dependencies is already flagged as a risk
  in `VERSION.md`.

### Requirements
- A path between two points of the live room that avoids non-walkable tiles, or a clear failure.
- Navigation data that is correct for the room on screen with no manual bake step, because rooms
  are picked at random per run.
- Bounded cost per frame with up to 20 enemies asking for paths.
- Logic testable without a scene.

## Decision

1. **Navigation data is a 2D cell grid, one cell per tile.** `GridBuilder` reads the room's
   `LevelData` and marks a cell walkable when every tile entry at that position is `Tile_Floor` or
   `Tile_Spawn_Enemy`. The tile data that draws the room is the single source of walkability; there
   is no separate navigation asset to author or keep in sync.
2. **The grid is rebuilt per room.** One `PathfindingGrid` object lives for the dungeon run and is
   rebuilt in place by `RoomGeneraterController.LoadRoom()` each time an un-cleared room is loaded.
   The grid is static between rebuilds.
3. **The search is A\* with 8-direction movement.** Integer step costs 10 (straight) and 14
   (diagonal), Octile heuristic, binary-heap open set ordered by `F` then `H`, neighbour lists
   precomputed at build time with the corner rule applied there.
4. **The result is a value object.** `NavPath { bool Success; List<Vector2> Waypoints }`, waypoints
   in world space from start to target. Failure is a normal result, not an exception.
5. **Requests are queued and time-sliced.** Callers post a `PathRequest` (start, target, callback);
   `PathRequestManager` solves at most `maxRequestsPerFrame` per frame, synchronously on the main
   thread, and answers by callback. No threads, jobs or coroutines.
6. **Destination choice and path following stay with the caller** (`EntityMovement`). The
   pathfinder answers "how do I get from A to B" only.

Details that are current behaviour but **not** part of this decision — the corner-cutting rule, the
1.001 tie-break factor, the cell-point alignment, the per-frame request count, door cells after a
door opens, and what happens on an unreachable target — are open questions in the GDD (OQ-1…OQ-10).
An owner answer to any of them amends the GDD, and this ADR only if it changes points 1–6.

### Architecture Diagram

```
Room JSON (LevelData) ──► RoomGeneraterController.LoadRoom()
                              │ paints Tilemaps
                              │ BuildGrid(Data, roomPosition)
                              ▼
                        PathfindingGrid ◄─── GridBuilder (walkability + neighbour lists)
                              ▲
        SetPathfindingGrid()  │  Grid
                              │
EntityMovement ──► EnemyManager.Instance ──► PathRequestManager (FIFO, N per frame)
   ▲  RequestPath(PathRequest)                     │
   │                                               ▼
   └────────── callback(NavPath) ◄──────── AStar.FindPath(grid, start, target)
```

### Key Interfaces

- `PathfindingGrid.BuildGrid(LevelData data, Vector2 originPosition)`
- `PathfindingGrid.GetNode(Vector2 world)` → `Node` or `null`; `GetNodeFromWorld(Vector2 world)` → `SearchNode` or `null`
- `AStar.FindPath(PathfindingGrid grid, Vector2 startWorld, Vector2 targetWorld)` → `NavPath`
- `PathRequest(Vector2 start, Vector2 target, Action<NavPath> callback)`
- `EnemyManager.SetPathfindingGrid(PathfindingGrid)`, `.RequestPath(PathRequest)`, `.GetNodeByPositionWorld(Vector2)` (ADR-0002)

## Alternatives Considered

### Alternative 1: A* on a per-room cell grid, 8 directions (CHOSEN — as implemented)
- **Pros**: walkability comes straight from the tile data that already defines the room, so a
  randomly picked room needs no bake and cannot drift from what is drawn; 900 cells is a trivial
  search space; plain C#, testable in EditMode with a hand-built `LevelData`; no package dependency;
  8 directions match the game's 8-direction character facing and animation.
- **Cons**: paths are cell-to-cell polylines (no smoothing); the agent has no radius, so wall
  corners need an explicit rule (GDD OQ-3); the grid is static, so anything that changes the room
  after load (an opened door) needs a rebuild or an explicit rule (GDD OQ-8); per-request cost grows
  with the cell count, not with the path length (state reset and allocations).

### Alternative 2: 2D NavMesh (Unity AI Navigation / NavMeshPlus-style surface on the Tilemap)
- **Description**: bake a navigation mesh from the Tilemap colliders and move enemies with
  `NavMeshAgent` or sampled NavMesh paths.
- **Pros**: any-angle paths, agent radius handled by the bake, local avoidance available.
- **Cons**: Unity's built-in NavMesh is 3D-oriented; 2D top-down use needs an extra package or a
  community wrapper and an XY-plane workaround — a new dependency in a project that already lists
  two non-reproducible ones. The mesh must be re-baked at runtime every time a room is painted,
  which is slower and less predictable than rebuilding a 900-cell array. Logic is hard to test
  outside a scene.
- **Rejection Reason**: more machinery and a new dependency for rooms that are a 30 × 30 tile grid
  by construction.

### Alternative 3: A* on a 4-direction grid
- **Description**: same grid, straight steps only, Manhattan heuristic (`Heuristic.Manhattan` exists
  in code, unused).
- **Pros**: no corner-cutting question at all; simpler neighbour rule.
- **Cons**: staircase paths across open floor — visibly unnatural for a top-down action game whose
  characters move and face in 8 directions; paths up to about 41 % longer than the 8-direction path
  across open ground.
- **Rejection Reason**: movement quality. The corner question it avoids can be answered by a
  stricter corner rule instead.

### Alternative 4: Flow field / Dijkstra map from the player
- **Description**: once per player cell change, compute the distance from the player to every cell;
  each enemy steps to its lowest-distance neighbour. One computation serves every enemy.
- **Pros**: cost independent of enemy count — the right shape for hordes of 30–50+.
- **Cons**: serves one shared target only. Flee points, wander points and the stand-off point at
  `PreferredDistance` are per-enemy destinations and would still need a point-to-point search. At 20
  enemies and 900 cells it buys nothing measurable.
- **Rejection Reason**: over-engineering at the confirmed enemy count. Already recorded as the
  deferred endgame in TD-035; revisit only if the roadmap targets hordes.

### Alternative 5: No pathfinding — steer straight at the target with obstacle avoidance
- **Pros**: no grid, no queue.
- **Cons**: fails in `CombatRoom_SmallMaze`, `_Spiral`, `_NarrowBridges`, `_TwoHalls` and every
  other room with concave walls.
- **Rejection Reason**: does not meet the requirement.

## Consequences

### Positive
- One source of truth: the room JSON defines both what is drawn and where enemies may walk.
- No bake step and no navigation asset per room; a new room file works as soon as it uses the
  existing tile names.
- The grid, search and result classes have no scene dependency and can be unit-tested (EditMode).
- Per-frame cost is capped by `maxRequestsPerFrame`.

### Negative
- Walkable tile names are a code constant (`Tile_Floor`, `Tile_Spawn_Enemy`). A new floor-like tile
  is a wall for enemies until `GridBuilder` is changed.
- The grid is static: runtime changes to a room are invisible to enemies unless the grid is rebuilt.
- Paths are for a point agent and are not smoothed; visual quality depends on the corner rule and on
  how `EntityMovement` follows waypoints.
- Every request resets the search state of all cells and allocates a heap array, a `HashSet` and a
  `List` (TD-034 item 4, TD-035) — acceptable at 900 cells and ≤ 20 enemies, not beyond.
- Reaching the service through `EnemyManager.Instance` keeps the ADR-0002 singleton coupling.

### Risks
- **Risk**: the cell point is the tile's integer coordinate while tiles may be drawn from that
  coordinate to `+1` (anchor 0.5) — a half-cell mismatch between the drawn wall and the grid's wall.
  **Mitigation**: GDD OQ-1; inbox NOTE-20261113-2; one Play Mode check.
- **Risk**: a larger room multiplies per-request cost (state reset + allocation scale with cells).
  **Mitigation**: GDD OQ-9 sets a grid-size limit; TD-035 lists the structural fixes.
- **Risk**: enemy counts above 20.
  **Mitigation**: TD-034 package first, then Alternative 4 (TD-035).
- **Risk**: the queue outlives its room and its requesters (GDD PF-Q4).
  **Mitigation**: GDD OQ-7.

## Performance Implications
- **CPU**: one search touches at most 900 cells; the fixed part per request is a 900-cell state
  reset. Requests per frame are capped. Not measured — GDD OQ-10 proposes a 1 ms per-frame budget.
- **Memory**: about 900 `Node` + 900 `SearchNode` objects and their neighbour lists per grid build
  (garbage on each room load); per request a 900-entry array, a `HashSet` and a `List` (garbage per
  request).
- **Load Time**: grid build happens inside room load; no bake.
- **Network**: N/A.

## Migration Plan
None — the decision is already implemented. If an owner answer to GDD OQ-1…OQ-10 changes code, the
change is a normal story against the GDD, with the acceptance criteria in GDD section 9 as the test
list.

## Validation Criteria
- GDD section 9, criteria 1–26, pass as EditMode tests.
- GDD section 9, criteria 27–34, pass in Play Mode in `LoadRandomMap`.
- No class under `Assets/Script/System/Pathfinding/` except `PathRequestManager` derives from
  `MonoBehaviour` (keeps the logic scene-free).
- This ADR's Status is set by the owner after review; it is not Accepted by being written.

## Related Decisions
- ADR-0002 — `EnemyManager` singleton exception, re-scoped to the pathfinding façade (Amendment 2026-08-21).
- ADR-0004 — VContainer; Open Question #2 (moving the façade off the singleton).
- `design/gdd/pathfinding-system.md` — rules, acceptance criteria and open questions.
- `docs/tech-debt-register.md` — TD-034 (enemy-count scaling package), TD-035 (horde rewrites, deferred).
- BUG-052 — live subsystems with no ADR; this ADR covers the Pathfinding part only.
