# Pathfinding (A*)

> **Status:** live · **Last verified:** 2026-11-22, `sprint-23` `1e5afb3c` (grid rules only, STATIC); rest 2026-11-13, `origin/sprint-22` `a8725a77`
> History: [CHANGELOG.md](CHANGELOG.md) · GDD: [design/gdd/pathfinding-system.md](../../../design/gdd/pathfinding-system.md) (reverse-documented, awaiting owner review) · ADR: [ADR-0006](../../architecture/adr-0006-grid-astar-pathfinding.md) (Proposed, retroactive) · façade: ADR-0002

## Purpose

Grid A* used by enemy movement to navigate the current room.

## Code — `Assets/Script/System/Pathfinding/` (11 files)

| Path | Contents |
|------|----------|
| `PathRequestManager.cs` | Request queue; required by `EnemyManager` (`[RequireComponent]`) |
| `Algorithm/` | `AStar`, `Heuristic`, `PriorityQueue` |
| `Data/` | `Node`, `NavPath` (was `Path` until `d9776bb8`), `PathRequest`, `SearchNode` |
| `Grid/` | `GridBuilder`, `PathfindingGrid` |
| `Utility/GridUtility.cs` | Empty placeholder class (one unused field) |

## Flow

```
RoomGeneraterController.Setting() → new PathfindingGrid() → EnemyManager.Instance.SetPathfindingGrid()
RoomGeneraterController.LoadRoom() → pathfindingGrid.BuildGrid(roomData, roomPosition)
EntityMovement → EnemyManager.Instance.RequestPath() / GetNodeByPositionWorld() → PathRequestManager → AStar
```

**Grid rules (`0e0dc874`, 2026-11-19).** `GridBuilder` places each node on the tile centre (cell + 0.5, 0.5);
`PathfindingGrid.GetGridPosition()` maps a world point with `FloorToInt` (was `RoundToInt`); a diagonal step
needs **both** side cells walkable (no corner cutting); `PathfindingGrid.IsLineWalkable()` checks a straight
segment with the same corner rule, exposed as `EnemyManager.IsLineWalkable()` and used by `EntityMovement` (`:113`).

`EnemyManager` is the service façade and the ratified singleton exception (ADR-0002); it lives in
`System/Enemy/`. A scene `EnemyManager` is required before any enemy `Start()`.

The code compiles into the `GameRPG` assembly (`Assets/Script/GameRPG.asmdef`, `9c11c672`). EditMode tests:
`Assets/Tests/EditMode/AStarTests.cs` (`47f66ddb`, mirrors QA-PATH-01..03; waypoints moved to tile centres and corner / mapping / line-walk tests
added in `0e0dc874`). The 2026-11-19 automated run reported 70 EditMode tests PASS (`routine-log.md`).

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-052 | No GDD / ADR for this subsystem — both now written (2026-11-13) and awaiting owner review; status is for the next triage |

Open design questions (10) are in the GDD, section *Open Questions*. Suspected defects from the
2026-11-13 read are inbox notes NOTE-20261113-2 … -5 (`production/qa/bug-inbox.md`).
