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
  call `ActivateInstant()` / `CastInstant()` / `DoInstant()` from a state class directly.
  ⚠️ **Renamed in `2a83469`** (2026-09-22): the three lost their `Try` prefix and their `bool`
  returns, because gating moved up to `AbilityInstance.CanStart()` → `AbilityDefinition.TryStart()`.
  `CanStart()` is the only thing that may refuse an activation; the three dispatch methods assume
  it already passed. Call `AbilityHolder.TryDoAbility(slot)`, never a dispatch method directly
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

> Re-verified against source 2026-09-22 (HEAD `2a83469`, post-fetch). ⚠️ **The project does not
> compile at this HEAD (BUG-088)** — unrelated file, but nothing below can be checked in the Editor
> until it is fixed. Three renames and three closures landed in `2a83469` and are folded in below.
>
> *Previous revision, 2026-09-22 against HEAD `d17fcc5`: two bug IDs cited were wrong and were
> corrected; four rules were added.*

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
  per-cast state in a field on one. The violation this rule was written against —
  `HasEnoughManaCondition.currentMana` / `.costMana` (BUG-077) — was ✅ **fixed in `2a83469` by
  deleting the class**; `Assets/Script/System/Abilities/Conditions/` is now empty. The rule stands
  regardless: `Stat.modifiers` (BUG-063) is the same defect and is still open.
  `SpawnEffectBase._context` / `.dir` are the same shape; they are load-bearing today, because
  `SpawnSummonEffect.Apply()` relies on `TryCast()` having set `_context` for it. That coupling is
  intentional given `Cast` always precedes `Do` — leave it alone unless you are redesigning the
  effect base, and do not "simplify" `TryCast()` without setting `_context` in `Apply()`
- `AbilityEffectDefinition.TryCast()` **is a gate**: it returns `false` to refuse the cast, and the
  base implementation tests `SubConditions` and calls `CheckPayCostValid()` before the per-effect
  cost is paid (`AbilityEffectDefinition.cs:11-22`, `AbilityInstance.cs:73-76`). ✅ It was renamed
  from `Casting()` in `2a83469` (2026-09-22) for exactly that reason — use the new name.
  ⚠️ **But a `false` return does not currently stop the cast (BUG-089).** `AbilityInstance.Casting()`
  responds to a refusal by calling `CancelHold()` and nothing else, and `CastInstant():54` then
  advances to `Do` anyway, so `Execute()` applies the refused effect. Write the gate correctly;
  do not rely on it being honoured until BUG-089 is fixed. ⚠️ `TryCast()` and `Apply()` run at
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
- **The order of `AbilityDefinition.Effects` is authored data, not an implementation detail.** It
  decides two things at once, and reordering the list in the Inspector changes both:
  1. **Execution order** — `AbilityInstance.Casting()` (`:68-82`) and `Execute()` (`:139-148`) both
     walk the list in index order, so `Effects[0]` casts first and applies first
  2. **Resource priority** — `Casting()` charges each effect’s cost as it goes, so `Effects[0]` has
     first claim on the player’s resources and a later effect can be refused because an earlier one
     already spent them

  Treat the order as a balance decision. This is intentional, confirmed by the owner 2026-09-22
- **Per-effect `Costs` is the price of an upgrade tier, not a go/no-go gate.** The intended
  semantics: cannot afford it → the effect still runs, at its **default** level; can afford it → the
  cost is charged and the effect runs at its **gained** level. So `Execute()` calling `Apply()`
  unconditionally is **correct** — a `false` from `TryCast()` means “no upgrade”, not “no effect”.
  On a `Hold` ability a refusal additionally **force-releases** the ability: `Casting():79` calls
  `CancelHold()`, which clears the flag `CastInstant():52` tests, so the ability leaves the channel
  and resolves into `Do`. That is deliberate
- ⚠️ **The tier feature is scaffolding — the gain fields do not exist yet.** No effect has a second
  power level, and nothing carries “paid” through to `Apply()`, so charging a cost today changes
  nothing. Do **not** author a per-effect `Costs` list until that is built (see `BUG-089.md`, which
  holds the specification): today it would take the player’s resources and give nothing back
- Cost is paid from **two disjoint lists** since `73ab8e7`: `AbilityDefinition.Costs` once at
  activation, and per-effect `AbilityEffectDefinition.Costs` inside `TryCast()`. The two are billed
  differently on purpose — a `Hold` ability re-enters `CastInstant()` while held, so a per-effect
  cost is a **channelled** cost that keeps draining. Put a one-off cost in `AbilityDefinition.Costs`
- ✅ **Ability-scope costs ARE affordability-checked, as of `2a83469`** (BUG-076 b′ fixed). The gate
  is `AbilityDefinition.TryStart()` (`:50-64`), reached from `AbilityInstance.CanStart():94` and
  through `AbilityHolder.TryDoAbility():107` — **before** any state change or payment. It reads each
  cost’s own `statType` through `IVitalComponent`, so it is generic: a non-Mana cost such as Avatar
  of Light’s 50 HP is now validated. `TryPayCost()` and `ValidateConditions()` are deleted; payment
  is `PayCost(Definition.Costs)` inside `ActivateInstant():44`, reachable only after the gate passed.
  **Author ability-scope costs in `AbilityDefinition.Costs` and let `TryStart()` gate them — do not
  write a per-stat condition class for it.** That is what `HasEnoughManaCondition` was, and it was
  deleted
- ⚠️ `TryStart()` does **not** guard null elements in `Conditions` (BUG-091), unlike the `Effects`
  walks either side of it. A missing-script entry in a `Conditions` list throws there. Check the
  list in the Inspector before binding an ability whose assets you did not author

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
