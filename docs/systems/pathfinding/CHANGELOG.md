# Pathfinding — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history and `git log`.

## 2026-11-14 — `Path` renamed `NavPath`; EditMode tests (doc-truth re-sync)
- **Commit:** `d9776bb8` "fix(build): rename the game's Path class to NavPath" (2026-11-12); `47f66ddb` (EditMode tests, `AStarTests.cs`). Both on `sprint-22`.
- **Changed:** `Data/Path.cs` → `Data/NavPath.cs`; `AStar.cs`, `PathRequest.cs`, `PathRequestManager.cs`, `EntityMovement.cs`, `QASystemsSuite.cs` updated to the new name.
- **From → To:** global class `Path` → `NavPath`.
- **Why:** commit message — with `GameRPG.asmdef` referenced by `Assembly-CSharp-firstpass`, the global `Path` shadowed `DG.Tweening.Plugins.Core.PathCore.Path` in the DOTween module scripts (CS1729, CS1929, CS0411).
- **Bugs:** none; the transient DOTween errors are NOTE-20261113-1.

## 2026-11-13 — GDD and ADR written (docs only)
- **Commit:** this change (branch `docs/gdd-pathfinding`, base `origin/sprint-22` `a8725a77`)
- **Changed:** `design/gdd/pathfinding-system.md` (new), `docs/architecture/adr-0006-grid-astar-pathfinding.md` (new); this README's header, `Data/` row (`Path` → `NavPath`), `GridUtility` row and Open issues
- **From → To:** "No GDD, no ADR" → reverse-documented GDD (41 rules: 3 CONFIRMED, 27 AS-BUILT, 11 OPEN) + retroactive ADR-0006 (Proposed)
- **Why:** BUG-052; QA needs a written source for test cases. No `.cs` change.
- **Bugs:** none opened or closed; inbox notes NOTE-20261113-2 … -5.

## 2026-10-05 — Documentation folder created
- **Commit:** none (docs only)
- **Changed:** this folder
- **From → To:** described only inside `CLAUDE.md` → `docs/systems/pathfinding/`
- **Why:** per-system documentation layout.
- **Bugs:** none.

## 2026-09-03 — Folder move
- **Commit:** `1c0742e0`
- **Changed:** location
- **From → To:** `Assets/Script/Pathfinding/` → `Assets/Script/System/Pathfinding/`
- **Why:** grouping under `System/` ("update folder").
- **Bugs:** none.
