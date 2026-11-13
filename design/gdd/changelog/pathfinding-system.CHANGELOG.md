# Changelog — `pathfinding-system.md`

Change log for [`design/gdd/pathfinding-system.md`](../pathfinding-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

## 2026-11-13 — Created
- **Commit:** this change (branch `docs/gdd-pathfinding`, base `origin/sprint-22` `a8725a77`)
- **Changed:** new document, reverse-documented from `Assets/Script/System/Pathfinding/`, `EnemyManager.cs`,
  `EntityMovement.cs`, `RoomGeneraterController.cs` and the 20 room files in `Assets/Data/Json/Room/`.
  41 rules, each labelled CONFIRMED (3), AS-BUILT (27) or OPEN (11); 34 acceptance criteria; 10 open
  questions for the owner. Evidence tier STATIC — nothing was run.
- **From → To:** no GDD (system "Implemented, undesigned") → reverse-documented GDD
- **Why:** BUG-052; QA needs a written source to derive pathfinding test cases from.
- **Bugs:** none opened or closed. Suspected defects recorded as inbox notes NOTE-20261113-2 … -5.
