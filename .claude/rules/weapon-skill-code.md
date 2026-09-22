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

> Re-verified against source 2026-09-22 (HEAD `d17fcc5`). Two bug IDs cited below were wrong and are
> corrected; four rules are added.

- **The spawn-damage contract is set-then-invoke, and it is deliberate.** A spawned object that
  detects a hit assigns `ctx.Services.NegativeReceiver` and *then* calls `_callback.Invoke(ctx)`;
  the effect's `if (negativeReceiver != null)` guard means "act if a receiver was supplied, skip if
  not". `SpawnProjectileBase.cs:31-33` is the reference implementation — copy that shape. For a
  multi-target summon, assign and invoke **once per target** inside the loop
- ⚠️ **No Abilities v2 effect deals damage at HEAD**, from two independent gaps in that contract,
  neither of them architectural: the projectile never runs its assign+invoke because it declares the
  3D trigger signature (**BUG-075**), and `LightningController.Execute()` invokes the callback
  without ever running its overlap query, which is still three comment lines (**BUG-072**). Fixing
  either one restores that path on its own
- Effect and condition `ScriptableObject`s are **shared, single-instance assets**. Never store
  per-cast state in a field on one. `HasEnoughManaCondition.currentMana` / `.costMana` (BUG-077)
  violates this and serializes runtime values into a committed `.asset` — do not copy it.
  `SpawnEffectBase._context` / `.dir` are the same shape; they are load-bearing today, because
  `SpawnSummonEffect.Apply()` relies on `Casting()` having set `_context` for it. That coupling is
  intentional given `Cast` always precedes `Do` — leave it alone unless you are redesigning the
  effect base, and do not "simplify" `Casting()` without setting `_context` in `Apply()`
- `AbilityEffectDefinition.Casting()` **is a gate**: it returns `false` to refuse the cast, and the
  base implementation already tests `SubConditions` and calls `CheckPayCostValid()` before the
  per-effect cost is paid (`AbilityEffectDefinition.cs:11-22`, `AbilityInstance.cs:83-86`). It is
  slated to be renamed `TryCasting()` for exactly that reason. ⚠️ `Casting()` and `Apply()` run at
  **different `AbilityState` phases of the same cast** (`Cast` and `Do`), so an effect that acts in
  both acts twice — **by design** for a two-object effect such as Consecrate (Cast-phase telegraph
  from `Prefab`, Do-phase payload from `summonPrefab`), and a bug otherwise. Know which you are
  writing
- 2D trigger callbacks are `OnTriggerEnter2D(Collider2D)`. `SpawnProjectileBase` uses the 3D
  signature and therefore never fires (BUG-075). ⚠️ The 3D form **compiles clean with no warning** —
  `UnityEngine.Collider` exists in every Unity project. Verify a new trigger handler with a
  `Debug.Log`, not by checking the Console for errors
- `AbilityContext.HoldTime` / `.HoldRatio` are **always `0f`** (BUG-083): they are plain fields
  snapshotted by `BuildContext()` before `StartHold()` zeroes the counter, and nothing rebuilds the
  context. Do not write a charge-scaling effect against them until that is fixed
- Cost is paid from **two disjoint lists** since `73ab8e7`: `AbilityDefinition.Costs` once at
  activation, and per-effect `AbilityEffectDefinition.Costs` inside `Casting()`. The two are billed
  differently on purpose — a `Hold` ability re-enters `TryCastInstant()` while held, so a per-effect
  cost is a **channelled** cost that keeps draining. Put a one-off cost in `AbilityDefinition.Costs`
- ⚠️ **Ability-scope costs are not affordability-checked** (BUG-076 b′). `TryPayCost()`
  (`AbilityInstance.cs:164-178`) pays unconditionally, and the only gate is
  `HasEnoughManaCondition`, which reads `StatType.Mana` and nothing else. Avatar of Light costs
  40 Mana **+ 50 HP**; the HP half is validated by nothing, and `Reduction()` clamps at zero, so the
  cast drains the player to 0 HP instead of being refused. Do not author a non-Mana ability-scope
  cost until this is fixed. The effect-scope list does **not** have this problem

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
