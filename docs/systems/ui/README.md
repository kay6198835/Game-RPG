# UI — UIFlow (current) + legacy UI

> **Status:** UIFlow live on Mock data; Real data providers are stubs · **Last verified:** 2026-10-05, HEAD `93ba6d8e` · **No GDD, no ADR** (BUG-052)
> History: [CHANGELOG.md](CHANGELOG.md) · **Deep reference:** `docs/ui/ui-ux-flow.md` (flow, flags, every panel, findings)

## Purpose

Everything the player sees outside the world: boot/menu/save/character creation, the loading
screen, and the in-game HUD and windows — built so UI work can proceed against mock data while
gameplay catches up.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/UIFlow/` (65 files, `namespace UIFlow`, UGUI + TextMeshPro) | |
| `Core/` | `SceneFlow` (static scene routing; every transition goes through `Loading`), `SceneNames`, `UIManager` (panel stack: `Open/Close/Back/CloseAll`, Esc handling), `UIPanel`, `UIInput`, `UIBootstrap`, `UIEvents` (static UI bus), `UIServices` (static Mock/Real locator), `UIDebugConfig` (SO) |
| `Data/UIDataModels.cs` | DTOs: `PlayerStatsData`, `SkillSlotData`, `SkillNodeData`, `CharacterClassInfo`, … |
| `Menu/` | `SplashPanel`, `LoginPanel`, `MainMenuPanel` + `MainMenuFlow`, `SaveSelectPanel` + `SaveSlotView`, `CharacterCreationPanel`, `SettingsPanel` + `KeyBindingRow` |
| `Loading/LoadingScreen.cs` | Async load of `SceneFlow.TargetScene`, minimum display time, tips |
| `Gameplay/` | `GameplayUIController`; `HUD/` (HUDPanel, StatBar, SkillHotbar, QuestTracker, NotificationFeed); `Windows/` (TabWindow, Inventory + InventorySlot + ItemTooltip, Character, Quest, SkillTree); `Shop/`; `Dialogue/`; `World/` (DamageNumber + spawner, WorldHealthBar); `PausePanel`, `GameOverPanel`; `Mock/` (GameplayMockController, MockEnemy) |
| `Services/` | `Interfaces/` (ILoginService, ISaveProvider, IPlayerDataProvider, IInventoryProvider, IQuestProvider); `Mock/` (all implemented, `MockCatalog`); `Real/` (stubs); `KeyBindings`, `SettingsStore` |
| `Editor/` | `UIFlowBuilder` (+ `.Gameplay`) — generates the scenes; `UIFlowSmokeTest` — menu-driven Play Mode walkthrough (not an NUnit test); `UIKit` |
| `Assets/Script/UI/` (legacy) | `UIController` (UI Toolkit MainMenu/Settings/Pause from `Assets/UI/Screens/*.uxml`), `StatsUIController` (VContainer-registered), `StatsScreenUIController`, `StatSlot` |
| `Assets/Script/Manager/UI/UIManager.cs` | Legacy **empty stub** (global namespace) — unrelated to `UIFlow.UIManager` |

Scenes: `Main/MainGamePlay` (menus), `Main/Loading`, `Main/GameplayUI` (additive in-game UI),
`Test/GameplayMock` (fake gameplay). Real gameplay scene: `Main/Test/LoadRandomMap`.

## Flow

```
MainGamePlay: Splash → (Login, unless skipLogin) → MainMenu → SaveSelect → CharacterCreation
  → SceneFlow.EnterGameplay()
      skipGameplayInit = true  → Loading → GameplayMock + GameplayUI (additive)
      skipGameplayInit = false → Loading → LoadRandomMap (+ GameplayUI only if bypassLoadRandomLogic = false)
In game: HUD root; panels pushed on UIManager's stack; Esc = Back / Pause
Death: ON_PLAYER_DEATH → RealPlayerDataProvider.PlayerDied → GameOverPanel → Respawn() = reload gameplay scene
```

`UIDebugConfig` flags (all default **on**): `useMockData`, `skipLogin`, `skipGameplayInit`,
`skipSaveLoad`, `bypassLoadRandomLogic`; plus `splashDuration`, `minLoadingTime`.

## Real vs Mock today

| Provider | Real implementation |
|----------|---------------------|
| `IPlayerDataProvider` | Death event only; stats, hotbar, classes, skill tree are placeholders / empty |
| `ISaveProvider` | `RealSaveProvider` implemented (114 lines) — not verified in this pass |
| `ILoginService`, `IInventoryProvider`, `IQuestProvider` | Stubs |

## Deviations from project rules

- `UIServices` is a static service locator chosen by flags — bypasses VContainer (ADR-0004).
- `UIEvents` is a static `event Action` bus — `manager-event-code.md` forbids `static Action` fields.
- Built on UGUI for menus; `VERSION.md` says new *menu* screens follow UI Toolkit.
- Comments in UIFlow source are Vietnamese (code comments are not covered by the English-only rule for stored docs).

## Open issues

| Issue | Summary |
|-------|---------|
| Demo item 25 | Wire Real providers: HP/Mana push event (`ON_PLAYER_TAKE_DAMAGE` does not exist), hotbar from `PlayerData.AbilityBindings` |
| BUG-087 | Game over works only via scene reload — no gameplay `Reborn()` |
| BUG-052 | No GDD / ADR for UIFlow |
| TD-017 | Legacy `Manager/UI/UIManager.cs` empty stub still compiled |
