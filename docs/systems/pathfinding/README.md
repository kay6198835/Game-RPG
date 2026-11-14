# Pathfinding (A*)

> **Status:** live · **Last verified:** 2026-11-14, `sprint-22` `a8725a77` (`Path` → `NavPath` rename only)
> History: [CHANGELOG.md](CHANGELOG.md) · **No GDD, no ADR** (BUG-052)

## Purpose

Grid A* used by enemy movement to navigate the current room.

## Code — `Assets/Script/System/Pathfinding/` (11 files)

| Path | Contents |
|------|----------|
| `PathRequestManager.cs` | Request queue; required by `EnemyManager` (`[RequireComponent]`) |
| `Algorithm/` | `AStar`, `Heuristic`, `PriorityQueue` |
| `Data/` | `Node`, `NavPath` (file `NavPath.cs`; was `Path` until `d9776bb8`), `PathRequest`, `SearchNode` |
| `Grid/` | `GridBuilder`, `PathfindingGrid` |
| `Utility/GridUtility.cs` | Grid helpers |

## Flow

```
RoomGeneraterController.Setting() → new PathfindingGrid() → EnemyManager.Instance.SetPathfindingGrid()
RoomGeneraterController.LoadRoom() → pathfindingGrid.BuildGrid(roomData, roomPosition)
EntityMovement → EnemyManager.Instance.RequestPath() / GetNodeByPositionWorld() → PathRequestManager → AStar
```

`EnemyManager` is the service façade and the ratified singleton exception (ADR-0002); it lives in
`System/Enemy/`. A scene `EnemyManager` is required before any enemy `Start()`.

The code compiles into the `GameRPG` assembly (`Assets/Script/GameRPG.asmdef`, `9c11c672`). EditMode tests:
`Assets/Tests/EditMode/AStarTests.cs` (`47f66ddb`, mirrors QA-PATH-01..03; Test Runner result not recorded yet).

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-052 | No GDD / ADR for this subsystem |
