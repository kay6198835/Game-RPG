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
| `System/Abilities/Core/` | `AbilityDefinition` (+ `AbilityActivationType`, `StatCost`, `AbilityState` enum Start/Cast/Do/Exit), `AbilityInstance`, `AbilityContext` (+ `IAbilityServices`, `EffectRecipient`), `AbilitySlot`, `AbilityEffectDefinition` (+ `StatImpactType`) → `DeliveryEffectBase` (holds `SubEffects`), `PayloadEffectBase` (2026-11-26), `AbilityConditionDefinition`, `IAbilityOwner` |
| `System/Abilities/Effects/` | **Delivery:** `SpawnEffectBase` → `SpawnProjectileEffect`, `SpawnSummonEffect`; `SelfApplyEffect` (2026-11-26). **Payload:** `DamageEffect` (2026-11-25); `StatsEffectBase` (file `StatEffectBase.cs`) → `RecoveryReductionStatsEffect`, `RecoveryReductionPerTimeForDuration`, `BuffDebuffStatsForDuration` |
| `System/Abilities/Conditions/` | empty |
| `System/Abilities/Runtime/` | `BaseController/SpawnMono`, `SpawnMono/{SpawnProjectileBase, ProjectileBody, SpawnSummonBase, SlashProjectile, DamageArea, CircleSummon, DamagesInManyPos, RainArrowsProjectile, Interface/ISpawn}` (`DamageArea` / `CircleSummon` were `LightningController` / `RuneCircleController` until `cae75317`), `ImpactMono/SourceEffect` |
| ~~`System/Skill_Ability/`~~ | **Deleted 2026-11-04.** v1 (`ActivateSkill`, `AbstractSkillSO`, Dash/Slash/Block/Dual, effect SOs, `InternalSkillSO`) removed with its six SO assets, `EntityWeapon`, `Spell` + `Spell.prefab`, `IEffectable`. `WeaponSO` moved to `System/Item/` |
| Owners | `Character/Base/AbilityHolderBase.cs` → `AbilityHolder` (player), `EntityAbilityHolder` (enemy) |
| Assets | `Assets/SO/Skill/Paladin/Ability/{Consecrate, Blessed Slash, Blessing, Avatar of Light}/` (live); `Assets/SO/Skill/{Archer,Mage}/` (temporary class abilities, `6c61ccdb`, GDD Phase 1 `ae0d6e82`) |

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
detects a hit sets `ctx.Target`, then invokes the callback; the spawn effect runs its `SubEffects`
against that target. **Spawn effects and spawned objects never compute damage** (2026-11-25, GDD AB-S11).

**Damage — two kinds of HP loss (2026-11-25, AB-S7 / AB-S11, STATIC — not run in Unity):**

| Wanted | Use | Path | Defense | TakeDamage state / knockback |
|---|---|---|---|---|
| Hit damage | `DamageEffect` (`baseDamage + coefficient × caster scalingStat`, Recipient = Target) | `IEffectReceiver.ApplyDamage` → `EffectReceiverBase` → `INegativeReceiver.TakeDamage` (`NegativeReceiverBase`) | yes | yes |
| HP loss, no reaction (DOT, drain) | `RecoveryReductionStatsEffect` / `RecoveryReductionPerTimeForDuration` | `IEffectReceiver.AdjustCurrentStats` → `vital.Reduction` | no | no (death via `CheckDeath`) |
| Weapon hit | — (combat, not an ability) | `INegativeReceiver.TakeDamage` directly | yes | yes |

`INegativeReceiver` on the hit collider is the hurtbox marker the spawned objects filter on.
**The hurtbox GameObject also carries the `EffectReceiver`** (2026-11-25: moved from its own `Core/EffectReceiver`
object onto `PlayerTest/Core/Takedamage` and `EnemyPrefab/Core/NegativeReciver`). So
`AbilityContext.TryGetRecipientComponent` resolves a **Target** with `TryGetComponent` on the hit collider, like
`INegativeReceiver`; only the Caster, or a capability on another core component, goes character root → `Core`.
`TrainingDummy` implements both interfaces on its collider object, so it takes ability damage too.

- **Projectile** (`SpawnProjectileBase : SpawnMono, IProjectilePayload`): flight and detection are
  delegated to the required `ProjectileBody` (`OnTriggerEnter2D`, `targetMask` / `blockMask`, pooled
  despawn). `SpawnProjectileBase.Launch()` passes lifetime `0` to `SpawnMono` (the body owns
  lifetime) and builds a `ProjectileConfig` from its serialized `speed`, `targetMask`, `blockMask`.
  `OnHit()` sets `_context.Target` and invokes the callback. `pierceCount` is serialized but unused.
  Direction: `SpawnProjectileEffect` now computes its own `dir` from `context.Forward`
  (removed from `SpawnEffectBase`).
- **Summon** (`SpawnSummonBase`): payload fires from an Animation Event calling `Execute()`.
  `DamageArea.Execute()` (was `LightningController`) does `OverlapCircleNonAlloc` and sets target + invokes once per hit.
  `DamagesInManyPos` (Rain Arrows) does the same per wave, each collider at most once per wave, and returns to
  the pool on `OnSkillEnd` (2026-11-25; was a fixed `damagePerArrow` 10 and `Destroy`).
  Consecrate = RuneCircle telegraph (Cast) + Lightning strike (Do), by design.

**Effect list order** is authored data: it sets execution order and resource priority.

**Delivery and Payload (2026-11-26, GDD AB-E15…E19, STATIC — not compiled, not run in Unity).**

```
AbilityDefinition.Effects        → Delivery only: who is hit   (SpawnProjectileEffect, SpawnSummonEffect, SelfApplyEffect)
  Delivery.SubEffects            → Payload only: what happens  (DamageEffect, BuffDebuff…, RecoveryReduction…)
    Payload                      → no SubEffects; Recipient = Target (the hit) or Caster (e.g. life steal)
```

- `SubEffects` lives on `DeliveryEffectBase` (moved from `AbilityEffectDefinition`, same field name, so the 8 spawn
  effect assets keep their lists). `ApplyOnHitEffects` moved with it.
- A Delivery never holds another Delivery; a chain (projectile → explosion) is written as its own Delivery.
- `SelfApplyEffect` sets `ctx.Target` to the caster's hurtbox collider (the GameObject with `INegativeReceiver`),
  runs the SubEffects, then restores `ctx.Target`. Blessing and Avatar of Light now use it; their four payloads
  moved to `Recipient = Target`.
- The lists are still typed `AbilityEffectDefinition`, so the Inspector does not stop a Payload in `Effects` or a
  Delivery in `SubEffects` — whether to type the lists is still open (session item B2).
- A dead character (`ICharacterInput.IsDeath`) takes no new stat / vital effect (`EffectReceiverBase.IsDead`).

## Status & stat effects — decisions 2026-11-25 `[PLANNED]`

> Owner decisions, 2026-11-25 (session). **Not in code yet.** GDD rules AB-E8…AB-E14
> (`design/gdd/abilities-v2-system.md` §7). Where the code below disagrees, the code is the old behaviour.

**Definitions (stat vs vital).**

| Term | Meaning | Owner | Changed by |
|---|---|---|---|
| **Stat** | `StatSheet` `FinalValue` = Adjusted + ModifierBonus + PrimaryBonus, for all 17 `StatType` | `IStatService` | Modifiers with a source (equipment, buff, debuff) — removable |
| **Max** | Only for a stat that backs a resource: max HP = stat `HP`, max Mana = stat `Mana`. The stat's `FinalValue` is the **cap** of the vital | `IStatService` | as Stat |
| **Current** (vital) | Resource amount, clamped to `[0, Max]` | `IVitalComponent` | Recovery / Reduction — not removable |

- A resource has a max **only if** its `VitalStatType` member has the same value as a `StatType` member.
  Adding a capped resource = add the `StatType` and a matching `VitalStatType`.
- **Coin** has no matching `StatType`: it grows without limit and is matched to no stat.
- `IEffectReceiver` methods named `…Max…` act on **stats** (modifiers), not only on HP/Mana max.

**Status.**

1. **Status config is PENDING.** `Status Status Config.asset` (`StatusStatsEffect`) and `StatusType` stay as
   reference data only. The Inspector auto-fill (`StatsEffectBase.SyncModifiersFromStatusType` /
   `AutoAssignStatusStatsEffect`) is to be switched off. There is **no** system that matches stats to a status.
2. **Game design picks the stats by hand** for each effect, from
   `design/balance/status-debuff-stat-combos.md` and the `StatusType` comments, so that they fit the
   ability's `StatusType`.
3. **One `StatusType` per `AbilityDefinition`.** An ability that needs two statuses is split into two abilities.
4. **Passed at runtime** through `AbilityContext` to every effect, on-hit `SubEffects` included. Effect assets
   do not store a `StatusType`, so one effect asset can serve two abilities without conflict.
5. **`StatImpactType` comes from the status:** buff (even value) → Recovery, debuff (odd value) → Reduction.
   An ability with `StatusType = None` that contains a stat effect is a **validation error**.
6. **Item effects** (`ModifierCurrentStatsEffect`, `ModifierMaxStatsEffect`) get their own `StatusType` field,
   same rule as 5.
7. The ability's `StatusType` is also the data hook for later work (cleanse, status icons, stacking, auto-match).

**Values and sign (GDD AB-E12, decided 2026-11-26, A1).**

- `StatModifier.value` is a **magnitude** in 0.1 … 999 (`StatModifier.MinValue` / `MaxValue`, `[Range]`; the constructor
  drops the sign and clamps — a 0 row becomes 0.1).
- `StatModifier.impactType` is serialized on every row — skills, items and equipment (`Weapon.cs:92`) alike.
- **Every reader of `Value` applies the sign from `ImpactType`** (Reduction → `−Value`, Recovery → `+Value`):
  `StatSheet.ApplyModifiers`, `EffectReceiverBase.AdjustCurrentStats`, QA items suite.
- Current HP / Mana (`AdjustCurrentStats`, `8377ebaf`): per vital row, `Utility.ModifierStatsCalculate(row, current)` gives the
  amount (Flat = `Value`, PercentAdd / PercentMult = `current × Value`), `ImpactType` the direction. A percent row reads the
  current value at that moment: 0 Mana → a percent ManaSurge gives 0; two 50 % HP Reductions remove 75 %.
- ⚠️ **Game design must know** (not blocked in code, no floor / cap — SD-05):
  - `PercentMult` Reduction `v` → `×(1 − v)`: `v = 1` zeroes the stat, `v > 1` makes it negative, two such rows multiply back to positive.
  - `PercentAdd` enters as `(1 + ΣRecovery − ΣReduction)`: Reductions summing past 1 + the Recoveries drive the stat below 0.
  - Max HP ≤ 0 clamps current HP to ≤ 0 and kills (`VitalStatsBase.OnMaxStatChanged` → `CheckDeath`).
  - Keep a percent Reduction in 0.1 … 1 and mind stacking.
- `SyncModifiersFromStatusType` re-stamps every row's `impactType` when `statusType` changes (hand-set values included).
  That is all the support there is: a hand-set row contradicting `statusType` afterwards is a design error (owner, 2026-11-26).
- A wrong effect class / `statusType` pair is a design error too: Avatar of Light `Recovery Stat Effect` and Blessing
  `Paladin Recovery Reduction Stats Effect` are `RecoveryReductionStatsEffect` (current vitals) with ArcaneSurge (a Duration
  status) → MagicDamage / ManaRegen rows, all skipped → no effect. Owner 2026-11-26: created and wired wrong, fix in the assets.
- *Was (2026-11-25, temporary):* values always positive, one conversion point in the receiver, `StatSheet` read the
  sign of `Value`, equipment kept "sign = direction".

**Tick and mixed statuses.**

- `RecoveryReductionPerTimeForDuration` must call a new `IEffectReceiver.AdjustCurrentPerTimeForDuration`
  (vitals per tick). Today it calls `AdjustMaxPerTimeForDuration`, whose body is empty.
- One effect = one kind of operation (instant vital / vital per tick / stat for a duration). A mixed status is
  **two effects in one ability**, both under the ability's one `StatusType`. Example, Burn:
  `RecoveryReductionPerTimeForDuration` with `[HP]` + `BuffDebuffStatsForDuration` with `[HPRegen]`.
  Poison: tick `[HP]` + duration `[PhysicalDamage]`.

**Migration when `StatusType` moves to the ability** — four effect assets carry one today and lose it:

| Asset | Class | `statusType` now |
|---|---|---|
| `Paladin/Ability/Avatar of Light/Effect/Recovery Stat Effect` | `RecoveryReductionStatsEffect` (instant) | 6 ArcaneSurge — config lists it as a *Duration* status |
| `Paladin/Ability/Avatar of Light/Effect/Gain For Time Effect` | `BuffDebuffStatsForDuration` | 6 ArcaneSurge |
| `Paladin/Ability/Blessing/Effect/Paladin Recovery Reduction Stats Effect` | `RecoveryReductionStatsEffect` (instant) | 6 ArcaneSurge — same mismatch |
| `Paladin/Ability/Blessing/Effect/Paladin Buff - Debuff Stat Effect` | `BuffDebuffStatsForDuration` | 0 None |

Avatar of Light and Blessing each hold an instant and a timed effect. Mixing kinds of operation under one status
is allowed (as Burn does); under rule 3 each ability gets **one** status, and is split only if its two effects
really are two different statuses (e.g. Heal + Haste).

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
| ~~BUG-073 / BUG-090~~ | *2026-11-16:* the ShootSpirit assets, `SpiritProjcetile.prefab` and every reference to the deleted condition asset are deleted; `SpawnEffectBase` warnings no longer name the deleted classes. Awaiting triage (NOTE-20261116-5) |
| BUG-089 | Closed by design (tier scaffolding) — reopen at demo/release |
| — (2026-11-25) | `RecoveryReductionPerTimeForDuration` → `AdjustMaxPerTimeForDuration`, body empty → every tick effect does nothing. Fix: `AdjustCurrentPerTimeForDuration` (see decisions above). Supersedes the "throws" wording of BUG-118 |
| — (2026-11-25) | `StatModifier.impactType` (`StatModifier.cs:34`, `df9e98fe`) has no `[SerializeField]` → loads as Recovery from every asset; `AdjustCurrentStats` turns a Reduction into a heal. *2026-11-26, decided (A1, GDD AB-E12):* `impactType` is `[SerializeField]` again, `value` is a magnitude 0…999 (`[Range]`, constructor drops the sign and clamps), and each reader of `Value` applies the sign from `ImpactType` (`StatSheet.ApplyModifiers`, `AdjustCurrentStats`, `Utility.ModifierStatsCalculate`, QA items suite). `AdjustCurrentStats` honours `Type` since `8377ebaf` (percent of the current value). Still open: floor (SD-05); non-vital rows skipped silently (design error by owner rule) |
| — (2026-11-25) | Status config auto-fill adds stats of the wrong kind for Burn `[HP, HPRegen]` / Poison `[HP, PhysicalDamage]` (tick effect ignores the stat row silently; duration effect turns HP into a max-HP modifier). Closed by "status config PENDING" |
| — (2026-11-25) | `EffectReceiverBase`: timed modifiers removed only at the end of a coroutine on the receiver (disable / death / pool → never removed); source key is a pooled `SourceEffect` (reuse can remove another effect's modifiers); `AdjustMaxStats` uses lifetime 0 → `SourceEffect` never despawned; `SourceEffectConfig.callback` never invoked. *2026-11-26:* cleanup rule decided (GDD AB-E20, `[PLANNED]`: clear in `OnDisable` / `ReloadComponent`, data only, `new object()` token); built together with the status registry, whose key is still open (OQ-4, session item B6). Dead guard added (AB-E19) |
| — (2026-11-25) | `VitalStatsBase.LoadVitalStats` skips Coin → Coin never in `currentStats`, `Recovery(Coin)` is a no-op (no caller today) |
| — (2026-11-25) | ~~Damage still computed in each spawn effect~~ → **fixed in working tree (STATIC)**: `DamageEffect` + `IEffectReceiver.ApplyDamage`; 8 spawn effect assets migrated with unchanged numbers. To check in Unity: assets import, CombatTest dummy, Rain Arrows despawn. Still open: real scaling stat / coefficient per ability (OQ-11); summons of Archer / Paladin scale with MagicDamage as before |
| — | GDD written 2026-11-13 (`design/gdd/abilities-v2-system.md`), reviewed by the owner the same day, 3 questions parked; still no ADR for Abilities v2 *(was: "No GDD for v2; no ADR deciding v1 vs v2 endgame")* |

> **2026-11-13 (GDD pass, `sprint-22` `a8725a77`, STATIC) — rows above not re-verified one by one, but two read differently in the files now:**
> `Lightning.prefab` `layerMask` is `m_Bits: 64` (layer `Enemy`), so the BUG-072 row looks done in data; `DamagesInManyPos` sets its
> direction in a `Launch()` override, so BUG-109 item 1 looks fixed in code. Both are bug-inbox NOTE-20261113-9 for the Saturday triage;
> statuses stay as the bug files say.

## Related

`design/gdd/abilities-v2-system.md` (current design), `.claude/rules/weapon-skill-code.md`, `design/gdd/skill-ability-system.md` (v1, superseded),
`docs/skill-reference.md`, [weapons](../weapons/README.md) (shared `ProjectileBody`).
