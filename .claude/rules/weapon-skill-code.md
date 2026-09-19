---
description: Weapon and skill/ability code standards — WeaponMelee, RangeWeapon, ActivateSkill, AbilityHolder
globs: ["Assets/Script/Weapons/**/*.cs", "Assets/Script/Skill_Ability/**/*.cs"]
---

# Weapon and Skill Code Standards

## Damage Application Contract

> Updated 2026-09-11. `EntityWeaponMelee.cs` has since been **deleted** (BUG-043 / BUG-046
> closed), so the comparison it used to draw is gone. The damage parameter is now `float`,
> not `int`.

- `MeleeWeapon.OnActivate()` MUST call `Physics2D.OverlapCircleNonAlloc` into a buffer cached
  in `Awake()`, then `INegativeReceiver.TakeDamage()` on every hit
- **`MeleeWeapon.OnActivate()` is the reference implementation** — copy it
- `TakeDamage(float amountDamage, Vector2 attackPosition)` — note `float`; always pass
  `transform.position` as the second arg
- ⚠️ `EntityAttack.Attack()` is **not** a reference implementation: it still hardcodes
  `TakeDamage(10, …)` and duplicates `EntityWeapon` (BUG-043). Do not copy it
- The weapon lifecycle is `CanAttack()` → `OnAttackEnter(player)` → `OnActivate()` (hit frame)
  → `OnDeactivate()` → `CanChain()`. There is no `CheckCanAttack()` any more

## ScriptableObject-First
- Attack stats (damage, range, animation override) live in `AttackSO`; ranged adds `RangeAttackSO` (bullet prefab, projectile count, spread, recovery time) — never hardcode in MonoBehaviour
- Skill parameters (cooldown, duration, effect) live in the `ActivateSkill` SO subclass
- New weapon types = new SO asset + new MonoBehaviour that reads from it

## Skill Lifecycle — TWO frameworks coexist

> Rewritten 2026-09-11. The composition framework in `prototypes/skill-enhance-abilities/` was
> promoted into `Assets/Script/System/Abilities/` on 2026-09-09 (`9b8d40f`, `5c7afba`) and
> `AbilityHolder` was rewritten to drive it. This section previously described only v1, which
> the player no longer uses. Which framework the endgame keeps is **an open decision with no
> ADR** — do not assume either is being retired.

### Abilities v2 — `System/Abilities/` — the PLAYER path (author new player abilities here)

- An ability is a **`AbilityDefinition` SO asset**, composed from a list of
  `AbilityEffectDefinition` + a list of `AbilityConditionDefinition` — not a subclass
- Runtime state lives in `AbilityInstance` (cooldown, hold time); phases are the `SkillState`
  enum: `None → Start → Cast → Do → Exit`
- Bound per `AbilitySlot` (Primary / Secondary / Utility / Ultimate) via `AbilityHolder.abilityBindings`
- `AbilityHolder : IAbilityOwner` drives it every frame from `PlayerSkillWeaponState` — do not
  call `TryActivateInstant()` / `TryCastInstant()` / `TryDoInstant()` from a state class directly
- New behaviour = a new `AbilityEffectDefinition` subclass, reusable across abilities. Costs go in
  `AbilityDefinition.Costs` (`List<StatCost>`), never hardcoded

### Abilities v1 — `System/Skill_Ability/` — the WEAPON and ENEMY path (maintenance only)

- Inheritance-based `ActivateSkill` SO; lifecycle `Enter(player)` → `Activate()` → `Cast()` [held]
  → `Do()` [released] → `Exit()`
- Still referenced by `WeaponStats.AbilityWeapon` / `.SkillWeapon`, `AttackSO.ability`,
  `Weapon.currentAbilitySO`, `EntityWeapon.currentAbilitySO` — it is live, not dead code
- **Do not add new `ActivateSkill` subclasses for player abilities.** Fix bugs here; author new
  work in v2

## Layer Masks
- Attack hitbox layer masks MUST be set in Inspector on `EntityData` or `WeaponStats.LayerMask` — never hardcode layer indices
- `Physics2D.OverlapCircleNonAlloc(pos, range, results, layerMask)` — always pass the configured mask

## Projectile Rules
- `Projectile.cs` handles raycast hit → `INegativeReceiver.TakeDamage()`
- `Spell.cs` extends Projectile and additionally calls `IEffectable.ApplyEffect()` — do not merge the two
- Projectiles must be pooled — never `Instantiate` a projectile in `Update()`
