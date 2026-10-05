# Object Pooling

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e`
> History: [CHANGELOG.md](CHANGELOG.md) · No GDD

## Purpose

Reuse runtime-spawned objects (enemies, projectiles, summons, items) instead of `Instantiate` /
`Destroy`, and inject their dependencies when first created.

## Code — `Assets/Script/System/PoolableService/`

| File | Contents |
|------|----------|
| `ObjectPoolManager.cs` | `: MonoBehaviour, IObjecPoolService`. One `Pool` per prefab (`pools` dictionary). `Get(prefab)`, `Spawn(prefab, Vector2 pos, Quaternion rot, parent)` → `GameObject`, `Spawn(ObjectPoolRequest)`, `Release(go, parent)`. `[Inject] Construct(IObjectResolver)` |
| `Pool.cs` | Per-prefab queue. `Spawn()` reuses an inactive member or instantiates + `resolver.InjectGameObject(obj)`; `Release()` deactivates, reparents to the pool (or `parent`), enqueues |
| `PoolMember.cs` | Back-reference to its `Pool`; `isInPool` flag guards double release |
| `LifetimeScope/Interface/IObjecPoolService.cs` | The injected contract (typo in name is intentional) |

## Rules

- Never `Instantiate` gameplay objects at runtime — go through `IObjecPoolService`.
- Objects are injected **once**, when the pool first instantiates them. Pooled objects must reset
  their own state in `OnEnable` / `OnDisable` (`Entity.OnEnable`, `ProjectileBody.OnDisable` do).
- Releasing twice is a no-op (`PoolMember.isInPool`).

## Consumers

`EnemySpawner`, `ItemSpawner`, `RangeWeapon`, `SpawnEffectBase` (abilities), `ProjectileBody`
(self-release), `SpawnMono` (lifetime despawn).
