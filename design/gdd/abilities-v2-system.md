---
status: reverse-documented
source: Assets/Script/System/Abilities/, Assets/Script/Character/Base/AbilityHolderBase.cs, Assets/SO/Skill/
date: 2026-11-13
verified-by: pending owner review
---

# Abilities v2 System Design

> 📜 Change log: [changelog/abilities-v2-system.CHANGELOG.md](changelog/abilities-v2-system.CHANGELOG.md)

> **Status**: Reverse-documented from code — **reviewed by the owner 2026-11-13** (16 questions decided; 3 parked: OQ-4 pending, OQ-9 and OQ-12 deferred; 8 decisions not in code yet — see Rule index) · `[IMPLEMENTED]` unless a rule says otherwise
> **Last Verified**: 2026-11-13, `origin/sprint-22` `a8725a77` · Evidence tier: **STATIC** (code and asset YAML read; nothing was run in Play Mode)
> **Supersedes**: [`skill-ability-system.md`](skill-ability-system.md) (Abilities v1, deleted 2026-11-04 in `6094aa10`)
> **Related**: [`class-abilities-archer-mage.md`](class-abilities-archer-mage.md) (intended Archer / Mage kits, design-first), `docs/systems/abilities/README.md`, ADR-0005, `.claude/rules/weapon-skill-code.md`

## How to read this document

This GDD is the source QA writes test cases from. Every rule carries one tag:

| Tag | Meaning | What QA does with it |
|---|---|---|
| **CONFIRMED** | The owner has decided it. The bug file or ADR that records the decision is cited | Test against it. A deviation is a defect |
| **AS-BUILT** | The code does this today. Nobody has confirmed it is the intended design | Test against it as a regression baseline. A deviation is a change, not automatically a defect |
| **OPEN** | The code is unclear, or code and assets disagree. The owner must decide | Do not write a pass / fail case. See [Open Questions](#open-questions) |

A CONFIRMED rule marked `[PLANNED]` is a decision the code does not follow yet: the rule text states
the design first and the current behaviour second. QA tests the design and reports the gap once
(Acceptance Criteria, section F).

Line numbers refer to `origin/sprint-22` `a8725a77`. Identifiers are spelled exactly as in code,
including intentional typos (`Obstracles`, `Paladin Spawn Consecrat Effect`, `DespawnOneselfAffterDuration`).

---

## Overview

Abilities v2 is the only ability framework in the game (**AB-D1**). An ability is a data asset
(`AbilityDefinition`) that lists an activation type, a cooldown, resource costs, conditions and an
ordered list of effect assets. A character owns up to four abilities, one per slot (Primary,
Secondary, Utility, Ultimate — keys `1` / `2` / `3` / `4` for the player). Each owned ability runs
as an `AbilityInstance`, which walks four phases per cast — **Start → Cast → Do → Exit** — pays
costs, lets every effect prepare (`TryCast`), applies every effect (`Apply`) and starts the
cooldown. Effects either change stats on a character or spawn a pooled world object (a projectile
or a summon) that reports its hits back to the effect. The same assets and the same runtime serve
the player and enemies; only the driver that steps the phases differs (animation events for the
player, frame updates for an enemy).

## Player Fantasy

*"My four keys are my class."* The basic attack is the rhythm of a fight; abilities are the
punctuation — a slash that travels, a rune that warns and then strikes, a short window where I hit
much harder. An ability costs something (Mana, sometimes HP), commits me to an animation in which I
cannot move, and then is gone for a cooldown, so pressing it at the wrong moment is a real mistake
and pressing it at the right one is the highlight of the room. Held abilities are meant to reward
patience: keep the key down to channel or charge, let go to release.

`[PLANNED]` The charge half of that fantasy is not delivered today (**AB-H5**), and three of the
four Paladin cooldowns are placeholder values (**OQ-11**).

---

## Detailed Rules

### 1. Data model

| ID | Tag | Rule |
|---|---|---|
| **AB-D1** | CONFIRMED | Abilities v2 (`Assets/Script/System/Abilities/`) is the only ability path for player and enemy. Abilities v1 (`ActivateSkill`) was deleted at owner request on 2026-11-04 (`6094aa10`; `docs/systems/abilities/README.md` › "v1 (legacy)") |
| **AB-D2** | AS-BUILT | An `AbilityDefinition` holds: `Id` (GUID, generated once), `DisplayName`, `Icon`, `ActivationType`, `Cooldown` (seconds, default 1), `Costs` (list of `StatCost { value, statType }`), `MaxHoldTime` (seconds, default 0), `Conditions`, `Effects` (ordered), `AnimatorOverride` (`AbilityDefinition.cs:14-36`) |
| **AB-D3** | CONFIRMED | Slot → ability bindings are authored on the character's data asset (`CharacterData.AbilityBindings`, i.e. `PlayerData` / `EntityData`), never on the weapon and never on the holder component. The serialized list on the holder is overwritten at `SetAbility()` (ADR-0005 `:242`, `:266-272`; `AbilityHolderBase.cs:70`, `:85-97`) |
| **AB-D4** | AS-BUILT | A character has at most one ability per `AbilitySlot` (`Primary`, `Secondary`, `Utility`, `Ultimate`). A binding with no ability is skipped. Two bindings for the same slot: the later one in the list wins (`AbilityHolderBase.cs:90-96`, `:142`) |
| **AB-D5** | CONFIRMED | `SetAbility()` discards every instance and builds new ones. It runs from `CharacterBase.ApplyCharacterData()` (`CharacterBase.cs:23`), so a class pick at the Champion pedestal and every enemy spawn start with all cooldowns at 0. Accepted as intended (owner decision 2026-11-13, OQ-16) |
| **AB-D6** | OPEN | *Deferred by the owner 2026-11-13: `Passive` is meant to be the innate-effect type, its design is not decided in this phase (OQ-12).* `AbilityActivationType` declares `Passive`, but no code gives it a behaviour. The player input treats every type other than `Active` as a held ability (`PlayerInputHandle.cs:284`); `AbilityInstance.CastInstant()` treats it as non-`Hold` (`AbilityInstance.cs:52`). No asset uses it. → **OQ-12** |
| **AB-D7** | AS-BUILT | `Conditions` (ability level) and `SubConditions` (effect level) are evaluated, but no concrete `AbilityConditionDefinition` class exists (`System/Abilities/Conditions/` is gone) and every live asset has both lists empty. Today conditions always pass |

### 2. Activation gate (the key press)

| ID | Tag | Rule |
|---|---|---|
| **AB-G1** | AS-BUILT | A press on a slot key asks `TryDoAbility(slot)`. It succeeds only when **all** hold: the slot has an ability, its `CooldownRemaining` is 0, and `AbilityDefinition.TryStart()` passes (`AbilityHolderBase.cs:161-172`, `AbilityInstance.cs:91-98`) |
| **AB-G2** | CONFIRMED | `TryStart()` is the single ability-level gate: every condition must be met **and** every entry of `Costs` must be affordable. It is generic over the cost stat (HP and Mana alike) (BUG-076, fixed by `2a83469`; `AbilityDefinition.cs:50-67`) |
| **AB-G3** | AS-BUILT | "Affordable" means `cost.value <= current value`. A cost exactly equal to the current value passes (`AbilityDefinition.cs:64` refuses only on `cost.value > current`) |
| **AB-G4** | AS-BUILT | A refused press does nothing the player can observe: no resource is spent, no cooldown starts, no ability phase begins, the animator controller is not changed, and no feedback is shown or played |
| **AB-G5** | AS-BUILT | A press is ignored while the player is already in the ability state (`PlayerInputHandle.cs:279`). There is no input buffer for abilities |
| **AB-G6** | AS-BUILT | The gate is evaluated at the moment of the key press, but the cast begins only when `PlayerBasicState` (Idle or Move) next reads the `IsSkill` flag. That read has the lowest input priority: equip / unequip, interact, pick up and attack are checked first (`PlayerBasicState.cs:38-72`). For an `Active` ability the flag lives for about one frame (`PlayerInputHandle.cs:284-288`), so an accepted press made while the player is in any other state (attacking, taking damage, …) is dropped without a cast, a cost or a cooldown. For a held ability the flag lasts while the key is down, so the cast begins when the player returns to Idle / Move |
| **AB-G7** | CONFIRMED | `[PLANNED]` **An ability can be activated only while the character is in the Idle or Move state.** A press in any other state does nothing at all: no gate result is kept, nothing is buffered, the animator controller is not touched. This includes a `Hold` ability whose key is pressed during another action and kept down: it does not start when the character returns to Idle / Move — the key must be pressed again (owner decision 2026-11-13, OQ-14, confirmed in follow-up). **Not implemented yet:** today an accepted press swaps the animator controller to the ability's `AnimatorOverride` immediately, inside the input callback (`AbilityHolderBase.cs:169`), even when the cast is then dropped by AB-G6 — for example mid-attack — and a held ability pressed during another action starts when the player returns to Idle / Move (bug-inbox NOTE-20261113-7) |
| **AB-G8** | CONFIRMED | `[PLANNED]` **Aim is taken when the key is released**, not when it is pressed: `Forward`, `DirectionIndex` and `TargetPoint` used by the effects are the values at release (owner decision 2026-11-13, OQ-15). For an `Active` ability press and release are the same moment. **Not implemented yet:** today the cast's `AbilityContext` is built once, at the key press — `Origin` (caster position), `Forward` (aim direction), `DirectionIndex` (8-direction index), `TargetPoint` (player: mouse world position; enemy: the target's position) (`AbilityInstance.cs:111-132`) — and does not change for the rest of the cast, including a long hold (bug-inbox NOTE-20261113-13) |

### 3. Cast life cycle

```
            press accepted (AB-G1)
                    │
                    ▼
   ┌────────┐ dispatch  ┌────────┐ dispatch, not held  ┌────────┐ dispatch  ┌────────┐
   │ Start  │──────────▶│  Cast  │────────────────────▶│   Do   │──────────▶│  Exit  │
   └────────┘ pay       └────────┘ TryCast each effect └────────┘ Apply all └────────┘
              ability      ▲   │   pay effect costs               effects,
              Costs        └───┘                                  start cooldown
                       dispatch while held
                       (Hold only, AB-H2)
```

A **dispatch** is one call of `AbilityHolderBase.HandleInput()`. Who makes the call differs per
character type (AB-L6 for the player, AB-N4 for an enemy).

| ID | Tag | Rule |
|---|---|---|
| **AB-L1** | AS-BUILT | A cast has four phases, `AbilityState.Start`, `Cast`, `Do`, `Exit`, always visited in that order. There is no `None` phase; an instance that has never been cast reports `Start` (`AbilityDefinition.cs:94-100`, `AbilityInstance.cs:14`) |
| **AB-L2** | AS-BUILT | **Start → Cast.** On the first dispatch the ability-level `Costs` are paid (AB-C1) and the phase becomes `Cast`. Nothing else happens (`AbilityInstance.cs:44-48`) |
| **AB-L3** | AS-BUILT | **Cast → Do.** On a dispatch in `Cast`, every effect's `TryCast()` runs in list order and its effect-level costs are paid (AB-C4). Then: an `Active` ability always moves to `Do`; a `Hold` ability moves to `Do` only if it is no longer being held, otherwise it stays in `Cast` and the next dispatch repeats this step (`AbilityInstance.cs:49-57`, `:70-84`) |
| **AB-L4** | AS-BUILT | **Do → Exit.** On a dispatch in `Do`, every non-null effect's `Apply()` runs in list order, **then** the cooldown starts, then the phase becomes `Exit` (`AbilityInstance.cs:58-63`, `:142-159`) |
| **AB-L5** | AS-BUILT | **Exit** does nothing (`AbilityInstance.Exit()` is empty). The instance returns to `Start` only when the next accepted press calls `StartHold()` (`AbilityHolderBase.cs:228-233`). BUG-079 tracks that the reset lives outside the instance; abilities do re-cast |
| **AB-L6** | AS-BUILT | **Player driver.** While in `PlayerSkillWeaponState`, each `AnimationOnAction` animation event makes exactly one dispatch. The animator bool `DoAB` is set to `false` in `Start` and to "not holding" in `Cast`, and is what lets the animator leave the Cast clip. The player leaves the state when an `AnimationEnd` event arrives while the phase is `Exit` (`PlayerSkillWeaponState.cs:28-53`). The number and timing of dispatches therefore come from the clips of the ability's `AnimatorOverride`, not from code. *Not verified:* the event layout of each clip |
| **AB-L7** | CONFIRMED | `[PLANNED]` During the ability state the player's velocity is set to zero on entry and the player cannot move, attack or start another ability. **Taking a hit interrupts the cast — for the player and for an enemy alike.** No effect is applied. What the interrupt costs depends on whether the ability cost was already paid: **not paid yet** (the hit lands before the `Start` dispatch) → nothing is spent **and no cooldown starts**; **already paid** → the cost stays spent (no refund, same as AB-L8) **and the ability goes on cooldown** for its full `Cooldown`, exactly as if it had been cast (owner decisions 2026-11-13, OQ-18 and follow-ups). **Not implemented yet:** today taking damage does not interrupt a cast on either side (HP still drops); only death does, and a cast that does not reach `Do` starts no cooldown (`PlayerUseWeaponState.cs:19`, `:32-36`; `EntityAbilityState.cs:34-41`; bug-inbox NOTE-20261113-13) |
| **AB-L8** | CONFIRMED | If the cast is cut by death after the `Start` dispatch but before the `Do` dispatch, the ability costs are already paid, no effect is applied and no cooldown starts. There is no refund, and this is intended (owner decision 2026-11-13, OQ-3). Revisit only if something other than death can ever interrupt a cast |
| **AB-L9** | CONFIRMED | **The player's animator controller is not restored after a cast.** The ability's `AnimatorOverride` stays active until something else replaces it (`PlayerSkillWeaponState.Exit()` only calls `base.Exit()`) (owner decision 2026-11-13, OQ-13). The enemy side differs and is AS-BUILT: `EntityAbilityState.Exit()` assigns `entityData.animationController` back (`EntityAbilityState.cs:74-80`, AB-N6); the owner asked to review that code |

### 4. Activation types and holding

| ID | Tag | Rule |
|---|---|---|
| **AB-H1** | AS-BUILT | **Active.** The press starts the cast and the hold flag is cleared automatically about one frame later (`PlayerInputHandle.cs:284-288`). The `Cast` phase therefore dispatches exactly once. Releasing or keeping the key down makes no difference |
| **AB-H2** | CONFIRMED | **Hold** is a channelled cast. While the key is down, every dispatch in `Cast` re-runs the effect preparation step (AB-L3) and stays in `Cast`. The animation event is the intended runtime checkpoint for it (BUG-076 owner review (a); BUG-089 gap 4). After release, the **next** dispatch runs the preparation step one more time and moves to `Do`. A hold spanning *N* Cast events while held therefore prepares *N + 1* times and applies once |
| **AB-H3** | AS-BUILT | While holding, the instance accumulates `CurrentHoldTime`. With `MaxHoldTime > 0` it is clamped to `MaxHoldTime`; with `MaxHoldTime = 0` it is not clamped (`AbilityInstance.cs:34-41`). The timer advances only in states that tick abilities (AB-CD3) |
| **AB-H4** | CONFIRMED | A held ability can be held for as long as the player wants: reaching `MaxHoldTime` does **not** release it. Only releasing the key (or a forced release, AB-C6) ends the hold (owner decision 2026-11-13, OQ-5) |
| **AB-H5** | CONFIRMED | `[PLANNED]` **The hold value handed to the effects is measured at release:** `HoldTime = min(time held, MaxHoldTime)` — the time actually held, capped at the maximum when the key was held longer — and `HoldRatio = HoldTime / MaxHoldTime` (owner decision 2026-11-13, OQ-5). **Not implemented yet — this is BUG-083 (open):** both values are always `0`, because the context is built at the press, before the timer starts, and never rebuilt. No effect reads either value yet, so charge scaling (e.g. Hawk Eye in `class-abilities-archer-mage.md`) cannot work until it is fixed |
| **AB-H6** | CONFIRMED | Whether an ability is `Active` or `Hold` is the ability designer's choice, per ability (owner decision 2026-11-13, OQ-6). Paladin Blessed Slash stays `Hold` as authored; with `MaxHoldTime = 0` and no effect reading the hold value, holding it today only keeps the Cast clip looping and the slash is released when the key is let go |

### 5. Cost model

Two independent cost lists exist. **Ability costs** (`AbilityDefinition.Costs`) are the price of the
cast. **Effect costs** (`AbilityEffectDefinition.Costs`) are the price of an upgrade tier of one effect.

| ID | Tag | Rule |
|---|---|---|
| **AB-C1** | AS-BUILT | Ability costs are checked at the key press (AB-G2) and paid once per cast, all entries, on the `Start` dispatch — not at the press (`AbilityInstance.cs:46`) |
| **AB-C2** | AS-BUILT | Paying subtracts the amount from the caster's current HP / Mana and clamps at 0 (`AbilityHolderBase.cs:119-122`, `VitalStatsBase.cs:92-103`). Affordability is not re-checked at payment time, so a resource that dropped between the press and the `Start` dispatch is simply drained to 0 |
| **AB-C3** | CONFIRMED | `[PLANNED]` **An HP cost can never reduce HP to 0.** An ability with an HP cost is castable only when `current HP > cost` (strictly greater); Mana and other costs keep `>=` (owner decision 2026-11-13, OQ-2). **Not implemented yet:** today the gate uses `>=` for every stat (AB-G3), payment is a vital reduction that does not raise the death flag, and a caster with HP exactly equal to the cost is left alive at 0 HP. Live case: Avatar of Light costs 40 HP (bug-inbox NOTE-20261113-6) |
| **AB-C4** | CONFIRMED | An effect-level `Costs` list is the price of an **upgrade tier** of that effect. On each `Cast` dispatch, for each effect in list order: if its `SubConditions` are met and its own costs are affordable, the costs are paid; otherwise nothing is paid for that effect (BUG-089 "Design intent confirmed by owner"; `AbilityInstance.cs:70-84`, `AbilityEffectDefinition.cs:13-34`) |
| **AB-C5** | CONFIRMED | An effect whose tier was refused **still applies** in `Do`, at its default level. `Apply()` is unconditional by design (BUG-089) |
| **AB-C6** | CONFIRMED | On a `Hold` ability a refused tier **force-releases** the ability: the channel ends and the cast proceeds to `Do` on that same dispatch (BUG-089 gap 2, "cưỡng chế tung chiêu"; `AbilityInstance.cs:79-82`, `:52-56`) |
| **AB-C7** | CONFIRMED | `[PLANNED]` No "gained level" exists yet: paying an effect cost changes nothing about what the effect does. The tier data shape is future work. **Do not author an effect-level `Costs` list until the tier fields exist** — it would take resources and give nothing back. No live effect asset has one. BUG-089 is closed as scaffolding and is to be reopened at the demo or release phase |
| **AB-C8** | AS-BUILT | Because of AB-H2, a held ability charges its effect costs on every `Cast` dispatch, including the final one after release (*N + 1* charges). An enemy charges them exactly twice (AB-N5) |
| **AB-C9** | AS-BUILT | A cost can only name a `VitalStatType` (`HP`, `Mana`, `Coin`). `Coin` has no current value in the vitals store, so it reads as 0 and any `Coin` cost above 0 makes the ability permanently uncastable (`VitalStatsBase.cs:46-56`, `:68-75`) |

### 6. Cooldown model

| ID | Tag | Rule |
|---|---|---|
| **AB-CD1** | AS-BUILT | The cooldown starts on the `Do` dispatch, immediately after all effects are applied, with `CooldownRemaining = AbilityDefinition.Cooldown`. No stat modifies it. A cast that never reaches `Do` starts no cooldown (`AbilityInstance.cs:58-63`, `:155-159`). **Decided exception, not implemented yet:** a cast interrupted by a hit after its cost was paid does start the cooldown; interrupted before the cost was paid, it does not (AB-L7); a cast cut by death does not need one (cooldowns reset on respawn, AB-D5) |
| **AB-CD2** | AS-BUILT | Cooldowns are per ability instance (per slot, per character). There is no global cooldown and one slot never blocks another |
| **AB-CD3** | AS-BUILT | Cooldowns and hold timers advance by `Time.deltaTime` only on frames where a state calls `AbilityHolderBase.Processing()`. **Player:** Idle, Move and the ability state (`PlayerBasicState.cs:37`, `PlayerSkillWeaponState.cs:58`). **Enemy:** the states built on `EntityBasicState` (when the enemy has an `EntityAbilityHolder`) and `EntityAbilityState` (`EntityBasicState.cs:45-47`, `EntityAbilityState.cs:42`). They do **not** advance while the player attacks, equips, interacts, picks up, takes damage or is dead |
| **AB-CD4** | CONFIRMED | `[PLANNED]` **Cooldowns and hold timers count in every state**, from one place, so a cooldown authored as *X* seconds always takes *X* seconds of game time (owner decision 2026-11-13, OQ-1). **Not implemented yet:** today they count only in the states listed in AB-CD3, which makes cooldowns longer than authored, never shorter (BUG-069 "Residual"; bug-inbox NOTE-20261113-12) |
| **AB-CD5** | AS-BUILT | When a cooldown starts, the holder raises `AbilityCooldownStarted(slot, duration)`; the HUD hotbar reads it (`AbilityHolderBase.cs:33`, `:141`) |
| **AB-CD6** | AS-BUILT | A press during a cooldown is refused (AB-G4). With `Cooldown = 0` the ability can be cast again as soon as the previous cast has left the ability state |

### 7. Effect contract

Every effect asset derives from `AbilityEffectDefinition` and takes part in a cast twice.

| ID | Tag | Rule |
|---|---|---|
| **AB-E1** | AS-BUILT | `TryCast(context)` runs in the `Cast` phase and answers "can this effect buy its tier?" (`SubConditions` + effect costs). `Apply(context)` runs in the `Do` phase and performs the effect. `Apply()` runs whatever `TryCast()` answered (AB-C5) |
| **AB-E2** | CONFIRMED | `SpawnSummonEffect.TryCast()` has a deliberate side effect: after its gate passes it spawns the cast-phase telegraph object (BUG-078, closed by design; `SpawnSummonEffect.cs:20-25`) |
| **AB-E3** | OPEN | *Pending by owner statement 2026-11-13 (OQ-4): `TryCast()` exists to carry the accumulation ("stacking") logic of held effects and of sub-effects; that design is not settled yet, so no rule is fixed here.* No general rule says whether a `TryCast()` override may have side effects. Known consequences of the one that does: on a held ability the telegraph is spawned on every `Cast` dispatch (AB-H2); an enemy spawns it twice (AB-N5); and once effect costs are used, a refused tier removes the telegraph while the strike still fires (BUG-089 gap 7, kept as a note). → **OQ-4** |
| **AB-E4** | CONFIRMED | The order of `AbilityDefinition.Effects` is authored data. It sets (1) execution order — `Effects[0]` prepares first and applies first — and (2) resource priority — `Effects[0]` has first claim on the caster's resources. Reordering the list is a balance change (BUG-089 gap 6; `.claude/rules/weapon-skill-code.md:128-136`) |
| **AB-E5** | AS-BUILT | Each effect names a `Recipient`: `Caster` (default) or `Target`. A stat effect finds the recipient character (caster, or the character owning the collider that was hit) and acts on its `IEffectReceiver`. If the recipient cannot be found — e.g. `Target` on an ability-level effect, where nothing has been hit — the effect silently does nothing (`StatEffectBase.cs:18-24`, `AbilityContext.cs:33-42`) |
| **AB-E6** | AS-BUILT | `SubEffects` of a spawn effect are **on-hit effects**: for every reported hit on a hurtbox, after damage, each sub-effect's `Apply()` runs in list order. They skip `TryCast()` and are never charged. They do not run when the hit object has no `INegativeReceiver` (`SpawnEffectBase.cs:36-44`) |
| **AB-E7** | AS-BUILT | Effect catalogue — see the table below |

**Effect catalogue (AB-E7).**

| Effect asset type | Phase work | What `Apply()` does | State today |
|---|---|---|---|
| `SpawnProjectileEffect` | `TryCast`: gate only | Spawns the projectile prefab (AB-S2); on hit deals damage (AB-S3) and runs on-hit effects | Works (BUG-075 fixed) |
| `SpawnSummonEffect` | `TryCast`: gate, then spawns the telegraph | Spawns the payload prefab at `TargetPoint` (AB-S4…S6) | Works; BUG-072 open on its last Inspector step (the mask now reads `Enemy`, NOTE-20261113-9) |
| `BuffDebuffStatsForDuration` | gate only | Adds the stat modifiers of its `StatModifierGroup` to the recipient for `duration` seconds, then removes them. Each application has its own source object, so one expiry does not remove another buff | Works in code; see NOTE-20261113-10 (source object never released) |
| `RecoveryReductionStatsEffect` | gate only | Applies the group's **vital** modifiers (current HP / Mana) once | **No-op**: vital modifiers cannot be authored (BUG-117, open). Both live assets of this type have nothing to apply |
| `RecoveryReductionPerTimeForDuration` | gate only | Meant to apply vital modifiers every `perTime` seconds, `timeCount` times | **Throws** `NotImplementedException` (BUG-118, open, dormant — no asset uses it) |

### 8. Spawn contract

| ID | Tag | Rule |
|---|---|---|
| **AB-S1** | CONFIRMED | **Set-then-invoke.** The spawned object that detects a hit assigns `AbilityContext.Target` (the hit `Collider2D`, or `null` when that collider has no `INegativeReceiver`) and then, in the next statement, invokes the effect's callback. The effect reads the target from the context; when the target is `null` it deals no damage and runs no on-hit effect. The null check is intentional (BUG-072 owner review 2026-09-22; `SpawnProjectileBase.cs:36-42`, `DamageArea.cs:22-31`) |
| **AB-S2** | AS-BUILT | **Projectile.** Spawned in `Do` at `Origin + direction × SpawnOffset`, rotated to the aim direction (`Forward`; if it is near zero, world right). It flies straight at the prefab's `speed` for the effect's `Lifetime` seconds. Touching a collider on the prefab's `blockMask` despawns it with no callback. Touching a collider on `targetMask` makes one callback and despawns it — a projectile hits **one** target (`pierceCount` is serialized but not read). Colliders on neither mask are ignored (`SpawnProjectileEffect.cs:14-36`, `ProjectileBody.cs:31-64`) |
| **AB-S3** | AS-BUILT | Projectile damage is `baseDamage` (flat, from the effect asset). The attack position passed to the receiver is the cast's `Origin`. The receiver then applies its own Defense |
| **AB-S4** | CONFIRMED | **Summon = two objects in one effect.** `Prefab` is the **telegraph**, spawned in the `Cast` phase at `TargetPoint` with a no-op callback. `summonPrefab` is the **payload**, spawned in the `Do` phase at `TargetPoint`. Telegraph-then-strike is the intended shape (BUG-078 owner review 2026-09-22; Consecrate = `RuneCircle.prefab` then `Lightning.prefab`) |
| **AB-S5** | AS-BUILT | The summon payload acts when an animation event on its own clip calls `Execute()`. `DamageArea` (Lightning) then collects up to `maxTargets` colliders on its `layerMask` within `radius` of itself and reports each one that has an `INegativeReceiver`, one callback per collider (`DamageArea.cs:22-31`). A payload clip with several `Execute()` events strikes several times |
| **AB-S6** | AS-BUILT | Summon damage per reported target is `baseDamage + caster MagicDamage`, with MagicDamage read at the moment of the strike (so a buff cast before the strike lands counts) (`SpawnSummonEffect.cs:33-43`) |
| **AB-S7** | CONFIRMED | `[PLANNED]` **Every damaging spawn effect names its own scaling stat and coefficient:** `damage = baseDamage + coefficient × caster stat`. Rain Arrows deals its damage through the effect callback so it follows the same rule and can carry on-hit effects (owner decision 2026-11-13, OQ-7). **Not implemented yet:** today a projectile uses no caster stat (AB-S3), a summon always adds MagicDamage at coefficient 1 whatever the class (AB-S6; the code comment says "need fix to flexible stat type"), and Rain Arrows (`DamagesInManyPos`) deals the prefab's fixed `damagePerArrow`, ignores the effect's `baseDamage` and never uses the callback (BUG-109 item 3). Still to be authored per ability: which stat and which coefficient (→ OQ-11). Still undecided: whether the hit position handed to the receiver is the strike point or the caster's position (→ OQ-7b) |
| **AB-S8** | CONFIRMED | **A spawned object leaves the world in exactly three ways** (owner decision 2026-11-13, OQ-17): (1) it **hits something** (a projectile touching a target or a blocker, AB-S2); (2) its **`Lifetime` runs out** (`Lifetime > 0` on the effect); (3) its **animation finishes** and an animation event on the clip despawns it, the same way a lifetime expiry does (`SpawnMono.DespawnOneSelf()`, back to the pool). With `Lifetime = 0` there is no timer, so way (3) — or (1) — must apply. As built: every live summon effect has `Lifetime = 0`. *Not verified:* that the `RuneCircle` and `Lightning` clips carry the despawn event. `RainArrows` ends with `Destroy()` on a pooled object instead of a pool release (BUG-109 item 2) |
| **AB-S9** | AS-BUILT | Which layers a spawned object can hit is set on the **prefab**, not derived from the caster. Today `Slash-VFX-GO` targets layer `Enemy` and is blocked by `Obstracles`; `Lightning` and `RainArrows` target `Enemy` |
| **AB-S10** | CONFIRMED | The attack position handed to the receiver (it sets the direction of the hit reaction) is **the caster's position** — as built (`currentContext.Origin`, AB-S3 / AB-S6). Using the real point of impact instead is preferred **if it is not complicated to do**; until someone does it, the caster's position is correct (owner decision 2026-11-13, OQ-7b) |

### 9. Enemy abilities

| ID | Tag | Rule |
|---|---|---|
| **AB-N1** | CONFIRMED | An enemy uses the same `AbilityDefinition` assets and the same runtime as the player. The owner component (`EntityAbilityHolder`) is optional on an enemy prefab; bindings come from `EntityData.AbilityBindings`; costs are paid from the enemy's own vitals (ADR-0005 `:230`, `:238-242`) |
| **AB-N2** | CONFIRMED | In the current phase no enemy uses abilities (owner design note, `docs/systems/enemy/CHANGELOG.md:39`, 2026-11-04). Verified: all 11 `EntityData` assets have `abilityBindings: []`. Rules AB-N3…N8 are therefore dormant |
| **AB-N3** | AS-BUILT | **Selection.** Each frame in a basic state, an enemy that has a target inside its attack range tries its slots in the order Primary, Secondary, Utility, Ultimate and casts the first one that passes the gate (AB-G1). This check comes before the weapon attack check, so an available ability always wins over a basic attack (`EntityBasicState.cs:45-59`, `AbilityHolderBase.cs:175-182`) |
| **AB-N4** | AS-BUILT | **Driver.** The enemy has no ability animation events. `EntityAbilityState` makes one dispatch per frame from `LogicUpdate`, so an `Active` ability runs Start, Cast, Do and Exit on four consecutive frames and the enemy returns to Idle. The enemy animator needs an `Ability` bool |
| **AB-N5** | AS-BUILT | **Hold on an enemy.** The `Cast` step is dispatched once on entry, the enemy then waits `MaxHoldTime` seconds, releases, and the `Cast` step is dispatched a second time before `Do`. An enemy hold is always exactly two preparation steps (`EntityAbilityState.cs:49-64`) |
| **AB-N6** | AS-BUILT | The enemy stops moving on entering the ability state; death interrupts it; on exit the enemy's own animator controller is restored |
| **AB-N7** | CONFIRMED | `[PLANNED]` **An ability cast by an enemy hits the player.** Enemies do not use the effect system in this phase (AB-N2); when they do, the first target is the player (owner decision 2026-11-13, OQ-8). **Not implemented yet:** target layers live on the prefab (AB-S9) and every current prefab targets `Enemy`, so an enemy casting any existing ability would hit other enemies and never the player. How the target side is chosen (by caster, or per prefab) is left to implementation (bug-inbox NOTE-20261113-11) |
| **AB-N8** | AS-BUILT | Enemy aim: `Forward` is the enemy's look direction and `TargetPoint` is its target's position, both taken when the ability is selected (AB-G8) |

---

## Formulas

All names are serialized field names or runtime members.

```
# Activation gate (AB-G1..G3)
canStart        = CooldownRemaining == 0
                  AND every condition.IsMet(context)
                  AND for every cost in AbilityDefinition.Costs:
                        cost.value <= caster.currentVital[cost.statType]

# Payment (AB-C2)
currentVital[statType] = max(0, currentVital[statType] - cost.value)

# Cooldown (AB-CD1, AB-CD3)
on Do dispatch:        CooldownRemaining = AbilityDefinition.Cooldown
on interrupt by a hit, cost already paid:  CooldownRemaining = AbilityDefinition.Cooldown   # AB-L7, decided, not implemented
on interrupt by a hit, cost not paid yet:  CooldownRemaining unchanged (0)
on a ticking frame:    CooldownRemaining = max(0, CooldownRemaining - Time.deltaTime)

# Hold (AB-H3, AB-H5)
on a ticking frame while holding:
    CurrentHoldTime += Time.deltaTime
    if MaxHoldTime > 0: CurrentHoldTime = min(CurrentHoldTime, MaxHoldTime)
HoldRatio = MaxHoldTime > 0 ? clamp01(CurrentHoldTime / MaxHoldTime) : 0     # always 0 today (BUG-083)

# Channel accounting for a Hold ability (AB-H2, AB-C8)
castDispatches   = N + 1          # N = Cast animation events while the key is down
effectCostPaid   = effect cost × castDispatches     (while affordable)
applyCount       = 1

# Damage handed to INegativeReceiver.TakeDamage (before the receiver's Defense)
projectileDamage = SpawnProjectileEffect.baseDamage                              # AB-S3
summonDamage     = SpawnSummonEffect.baseDamage + caster.GetStatValue(MagicDamage)   # AB-S6
rainArrowDamage  = DamagesInManyPos.damagePerArrow    # per wave, each enemy at most once per wave

# Projectile reach (AB-S2)
maxRange         = SpawnOffset + speed × Lifetime
# Blessed Slash: 0.8 + 15 × 8 = 120.8 units (effectively "until it hits something")

# Timed buff (BuffDebuffStatsForDuration)
t = 0:          recipient stat modifiers += statModifier.statsModifiers
t = duration:   those modifiers are removed
```

`ModifierType`: `Flat = 100`, `PercentAdd = 200` (0.10 = +10 %), `PercentMult = 300`. How a modifier
changes a stat is defined in [`stat-system.md`](stat-system.md).

---

## Ability Catalogue (as authored on `origin/sprint-22`)

`statType` 100 = HP, 101 = Mana. Layer `Enemy` = mask 64, `Obstracles` = mask 1024.

### Paladin — `Assets/SO/Player/Paladin/Paladin.asset` `[IMPLEMENTED]`

| Slot (key) | Ability | Type | `MaxHoldTime` | Cooldown | Cost | Effects, in order |
|---|---|---|---|---|---|---|
| Primary (`1`) | Blessed Slash | Hold | 0 | 1 s | 5 Mana | [0] `SpawnProjectileEffect` — prefab `Slash-VFX-GO`, `baseDamage` 30, `SpawnOffset` 0.8, `Lifetime` 8 s; prefab `speed` 15, targets `Enemy`, blocked by `Obstracles`, `pierceCount` 0 |
| Secondary (`2`) | Consecrate | Active | 0 | 1 s | 15 Mana | [0] `SpawnSummonEffect` — telegraph `RuneCircle`, payload `Lightning`, `baseDamage` 30 (+ MagicDamage), `Lifetime` 0; `Lightning`: `radius` 2, targets `Enemy`, `maxTargets` 4 |
| Utility (`3`) | Blessing | Active | 0 | 1 s | 10 Mana | [0] `BuffDebuffStatsForDuration` on Caster — PhysicalDamage `PercentAdd` 2 (+200 %), STR `Flat` +20, `duration` 3 s · [1] `RecoveryReductionStatsEffect` — nothing authored, no-op (BUG-117) |
| Ultimate (`4`) | Avatar of Light | Active | 0 | 1 s | 40 HP + 50 Mana | [0] `RecoveryReductionStatsEffect` ("Recovery Stat Effect") — nothing authored, no-op (BUG-117) · [1] `BuffDebuffStatsForDuration` ("Gain For Time Effect") on Caster — MagicDamage `Flat` +100000, `duration` 20 s |

An unreferenced asset sits in the Blessed Slash folder
(`Effect/Paladin Recovery Reduction Stats Effect.asset`, old format, HP 30); no ability lists it.

### Archer — `Assets/SO/Player/Archer/Archer.asset`

| Slot (key) | Bound asset | Type | Cooldown | Cost | Effects |
|---|---|---|---|---|---|
| Primary (`1`) | `Hawk Eye` | Active | 1 s | none | **none** (shell) |
| Secondary (`2`) | `Kick-back Trap` | Active | 1 s | none | **none** (shell) |
| Utility (`3`) | `Volley` | Active | 1 s | none | **none** (shell) |
| Ultimate (`4`) | `Rain Arrows` | Active | 1 s | 15 Mana | [0] `SpawnSummonEffect` — telegraph `RuneCircle`, payload `RainArrows`, effect `baseDamage` 30 (**not used**), `Lifetime` 0; `RainArrows`: `damagePerArrow` 10, `arrowRadius` 0.3, targets `Enemy`, `maxTargetsPerPoint` 3 (BUG-109 open) |

### Mage — `Assets/SO/Player/Mage/Mage.asset`

| Slot (key) | Bound asset | Type | Cooldown | Cost | Effects |
|---|---|---|---|---|---|
| Primary (`1`) | `Fireball` | Active | 1 s | none | **none** (shell) |
| Secondary (`2`) | `Chain Lightning` | Active | 1 s | none | **none** (shell) |
| Utility (`3`) | `Frost Nova` | Active | 1 s | none | **none** (shell) |
| Ultimate (`4`) | `Meteor` | Active | 1 s | none | **none** (shell) |

A **shell** has an animator override but no cost and no effect: the press passes the gate, the cast
animation plays, nothing happens in the world, and a 1 s cooldown starts.

### Authored but bound to no character

| Asset | Type | `MaxHoldTime` | Cooldown | Cost | Effect |
|---|---|---|---|---|---|
| `Archer/…/Hawk Eye (GDD)` | Hold | 1.2 s | 2 s | 8 Mana | `SpawnProjectileEffect` — `Slash-VFX-GO`, `baseDamage` 84, `Lifetime` 0.73 s |
| `Archer/…/Rain Arrows (GDD)` | Active | 0 | 35 s | 50 Mana | `SpawnSummonEffect` — `RuneCircle` + `Lightning`, `baseDamage` 63 |
| `Archer/…/Piercing Shot` | Active | 0 | 1 s | none | none (shell) |
| `Mage/…/Fireball (GDD)` | Active | 0 | 0.6 s | none | `SpawnProjectileEffect` — `Slash-VFX-GO`, `baseDamage` 56, `SpawnOffset` 0.7, `Lifetime` 0.6 s |
| `Mage/…/Chain Lightning (GDD)` | Active | 0 | 4 s | 22 Mana | `SpawnSummonEffect` — `RuneCircle` + `Lightning`, `baseDamage` 25 |
| `Mage/…/Meteor (GDD)` | Active | 0 | 40 s | 80 Mana | `SpawnSummonEffect` — `RuneCircle` + `Lightning`, `baseDamage` 110 |
| `ShootSpirit/SpiritBomd`, `ShootSpirit/MainEffect/ShootSpirit` | — | — | — | — | Legacy; reference deleted scripts (BUG-073, BUG-090). Not part of this design |

**AB-K1 (CONFIRMED).** The Archer and Mage slots are **intentionally empty in this phase**: skills and
effects have not been attached to those classes yet (owner decision 2026-11-13, OQ-10). The class
data binds the shells; the `(GDD)` variants that carry the numbers of
`class-abilities-archer-mage.md` are bound to nothing. QA tests the shell behaviour only (AC-46)
and does not test Archer / Mage ability effects until they are attached (bug-inbox NOTE-20261113-8).

**AB-K2 (CONFIRMED).** The Paladin values are **temporary test values**, kept as they are during
the test phase (owner decision 2026-11-13, OQ-11): every cooldown is the default 1 s (the Ultimate
included), Avatar of Light adds +100000 MagicDamage for 20 s, Blessing adds +200 % PhysicalDamage.
QA uses them to check that the mechanics work (section C), not to judge balance. The scaling stat
and coefficient of AB-S7 are also still to be authored per ability.

### Enemies

No enemy has an ability bound (AB-N2).

---

## Edge Cases

| # | Situation | What happens | Rule |
|---|---|---|---|
| EC-1 | Press a slot with no ability bound | Nothing | AB-G1 |
| EC-2 | Press while Mana (or HP) is below an ability cost | Nothing: no cast, no spend, no cooldown, no feedback | AB-G2, AB-G4 |
| EC-3 | Press with the resource exactly equal to the cost | Mana: cast proceeds, Mana ends at 0. HP: **design** — refused (HP must be greater than the cost); **today** — cast proceeds and the caster stays alive at 0 HP | AB-G3, AB-C3 (`[PLANNED]`) |
| EC-4 | Press during cooldown | Nothing | AB-CD6 |
| EC-5 | Press a second ability key during a cast | Ignored, not buffered | AB-G5 |
| EC-6 | Press an `Active` ability while attacking or taking damage | **Design** — nothing happens at all; **today** — gate may pass, cast is dropped, animator controller is swapped | AB-G7 (`[PLANNED]`), AB-G6 |
| EC-7 | Hold a `Hold` ability key while attacking, keep it down | **Design** — nothing; the key must be pressed again in Idle / Move; **today** — cast begins when the player returns to Idle / Move | AB-G7 (`[PLANNED]`), AB-G6 |
| EC-8 | Take non-lethal damage during a cast | **Design** — the cast is interrupted (player or enemy); no effect; cost already paid → stays spent and the ability goes on cooldown; cost not paid yet → nothing spent, no cooldown; **today** — HP drops, cast continues | AB-L7 (`[PLANNED]`) |
| EC-9 | Die during a cast, before `Do` | Cast ends; ability cost stays spent; no effect; no cooldown (intended) | AB-L8 |
| EC-10 | Resource drops between the press and the `Start` dispatch | Payment drains what is left to 0; the cast continues | AB-C2 |
| EC-11 | Two abilities cost the same resource | Each is gated on its own at its own press | AB-CD2 |
| EC-12 | Projectile touches a wall (`Obstracles`) | Despawns, no damage | AB-S2 |
| EC-13 | Projectile touches two enemies in a line | Only the first one touched is hit | AB-S2 |
| EC-14 | Projectile touches a target-layer collider with no `INegativeReceiver` | Despawns; no damage; no on-hit effects | AB-S1 |
| EC-15 | Summon strike with no enemy in `radius` | Animation plays, nothing is damaged; cost and cooldown are still spent | AB-S5 |
| EC-16 | More than `maxTargets` (4) enemy colliders inside a Lightning strike | At most 4 colliders are considered; which ones is engine-defined | AB-S5 |
| EC-17 | An effect slot in `Effects` is empty (null) | Skipped in both `Cast` and `Do` | AB-L3, AB-L4 |
| EC-18 | A spawn effect has no `Prefab` | Warning logged, nothing spawned for that step; the rest of the cast continues | `SpawnEffectBase.cs:11-15` |
| EC-19 | Same timed buff cast again before it expires | The modifiers stack (two independent applications); each expires on its own timer | AB-E7 |
| EC-20 | Stat effect with `Recipient = Target` used as an ability-level effect | Does nothing (no target exists yet) | AB-E5 |
| EC-21 | Class re-picked at the Champion pedestal | All four instances rebuilt; cooldowns reset to 0 | AB-D5 |
| EC-22 | Cooldown running while the player spends time attacking | **Design** — the cooldown keeps counting; **today** — it pauses for that time | AB-CD4 (`[PLANNED]`), AB-CD3 |
| EC-23 | Cast a shell ability (Archer 1-3, Mage 1-4) | Animation and 1 s cooldown; no world effect (intended for this phase) | AB-K1 |

---

## Dependencies

| System | Direction | Contract |
|---|---|---|
| Character / Core hub (`Character/Base/`) | Abilities depend on it | `AbilityHolderBase<TCore>` is a core component; it resolves vitals, stats and input through `Core.TryGetCapability`. `CharacterBase.ApplyCharacterData()` calls `SetAbility()` |
| Character data (`CharacterData`, `PlayerData`, `EntityData`) | Abilities depend on it | `AbilityBindings` (slot → `AbilityDefinition`) |
| Stat System ([`stat-system.md`](stat-system.md)) | both ways | Reads `MagicDamage` for summon damage; writes timed stat modifiers through `IEffectReceiver` → `IStatService.AddModifiersFromSource` / `RemoveModifiersFromSource` |
| Vitals (`VitalStatsBase`) | Abilities depend on it | Reads current HP / Mana for the gate; pays costs with `Reduction()` |
| Damage & Health (`INegativeReceiver`) | Abilities → damage | `TakeDamage(float amountDamage, Vector2 attackPosition)` on the hit collider |
| Input (`PlayerInputHandler`, `ICharacterInput`) | Abilities depend on it | Four slot actions (keys `1`-`4`), `AimDirection`, `AimDirectionIndex`, `AimPoint` |
| Player state machine | drives abilities | `PlayerBasicState` enters `PlayerSkillWeaponState` on `IsSkill`; the latter dispatches on `AnimationOnAction` |
| Enemy AI | drives abilities | `EntityBasicState` selects, `EntityAbilityState` dispatches; cast range = attack range |
| Animation System ([`animation-system.md`](animation-system.md)) | Abilities depend on it | `AbilityDefinition.AnimatorOverride`; player parameters `StateSkill`, `DoAB`, bool `Ability`; clip events `AnimationOnAction` / `AnimationEnd`; summon clips call `Execute()` |
| Object pooling (`IObjectPoolService`) | Abilities depend on it | Every spawned object is taken from and returned to the pool; it is the only member of `IAbilityServices` |
| Weapons ([`weapons-system.md`](weapons-system.md)) | shared component | `ProjectileBody` / `ProjectileConfig` / `IProjectilePayload` are shared with ranged weapons. Abilities do not need a weapon: they can be cast unarmed |
| UIFlow HUD | depends on abilities | Hotbar icons from `AbilityBindings`; cooldown display from `AbilityCooldownStarted` |
| Class kits ([`class-abilities-archer-mage.md`](class-abilities-archer-mage.md)) | depends on abilities | Design-first content that this framework must be able to express |

---

## Tuning Knobs

| Knob | Where | Effect | Notes |
|---|---|---|---|
| `Cooldown` | `AbilityDefinition` | Seconds before the slot can be cast again | Default 1; all bound abilities use 1 |
| `Costs[]` (`value`, `statType`) | `AbilityDefinition` | Price of a cast, HP and / or Mana | Do not use `Coin` (AB-C9) |
| `ActivationType` | `AbilityDefinition` | `Active` (press) or `Hold` (channel until release) | `Passive` has no behaviour (AB-D6) |
| `MaxHoldTime` | `AbilityDefinition` | Clamp of the hold timer; enemy hold length | Has no gameplay effect for the player today (AB-H4, AB-H5) |
| `Effects[]` order | `AbilityDefinition` | Execution order and resource priority | A balance decision (AB-E4) |
| `AnimatorOverride` | `AbilityDefinition` | Clips of the cast; their events set how fast the phases advance and how often a hold dispatches | AB-L6 |
| `Costs[]`, `SubConditions[]` | effect asset | Tier price / tier condition | **Do not author** until tiers exist (AB-C7) |
| `Recipient` | effect asset | Caster or Target | AB-E5 |
| `SubEffects[]` | spawn effect asset | On-hit effects | AB-E6 |
| `baseDamage` | `SpawnProjectileEffect`, `SpawnSummonEffect` | Damage per hit | Summon adds MagicDamage (AB-S6) |
| `SpawnOffset`, `Lifetime` | spawn effect asset | Spawn distance in front of the caster (projectile); seconds until despawn | `Lifetime` 0 on a summon = no timed despawn (AB-S8) |
| `Prefab` / `summonPrefab` | `SpawnSummonEffect` | Telegraph / payload | AB-S4 |
| `speed`, `targetMask`, `blockMask` | projectile prefab (`SpawnProjectileBase`) | Flight speed; what it hits; what stops it | `pierceCount` is not read |
| `radius`, `layerMask`, `maxTargets` | `DamageArea` prefab (Lightning) | Strike area, what it hits, cap on colliders per strike | |
| `damagePerArrow`, `arrowRadius`, `enemyMask`, `maxTargetsPerPoint`, impact waves | `DamagesInManyPos` prefab (RainArrows) | Rain Arrows damage and pattern per direction | Ignores the effect asset's `baseDamage` |
| `statModifier`, `duration` | `BuffDebuffStatsForDuration` | Which stats change, by how much, for how long | |

---

## Acceptance Criteria

Format: **Given / When / Then**, with numbers. Use a Paladin unless stated. "Test today" says whether
the criterion can be checked on the current build; criteria on OPEN rules are listed separately and
must not be used as pass / fail until the owner answers.

### A. Gate, cost and cooldown — CONFIRMED / AS-BUILT

| ID | Criterion | Rule | Test today |
|---|---|---|---|
| AC-01 | Given Mana = 14, when Consecrate (`2`, 15 Mana) is pressed, then no cast animation plays, Mana stays 14, no rune or lightning spawns, and the hotbar shows no cooldown | AB-G2, AB-G4 | Yes |
| AC-02 | Given Mana = 15, when Consecrate is pressed, then the cast runs and Mana = 0 after the Start phase | AB-G3, AB-C1 | Yes |
| AC-03 | Given Mana = 100, when Blessing (`3`, 10 Mana) is cast to completion, then Mana = 90 — the cost is charged exactly once | AB-C1 | Yes |
| AC-04 | Given HP = 39 and Mana = 100, when Avatar of Light (`4`, 40 HP + 50 Mana) is pressed, then nothing happens and HP = 39, Mana = 100 | AB-G2 | Yes |
| AC-05 | Given HP = 100 and Mana = 49, when Avatar of Light is pressed, then nothing happens and HP = 100, Mana = 49 (no partial payment) | AB-G2 | Yes |
| AC-06 | Given HP = 100 and Mana = 100, when Avatar of Light is cast, then HP = 60 and Mana = 50 | AB-C1 | Yes |
| AC-07 | Given Consecrate has just reached its Do phase, when `2` is pressed again while the player stands idle for less than 1.0 s, then nothing happens; pressed after 1.0 s of idle time, it casts | AB-CD1, AB-CD6 | Yes |
| AC-08 | Given Consecrate is on cooldown, when Blessing (`3`) is pressed, then Blessing casts — one slot's cooldown does not block another | AB-CD2 | Yes |
| AC-09 | When any ability reaches its Do phase, then the hotbar slot for that ability starts a cooldown display of `Cooldown` seconds (1 s for every Paladin ability) | AB-CD5 | Yes |
| AC-10 | Given a slot with no ability bound (use `BaseData.asset`), when its key is pressed, then nothing happens and no error is logged | AB-G1 | Yes |
| AC-11 | Given a cast is in progress, when another ability key is pressed, then it is ignored and does not fire after the cast ends | AB-G5 | Yes |
| AC-12 | Given a cast is in progress, when WASD is held, then the player does not move until the cast ends | AB-L7 | Yes |
| AC-13 | *Build behaviour only — superseded as a requirement by AC-44 (owner decision 2026-11-13).* Given a cast is in progress, when the player takes a non-lethal hit, then HP drops by the hit and the cast still completes (effect applied, cooldown started) | AB-L7 | Yes — expected to change |
| AC-14 | Given the Paladin is picked again at the Champion pedestal while an ability is on cooldown, then that ability is castable immediately | AB-D5 | Yes |

### B. Life cycle and holding — CONFIRMED / AS-BUILT

| ID | Criterion | Rule | Test today |
|---|---|---|---|
| AC-15 | For one cast of an `Active` ability, the phase order logged is Start, Cast, Do, Exit, each exactly once | AB-L1, AB-H1 | Yes (debug log or `AbilityHolder` Inspector) |
| AC-16 | For an `Active` ability, holding the key down for 3 s or tapping it gives the same result: one cast | AB-H1 | Yes |
| AC-17 | Given Blessed Slash (`1`, Hold) is pressed and the key is kept down for 2 s, then no slash is spawned during those 2 s; when the key is released, exactly one slash is spawned | AB-H2, AB-L3 | Yes |
| AC-18 | Given Blessed Slash is held for 2 s and released with Mana = 100 at the start, then Mana = 95 — the ability cost is charged once, not per loop (no effect costs are authored) | AB-C1, AB-C7 | Yes |
| AC-19 | In every effect asset under `Assets/SO/Skill/`, `Costs` and `SubConditions` are empty | AB-C7 | Yes (asset scan) |
| AC-20 | Given an ability with effects `[A, B]`, then A's spawn / log precedes B's in both the Cast and Do phases; with the list reordered to `[B, A]`, B precedes A | AB-E4 | Yes (test asset) |
| AC-21 | `[PLANNED]` Given a `Hold` ability whose effect has a cost the caster cannot pay, when it is cast, then the effect still applies once at its default level, nothing is charged for the effect, and the ability fires without waiting for the key release | AB-C5, AB-C6 | No — needs a test asset; untested design (BUG-089 reopen list, gap 2) |

### C. Effects and spawned objects — CONFIRMED / AS-BUILT

| ID | Criterion | Rule | Test today |
|---|---|---|---|
| AC-22 | Given a TrainingDummy with Defense = 0 in front of the Paladin, when Blessed Slash hits it, then it loses exactly 30 HP, whatever the Paladin's PhysicalDamage or MagicDamage | AB-S3 | Yes |
| AC-23 | Given two enemies in a line, when Blessed Slash is fired along the line, then only the nearer one loses HP and the slash disappears on it | AB-S2 | Yes |
| AC-24 | Given a wall between the Paladin and an enemy, when Blessed Slash is fired, then the slash disappears at the wall and the enemy loses 0 HP | AB-S2 | Yes |
| AC-25 | When Blessed Slash is released, then the slash appears 0.8 units from the player in the aim direction taken at the key press, and travels at 15 units / s | AB-S2, AB-G8 | Yes |
| AC-26 | When Consecrate is cast, then a rune circle appears at the mouse position taken at the key press **before** the lightning, and the lightning appears at the same position — two objects, in that order | AB-S4, AB-E2 | Yes |
| AC-27 | Given a TrainingDummy with Defense = 0 within 2 units of the strike point and a Paladin whose MagicDamage is *M*, when the Consecrate lightning strikes, then the dummy loses exactly `30 + M` HP per strike event | AB-S6 | Yes — closes BUG-072 if it passes |
| AC-28 | Given a TrainingDummy more than 2 units from the strike point, when the lightning strikes, then it loses 0 HP; Mana is still spent and the cooldown still starts | AB-S5 | Yes |
| AC-29 | Given the Stats panel is open, when Blessing is cast, then PhysicalDamage and STR rise for 3 s (STR by exactly 20) and return to their previous values afterwards | AB-E7 | Yes |
| AC-30 | Given Blessing is cast twice 1.5 s apart, then STR is +40 between the second cast and the first expiry, +20 until the second expiry, then back to base | AB-E7, EC-19 | Yes |
| AC-31 | Given Avatar of Light is cast, then MagicDamage is higher by 100000 for 20 s and returns to its previous value afterwards | AB-E7 | Yes |
| AC-32 | Given Avatar of Light is active, when Consecrate strikes a Defense-0 dummy, then the damage uses the buffed MagicDamage (`30 + M + 100000`) | AB-S6 | Yes |
| AC-33 | Casting Blessing or Avatar of Light changes current HP and Mana only by the cost — the "recovery" effect in each restores nothing | AB-E7 (BUG-117) | Yes — expected to change when BUG-117 is fixed |
| AC-34 | Given a slash hits a collider on layer `Enemy` that has no `INegativeReceiver`, then the slash disappears, nothing is damaged and no error is logged | AB-S1 | Yes (test object) |

### D. Enemy — dormant

| ID | Criterion | Rule | Test today |
|---|---|---|---|
| AC-35 | Every `EntityData` asset has an empty `abilityBindings` list | AB-N2 | Yes (asset scan) |
| AC-36 | `[PLANNED]` Given an enemy with an `EntityAbilityHolder`, an `Active` ability and a target in attack range, then it casts the ability before using its weapon, pays the cost from its own Mana / HP, and does not cast again until `Cooldown` seconds of ticking time have passed | AB-N1, AB-N3 | No — no enemy is set up for it |

### E. Criteria waiting on an owner decision — do not use as pass / fail

| ID | Observation on the current build | Open rule |
|---|---|---|
| — | *None left.* The three questions still open (OQ-4 `TryCast()` stacking, OQ-9 enemy hold, OQ-12 `Passive`) have no live content to observe |

(AC-O1 … AC-O6 were all decided on 2026-11-13 and moved to section F: AC-37, AC-38, AC-41, AC-42, AC-43, AC-46.)

### F. Decided by the owner on 2026-11-13 — design is fixed, code not changed yet

A failure here is the known gap between the decision and the current build, not a new defect.
Report it against the rule, once.

| ID | Criterion | Rule | Result expected on the current build |
|---|---|---|---|
| AC-37 | Given HP = 40 and Mana = 100, when Avatar of Light (40 HP + 50 Mana) is pressed, then nothing happens and HP = 40, Mana = 100. Given HP = 41, the cast runs and HP = 1 | AB-C3 | **Fails** at HP = 40: the cast runs, HP = 0, the player stays alive (NOTE-20261113-6) |
| AC-38 | Given Consecrate has just reached its Do phase (cooldown 1 s), when the player attacks continuously for 3 s and then presses `2`, then Consecrate casts | AB-CD4 | **Fails**: the cooldown did not count during the attacks (NOTE-20261113-12) |
| AC-39 | Given any ability with `Cooldown` = *X*, then the time from its Do phase until it can be cast again is *X* s (± 1 frame) whatever the player does in between — attack, take damage, pick up, interact | AB-CD4 | **Fails** for every state outside Idle / Move / ability state |
| AC-40 | Given a spawn effect authored with `baseDamage` = *B*, scaling stat *S* and coefficient *k*, when it hits a Defense-0 target, then the target loses exactly `B + k × S` HP; with *k* = 0 it loses exactly *B* | AB-S7 | **Not testable**: the two fields do not exist. Until they do, AC-22, AC-27 and AC-32 describe the build |
| AC-41 | Given the player dies after the Start phase and before the Do phase of a cast, then the ability cost stays spent, no effect is applied and the ability is not on cooldown after respawn | AB-L8 | Passes (hard to time by hand; use a debug kill) |
| AC-42 | Given the player is in a sword swing (or any state other than Idle / Move), when `2` (Consecrate) is pressed with enough Mana, then nothing happens: no cast now or later, Mana unchanged, no cooldown, and the swing animation is not altered | AB-G7 | **Fails**: no cast, but the character's animation set is swapped mid-swing (NOTE-20261113-7) |
| AC-43 | Given Blessed Slash is held, when the mouse is moved to a new direction and the key is released, then the slash flies in the direction aimed **at release** | AB-G8 | **Fails**: it flies in the direction aimed at the press (NOTE-20261113-13) |
| AC-44 | Given a cast is in progress and the cost is already paid (after the Start phase), when the player takes a non-lethal hit before the Do phase, then the cast stops, no effect is applied, the cost stays spent and the ability **is on cooldown for its full `Cooldown`** (1 s for Consecrate). Given the hit lands before the Start phase, the cast stops, nothing was spent and the ability is **not** on cooldown — it can be pressed again at once. The same holds for an enemy hit while casting | AB-L7 | **Fails**: the cast completes (this replaces AC-13, which describes the build). Enemy half not testable (AB-N2) |
| AC-45 | Given a `Hold` ability with `MaxHoldTime` = 1.2 s: held 0.6 s then released → the effects receive `HoldTime` = 0.6, `HoldRatio` = 0.5; held 3 s then released → `HoldTime` = 1.2, `HoldRatio` = 1.0, and the ability did not fire by itself at 1.2 s | AB-H4, AB-H5 | **Fails / not testable**: both values are always 0 (BUG-083) and no bound ability has `MaxHoldTime > 0`. The "did not fire by itself" half passes |
| AC-46 | Given an Archer (keys `1`-`3`) or a Mage (keys `1`-`4`), when the key is pressed, then the cast animation plays, HP and Mana are unchanged, nothing is spawned, and the slot is on cooldown for 1 s | AB-K1 | Passes — intended for this phase |
| AC-47 | `[PLANNED]` Given an enemy that casts a damaging ability at the player, then the player loses HP and no other enemy does | AB-N7 | **Not testable**: no enemy has an ability (AB-N2); every prefab targets `Enemy` |
| AC-48 | Every spawned ability object is gone from the scene (returned to the pool) after one of: it hit something, its `Lifetime` ran out, or its animation finished. After casting Consecrate 10 times and waiting 10 s, no `RuneCircle` or `Lightning` instance is active | AB-S8 | To be run — not verified statically. Rain Arrows is expected to fail (BUG-109 item 2) |

---

## Open Questions

The owner reviewed this list on 2026-11-13. A struck-through row is **decided**: the first cell
names the decision and the rule that now carries it, and the rule text is authoritative — the
"Options" and "Recommendation" cells are kept only as the record of what was asked. Three rows are
parked and still open: **OQ-4** (pending), **OQ-9** (deferred until the demo is done — raise it
again then) and **OQ-12** (deferred).

| # | Question | Options | Recommendation |
|---|---|---|---|
| ~~OQ-1~~ ✅ **decided 2026-11-13: (a)** → AB-CD4 | Should cooldowns (and hold timers) count in every state, or only in Idle / Move / ability state as today? (AB-CD3, AB-CD4) | (a) Count always, from one place (the holder's own update) · (b) Keep as is and document "attacking pauses cooldowns" as a mechanic · (c) Count always except while dead | **(a)**. A cooldown the HUD shows as "1 s" should take 1 s. The current pause is invisible to the player and makes the hotbar display wrong |
| ~~OQ-2~~ ✅ **decided 2026-11-13: (a)** → AB-C3 | Can an HP cost kill, and what is the minimum HP to cast? (AB-C3, AB-G3) | (a) Require `HP > cost` — the cast can never reduce HP to 0 · (b) Allow `HP >= cost` and let the cost kill (route it through damage / death) · (c) Allow it but leave 1 HP | **(a)**. One-character change in the gate for HP costs only, and it removes the "alive at 0 HP" state. Keep `>=` for Mana |
| ~~OQ-3~~ ✅ **decided 2026-11-13: (a)** → AB-L8 | Cost is paid at the Start animation event; cooldown starts at Do. If the caster dies in between, the cost is lost. Accept? Should anything other than death be able to interrupt? (AB-L8) | (a) Accept, no refund · (b) Pay at Do instead of Start · (c) Refund on interruption | **(a)**, written down as intended. It only happens on death today, where a refund is meaningless. Revisit if stagger / interrupt is ever added |
| **OQ-4** ⏸ **pending (owner, 2026-11-13)** — `TryCast()` is reserved for the stacking logic of held effects and sub-effects; that design is not settled | May a `TryCast()` override have side effects? (AB-E3) | (a) No — move the telegraph out of `TryCast()` into an explicit cast-phase hook · (b) Yes, but only cosmetic ones (telegraphs, VFX, sound), never damage or resources, and at most once per cast · (c) Yes, unrestricted | **(b)**. It keeps Consecrate as is, and "once per cast" removes the repeated telegraph on held abilities and on enemies. Also decide here whether a refused tier should still show the telegraph (BUG-089 gap 7) |
| ~~OQ-5~~ ✅ **decided 2026-11-13: (b)** hold freely, value = time held capped at `MaxHoldTime`, measured at release → AB-H4, AB-H5 (BUG-083 is the fix) | What is holding for, on the player side? What should `HoldTime` / `HoldRatio` mean and when are they measured? Should the ability release by itself at `MaxHoldTime`? (AB-H4, AB-H5, BUG-083) | (a) Charge: measure at release, effects scale by `HoldRatio`, auto-release at `MaxHoldTime` · (b) Charge without auto-release (hold at full charge as long as wanted) · (c) Channel only: remove the two fields | **(a)** for charge abilities such as Hawk Eye (`MaxHoldTime` 1.2 s in the design-first GDD). It needs BUG-083 fixed first. Decide (b) vs (a) per ability if needed |
| ~~OQ-6~~ ✅ **decided 2026-11-13:** `Hold` or `Active` is the designer's choice per ability → AB-H6 | Is Blessed Slash meant to be `Hold`? It has `MaxHoldTime = 0` and nothing to charge. (AB-H6) | (a) Make it `Active` · (b) Keep `Hold` and give it a charge (damage or range by `HoldRatio`) · (c) Keep as is | **(a)** until OQ-5 is implemented; then (b) if a charged slash is wanted |
| ~~OQ-7~~ ✅ **decided 2026-11-13: (a)** → AB-S7 | How should ability damage scale with the caster? (AB-S7) | (a) Each spawn effect names a scaling stat and a coefficient: `damage = baseDamage + coefficient × stat` · (b) Projectiles add PhysicalDamage, summons add MagicDamage · (c) Flat damage everywhere | **(a)**. It covers Paladin (physical) and Mage (magic) with one rule, and matches the code comment "need fix to flexible stat type". Rain Arrows should then deal damage through the effect callback so it follows the same rule and can carry on-hit effects. Also decide whether the hit position passed to the receiver should be the strike point rather than the caster's position |
| ~~OQ-7b~~ ✅ **decided 2026-11-13: (a)**, and (b) if it is not complicated → AB-S10 | Split out of OQ-7, not decided with it: which position is handed to the receiver as the attack position (it sets the direction of the hit reaction) — the caster's position at the key press (today, AB-S3 / AB-S6) or the point where the projectile / strike actually landed? | (a) The caster's position (today) · (b) The projectile's position at impact / the centre of the strike | **(b)**. An enemy hit by a lightning strike behind it should react away from the strike, not away from the player |
| ~~OQ-8~~ ✅ **decided 2026-11-13:** an enemy's ability hits the player → AB-N7 | When an enemy casts, who can its projectile / summon hit? (AB-N7, AB-S9) | (a) Target layers are chosen by the caster's side at spawn time · (b) Separate prefabs per side (player version, enemy version) · (c) Enemies never use these abilities | **(a)**. One asset per ability stays true (AB-N1). Not urgent while AB-N2 holds |
| **OQ-9** ⏸ **deferred (owner, 2026-11-13)** — enemies do not use abilities in this phase; **raise again once the demo is done** | An enemy hold runs the Cast step exactly twice; a player hold runs it once per animation event. Should they match? (AB-N5, AB-C8) | (a) Enemy: one Cast step, wait `MaxHoldTime`, then Do · (b) Enemy: repeat the Cast step on a fixed interval while waiting · (c) Leave as is | **(a)**. Simplest to reason about and to test; decide together with OQ-4 |
| ~~OQ-10~~ ✅ **decided 2026-11-13: (a)** empty on purpose in this phase → AB-K1 | Archer and Mage bind empty shells while the `(GDD)` assets with numbers are bound to nothing. Which set is the truth, and what should QA test? (AB-K1) | (a) Shells are intentional until Phase 2; QA skips Archer 1-3 and Mage 1-4 · (b) Bind the `(GDD)` assets now as temporary content · (c) Delete one of the two sets | **(a)** now, with (b) as soon as the placeholder prefabs are acceptable. Either way, record the decision in `class-abilities-archer-mage.md` |
| ~~OQ-11~~ ✅ **decided 2026-11-13: (a)** temporary test values, kept for now → AB-K2 | What are the real Paladin numbers? Every cooldown is 1 s, Avatar of Light gives +100000 MagicDamage for 20 s on a 1 s cooldown, Blessing gives +200 % PhysicalDamage. (AB-K2) | (a) Treat as placeholders and tune against the ±15 % target in `class-abilities-archer-mage.md` · (b) Confirm as intended | **(a)**. QA should not derive balance cases from these values until tuned |
| **OQ-12** ⏸ **deferred (owner, 2026-11-13)** — `Passive` = innate effect; design not decided in this phase | What is `Passive`? (AB-D6) | (a) Out of scope: mark the value "not supported" · (b) Design it: effects applied on equip, no key, no cooldown | **(a)** for the demo |
| ~~OQ-13~~ ✅ **decided 2026-11-13: (b)** not restored → AB-L9 | Should the player's animator controller be restored after a cast, as the enemy's is? (AB-L9) | (a) Yes, restore the character's own controller on leaving the ability state · (b) No — the next weapon / ability override replaces it anyway | **(a)**, for symmetry and so an ability's clips can never leak into movement or attack animation |
| ~~OQ-14~~ ✅ **decided 2026-11-13: (a)** abilities activate only in Idle / Move → AB-G7 | What should a press outside Idle / Move do? (AB-G6, AB-G7) | (a) Reject it before the gate — nothing changes · (b) Buffer it and cast when the current action ends (as attacks are buffered) · (c) Leave as is | **(a)** as the minimum (it removes the animator swap); **(b)** if ability buffering is wanted for feel |
| ~~OQ-15~~ ✅ **decided 2026-11-13:** aim is taken at key release → AB-G8 | Is aim locked at the key press or taken at release for a held ability? (AB-G8) | (a) Lock at press (today) · (b) Re-read aim at the Do phase · (c) Per ability | **(b)**. A held, aimed ability that ignores where the player is aiming on release feels wrong; it comes almost free with the BUG-083 fix |
| ~~OQ-16~~ ✅ **decided 2026-11-13: (a)** → AB-D5 | Cooldowns reset to 0 on a class pick and on respawn (AB-D5). Accept? | (a) Accept · (b) Carry cooldowns over | **(a)** |
| ~~OQ-17~~ ✅ **decided 2026-11-13:** three ways — on hit, on lifetime end, on an animation-end event → AB-S8 | Who is responsible for removing a summon object when `Lifetime = 0`? (AB-S8) | (a) Rule: every summon prefab releases itself from its last animation event (`DespawnOneSelf()`) · (b) Rule: `Lifetime` must be greater than 0 on every summon effect | **(a)** plus fixing BUG-109 item 2, so one rule covers telegraphs and payloads |
| ~~OQ-18~~ ✅ **decided 2026-11-13: (b)** a hit interrupts; a cost already paid stays spent → AB-L7 | A cast cannot be interrupted by damage (AB-L7). Intended "commitment with super-armour", or should a hit cancel it? | (a) Intended · (b) Hits interrupt (then OQ-3 needs a refund rule) | **(a)**. It matches the "committed" combat pillar and avoids a refund rule |

---

## Rule index

| Tag | Count | Rules |
|---|---|---|
| CONFIRMED | 31 | AB-D1, D3, D5 · AB-G2, G7, G8 · AB-L7, L8, L9 · AB-H2, H4, H5, H6 · AB-C3, C4, C5, C6, C7 · AB-CD4 · AB-E2, E4 · AB-S1, S4, S7, S8, S10 · AB-N1, N2, N7 · AB-K1, K2 |
| ↳ of which `[PLANNED]` | 8 | Decided, **code does not follow yet**: AB-G7 (Idle / Move only), AB-G8 (aim at release), AB-L7 (a hit interrupts), AB-H5 (hold value at release, BUG-083), AB-C3 (`HP > cost`), AB-CD4 (cooldown counts in every state), AB-S7 (scaling stat + coefficient), AB-N7 (enemy ability hits the player) |
| AS-BUILT | 39 | AB-D2, D4, D7 · AB-G1, G3, G4, G5, G6 · AB-L1…L6 · AB-H1, H3 · AB-C1, C2, C8, C9 · AB-CD1, CD2, CD3, CD5, CD6 · AB-E1, E5, E6, E7 · AB-S2, S3, S5, S6, S9 · AB-N3, N4, N5, N6, N8 |
| OPEN | 2 | AB-D6 (`Passive`, deferred) · AB-E3 (`TryCast()` side effects / stacking, pending) |
| **Total** | **72** | AB-S10 was added on 2026-11-13 for OQ-7b. **3 questions remain**, all parked by the owner: OQ-4 (pending — `TryCast()` stacking design), OQ-9 (deferred — enemy hold, raise again after the demo), OQ-12 (deferred — `Passive`). Decided 2026-11-13: OQ-1, 2, 3, 5, 6, 7, 7b, 8, 10, 11, 13, 14, 15, 16, 17, 18 |
