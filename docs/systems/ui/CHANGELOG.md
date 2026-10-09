# UI — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-10-05 — Mock and test-only flow removed
- **Changed:** deleted `Services/Mock/` (6 files incl. `MockCatalog`), `Gameplay/Mock/` (`GameplayMockController`, `MockEnemy`), `UIFlowSmokeTest`, `Assets/Scenes/Test/GameplayMock.unity` (and its Build Settings entry); removed flags `useMockData`, `skipGameplayInit`, `skipSaveLoad`, `bypassLoadRandomLogic` and `UIServices.RebuildProviders()` / `OnSaveSelected()`; loading tips moved into `LoadingScreen.DefaultTips`
- **From → To:** Mock/Real providers switched by flags → `UIServices` always builds the `Real*` providers; `EnterGameplay()` chose mock or real scene → always `LoadRandomMap` + `GameplayUI`; 64 → 55 `.cs` files. UI component logic (all panels, `WorldHealthBar`, `DamageNumber`) kept unchanged
- **Why:** owner request — start building the UI/UX against the real game without mock and fake-behaviour paths in the way
- **Bugs:** none closed; no automated UI check remains (verify by playing)

## 2026-10-05 — Real game from the menu; HUD bound to the player
- **Commit:** `cb0de496`
- **Changed:** `UIDebugConfig` defaults, `RealPlayerDataProvider`, HUD, `MainMenuFlow`; deleted `CharacterCreationPanel`, `StartScene`, `UISample`, `MainMenu.cs`, `Manager/UI/UIManager.cs`, 2024 `Prefab/UI/*`
- **From → To:**
  - flags default to mock (all on) → real game (only `skipLogin` on); New Game / Continue load `LoadRandomMap` + `GameplayUI`
  - `RealPlayerDataProvider` death event only → HP/Mana, stats, hotbar, cooldowns bound on `ON_PLAYER_READY`
  - New Game → character creation → New Game creates a "Paladin N" save directly
  - entry scene `StartScene` → `MainGamePlay` (build index 0)
- **Why:** game-completion phase — "the UIFlow menu now starts the real dungeon and the HUD shows live player data" (commit message)
- **Bugs:** closes demo items 9 (mostly) and 25 (player provider); TD-017 resolved by deletion

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
