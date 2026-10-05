# Abilities — v2 (live) + v1 (legacy)

> **Status:** v2 live for player and enemy; v1 legacy, enemy weapon only · **Last verified:** 2026-10-05, HEAD `93ba6d8e`
> History: [CHANGELOG.md](CHANGELOG.md) · Diagrams: `docs/diagrams/ability-system-diagrams.md`

## Purpose

Composable, data-driven abilities: an ability is an `AbilityDefinition` SO built from a list of
effects and conditions, run per owner by an `AbilityInstance`.

## Code

| Path | Contents |
|------|----------|
| `System/Abilities/Core/` | `AbilityDefinition` (+ `AbilityActivationType`, `StatCost`, `AbilityState` enum Start/Cast/Do/Exit), `AbilityInstance`, `AbilityContext` (+ `IAbilityServices`, `EffectRecipient`), `AbilitySlot`, `AbilityEffectDefinition` (+ `StatImpactType`), `AbilityConditionDefinition`, `IAbilityOwner` |
| `System/Abilities/Effects/` | `SpawnEffectBase` → `SpawnProjectileEffect`, `SpawnSummonEffect`; `StatsEffectBase` (file `StatEffectBase.cs`) → `RecoveryReductionStatsEffect`, `RecoveryReductionPerTimeForDuration`; `BuffDebuffStatsForDuration` |
| `System/Abilities/Conditions/` | empty |
| `System/Abilities/Runtime/` | `BaseController/SpawnMono`, `SpawnMono/{SpawnProjectileBase, ProjectileBody, SpawnSummonBase, SlashProjectile, LightningController, RuneCircleController, Interface/ISpawn}` |
| `System/Skill_Ability/` | v1: `ActivateSkill`, `AbstractSkillSO`, Dash/Slash/Block/Dual abilities, effect SOs, `InternalSkillSO`, `WeaponSO` |
| Owners | `Character/Base/AbilityHolderBase.cs` → `AbilityHolder` (player), `EntityAbilityHolder` (enemy) |
| Assets | `Assets/SO/Skill/Paladin/Ability/{Consecrate, Blessed Slash, Blessing, Avatar of Light}/` (live) |

## How it works (v2)

```
input (keys 1-4) → AbilityHolder.TryDoAbility(slot)
  → AbilityInstance.CanStart() → AbilityDefinition.TryStart()   (conditions + ability-scope costs, generic over StatType)
  → ActivateInstant (pay Costs) → CastInstant (each effect TryCast(); per-effect cost = upgrade tier)
  → DoInstant → Execute() → effect.Apply(context) for every effect, in list order
  → StartCooldown; Tick() via Processing()
```

`AbilityContext`: Caster, Origin, Forward, TargetPoint, HoldTime, HoldRatio, `Target`
(`Collider2D`), `Services` (`IAbilityServices` = **`Pool` only**). Character data comes from
`IAbilityOwner.GetCurrentStatValue()` / `PayCost()`, so one definition runs for player or enemy.

**Spawn effects.** `SpawnEffectBase.Apply()` → `Services.Pool.Spawn(prefab)` →
`SpawnMono.Launch(lifetime, ctx, callback)`. The hit contract is **set-then-invoke**: whoever
detects a hit sets `ctx.Target`, then invokes the callback; the effect resolves `INegativeReceiver`
from the target and skips if none.

- **Projectile** (`SpawnProjectileBase : SpawnMono, IProjectilePayload`): flight and detection are
  delegated to the required `ProjectileBody` (`OnTriggerEnter2D`, `targetMask` / `blockMask`, pooled
  despawn). `SpawnProjectileBase.Launch()` passes lifetime `0` to `SpawnMono` (the body owns
  lifetime) and builds a `ProjectileConfig` from its serialized `speed`, `targetMask`, `blockMask`.
  `OnHit()` sets `_context.Target` and invokes the callback. `pierceCount` is serialized but unused.
  Direction: `SpawnProjectileEffect` now computes its own `dir` from `context.Forward`
  (removed from `SpawnEffectBase`).
- **Summon** (`SpawnSummonBase`): payload fires from an Animation Event calling `Execute()`.
  `LightningController.Execute()` does `OverlapCircleNonAlloc` and sets target + invokes once per hit.
  Consecrate = RuneCircle telegraph (Cast) + Lightning strike (Do), by design.

**Effect list order** is authored data: it sets execution order and resource priority.

## v1 (legacy)

`ActivateSkill` SO, lifecycle `Enter → Activate → Cast → Do → Exit`. Since 2026-10-05 referenced
only by `EntityWeapon.currentAbilitySO`; the player weapon fields that pointed at it were deleted.
Do not add new `ActivateSkill` subclasses.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-092 | `RecoveryReductionPerTimeForDuration.perTime` / `timeCount` not serialized → one instant tick |
| BUG-072 | `Lightning.prefab` `layerMask` unset → summon hits nothing |
| BUG-071 | HoT/DoT tick chain cannot be stopped |
| BUG-083 | `HoldTime` / `HoldRatio` always `0f` |
| BUG-068 | `AbilityHolderBase.CurrentActivationType` (`:28`) unguarded |
| BUG-079 | `AbilityInstance.Exit()` body commented out |
| BUG-073 / BUG-090 | ShootSpirit assets and `Has Enough Mana Condition.asset` reference deleted scripts |
| BUG-089 | Closed by design (tier scaffolding) — reopen at demo/release |
| — | No GDD for v2; no ADR deciding v1 vs v2 endgame |

## Related

`.claude/rules/weapon-skill-code.md`, `design/gdd/skill-ability-system.md` (v1 only),
`docs/skill-reference.md`, [weapons](../weapons/README.md) (shared `ProjectileBody`).
