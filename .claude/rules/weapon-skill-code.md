---
description: Weapon and skill/ability code standards — WeaponMelee, RangeWeapon, ActivateSkill, AbilityHolder
globs: ["Assets/Script/Weapons/**/*.cs", "Assets/Script/Skill_Ability/**/*.cs"]
---

# Weapon and Skill Code Standards

## Damage Application Contract
- `WeaponMelee.Attack()` MUST call `Physics2D.OverlapCircleNonAlloc` then `INegativeReciver.TakeDamage()`
- Mirror `EntityWeaponMelee.Attack()` exactly — it is the reference implementation
- `TakeDamage(int amount, Vector2 attackPosition)` — always pass transform.position as second arg

## ScriptableObject-First
- Attack stats (damage, range, animation override) live in `AttackSO` — never hardcode in MonoBehaviour
- Skill parameters (cooldown, duration, effect) live in the `ActivateSkill` SO subclass
- New weapon types = new SO asset + new MonoBehaviour that reads from it

## Skill Lifecycle Contract (ActivateSkill)
- Override sequence: `Enter(player)` → `Activate()` → `Cast()` [button held] → `Do()` [released] → `Exit()`
- `Do()` for one-shot skills; `Cast()` + `Do()` for hold-release skills — never skip steps
- `AbilityHolder` drives the lifecycle every frame — do not call lifecycle methods from state classes directly

## Layer Masks
- Attack hitbox layer masks MUST be set in Inspector on `EntityData` or `AttackSO` — never hardcode layer indices
- `Physics2D.OverlapCircleNonAlloc(pos, range, results, layerMask)` — always pass the configured mask

## Projectile Rules
- `Projectile.cs` handles raycast hit → `INegativeReciver.TakeDamage()`
- `Spell.cs` extends Projectile and additionally calls `IEffectable.ApplyEffect()` — do not merge the two
- Projectiles must be pooled — never `Instantiate` a projectile in `Update()`
