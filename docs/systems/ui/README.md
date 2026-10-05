# UI — UIFlow (current) + legacy UI

> **Status:** UIFlow live and **runs the real game by default**; player data is real, inventory/quest/login still stubs · **Last verified:** 2026-10-05, merge of `7d1b5c79` into `cb0de496` · **No GDD, no ADR** (BUG-052)
> History: [CHANGELOG.md](CHANGELOG.md) · **Deep reference:** `docs/ui/ui-ux-flow.md` (flow, flags, every panel, findings)

## Purpose

Everything the player sees outside the world: boot/menu/save, the loading screen, and the in-game
HUD and windows. Mock data and the fake-gameplay scene were removed on 2026-10-05; the UI always runs
against the real game, and screens with no gameplay source yet (inventory, quests, skill tree) show empty.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/UIFlow/` (64 `.cs` files, `namespace UIFlow`, UGUI + TextMeshPro) | |
| `Core/` | `SceneFlow` (static scene routing; every transition goes through `Loading`), `SceneNames`, `UIManager` (panel stack: `Open/Close/Back/CloseAll`, Esc handling), `UIPanel`, `UIInput`, `UIBootstrap`, `UIEvents` (static UI bus), `UIServices` (static locator, always the Real providers), `UIDebugConfig` (SO: `skipLogin`, timings) |
| `Data/UIDataModels.cs` | DTOs: `PlayerStatsData`, `SkillSlotData`, `SkillNodeData`, `CharacterClassInfo`, … |
| `Menu/` | `SplashPanel`, `LoginPanel`, `MainMenuPanel` + `MainMenuFlow`, `SaveSelectPanel` + `SaveSlotView`, `SettingsPanel` + `KeyBindingRow`. `CharacterCreationPanel` deleted in `cb0de496` — New Game creates a "Paladin N" save |
| `Loading/LoadingScreen.cs` | Async load of `SceneFlow.TargetScene`, minimum display time, tips |
| `Gameplay/` | `GameplayUIController`; `HUD/` (HUDPanel, StatBar, SkillHotbar, QuestTracker, NotificationFeed); `Windows/` (TabWindow, Inventory + InventorySlot + ItemTooltip, Character, Quest, SkillTree); `Shop/`; `Dialogue/`; `World/` (DamageNumber + spawner); `PausePanel`, `GameOverPanel` |
| `Services/` | `Interfaces/` (ILoginService, ISaveProvider, IPlayerDataProvider, IInventoryProvider, IQuestProvider); `Real/` (player data wired; login / inventory / quest stubs); `KeyBindings`, `SettingsStore` |
| `Editor/` | `UIFlowBuilder` (+ `.Gameplay`) — generates the scenes; `UIKit`. (`UIFlowSmokeTest` deleted 2026-10-05) |
| `Assets/Script/UI/` (legacy) | `UIController` (UI Toolkit MainMenu/Settings/Pause from `Assets/UI/Screens/*.uxml`), `StatsUIController` (VContainer-registered), `StatsScreenUIController`, `StatSlot` |

Scenes: `Main/MainGamePlay` (menus, **build index 0**), `Main/Loading`, `Main/GameplayUI` (additive in-game UI).
Real gameplay scene: `Main/Test/LoadRandomMap`. `StartScene`,
`UISample`, `MainMenu.cs`, the legacy `Manager/UI/UIManager.cs` stub and the 2024 `Assets/Prefab/UI/`
prefabs were deleted in `cb0de496`; `Test/GameplayMock` was deleted with the mock removal (2026-10-05).

## Flow

```
MainGamePlay: Splash → (Login, unless skipLogin) → MainMenu
  New Game → create "Paladin N" save → SceneFlow.EnterGameplay()
  Continue / Load → SaveSelect → SceneFlow.EnterGameplay()
      → Loading → LoadRandomMap + GameplayUI (additive)
In game: HUD root; panels pushed on UIManager's stack; Esc = Back / Pause
Player ready: VitalStatsComponent.Reborn() → Emit(ON_PLAYER_READY, ICharacter) → RealPlayerDataProvider binds
  IVitalComponent.CurrentStatsChanged (HP/Mana), IStatService (max/level/stats), CharacterData.AbilityBindings (hotbar),
  AbilityHolderBase.AbilityCooldownStarted (cooldowns)
Death: ON_PLAYER_DEATH → RealPlayerDataProvider.PlayerDied → GameOverPanel → Respawn() = reload gameplay scene
```

`UIDebugConfig` holds `skipLogin` (default **on**), `splashDuration` and `minLoadingTime`. The four mock flags
(`useMockData`, `skipGameplayInit`, `skipSaveLoad`, `bypassLoadRandomLogic`) were removed on 2026-10-05.

## What is wired today

| Provider | Real implementation |
|----------|---------------------|
| `IPlayerDataProvider` | ✅ Real (`cb0de496`): HP/Mana, max values, level, stats, 4-slot hotbar with icons, cooldowns, death. Skill tree and EXP have no gameplay source (EXP bar hidden) |
| `ISaveProvider` | `RealSaveProvider` implemented (114 lines) — not verified in this pass |
| `ILoginService`, `IInventoryProvider`, `IQuestProvider` | Stubs |

## Deviations from project rules

- `UIServices` is a static service locator — bypasses VContainer (ADR-0004).
- `UIEvents` is a static `event Action` bus — `manager-event-code.md` forbids `static Action` fields.
- Built on UGUI for menus; `VERSION.md` says new *menu* screens follow UI Toolkit.
- Comments in UIFlow source are Vietnamese (code comments are not covered by the English-only rule for stored docs).

## Open issues

| Issue | Summary |
|-------|---------|
| — | Login, Inventory, Quest Real providers still stubs; minimap placeholder disabled |
| BUG-087 | Game over works only via scene reload — no gameplay `Reborn()` |
| BUG-052 | No GDD / ADR for UIFlow |
