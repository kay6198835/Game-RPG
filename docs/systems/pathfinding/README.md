# Pathfinding (A*)

> **Status:** live · **Last verified:** 2026-11-13, `origin/sprint-22` `a8725a77` (STATIC)
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

`EnemyManager` is the service façade and the ratified singleton exception (ADR-0002); it lives in
`System/Enemy/`. A scene `EnemyManager` is required before any enemy `Start()`.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-052 | No GDD / ADR for this subsystem — both now written (2026-11-13) and awaiting owner review; status is for the next triage |

Open design questions (10) are in the GDD, section *Open Questions*. Suspected defects from the
2026-11-13 read are inbox notes NOTE-20261113-2 … -5 (`production/qa/bug-inbox.md`).
