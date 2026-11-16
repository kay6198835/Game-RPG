# Pathfinding — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history and `git log`.

## 2026-11-14 — `Path` renamed `NavPath`; EditMode tests (doc-truth re-sync)
- **Commit:** `d9776bb8` "fix(build): rename the game's Path class to NavPath" (2026-11-12); `47f66ddb` (EditMode tests, `AStarTests.cs`). Both on `sprint-22`.
- **Changed:** `Data/Path.cs` → `Data/NavPath.cs`; `AStar.cs`, `PathRequest.cs`, `PathRequestManager.cs`, `EntityMovement.cs`, `QASystemsSuite.cs` updated to the new name.
- **From → To:** global class `Path` → `NavPath`.
- **Why:** commit message — with `GameRPG.asmdef` referenced by `Assembly-CSharp-firstpass`, the global `Path` shadowed `DG.Tweening.Plugins.Core.PathCore.Path` in the DOTween module scripts (CS1729, CS1929, CS0411).
- **Bugs:** none; the transient DOTween errors are NOTE-20261113-1.

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
