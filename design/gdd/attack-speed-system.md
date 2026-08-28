---
status: authored
source: Assets/Script/Weapons/MeleeWeapon/, Assets/Script/StatSystem/
date: 2026-08-05
revised: 2026-08-20 (documentation audit — Implementation Map retargeted from the deleted
WeaponMelee.cs to Weapon.cs; the "player has no StatsSO" blocker cleared. Design unchanged.)
verified-by: Kiet
---

# Attack Speed System Design

**Status**: In Design

> This GDD answers one question: **how is the `AttackSpeed` stat applied when every
> combo stage has a different animation length?** It defines the layer model, the
> formula chain, and the exact code touchpoints. It does not redefine the combo
> sequence itself — that lives in [weapons-system.md](weapons-system.md).

---

## Overview

Attack speed is a **playback multiplier on attack animations**, sourced from character
stats and equipped weapons. It compresses the timing of every melee attack uniformly
without changing the combo's internal rhythm and without ever truncating an animation.

The system resolves an apparent contradiction in the earlier design. Each combo stage
(`AttackSO`) owns its own animation duration — light hits are short, heavy hits are long.
Separately, `AttackSpeed` was documented as "attacks per second", a fixed rate. Those two
readings cannot both be inputs.

They are not in conflict, because they operate on different layers:

| Layer | Owns | Answers |
|-------|------|---------|
| Animation duration | Per-stage clip length | *What is the combo's internal rhythm?* — light / light / heavy |
| `AttackSpeed` stat | A single global multiplier | *How compressed is that whole rhythm?* |

**Attacks-per-second is an emergent output of this system, never an input constraint.**

---

## Player Fantasy

Investing in attack speed makes the character feel visibly, physically faster. The same
weapon the player has used all run suddenly snaps through its combo — the wind-up shortens,
the recovery shortens, and the third heavy hit lands before the enemy has recovered from
the second. Nothing about the moveset changes; the player simply gets more of it per second.

Weapon choice remains a real decision because it sets the baseline: a dagger starts fast and
stays fast, a greatsword starts slow and becomes merely workable. Stacking speed on a
greatsword is a legitimate build, not a correction of a mistake.

Crucially, speed never costs the player legibility. Animations always play to completion, so
a fast character still reads as the same character — never a stuttering, clipped one.

---

## Detailed Rules

### Layer model

Attack speed resolves from three multiplicative layers, evaluated once per combo stage at
the moment that stage begins:

1. **Weapon layer (equipment)** — `WeaponStats.baseAttackSpeed`. The weapon's identity.
   A dagger is fast, a greatsword is slow. Set per weapon SO.
2. **Character layer (upgrade)** — `StatsSO` value for `StatType.AttackSpeed`, expressed as
   a percentage bonus. This is what the player upgrades through levels, primary-stat
   allocation, and per-run cards.
3. **Stage layer (design tuning)** — `AttackSO.attackRate`. A per-stage nudge that lets a
   designer make the finisher deliberately heavier without re-authoring the animation.
   Defaults to `1.0`.

Layers combine **multiplicatively**. Multiple attack-speed buffs stack **additively within
layer 2** — this is already handled by the existing `StatModifier` stack on `StatsSO`.

### Application

The resolved multiplier is written to `Animator.speed` and simultaneously divides the
stage's cached duration. Because the animation plays faster by exactly the factor the
duration shrinks, the hit frame, the combo window, and the recovery all scale together
automatically. No timing value needs to be scaled by hand.

### Combo behaviour

The multiplier is **identical for every stage of a combo**. A combo authored as
light / light / heavy stays light / light / heavy at any speed — only the wall-clock
duration of the whole chain changes. There is **no per-stage rate gate**; introducing one
would desynchronise the stages from each other and break the chain.

Combo index advancement, the combo window, and the reset condition are unchanged from
[weapons-system.md](weapons-system.md). This system only scales their timing.

### Stat semantics

`StatType.AttackSpeed` is a **percentage bonus**, where `30` means `+30%`. It is not
attacks-per-second. Attacks-per-second is not expressible as a single authored number when
each combo stage has a different duration.

This places `AttackSpeed` in the *percentage / soft-capped* group already defined in
[stat-system.md](stat-system.md): `perLevel = 0`, and the resolved multiplier is clamped.

### Rejected alternative — the GCD model

Enforcing "n attacks per second" as a hard gate independent of animation length is
explicitly rejected. When the gate is shorter than the clip, the only options are cutting
the animation (visually broken) or letting the gate do nothing (pointless). This is the
MMO global-cooldown model; it is correct for MMOs, where animation is decoupled from
timing, and wrong for an animation-driven action game.

---

## Formulas

```
# Layer 1 — weapon (equipment)
weaponSpeed = WeaponStats.baseAttackSpeed
              [dagger 1.4, sword 1.0, greatsword 0.7]

# Layer 2 — character stat (upgrade)
statMult    = 1 + StatsSO.GetStatValue(StatType.AttackSpeed) / 100
              [AttackSpeed = 30 → statMult = 1.30]

# Layer 3 — per-stage rhythm (design tuning)
stageRate   = AttackSO.attackRate
              [light 1.0, heavy 0.8]

# Resolved multiplier
speedMult   = clamp(weaponSpeed * statMult * stageRate, MIN_SPEED_MULT, MAX_SPEED_MULT)
              MIN_SPEED_MULT = 0.5
              MAX_SPEED_MULT = 3.0

# Application
Animator.speed = speedMult
deplayTime     = Utility.DurationNextAttack(overrides) / speedMult
comboWindow    = deplayTime + comboGrace / speedMult
```

Display-only derived value, for HUD and balance spreadsheets. It is **never** used as a gate:

```
effectiveAPS = speedMult / averageBaseStageDuration
               averageBaseStageDuration = mean clip length across the combo's AttackSO list
```

Worked example — sword (`baseAttackSpeed = 1.0`), `AttackSpeed = 50`, heavy finisher
(`attackRate = 0.8`), base clip length `0.9s`:

```
speedMult  = 1.0 * 1.50 * 0.8 = 1.20
deplayTime = 0.9 / 1.20       = 0.75s
```

---

## Edge Cases

| Scenario | Expected behaviour |
|----------|--------------------|
| `speedMult` pushed very high; animation shorter than the hit frame | Clamped at `MAX_SPEED_MULT = 3.0`. The hit fires on an animation event, so it scales with playback automatically and cannot be skipped |
| Slow debuff drives the multiplier toward zero | Clamped at `MIN_SPEED_MULT = 0.5`. The player is slowed, never frozen |
| Stat changes mid-combo (card picked, buff expires) | Recomputed on the next `Weapon.OnAttackEnter()` call (was `SetAnimation()`). The currently playing clip is not retro-scaled — no visual snap |
| Weapon swapped mid-combo | Combo index resets to 0; the new weapon's `baseAttackSpeed` applies from stage 1 |
| `attackRate` left at 0 on an `AttackSO` | Treated as a data error — clamp guarantees a floor of `MIN_SPEED_MULT`; add `[Range(0.1f, 2f)]` on the field to prevent authoring it |
| `AnimatorOverrideController` missing a directional clip | `Utility.DurationNextAttack()` averages over present clips only — unchanged current behaviour |
| Entity (enemy) attacks | Same model applies via `EntityWeaponMelee`; enemies read `AttackSpeed` from their own `StatsSO`. Enemy wiring is out of scope for this doc. ⚠️ Note the enemy stat store is `EntityStatsSO` (base/modifier/amount floats), **not** the `StatsSO` this design assumes — the two stat systems are unreconciled, so enemy attack speed needs a decision first |

---

## Dependencies

| System | Relationship |
|--------|-------------|
| **Stat System** (`StatsSO`, `StatType`, `DerivedStatFormula`) | Supplies layer 2. `AttackSpeed` must be authored as a percentage-group stat with `perLevel = 0` |
| **Weapons System** (`Weapon`, `MeleeWeapon`, `RangeWeapon`, `WeaponStats`, `AttackSO`) | Supplies layers 1 and 3, and owns the single application point in `Weapon.OnAttackEnter()` |
| **Animation System** (`AnimatorOverrideController`, `AnimationEventManager`) | `Animator.speed` is the applied output. Hit frames fire as animation events and therefore scale for free |
| **Character System** (`PlayerAttackState`, `Weapon.CanChain()`) | Consumes `chainWindow` (formerly `deplayTime`) as the combo gate; requires no change |
| **Per-Run Upgrades** | Attack-speed cards add `StatModifier`s through the existing `StatsSO` API |
| **HUD** | May display `effectiveAPS`; subscribes to `StatsSO.OnStatChanged` |

**Blocking prerequisite — ✅ CLEARED 2026-08-20.** This section previously read "the player
currently has no `StatsSO`". That is no longer true: `Player.cs:21` holds
`[SerializeField] private StatsSO stats` with a public `Stats` property, and
`Weapon.Equid()` already applies the equipped weapon's `StatModifierGroup` to it
(`Weapon.cs:83`), removing it by source on unequip (`Weapon.cs:96`). Layer 2 can be wired
today by reading `player.Stats.GetStatValue(StatType.AttackSpeed)` — no new
`PlayerStats : CoreCompoment` component is required, though one is still a reasonable
refactor if the Core hub is preferred as the access point. `PlayerData` remains a separate
SO holding only `maxHealth` / `currentHealth` / `movementVelocities`.

---

## Tuning Knobs

| Knob | Location | Meaning | Suggested range |
|------|----------|---------|-----------------|
| `baseAttackSpeed` | `WeaponStats` (new field) | Weapon identity — the speed baseline | 0.6 – 1.5 |
| `AttackSpeed` | `StatsSO` derived stat | Player's percentage bonus | 0 – 200 (%) |
| `attackRate` | `AttackSO` (exists, currently unread — confirmed 2026-08-20) | Per-stage rhythm nudge | 0.7 – 1.2 |
| `MIN_SPEED_MULT` | `GameConstants` | Floor — slow debuffs cannot freeze the player | 0.5 |
| `MAX_SPEED_MULT` | `GameConstants` | Ceiling — protects animation legibility | 3.0 |
| `comboGrace` | `Weapon` (there is no such field yet — `chainWindow` is the raw clip duration) | Input forgiveness added to the combo window | 0.1 – 0.3s |

Per-entity `AttackSpeed` coefficients belong in
`ToolExcel/stat_system_formula_reference.xlsx`, consistent with
[stat-system.md](stat-system.md) — not in this document.

---

## Implementation Map

> **Retargeted 2026-08-20.** This section previously pointed at `WeaponMelee.SetAnimation()`.
> That file and method no longer exist — the weapon layer was rewritten and the stage machine
> moved up into the abstract `Weapon` base. **The design itself is unaffected**: the single
> application point survived the refactor intact, and the `/ Anim.speed` division this design
> depends on is still there. Only the file and line targets changed.

The architecture still anticipates this change. `Weapon.OnAttackEnter(Player)` reads:

```csharp
currentStage = stats.GetStage(CurrentStageIndex);
player.Anim.speed = 1f;                                                  // ← replace with speedMult
chainWindow = Utility.DurationNextAttack(
    Utility.GetOverrideClips(currentStage.directionAttackAnimatorOV, "Attack")) / player.Anim.speed;
player.Anim.runtimeAnimatorController = currentStage.directionAttackAnimatorOV;
```

The division by `Anim.speed` is already in place, and `chainWindow` is what the old design
called `deplayTime`. Only the hardcoded `1f` becomes the computed multiplier — and because
this is on the abstract base rather than the melee subclass, **ranged weapons get attack
speed for free**, which the original design did not account for. Decide whether that is
intended before implementing.

| File | Change |
|------|--------|
| [`Weapon.cs:49`](../../Assets/Script/Weapons/Weapon.cs) | Replace `player.Anim.speed = 1f` with the resolved `speedMult` (was `WeaponMelee.cs:72`) |
| [`WeaponStats.cs`](../../Assets/Script/Weapons/WeaponStats.cs) | Add `[SerializeField] protected float baseAttackSpeed = 1f` + property. It sits on the shared base, so it applies to melee and ranged alike |
| [`AttackSO.cs:11`](../../Assets/Script/Weapons/MeleeWeapon/AttackSO.cs) | `attackRate` gains its layer-3 meaning. It already carries `[Range(0.1f, 10f)]`; tighten to `[Range(0.1f, 2f)]` per the tuning table below |
| [`StatType.cs:25`](../../Assets/Script/StatSystem/StatType.cs) | Correct the comment from "số đòn / giây" to "% attack speed bonus" |
| [`GameConstants.cs`](../../Assets/Script/Utility/GameConstants.cs) | Add `MIN_SPEED_MULT` / `MAX_SPEED_MULT` under `SettingStats` |
| `Player.Stats` | Already exists (`Player.cs:21`) — read layer 2 through `player.Stats.GetStatValue(StatType.AttackSpeed)`. No new component required |

⚠️ Blocker for the stat half: `StatsSO.RecalculateDerived()` currently skips its update when
**any one** of four values already matches, because the guard uses `||` where it needs `&&`
(`StatsSO.cs:272`). Until that is fixed, `AttackSpeed` will not recalculate on level-up or
point allocation unless `isDevMode` is on (TD-038).

Existing helpers to reuse — do not reimplement:
[`Utility.DurationNextAttack()`](../../Assets/Script/Utility/Utility.cs),
[`Utility.GetOverrideClips()`](../../Assets/Script/Utility/Utility.cs),
[`StatsSO.GetStatValue()`](../../Assets/Script/StatSystem/StatsSO.cs).

---

## Acceptance Criteria

- [ ] `speedMult` resolves as `weaponSpeed × statMult × stageRate`, clamped to `[0.5, 3.0]`
- [ ] With `AttackSpeed = 0`, the full 3-hit combo takes the same wall-clock time as before this system existed (no regression)
- [ ] With `AttackSpeed = 100`, the full 3-hit combo completes in approximately half that time
- [ ] The combo's internal rhythm is preserved at every speed — the heavy finisher remains proportionally the longest stage
- [ ] No attack animation is ever truncated or clipped at any multiplier within the clamp range
- [ ] The hit frame lands correctly at every multiplier — damage is applied exactly once per stage
- [ ] Equipping a faster weapon raises attack speed without any change to the stat value
- [ ] A stat change mid-combo takes effect from the next stage, with no visual snap on the playing clip
- [ ] EditMode unit test: `ResolveSpeedMultiplier_WithStatAndWeapon_ReturnsClampedProduct()`
- [ ] EditMode unit test: `ResolveSpeedMultiplier_ExceedsMaximum_ClampsToMaxSpeedMult()`
- [ ] Playtest sign-off that fast attacks still read clearly and the combo feels fluid rather than stuttery
