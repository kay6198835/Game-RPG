# Pathfinding (A*)

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` (no code change since the 2026-09-03 folder move)
> History: [CHANGELOG.md](CHANGELOG.md) · **No GDD, no ADR** (BUG-052)

## Purpose

Grid A* used by enemy movement to navigate the current room.

## Code — `Assets/Script/System/Pathfinding/` (11 files)

| Path | Contents |
|------|----------|
| `PathRequestManager.cs` | Request queue; required by `EnemyManager` (`[RequireComponent]`) |
| `Algorithm/` | `AStar`, `Heuristic`, `PriorityQueue` |
| `Data/` | `Node`, `Path`, `PathRequest`, `SearchNode` |
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

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-052 | No GDD / ADR for this subsystem |
