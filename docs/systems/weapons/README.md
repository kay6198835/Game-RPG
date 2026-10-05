# Weapons + Projectiles

> **Status:** live (melee + ranged, player and enemy) · **Last verified:** 2026-10-05, HEAD `93ba6d8e`
> History: [CHANGELOG.md](CHANGELOG.md) · GDD: `design/gdd/weapons-system.md`

## Purpose

Equippable weapons for any character (`IWeaponHolder`), data-driven by SO stages, plus the shared
pooled projectile used by ranged weapons and projectile abilities.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/Weapons/Weapon.cs` | Abstract base, `[RequireComponent(Collider2D)]`. `CanAttack()` → `OnAttackEnter(user)` → `OnActivate()` (hit frame) → `OnDeactivate()` → `CanChain()`; `Equid()` / `UnEquid()` |
| `Weapons/WeaponStats.cs` | Abstract SO: `LayerMask`, `AttackStages`, `StatModifiers` (`StatModifierGroup`) |
| `Weapons/MeleeWeapon/` | `MeleeWeapon` (reference implementation: `OverlapCircleNonAlloc` → `TakeDamage`), `MeleeWeaponStats`, `SwordAndShield` (empty), `AttackSO` |
| `Weapons/RangeWeapon/` | `RangeWeapon`, `RangeWeaponStats` (`ProjectileCount`), `RangeAttackSO` |
| `System/Abilities/Runtime/SpawnMono/ProjectileBody.cs` | Shared projectile + `struct ProjectileConfig` |
| `Interface/IProjectilePayload.cs`, `IWeaponHolder.cs` | Contracts |
| `Character/Base/WeaponHolderBase.cs` | Equip / unequip for both sides; injects the weapon |
| Assets | `Assets/SO/Weapons/` incl. `RangeWeapons/Player Range Weapon/` (3 stages + stats, added 2026-09-28) |

## How it works

**Equip.** `CharacterData.DefaultWeapon` (`WeaponSO`) names a prefab carrying a `Weapon`.
`WeaponHolderBase.Equid()` calls `resolver.InjectGameObject(weapon.gameObject)` (VContainer) and
stores the weapon; a second call unequips. So every weapon — default spawn or pickup — receives its
`[Inject]` dependencies at equip time.

**Melee.** `PlayerAttackState [OnActivate]` → `WeaponHolder.MakeDamage()` → `MeleeWeapon.OnActivate()`
→ `OverlapCircleNonAlloc(pos, attackRange, buffer, LayerMask)` → `INegativeReceiver.TakeDamage(finalDamage, pos)`.

**Damage formula** (`WeaponHolderBase.CalculateCurrentDamage()`, passed to `OnActivate(finalDamage)`):
`finalDamage = PhysicalDamage + currentStage.attackDamage (+ CritDamage if RollChance(CritChance))`.
The receiver then applies `Mitigate()` (enemy: subtract `Defense`, clamp at 0). Field was `attackDamege` until 2026-09-01 — see BUG-095.

**Ranged.** `RangeWeapon : Weapon, IProjectilePayload`:

```
OnActivate(): firePoint = owner position + aimDirection * spawnOffset
  for i in ProjectileCount: angle fanned across SpreadAngle
    go = IObjecPoolService.Spawn(stage.ProjectilePrefab, firePoint, rotation)
    go.ProjectileBody.Launch(rotation * right, stage.ProjectileConfig, payload: this)
ProjectileBody.OnTriggerEnter2D:
    layer in blockMask  → release
    layer in targetMask → payload.OnHit(target, pos) → release
RangeWeapon.OnHit(): target.TryGetComponent(INegativeReceiver) → TakeDamage(currentStage.attackDamage, pos)
    ⚠️ ignores finalDamage — no PhysicalDamage / crit on ranged hits (BUG-093)
```

`RangeAttackSO`: `ProjectilePrefab`, `ProjectileCount` (1-20), `SpreadAngle` (0-90),
`RecoveryTime` (0.02-5), `ProjectileConfig` (speed 1-60, lifetime 0.1-30, `targetMask`, `blockMask`).

**ProjectileBody** (`[RequireComponent(Rigidbody2D)]`): gravity 0, collider forced to trigger;
`Launch(direction, config, payload)` sets velocity and starts a lifetime despawn coroutine;
`OnDisable` clears payload and velocity so a pooled instance never carries the previous shot;
release returns to the pool, or `Destroy` if it was not spawned by the pool.

## Rules

- Weapons never reference `Player`, `WeaponHolder` or `PlayerInputHandler` — use `IWeaponHolder`.
- No weapon, no attack — there is no fallback attack path.
- Layer masks set in the Inspector only.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-095 | `AttackSO.attackDamege` → `attackDamage` rename (2026-09-01) had no `[FormerlySerializedAs]`; `SnS_State1-3.asset` still store `attackDamege: 55` → the player sword loses its 55 stage damage — `WeaponHolderBase.CalculateCurrentDamage()` (`:92-98`) computes `PhysicalDamage + attackDamage (+ CritDamage on crit)`, so a hit deals only the character's `PhysicalDamage`. Confirm in Play Mode |
| BUG-093 | `RangeWeapon.nextFireTime` written (`:67`) but never read → `RecoveryTime` has no effect; gizmo draws `firePoint + firePoint * range`; `OnHit()` ignores `finalDamage` (no `PhysicalDamage`/crit) and reads `currentStage` at hit time |
| — | `firePoint` is a serialized `Vector2` overwritten on every shot — it is runtime state, not authored data |
| — | `bullet.cs` was the "known debt" example in `gameplay-code.md`; it is deleted, `Projectile.cs` / `Spell.cs` still look down with `GetComponentInChildren` |

## Related

[abilities](../abilities/README.md) (projectile abilities share `ProjectileBody`),
[object-pooling](../object-pooling/README.md), `.claude/rules/weapon-skill-code.md`.
