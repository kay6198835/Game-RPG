# Weapons + Projectiles

> **Status:** live (melee + ranged, player and enemy) · **Last verified:** 2026-11-15, `sprint-22` `063b5f87` (Open issues only); 2026-11-05, `sprint-21` `028fdb90` (Equip paragraph); rest 2026-10-05, HEAD `93ba6d8e`
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

**Equip.** `CharacterData.Weapon` (a `Weapon` prefab component; *was* `DefaultWeapon` / `WeaponSO`
until `cae75317`) is spawned from the pool by `WeaponHolderBase.EquipDefaultWeapon()`, which first
unequips and releases the previous weapon, returns when the data has no weapon, then calls
`resolver.InjectGameObject(instance)` (VContainer) and `Equid()`. It runs from
`CharacterBase.ApplyCharacterData()` on every `SetCharacterData()` — player (Champion pick) and enemy
(every spawn, `EnemySpawner`) — since `6094aa10`. Floor pickup does not reach this path (BUG-110).

**Melee.** `PlayerAttackState [OnActivate]` → `WeaponHolder.MakeDamage()` → `MeleeWeapon.OnActivate()`
→ `OverlapCircleNonAlloc(pos, attackRange, buffer, LayerMask)` → `INegativeReceiver.TakeDamage(finalDamage, pos)`.

**Damage formula** (`WeaponHolderBase.CalculateCurrentDamage()`, passed to `OnActivate(finalDamage)`):
`finalDamage = PhysicalDamage + currentStage.attackDamage (+ CritDamage if RollChance(CritChance))`.
The receiver then applies `Mitigate()` (enemy: subtract `Defense`, clamp at 0). Field was `attackDamege` until 2026-09-01 — see BUG-095.

**Ranged.** `RangeWeapon : Weapon, IProjectilePayload`:

```
OnActivate(): firePoint = owner position + aimDirection * spawnOffset
  for i in ProjectileCount: angle fanned across SpreadAngle
    go = IObjectPoolService.Spawn(stage.ProjectilePrefab, firePoint, rotation)
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
| BUG-125 | Melee hits add the attack-stage damage twice: `WeaponHolderBase.CalculateCurrentDamage()` (`:90-98`) adds `CurrentStage.attackDamage`, then `Weapon.FinalDamage()` (`Weapon.cs:67-70`, `38d40925`) adds it again; `MeleeWeapon.OnActivate()` calls `TakeDamage(FinalDamage(…))`. Player and enemy (S2, Open) |
| BUG-127 | Mage basic-attack bolt `speed: 1` / `lifetime: 4` (`Mage Attack SO State 1.asset`) — barely reaches (S3, Open) |
| BUG-103 | `RangeWeapon.nextFireTime` written but never read → `RecoveryTime` has no effect; gizmo draws `firePoint + firePoint * range` (S3, Open). *Was listed as "BUG-093" — ID collision, see BUG-101* |
| — | `OnHit()` ignores `finalDamage` (no `PhysicalDamage`/crit) and reads `currentStage` at hit time — not re-checked this pass; damage-model question is inbox NOTE-20261104-4 (Needs owner) |
| ~~BUG-095~~ → BUG-105 | *Was:* "`attackDamege` → `attackDamage` rename … the player sword loses its 55 stage damage". That defect is **BUG-105**, FIXED in data by `fbab752a` (2026-11-05). BUG-095 is the `Arrow.prefab` collider bug |
| — | `firePoint` is a serialized `Vector2` overwritten on every shot — it is runtime state, not authored data |
| — | `bullet.cs` was the "known debt" example in `gameplay-code.md`; it is deleted, `Projectile.cs` / `Spell.cs` still look down with `GetComponentInChildren` |

## Related

[abilities](../abilities/README.md) (projectile abilities share `ProjectileBody`),
[object-pooling](../object-pooling/README.md), `.claude/rules/weapon-skill-code.md`.
