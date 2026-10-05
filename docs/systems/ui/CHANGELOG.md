# UI — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-09-29 … 10-05 — UIFlow added
- **Commits:** `a827bc46` (test UI), `0c38633d` (core, providers, flags), `7a19e2db` (menu, loading, in-game panels), `b347508e` (scene builder, smoke test), `096482d0` (MainGamePlay, Loading, GameplayUI, GameplayMock scenes), `31077371` (beginner guide), `f8f180d0` (`docs/ui/ui-ux-flow.md`)
- **Changed:** new `Assets/Script/UIFlow/` (65 files), 4 scenes, `UIDebugConfig` asset
- **From → To:** UI Toolkit main/settings/pause menus + stats screen only, no player HUD → complete UGUI flow from splash to game over, HUD, inventory, quests, skill tree, shop, dialogue, damage numbers, running on mock data behind five flags
- **Why:** build and test the full player-facing flow without waiting for gameplay data.
- **Bugs:** BUG-087 → PARTIAL (first `ON_PLAYER_DEATH` subscriber).

## 2026-08-21 / 08-22 — Stats screen (UI Toolkit) + DI wiring
- **Commits:** `5e5d1c5e`, `63e9adcc`, `207f34fa`, `aa4e620c`
- **Changed:** `StatsScreenUIController`, `StatSlot`, `StatsUIController` registered in VContainer
- **From → To:** UGUI stats panel → UI Toolkit port; stats UI resolved through DI
- **Why:** stat allocation UI.
- **Bugs:** none.
