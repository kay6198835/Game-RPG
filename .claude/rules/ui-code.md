---
description: UI code standards — UIManager, HUD, health bar, menus
globs: ["Assets/Script/Manager/UI/**/*.cs", "Assets/Script/MainMenu/**/*.cs"]
---

# UI Code Standards

## No Game State Ownership
- UI scripts must NEVER own or mutate game state (health, mana, damage values)
- UI reads from `PlayerData` ScriptableObject or subscribes to `EventManager` events — it never writes
- `UIManager` is a display layer only; it must not contain game logic

## Event-Driven Updates
- Health bar, mana bar, and score displays update via `EventManager` subscriptions, not polling in Update
- Subscribe in `OnEnable()`, unsubscribe in `OnDisable()` — never in `Awake/Start` alone
- Use `EventManager.Resgister(EventID, callback)` — note: intentional typo in source, use as-is

## Localization Ready
- All player-visible strings must go through `TextMeshProUGUI` — never use legacy `Text` component
- String literals visible to players must be defined as constants or loaded from a data source, not inline

## Performance
- Pool UI elements in lists (upgrade cards, item grids) — use `ObjectPool<T>` or manual pooling
- Use `CanvasGroup.alpha` for show/hide transitions — never `SetActive` for animated elements
- Disable Canvas components when off-screen — don't just make them invisible

## No Direct References to Enemies/Player
- UI must not hold a direct `GameObject` reference to the player or enemies
- Get player data via `PlayerData` SO injected in Inspector or via `EventManager`
