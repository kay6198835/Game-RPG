# Dependency Injection (VContainer) + Player Service

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` · **ADR:** ADR-0004 · VContainer 1.19.0 (git package)
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

Wire cross-system services into scene components and runtime-spawned objects without singletons.

## Code

| Path | Contents |
|------|----------|
| `System/LifetimeScope/GameLifetimeScope.cs` | Composition root (`: LifetimeScope`) |
| `System/LifetimeScope/Interface/` | `IObjecPoolService`, `IPlayerService`, `IPlayerStatService : IStatService` |
| `System/PlayerSystem/PlayerManager.cs` | `: MonoBehaviour, IPlayerService` — spawns and exposes the player |

## Registrations (`GameLifetimeScope.Configure()`)

```csharp
builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();
builder.RegisterComponentInHierarchy<PlayerManager>().AsSelf().As<IPlayerService>();
builder.Register<IPlayerStatService>(r => r.Resolve<PlayerManager>().StatService, Lifetime.Singleton);
builder.RegisterComponentInHierarchy<EnemySpawner>();
builder.RegisterComponentInHierarchy<StatsUIController>();
builder.RegisterComponentInHierarchy<ItemSpawner>();
builder.RegisterComponentInHierarchy<RoomGeneraterController>();
builder.RegisterComponentInHierarchy<RoomGridController>();
builder.RegisterComponentInHierarchy<LevelManager>();
```

`StatsScreenUIController` and `StatPointAllocator` stay commented out (not guaranteed in the scene).

## How objects get their dependencies

| Object kind | Mechanism |
|-------------|-----------|
| Scene component listed above | `RegisterComponentInHierarchy` (finds, never spawns — missing = throw at `Awake`) |
| The player | `PlayerManager.SpawnPlayer()` → `resolver.Instantiate(playerPrefab, spawnPoint, parent)` injects the whole hierarchy before `Awake()`; `Camera.main` is parented to the player |
| Pooled objects | `Pool.Spawn()` → `resolver.InjectGameObject(obj)` on first instantiate |
| Weapons | `WeaponHolderBase.Equid()` → `resolver.InjectGameObject(weapon.gameObject)` |

`PlayerManager.Player` is lazy: the first consumer (the `IPlayerStatService` factory during
container build, or `PlayerManager.Awake()`) spawns the player, so registration order does not matter.

## Rules

- `GameLifetimeScope` holds no gameplay state or logic.
- Inject behind an interface; never resolve the container manually from gameplay code.
  ⚠️ Existing exceptions: `PlayerManager` is registered `AsSelf()` and injected concretely into
  `RoomGridController`; `LevelManager` is injected concretely into `RoomGeneraterController`.
- Siblings inside one character still use `Core.GetCoreComponent<T>()`.
- UIFlow does **not** use this container — it has its own static `UIServices` locator (see [ui](../ui/README.md)).
