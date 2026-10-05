# Event System (EventManager)

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` · **23 `EventID` values, unchanged since 2026-09-07**
> History: [CHANGELOG.md](CHANGELOG.md) · Rules: `.claude/rules/manager-event-code.md`

## Purpose

Static, typeless publish/subscribe bus for cross-system gameplay events.

## Code — `Assets/Script/Manager/EventManager.cs`

```csharp
public static class EventManager
{
    static Dictionary<EventID, Action<object>> _events;
    static void Resgister(EventID, Action<object>);    // typo intentional
    static void UnResgister(EventID, Action<object>);
    static void Emit(EventID, object obj = null);
}
```

## EventID values

| Group | Values | Producers → consumers (where known) |
|-------|--------|--------------------------------------|
| Room / map | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`, `ON_CLEAR_ENEMY`, `ON_ROOM_CLEAR` | `DoorController` → `RoomGridController`, `MapGridController`; `ON_ROOM_CLEAR` has **no producer** |
| Spawn | `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY` | `RoomGeneraterController` → `EnemySpawner` → `RoomCell` |
| Life cycle | `ON_PLAYER_DEATH`, `ON_ENEMY_DEATH`, `ON_REALOAD_GAME` | `PlayerDeathState` (every frame, BUG-086) → `UIFlow.RealPlayerDataProvider` (only subscriber); `EntityDeathState` → `RoomCell`; `ON_REALOAD_GAME` 0 emitters, 0 subscribers |
| Stats UI | `ON_OPEN_STATS_PLAYER_UI`, `ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`, `ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`, `ON_RESTORE_STATS_BY_UI`, `ON_RESET_STATS_UI_SESSION` | Stats UI / `StatPointAllocator` |
| Item | `ON_DROP_ITEM`, `ON_COLLECT_ITEM` | `ItemController` → `ItemSpawner` (release to pool). `ON_DROP_ITEM` has **no producer and no consumer** — drops are driven by `ON_ENEMY_DEATH` |
| Debug | `ON_TEST` | no producer, no consumer (`FastTest/FasTestEnemyDeath` emits `ON_ENEMY_DEATH`, not this) |

Missing: `ON_PLAYER_TAKE_DAMAGE` (needed by the UIFlow HUD and named by `ui-code.md`; never existed).

## Rules

- Register in `OnEnable`, unregister in `OnDisable`.
- New events = a new `EventID` value; never `static Action` fields on gameplay classes.
  ⚠️ UIFlow keeps its own separate static bus `UIFlow.UIEvents` (static `event Action`s for dialogue,
  shop, notification, damage number, skill used) — scoped to UI, but it is the pattern this rule forbids.
- `AnimationEventManager` is a separate, dead dictionary (`Emit()` has zero callers).
