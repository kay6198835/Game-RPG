---
description: Weapon and skill/ability code standards — WeaponMelee, RangeWeapon, ActivateSkill, AbilityHolder
globs: ["Assets/Script/Weapons/**/*.cs", "Assets/Script/Skill_Ability/**/*.cs"]
---

# Weapon and Skill Code Standards

## Damage Application Contract

> Updated 2026-08-20. `WeaponMelee.cs` was renamed `MeleeWeapon.cs` and the hit frame moved
> from `Attack()` to `OnActivate()`. The old "mirror `EntityWeaponMelee`" instruction is now
> backwards — that class still uses the allocating `Physics2D.OverlapCircle` (BUG-046).

- `MeleeWeapon.OnActivate()` MUST call `Physics2D.OverlapCircleNonAlloc` into a buffer cached
  in `Awake()`, then `INegativeReceiver.TakeDamage()` on every hit
- **`MeleeWeapon.OnActivate()` is the reference implementation** — copy it, not
  `EntityWeaponMelee.Attack()`
- `TakeDamage(int amount, Vector2 attackPosition)` — always pass `transform.position` as the
  second arg
- The weapon lifecycle is `CanAttack()` → `OnAttackEnter(player)` → `OnActivate()` (hit frame)
  → `OnDeactivate()` → `CanChain()`. There is no `CheckCanAttack()` any more

## ScriptableObject-First
- Attack stats (damage, range, animation override) live in `AttackSO`; ranged adds `RangeAttackSO` (bullet prefab, projectile count, spread, recovery time) — never hardcode in MonoBehaviour
- Skill parameters (cooldown, duration, effect) live in the `ActivateSkill` SO subclass
- New weapon types = new SO asset + new MonoBehaviour that reads from it

## Skill Lifecycle Contract (ActivateSkill)
- Override sequence: `Enter(player)` → `Activate()` → `Cast()` [button held] → `Do()` [released] → `Exit()`
- `Do()` for one-shot skills; `Cast()` + `Do()` for hold-release skills — never skip steps
- `AbilityHolder` drives the lifecycle every frame — do not call lifecycle methods from state classes directly

## Layer Masks
- Attack hitbox layer masks MUST be set in Inspector on `EntityData` or `WeaponStats.LayerMask` — never hardcode layer indices
- `Physics2D.OverlapCircleNonAlloc(pos, range, results, layerMask)` — always pass the configured mask

## Projectile Rules
- `Projectile.cs` handles raycast hit → `INegativeReceiver.TakeDamage()`
- `Spell.cs` extends Projectile and additionally calls `IEffectable.ApplyEffect()` — do not merge the two
- Projectiles must be pooled — never `Instantiate` a projectile in `Update()`
