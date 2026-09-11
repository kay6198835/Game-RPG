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
- Get player data via an SO injected in the Inspector, via `EventManager`, or via an injected
  service interface — never by reaching for the player object

## Where player values come from (corrected 2026-09-11)

The Sprint 12 stat refactor split ownership; UI must read from the right side:

| Value | Source |
|---|---|
| **Current** HP / Mana | `VitalStatsComponent.GetCurrentStatValue(StatType)` (`IVitalComponent`) |
| **Max** / derived stat values | `StatHandler.GetStatValue(StatType)` (`IPlayerStatService`) |
| Stat rows for display | `StatHandler.GetFullViewStats()` → `StatsViewDTO` |

`PlayerData.currentHealth` is **not** a valid source — it is never written (Bug #6).
`StatsSO` no longer exists; the profile type is `BaseStatsSO`.

## Dependency Injection (added 2026-09-11)
- `StatsUIController` is registered in `GameLifetimeScope` and resolved through VContainer
- `StatsScreenUIController` and `StatPointAllocator` are **deliberately not registered** (both lines
  are commented out in `Configure()`) because they are not guaranteed to be in the scene hierarchy
  at `Awake()`. Do not uncomment them without confirming scene placement —
  `RegisterComponentInHierarchy<T>()` finds but never spawns, and throws when the component is absent
- UI still must not own game state: injecting `IPlayerStatService` is for **reading**.
  ⚠️ `StatsUIController` currently calls `AddPrimaryPoint()`, which writes gameplay state from the
  UI layer — a standing violation of the first rule in this file, not a pattern to copy
