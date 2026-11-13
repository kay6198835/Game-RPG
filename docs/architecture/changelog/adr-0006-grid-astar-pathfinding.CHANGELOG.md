# Changelog — `adr-0006-grid-astar-pathfinding.md`

Change log for [`docs/architecture/adr-0006-grid-astar-pathfinding.md`](../adr-0006-grid-astar-pathfinding.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

## 2026-11-13 — Created (retroactive)
- **Commit:** this change (branch `docs/gdd-pathfinding`, base `origin/sprint-22` `a8725a77`)
- **Changed:** new ADR, Status **Proposed**. Records the decision the code already embodies: A* on a
  per-room cell grid built from `LevelData`, 8 directions, queued requests. Alternatives weighed
  today: 2D NavMesh, 4-direction grid, flow field, no pathfinding.
- **From → To:** no ADR for Pathfinding → ADR-0006 (written after implementation; no `.cs` change)
- **Why:** BUG-052 (live subsystem with no ADR). Does not restate ADR-0002, which owns the
  `EnemyManager` singleton façade.
- **Bugs:** none.
