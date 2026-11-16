# Abilities — v2 (v1 deleted 2026-11-04)

> **Status:** v2 live for player and enemy; v1 deleted 2026-11-04 (`6094aa10`, on `sprint-21`) · **Last verified:** 2026-11-05, `sprint-21` `028fdb90` (status, code table, assets row only)
> History: [CHANGELOG.md](CHANGELOG.md) · Diagrams: `docs/diagrams/ability-system-diagrams.md`
> **Design:** `design/gdd/abilities-v2-system.md` (2026-11-13, reverse-documented at `sprint-22` `a8725a77`, awaiting owner review) —
> rules tagged CONFIRMED / AS-BUILT / OPEN, the per-class ability catalogue and acceptance criteria. Reviewed by the owner
> 2026-11-13: 3 questions parked (OQ-4, OQ-9, OQ-12). **Eight decisions are not in code yet** — GDD AB-G7 (activate only in
> Idle / Move), AB-G8 (aim at release), AB-L7 (a hit interrupts a cast), AB-H5 (hold value at release, BUG-083), AB-C3
> (`HP > cost`), AB-CD4 (cooldown counts in every state), AB-S7 (scaling stat + coefficient), AB-N7 (enemy ability hits the player).
> This README stays the short code map; where the two disagree on behaviour, re-read the code and fix both.

## Purpose

Composable, data-driven abilities: an ability is an `AbilityDefinition` SO built from a list of
effects and conditions, run per owner by an `AbilityInstance`.

## Code

| Path | Contents |
|------|----------|
| `System/Abilities/Core/` | `AbilityDefinition` (+ `AbilityActivationType`, `StatCost`, `AbilityState` enum Start/Cast/Do/Exit), `AbilityInstance`, `AbilityContext` (+ `IAbilityServices`, `EffectRecipient`), `AbilitySlot`, `AbilityEffectDefinition` (+ `StatImpactType`), `AbilityConditionDefinition`, `IAbilityOwner` |
| `System/Abilities/Effects/` | `SpawnEffectBase` → `SpawnProjectileEffect`, `SpawnSummonEffect`; `StatsEffectBase` (file `StatEffectBase.cs`) → `RecoveryReductionStatsEffect`, `RecoveryReductionPerTimeForDuration`; `BuffDebuffStatsForDuration` |
| `System/Abilities/Conditions/` | empty |
| `System/Abilities/Runtime/` | `BaseController/SpawnMono`, `SpawnMono/{SpawnProjectileBase, ProjectileBody, SpawnSummonBase, SlashProjectile, DamageArea, CircleSummon, DamagesInManyPos, RainArrowsProjectile, Interface/ISpawn}` (`DamageArea` / `CircleSummon` were `LightningController` / `RuneCircleController` until `cae75317`), `ImpactMono/SourceEffect` |
| ~~`System/Skill_Ability/`~~ | **Deleted 2026-11-04.** v1 (`ActivateSkill`, `AbstractSkillSO`, Dash/Slash/Block/Dual, effect SOs, `InternalSkillSO`) removed with its six SO assets, `EntityWeapon`, `Spell` + `Spell.prefab`, `IEffectable`. `WeaponSO` moved to `System/Item/` |
| Owners | `Character/Base/AbilityHolderBase.cs` → `AbilityHolder` (player), `EntityAbilityHolder` (enemy) |
| Assets | `Assets/SO/Skill/Paladin/Ability/{Consecrate, Blessed Slash, Blessing, Avatar of Light}/` (live); `Assets/SO/Skill/{Archer,Mage}/` (temporary class abilities, `6c61ccdb`, GDD Phase 1 `ae0d6e82`); `Assets/SO/Skill/ShootSpirit/` (legacy, BUG-073 / BUG-090) |

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
  `DamageArea.Execute()` (was `LightningController`) does `OverlapCircleNonAlloc` and sets target + invokes once per hit.
  Consecrate = RuneCircle telegraph (Cast) + Lightning strike (Do), by design.

**Effect list order** is authored data: it sets execution order and resource priority.

## v1 (legacy)

**Deleted 2026-11-04 (owner request).** Nothing referenced it any more except the abstract, unused
`EntityWeapon` and `EntityEffectStats` (which wrote the shared stat asset). All v2 now; there is no v1 path.

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
| — | GDD written 2026-11-13 (`design/gdd/abilities-v2-system.md`), reviewed by the owner the same day, 3 questions parked; still no ADR for Abilities v2 *(was: "No GDD for v2; no ADR deciding v1 vs v2 endgame")* |

> **2026-11-13 (GDD pass, `sprint-22` `a8725a77`, STATIC) — rows above not re-verified one by one, but two read differently in the files now:**
> `Lightning.prefab` `layerMask` is `m_Bits: 64` (layer `Enemy`), so the BUG-072 row looks done in data; `DamagesInManyPos` sets its
> direction in a `Launch()` override, so BUG-109 item 1 looks fixed in code. Both are bug-inbox NOTE-20261113-9 for the Saturday triage;
> statuses stay as the bug files say.

## Related

`design/gdd/abilities-v2-system.md` (current design), `.claude/rules/weapon-skill-code.md`, `design/gdd/skill-ability-system.md` (v1, superseded),
`docs/skill-reference.md`, [weapons](../weapons/README.md) (shared `ProjectileBody`).
