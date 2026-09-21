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

> Corrected 2026-09-21 against HEAD `15242e6`. The enum is **`AbilityState`**, not `SkillState`
> (renamed when the framework left `prototypes/`), and it has **no `None` member`**. The effect
> layer this section described (`ShootObjectEffect`, `DamageInFrontEffect`, `LungeForwardEffect`)
> was deleted and replaced by the `SpawnEffectBase` / `StatsEffectBase` hierarchies.

- An ability is a **`AbilityDefinition` SO asset**, composed from a list of
  `AbilityEffectDefinition` + a list of `AbilityConditionDefinition` — not a subclass
- Runtime state lives in `AbilityInstance` (cooldown, hold time); phases are the **`AbilityState`**
  enum: `Start → Cast → Do → Exit` (declared in `AbilityDefinition.cs`, not its own file)
- Bound per `AbilitySlot` (Primary / Secondary / Utility / Ultimate) via `AbilityHolder.abilityBindings`,
  which is **overwritten in `Start()`** from `Player.Data.AbilityBindings` — author the bindings on the
  `PlayerData` SO, not on the component
- `AbilityHolder : IAbilityOwner` drives it every frame from `PlayerSkillWeaponState` — do not
  call `TryActivateInstant()` / `TryCastInstant()` / `TryDoInstant()` from a state class directly
- New behaviour = a new `AbilityEffectDefinition` subclass, reusable across abilities. Costs go in
  `AbilityDefinition.Costs` (`List<StatCost>`), never hardcoded

**The two effect hierarchies — pick the right base:**

| Base | Use for | Concrete subclasses |
|---|---|---|
| `SpawnEffectBase` | anything that instantiates a pooled world object | `SpawnProjectileEffect`, `SpawnSummonEffect` |
| `StatsEffectBase` | anything that reads a `StatModifierGroup` and applies it via `IVitalComponent` | `RecoveryReductionStatsEffect`, `RecoveryReductionPerTimeForDuration` |
| `AbilityEffectDefinition` direct | a one-off that fits neither | `BuffDebuffStatsForDuration` |

- Spawned objects extend **`SpawnMono`** (`Runtime/BaseController/`), then `SpawnProjectileBase` or
  `SpawnSummonBase` (`Runtime/SpawnMono/`). A summon fires its payload from a Unity Animation Event
  calling `Execute()`; a projectile fires from its trigger callback
- Spawning goes through `context.Services.Pool.Spawn(...)` — never `Instantiate`
- Cross-system access inside an effect goes through `AbilityContext.Services` (`IAbilityServices`:
  `Pool`, `Stats`, `ResourceReceiver`, `Vital`, `NegativeReceiver`). Do not reach for a singleton or
  `FindObjectOfType` from an effect asset

**Rules that exist because of open bugs — do not copy the surrounding code:**

- Effect and condition `ScriptableObject`s are **shared, single-instance assets**. Never store
  per-cast state in a field on one. `SpawnEffectBase._context` / `.dir` (BUG-079) and
  `HasEnoughManaCondition.currentMana` / `.costMana` (BUG-077) both violate this; the second one
  serializes runtime values into a committed `.asset`
- `AbilityEffectDefinition.Casting()`'s return value has **no settled contract** (BUG-076, BUG-078).
  Until an ADR or GDD defines it, do not add a new override of `Casting()`
- 2D trigger callbacks are `OnTriggerEnter2D(Collider2D)`. `SpawnProjectileBase` uses the 3D
  signature and therefore never fires (BUG-075)

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
