# Archer / Mage Abilities — Task Log (2026-11-02)

> Branch `sprint-21`. Single log for the two-phase task "design (GDD) then create assets for the
> 4 Archer and 4 Mage abilities". **State: Phase 1 approved by the owner (2026-11-02), Phase 2 done.**
>
> File rules for this task: create only; never modify, delete, rename or move an existing file;
> no new `.cs`; no push. Local commits on `sprint-21` allowed by the owner (chat, 2026-11-02),
> which overrides "no commit" in the task brief.
>
> **Evidence tier: STATIC.** Everything below was read from code and asset YAML. Nothing was
> compiled or run in Play Mode. Code was read **as it stands in the working tree**, which carries
> the owner's uncommitted edits to `AbilityHolderBase.cs`, `EffectReceiverBase.cs`,
> `IEffectReceiver.cs`, `BuffDebuffStatsForDuration.cs`, `RecoveryReductionPerTimeForDuration.cs`,
> `DamagesInManyPos.cs`, `Player.cs`, `PlayerSkillWeaponState.cs` and others (see §6). Line numbers
> refer to those working-tree versions.

---

## 1. Summary

| Ability | Slot | Class | Status | One-line reason |
|---|---|---|---|---|
| **Kick-back Trap** | Secondary | Archer | **PENDING** | No dash/hop, no placed trap with a trigger, no root, no knockback |
| **Volley** | Utility | Archer | **PENDING** | No fan of projectiles (every projectile flies along `context.Forward`); no slow on enemies |
| **Frost Nova** | Utility | Mage | **PENDING** | No caster-centred area; no freeze/chill/shatter; enemies cannot receive status effects |
| Hawk Eye | Primary | Archer | PARTIAL | Hold → one projectile with flat damage; no charge, distance, pierce, armour pen or stat scaling |
| Rain Arrows | Ultimate | Archer | PARTIAL | Telegraph + one area strike at the cursor (Lightning); no 3 s waves, no slow; scales on `MagicDamage` |
| Fireball | Primary | Mage | PARTIAL ⚠️ | Free single-target projectile; no explosion, no Burn. **Runtime-blocked for Mage, see W1** |
| Chain Lightning | Secondary | Mage | PARTIAL ⚠️ | Area strike at the cursor, ≤ 4 targets at full damage; no auto-target, chain, falloff or stun. **W1** |
| Meteor | Ultimate | Mage | PARTIAL ⚠️ | Telegraph + strike at the cursor, damage matches the GDD formula; no stun, knockback or burning ground. **W1** |

READY: none.

## 2. Pending & Warnings

### Warnings (read first)

**W1 — Mage gets no abilities at runtime (blocker, BUG-107 item 2).** `Mage.asset` has
`<Weapon>k__BackingField: {fileID: 0}`. `Player.SetCharacterData()` calls
`weaponHolder.EquipDefaultWeapon()` (`Player.cs:93`) **before** `abilityHolder.SetAbility()`
(`Player.cs:95`). `EquipDefaultWeapon()` evaluates `Core.Data?.Weapon.gameObject`
(`WeaponHolderBase.cs:52`) with a null `Weapon`, which throws, so `SetAbility()` never runs and the
Mage has no abilities (old shells or new assets). Fix options: guard `Data.Weapon == null` in
`EquipDefaultWeapon()` (the BUG-107 item 2 fix), or call `SetAbility()` first. Code change — not
done here (no `.cs` edits allowed). STATIC; confirm in Play Mode.

**W2 — Ability damage ignores the GDD stat scaling.** `SpawnProjectileEffect.Execute()` deals
`baseDamage` only (`SpawnProjectileEffect.cs:27`); `SpawnSummonEffect.SummonExecute()` deals
`baseDamage + MagicDamage` with the coefficient fixed at 1.0 and the stat fixed to `MagicDamage`
(`SpawnSummonEffect.cs:37-40`, comment "need fix to flexible stat type"). So every PARTIAL asset is
tuned to match the GDD at **level 1** and drifts as the player levels (§7).

**W3 — Enemies cannot receive any status or stat effect.** No enemy prefab carries
`EntityEffectReceiver` (script GUID `25aba943df6c9f54ab70a3754242879b` is referenced by **0**
prefabs), so `AbilityContext.TryGetRecipientComponent<IEffectReceiver>(Target)` returns false and
any on-hit `SubEffect` aimed at an enemy is a silent no-op. Enemy movement also never reads the
`MoveSpeed` stat (only `PlayerMoveState.cs:21` does), so even a working stat debuff could not slow
an enemy. This alone blocks slow, root, freeze, chill and Burn.

**W4 — Q1 (20% damage floor) is decided but not implemented.** Mitigation is still
`max(0, raw − Defense)` (`EntityNegativeReciver.DamageCalculate`). Balance numbers in the GDD for the
floor model do not apply until that code changes.

**W5 — Phase timing comes from the animation clips**, which are 2.3–5.0 s long (Phase 1 W2). Cost is
paid at `ActivateInstant()` (Start's `AnimationOnAction`, `AbilityInstance.cs:44-48`) and cooldown
starts in `DoInstant()` (`AbilityInstance.cs:58-63`) — not at Cast and at the end of Exit as the GDD
says. Every ability will feel much slower than designed until the clips are re-timed (GDD Q7).

**W6 — All three summon-based abilities share one visual** (RuneCircle telegraph + Lightning strike,
the Paladin Consecrate prefabs), and both projectile abilities use the Paladin `Slash-VFX-GO`. No
arrow, fireball or meteor visual exists that carries a `SpawnMono` component. `Arrow.prefab` has
only `ProjectileBody` (weapon arrow) and cannot be used by `SpawnProjectileEffect`.

**W7 — Existing shells left untouched (create-only rule, Q-L1).** New assets carry a ` (GDD)`
suffix in their own folders. `Archer.asset` / `Mage.asset` still bind the old empty shells.

### Pending abilities

#### P1 — Kick-back Trap (Archer, Secondary)

- **Missing:** (a) a caster displacement (hop/dash) — no effect moves the caster; `IAbilityOwner`
  (`IAbilityOwner.cs:4-13`) exposes no movement; (b) a placed object that waits and triggers on
  overlap — `SpawnSummonBase` fires only from an Animation Event (`SpawnSummonBase.cs:11-15`),
  `DamageArea.Execute()` checks once (`DamageArea.cs:22-31`); (c) root and knockback on enemies —
  no status system, and W3.
- **Why not:** the core of the ability is the escape. Without displacement there is nothing to
  approximate; a damage area at the feet would be a different ability.
- **Minimal addition:** `DisplaceCasterEffect : AbilityEffectDefinition` (fields: `distance`,
  `duration`, `awayFromAim`, `invulnerable`) driving the player through a new `IMovement`/dash
  method; `TrapSummon : SpawnSummonBase` (fields: `armDelay`, `triggerRadius`, `lifetime`,
  `maxAlive`) polling `OverlapCircleNonAlloc` until triggered; a status-effect component on enemies
  (`IStatusReceiver` with Root / Knockback). **Effort:** L (3–5 d). **ADR:** yes — status effects and
  caster displacement are new cross-system contracts (GDD Q6).
- **Fix first:** W3 (enemy receiver); BUG-106 is not blocking for the player.
- **Temporary option:** none worth shipping. Re-use the Paladin `Dash Ability` (v1) only if the
  owner accepts a v1 dependency.

#### P2 — Volley (Archer, Utility)

- **Missing:** (a) angular spread — `SpawnProjectileEffect.Angle()` always uses `context.Forward`
  (`SpawnProjectileEffect.cs:14-21`) and `SpawnProjectileBase.Launch()` launches along
  `context.Forward` (`SpawnProjectileBase.cs:33`), so five effects stack into one line; (b) slow on
  hit — W3.
- **Why not:** the fan is the identity (shotgun close, net at range). Five overlapping projectiles
  is a single heavy shot that duplicates Hawk Eye.
- **Minimal addition:** `SpawnFanProjectileEffect : SpawnProjectileEffect` with `count`,
  `spreadAngle`, spawning `count` projectiles with per-index angle offsets and passing the rotated
  direction into `Launch` (needs `SpawnProjectileBase.Launch` to accept a direction instead of
  reading `context.Forward`). Slow needs the status component of P1. **Effort:** S for the fan
  (0.5–1 d), slow shares P1's cost. **ADR:** no for the fan; yes for slow (with P1).
- **Fix first:** W2 (stat scaling) if Volley should scale with `PhysicalDamage`.
- **Temporary option:** none (see "why not").

#### P3 — Frost Nova (Mage, Utility)

- **Missing:** (a) an area centred on the caster — `SpawnSummonEffect.Apply()` always spawns at
  `context.TargetPoint` = cursor (`SpawnSummonEffect.cs:16-21`); (b) freeze / chill / shatter — W3
  and no status system; (c) Tank/Boss conversion rules.
- **Why not:** Frost Nova is the Mage's defensive panic button. A damage strike at the cursor gives
  neither the defence nor the setup for Meteor.
- **Minimal addition:** a `spawnAtCaster` flag on `SpawnSummonEffect` (or
  `SpawnSelfAreaEffect : SpawnSummonEffect` overriding the spawn position) plus a `Freeze` status
  (shares P1's `IStatusReceiver`) and a damage-taken hook for Shatter in `NegativeReceiverBase`.
  **Effort:** M (2–3 d) on top of the status component. **ADR:** yes (status effects).
- **Fix first:** W3, W1.
- **Temporary option:** none worth shipping.

---

## 3. Phase 1 summary

- **GDD:** [`design/gdd/class-abilities-archer-mage.md`](../../design/gdd/class-abilities-archer-mage.md)
  (commit `ae0d6e82`, decisions recorded in a later commit).
- **Owner decisions (2026-11-02, "Duyệt theo đề xuất"):**
  - Q1 — 20% damage floor (not implemented, W4).
  - Q2 — Fireball is the Mage's free basic attack.
  - Q3 — Piercing Shot merges into Hawk Eye; Hawk Eye uses the Piercing Shot clips.
  - Q5 — abilities never crit.
  - Q-L1 — no explicit answer; applied the create-only default: new assets with a ` (GDD)` suffix.
- **Process notes:** `/design-system` run non-interactively (owner reviews the whole GDD);
  `review_mode: lean`, so `creative-director`, `art-director`, `qa-lead` were not consulted. Steps
  that write other files were skipped by the create-only rule: `production/session-state/active.md`,
  `design/registry/entities.yaml`, `design/gdd/systems-index.md`, dependency back-links in
  `stat-system.md`, `character-system.md`, `animation-system.md`.

### Paladin baseline (as authored)

| Ability | ActivationType | Cooldown | Costs | MaxHoldTime | Effect chain | Numbers |
|---|---|---|---|---|---|---|
| Blessed Slash | Hold (1) | 1 | 5 Mana | 0 | Spawn Slash projectile | `baseDamage` 30, SpawnOffset 0.8, Lifetime 8 |
| Consecrate | Active (0) | 1 | 15 Mana | 0 | Spawn Summon (RuneCircle → Lightning) | `baseDamage` 30 + `MagicDamage` |
| Blessing | Active (0) | 1 | 10 Mana | 0 | Buff/Debuff for duration + Recovery/Reduction (empty) | `PhysicalDamage` +2 PercentAdd, STR +20 Flat, 3 s — stored under the old `modifiers` key, so it loads empty (BUG-121) |
| Avatar of Light | Active (0) | 1 | 40 HP + 50 Mana | 0 | Gain For Time + Recovery (empty) | `MagicDamage` +100000 Flat, 20 s (test value) |

### Balance-check result

**CRITICAL ISSUES** under the live flat mitigation (Archer effectively cannot kill the authored
Boss); **CONCERNS** under the decided 20% floor.

| Class | L1 DPS | Δ vs Paladin mid 61.5 | L10 DPS | Δ vs Paladin mid 83.1 | Burst 5 s L1 / L10 | Dmg/Mana L1 / L10 |
|---|---|---|---|---|---|---|
| PaladinBasic | 64.3 | — | 90.6 | — | 351 / 494 | — |
| PaladinRef | 58.8 | — | 75.6 | — | 229 / 298 | 25.2 / 26.7 |
| Archer | 57.6 | −6% | 85.0 | +2% | 392 / 569 | 25.2 / 30.4 |
| Mage | 54.9 | −11% | 92.2 | +11% | 442 / 691 | 24.9 / 13.2 |

| Priority | Issue | Suggested fix |
|---|---|---|
| P1 | Flat Defense zeroes small hits (Archer vs Boss 2384 s at L1) | 20% floor — decided (W4) |
| P2 | Mage scaling steepest (−11% → +11%) | Keep `Scale` ≤ 0.5; re-check at L15/L20 |
| P3 | Ultimate opener burst +12–40% over PaladinBasic | Accept; watch pack rooms |
| P4 | Archer kites melee forever (MoveSpeed 3.2 > 2.79) | Room composition |
| P5 | Paladin abilities DPS-negative as authored | Paladin pass |
| P6 | Frost Nova ~38% control uptime | Cap CDR on it at 30% |

These are the **design** numbers. The temporary assets do not reproduce them (§7).

### Design-review result

Independent reviewer: **NEEDS REVISION**, 7 blocking issues. Six fixed in the GDD the same day
(Freeze 2.0 s for the Nova→Meteor combo; Shatter on direct hits only; one cooldown model; Kick-back
Trap triggers on an enemy already on it; Hawk Eye distance + AC 95.0 + max 132.9; balance AC
re-scoped to a simulation acceptance). The seventh (Q1/Q2/Q3/Q5) closed by the owner's decision.
Not re-reviewed after the fixes.

---

## 4. Capability matrix + bug re-check

### Effects (`Assets/Script/System/Abilities/Effects/`)

| Effect | What it does | Parameters | Works today? | Prefab |
|---|---|---|---|---|
| `SpawnProjectileEffect` | Spawns one projectile at `Origin + Forward × SpawnOffset`, flies along `Forward`; on hit `TakeDamage(baseDamage)` then runs `SubEffects` on the target | `Prefab`, `SpawnOffset`, `Lifetime`, `baseDamage` | Yes (Paladin Blessed Slash). Flat damage only; stops at first hit | `Slash-VFX-GO.prefab` (speed 15, targetMask layer 6, blockMask layer 10) |
| `SpawnSummonEffect` | Cast phase: spawns `Prefab` at the cursor (telegraph). Do phase: spawns `summonPrefab` at the cursor; each target its `Execute()` reports takes `baseDamage + MagicDamage`, then `SubEffects` | `Prefab`, `summonPrefab`, `Lifetime`, `baseDamage` | Yes (Consecrate) | `RuneCircle.prefab` (telegraph, ~1 s), `Lightning.prefab` (`DamageArea`, radius 2, max 4 targets, layer 6) |
| `RecoveryReductionStatsEffect` | Instant HP/Mana change via `IEffectReceiver.AdjustCurrentStats` | `statModifier.vitalModifiers` | **No** — `VitalModifier` is not `[Serializable]` (BUG-117), list never authored | — |
| `RecoveryReductionPerTimeForDuration` | HoT/DoT | `perTime`, `timeCount` | **No** — throws (BUG-118) | — |
| `BuffDebuffStatsForDuration` | Timed max-stat modifiers via `AdjustMaxForDuration` | `statModifier.statsModifiers`, `duration` | Player only (needs `sourceEffect` on the receiver; set on `PlayerTest.prefab`). Enemies: no receiver (W3) | `SourceEffect.prefab` |

### Runtime (`Runtime/`)

| Runtime | Behaviour | Gaps |
|---|---|---|
| `SpawnMono` | Launch / timed despawn via pool | — |
| `SpawnProjectileBase` + `ProjectileBody` | Pooled 2D trigger projectile, target/block masks | `pierceCount` declared, never read; body releases on first hit (`ProjectileBody.cs:61-63`) |
| `SpawnSummonBase` | Fires callback from an Animation Event | One shot per event |
| `DamageArea` | Overlap circle at its position, callback per target | Radius / max targets per prefab |
| `CircleSummon` | Telegraph, no damage | — |
| `DamagesInManyPos` (RainArrows.prefab) | Multi-point arrow waves from 26–29 animation events per direction | **Broken** (BUG-109); flat `damagePerArrow` 10, ignores the effect |
| `SourceEffect` | Identity object for timed buffs | Body is empty |

### Core

- Activation: `Active`, `Hold` (release ends hold), `Passive` (unused).
- Costs: ability-level `List<StatCost>` (`VitalStatType`), gated by `TryStart()`, paid at
  `ActivateInstant()`. Per-effect `Costs` exist (BUG-089 design) — **not used** by any new asset.
- Context: `Origin`, `Forward`, `DirectionIndex`, `TargetPoint` (cursor, **no range clamp**),
  `HoldTime`/`HoldRatio` (read by no effect), `Target` (hit collider).

### Bug re-check (working tree, 2026-11-02)

| Bug | Status in code | Evidence |
|---|---|---|
| BUG-118 HoT/DoT throws | **Still present** | `RecoveryReductionPerTimeForDuration.cs:11` → `EffectReceiverBase.AdjustMaxPerTimeForDuration()` `throw new NotImplementedException()` (`EffectReceiverBase.cs:94-97`) |
| BUG-083 `HoldRatio` | **Still present** | `BuildContext()` runs only from `CanStart()` (`AbilityInstance.cs:95`), before `StartHold()` zeroes the timer; it returns the previous hold. No effect reads `HoldRatio` |
| BUG-072 `layerMask` = 0 | **Fixed in data** | `Lightning.prefab:123-125` `m_Bits: 64` |
| BUG-109 Rain Arrows | **Still present** | `DamagesInManyPos.OnEnable()` reads `_context.DirectionIndex` before `Launch()` (`:26-29`); `OnSkillEnd()` → `Destroy(gameObject)` on a pooled object (`:74`); damage is the prefab's `damagePerArrow` 10 |
| BUG-117 `VitalModifier` | **Still present** | `VitalModifier.cs:4` has no `[Serializable]`; negative values route to `Reduction(negative)` = a heal (`EffectReceiverBase.cs:44-50`) |
| BUG-121 `StatModifierGroup` rename | **Still present** | `statsModifiers` (`StatModifierGroup.cs:24`) has no `[FormerlySerializedAs]`; Paladin Blessed Slash recovery (`authoredModifiers`) and Blessing (`modifiers`) load empty |
| BUG-089 per-effect `Costs` | By design, unused | No new asset sets effect `Costs` |
| BUG-100 buff expiry strips item buffs | **Looks fixed in the uncommitted working tree** | Each timed buff uses its own spawned `SourceEffect` as source (`EffectReceiverBase.cs:81-87`). Depends on `sourceEffect` being assigned; uncommitted |
| BUG-119 heal above max | **Still present** | `VitalStatsBase.Recovery()` adds without clamping after the add (`:60-73`) |
| BUG-106 setup only from `SetCharacterData` | **Still present** | `SetAbility()` called only at `Player.cs:95`; enemies never get abilities. Not blocking for player classes |
| BUG-107 item 2 (null weapon) | **Still present — blocks Mage (W1)** | `WeaponHolderBase.cs:52` |
| `pierceCount` unused | **Still present** | `SpawnProjectileBase.cs:11` declared, never read |

### Mechanisms that do not exist

Dash/blink (caster displacement), knockback, slow (on enemies), stun, root, freeze, pierce, chain,
homing, fan/spread, mark, shield, summon with AI, caster-centred area, on-hit explosion at the hit
point, true damage (Defense bypass), per-ability stat scaling coefficient, range clamp for
cursor-targeted abilities.

---

## 5. Behavior → system mapping

Legend: ✅ met · 🟡 approximated · ❌ not possible.

### Hawk Eye — PARTIAL

| GDD item | Status | Mapping |
|---|---|---|
| Identity / slot | ✅ | Primary, uses the Piercing Shot override controller (Q3) |
| Activation Hold, release fires | ✅ | `ActivationType: 1`, `MaxHoldTime: 1.2`; release = `CancelHold` path as Blessed Slash |
| Move at 50% / rotate while drawing | ❌ | No per-ability move-speed rule |
| Dash cancels draw | ❌ | No cancel path in `AbilityInstance` |
| Timeline / cost on release | 🟡 | Cost at Start, cooldown at Do (W5) |
| Targeting along aim, spawn 0.8 u, 11 u | ✅ | `SpawnOffset 0.8`, `Lifetime 0.73` × speed 15 ≈ 11 u |
| Hitbox / projectile speed 20 | 🟡 | Prefab speed 15 (fixed per prefab) |
| Charge scaling, distance mult | ❌ | No effect reads hold time or travel distance |
| Pierce on full charge | ❌ | `pierceCount` unused |
| Armour penetration | ❌ | No penetration in `TakeDamage` |
| Damage scaling with `PhysicalDamage` | ❌ | Flat `baseDamage` 84 (= GDD full charge at 5 u, L1) |
| Feedback (charge line, flash) | ❌ | Slash VFX only (W6) |

### Rain Arrows — PARTIAL

| GDD item | Status | Mapping |
|---|---|---|
| Activation / cost 50 / CD 35 | ✅ | Active, Mana 50, Cooldown 35 |
| Ground target at cursor | 🟡 | `TargetPoint` = cursor, **no 8 u clamp** |
| Telegraph 0.6 s | 🟡 | RuneCircle at Cast (~1 s clip) |
| 8 waves over 3 s, radius 2.5, 16 targets | 🟡 | One Lightning strike, radius 2, ≤ 4 targets |
| Slow 20% in zone | ❌ | W3 |
| Free to move during rain | 🟡 | Not applicable (single strike) |
| Damage `10 + 0.25 × PhysicalDamage` per wave | 🟡 | `63 + MagicDamage` once = 92.2 at L1 (GDD expected total 92.6) |
| Real Rain Arrows visual (`RainArrows.prefab`) | ❌ | BUG-109 |
| Animation | ✅ | Archer Volley override (`erika_attack_up45`, upward shot) |

### Fireball — PARTIAL (runtime-blocked, W1)

| GDD item | Status | Mapping |
|---|---|---|
| Free, CD 0.6 | ✅ | `Costs: []`, Cooldown 0.6 |
| Auto-repeat while held | ❌ | No repeat input |
| Move at 70% while casting | ❌ | — |
| Projectile, range 9 u | 🟡 | Slash VFX, speed 15, Lifetime 0.6 (≈ 9 u), spawn 0.7 u |
| Explosion radius 1.4 u on hit/wall/range end | ❌ | Projectile stops on first hit, no splash |
| Burn 3 s True damage | ❌ | BUG-118 + W3 |
| Damage `44 + 0.2 × MagicDamage` | 🟡 | Flat 56 = 49.8 impact + 5.8 Burn equivalent per cycle (L1) |

### Chain Lightning — PARTIAL (runtime-blocked, W1)

| GDD item | Status | Mapping |
|---|---|---|
| Cost 22 / CD 4 | ✅ | |
| Auto-target nearest in 60° cone, 7 u; fizzle if none | ❌ | Fires at the cursor, always pays |
| 4 targets, ×0.8 falloff, no repeat | 🟡 | `DamageArea` hits ≤ 4 enemies within 2 u of the cursor, all at full damage |
| Stun 0.2 s | ❌ | W3 |
| Damage `40 + 0.5 × MagicDamage` | 🟡 | `25 + MagicDamage` = 54.2 at L1 (GDD 54.6) |
| Beam visual | 🟡 | Lightning strike (fits the theme) |

### Meteor — PARTIAL (runtime-blocked, W1)

| GDD item | Status | Mapping |
|---|---|---|
| Cost 80 / CD 40 | ✅ | |
| Cursor target, 9 u clamp | 🟡 | No clamp |
| Telegraph 0.9 s | 🟡 | RuneCircle (~1 s) |
| Impact `110 + 1.0 × MagicDamage` | ✅ | Exactly `baseDamage 110` + `MagicDamage` |
| Radius 3, 16 targets | 🟡 | Radius 2, ≤ 4 |
| Stun 1 s, knockback 1.5 u | ❌ | W3 |
| Burning ground 3 s True damage | ❌ | BUG-118 + W3 |
| Big impact feedback | 🟡 | Lightning strike visual |

### Kick-back Trap, Volley, Frost Nova — PENDING

Every row of their behaviour specs maps to ❌ except cost/cooldown, which the ability definition
could carry. Details and the minimal additions are in §2.

---

## 6. Created files

All GUIDs were generated new, checked against every `.meta` under `Assets/` (0 duplicates after
creation), and every GUID referenced by the new assets resolves to an existing file
(script, prefab, override controller or the new effect asset). Prefab `fileID`s were checked against
the component anchors in the prefab YAML (`Slash-VFX-GO.prefab:153`, `RuneCircle.prefab:110`,
`Lightning.prefab:110`).

| Path | Kind | GUID |
|---|---|---|
| `Assets/SO/Skill/Archer/Ability/Hawk Eye (GDD)/` | folder | `10e99b6849df48dd998759e11bc72093` |
| `…/Hawk Eye (GDD)/Effect/` | folder | `54689eb52d97445ebc5ae3aa14f7de80` |
| `…/Hawk Eye (GDD)/Effect/Archer Hawk Eye Projectile Effect.asset` | `SpawnProjectileEffect` | `4a1e0734ea5d4f519853b22e57458b45` |
| `…/Hawk Eye (GDD)/Hawk Eye (GDD).asset` | `AbilityDefinition` | `1b1a9b36bae642169ab9f0f8f83ef674` |
| `Assets/SO/Skill/Archer/Ability/Rain Arrows (GDD)/` | folder | `7ba4ab5a77674bbe8789e082f9b9b8a1` |
| `…/Rain Arrows (GDD)/Effect/` | folder | `13fdb4033f994f01b0410806721b697b` |
| `…/Rain Arrows (GDD)/Effect/Archer Rain Arrows Summon Effect.asset` | `SpawnSummonEffect` | `3299a941309649a88559ac421949c4fd` |
| `…/Rain Arrows (GDD)/Rain Arrows (GDD).asset` | `AbilityDefinition` | `324e73f00f3c40969e4ab775196620db` |
| `Assets/SO/Skill/Mage/Ability/Fireball (GDD)/` | folder | `e667eacc544b408ba96c9c3b35ff5e9e` |
| `…/Fireball (GDD)/Effect/` | folder | `9679395533e74362852c93140d988792` |
| `…/Fireball (GDD)/Effect/Mage Fireball Projectile Effect.asset` | `SpawnProjectileEffect` | `9402e0ab00044fd0b1d9397190a82e7f` |
| `…/Fireball (GDD)/Fireball (GDD).asset` | `AbilityDefinition` | `5d4ed16008844f538911ee974b861525` |
| `Assets/SO/Skill/Mage/Ability/Chain Lightning (GDD)/` | folder | `f553f6b94a194decb333845c732cb814` |
| `…/Chain Lightning (GDD)/Effect/` | folder | `88f1cff35ea14caabd10db5fab574718` |
| `…/Chain Lightning (GDD)/Effect/Mage Chain Lightning Summon Effect.asset` | `SpawnSummonEffect` | `669f25f3122e4b71a457767f13575503` |
| `…/Chain Lightning (GDD)/Chain Lightning (GDD).asset` | `AbilityDefinition` | `5c1882097e2b488ebcb5cd22936e1c83` |
| `Assets/SO/Skill/Mage/Ability/Meteor (GDD)/` | folder | `104f05adc7f64eeb88bd1d4d59846b99` |
| `…/Meteor (GDD)/Effect/` | folder | `2213996587b446ef85d4937628bf0c18` |
| `…/Meteor (GDD)/Effect/Mage Meteor Summon Effect.asset` | `SpawnSummonEffect` | `78bfa77a164c4cf98ed0aeba49be3382` |
| `…/Meteor (GDD)/Meteor (GDD).asset` | `AbilityDefinition` | `547df0f6d5d14b74823a372eedf659b8` |
| `design/gdd/class-abilities-archer-mage.md` | GDD | — |
| `production/qa/archer-mage-abilities-log-2026-11-02.md` | this log | — |

Each `.asset` and folder above has a `.meta` holding the GUID shown. No prefab was created.

References used (existing, read-only): `AbilityDefinition.cs` `39d6bd5f…`,
`SpawnProjectileEffect.cs` `2bff5def…`, `SpawnSummonEffect.cs` `e3551c5d…`, `Slash-VFX-GO.prefab`
`5f61048e…`, `RuneCircle.prefab` `d34555be…`, `Lightning.prefab` `f61a428b…`, override controllers
Archer Piercing Shot `27d76017…`, Archer Volley `8b72a884…`, Mage Fireball `9639b7b7…`, Mage Chain
Lightning `1d2c8cf5…`, Mage Meteor `44ba8533…`.

**Owner's pre-existing uncommitted changes (`M` at session start, untouched):**
`Paladin_PowerUp_State2_dir 0.anim`, `AbilityHolderBase.cs`, `CharacterData.cs`,
`EffectReceiverBase.cs`, `StatHandlerBase.cs`, `EntityStatsHandler.cs`, `RegenComponent.cs`,
`StatHandler.cs`, `Player.cs`, `PlayerSkillWeaponState.cs`, `IEffectReceiver.cs`,
`BuffDebuffStatsForDuration.cs`, `RecoveryReductionPerTimeForDuration.cs`, `DamagesInManyPos.cs`,
`BaseStatsSO.cs`.

---

## 7. GDD deviations

| Ability | Field | GDD | Asset | Why |
|---|---|---|---|---|
| All | Cost timing | paid at Cast (Hawk Eye: on release) | paid at Start's OnAction | `AbilityInstance.ActivateInstant()` |
| All | Cooldown start | when Exit ends | at Do | `AbilityInstance.DoInstant()` |
| All | Mitigation | 20% floor (Q1) | flat Defense | W4 |
| Hawk Eye | Damage | `(25 + 0.5 P) × (1 + 0.8 c) × dist` | flat 84 | No scaling (W2). L1 full charge at 5 u = 84.0; L10 DEX GDD 117.6 → −29% |
| Hawk Eye | Speed | 20 u/s | 15 u/s (prefab) | Prefab-owned field; Lifetime 0.73 keeps ≈ 11 u |
| Hawk Eye | Pierce / pen / charge | yes | none | §4 |
| Rain Arrows | Shape | 8 waves × 3 s, r 2.5, 16 targets | 1 strike, r 2, 4 targets | Only working area runtime is `DamageArea` |
| Rain Arrows | Damage | `10 + 0.25 P` per wave | `63 + MagicDamage` once | Summon damage hard-wired to `MagicDamage` (W2). L1 92.2 vs 92.6; L10 103 vs 134.7 (−24%) |
| Rain Arrows | Animation | own clip | Archer Volley override | No Rain Arrows override for Archer exists |
| Fireball | Damage | `44 + 0.2 M` + Burn | flat 56 | Burn folded in (L1); L10 GDD ≈ 75 → −25% |
| Fireball | Speed / AoE | 11 u/s, r 1.4 explosion | 15 u/s, single target | Prefab-owned; no splash |
| Chain Lightning | Damage | `40 + 0.5 M`, ×0.8 per jump | `25 + M` to each of ≤ 4 | L1 54.2 vs 54.6 first target; L10 132.5 vs 93.75 (+41%); multi-target total +35% at L1 |
| Chain Lightning | Targeting | auto, cone, fizzle | cursor point | §5 |
| Meteor | Radius / targets | 3.0 / 16 | 2.0 / 4 | Lightning prefab |
| Meteor | Burning ground, stun | yes | none | §4 |
| All cursor abilities | Range clamp | 8–9 u | none | `TargetPoint` is unclamped |

Net effect on balance: at L1 the temporary Archer kit is close to the GDD; at L10 Hawk Eye and Rain
Arrows fall behind (no scaling) and Chain Lightning overshoots. Re-run the simulation once W2 is fixed.

---

## 8. Manual steps for owner

1. **Fix W1 first** (Mage null weapon) or none of the Mage abilities will equip.
2. In `Assets/SO/Player/Archer/Archer.asset` → `abilityBindings`: Primary → `Hawk Eye (GDD)`,
   Ultimate → `Rain Arrows (GDD)`. Leave Secondary / Utility on the old shells or empty (PENDING).
3. In `Assets/SO/Player/Mage/Mage.asset` → `abilityBindings`: Primary → `Fireball (GDD)`,
   Secondary → `Chain Lightning (GDD)`, Ultimate → `Meteor (GDD)`. Utility stays PENDING.
4. Open each new asset once in the Inspector so Unity re-serializes it (the `id` field is already
   filled; `OnValidate` will not overwrite it).
5. Play Mode smoke on the TrainingDummy (`FastTest/CombatTest`): cast each ability once, check the
   damage numbers in §7 at L1, and that the Hold release on Hawk Eye fires.
6. Animation: re-time the ability clips (2.3–5.0 s, GDD Q7); Hawk Eye now uses the Piercing Shot
   clips — check the Hold loop (State 2) holds while `1` is held.
7. VFX: arrow, fireball and meteor visuals with a `SpawnMono` component (W6).
8. Decide what to do with the old empty shells and the unreferenced
   `Rain Arrow/Effect/Archer Spawn Rain Arrows Effect.asset`.
9. Code backlog that turns PARTIAL into READY / unblocks PENDING (needs owner, no `.cs` here):
   per-effect stat type + coefficient (W2), `pierceCount`, charge/hold scaling (BUG-083),
   fan projectile, caster-centred area, enemy `EntityEffectReceiver` + status effects (W3, ADR),
   20% damage floor (W4), BUG-109, BUG-118, BUG-117.

## 9. Open questions

| # | Question |
|---|---|
| OQ1 | W1: guard the null weapon (BUG-107 item 2) or give the Mage a placeholder weapon? Q2 says Mage has no weapon, so the guard is the consistent fix. |
| OQ2 | Do you want stand-in assets for the three PENDING abilities anyway (e.g. Frost Nova as a Lightning strike at the cursor), or keep them empty until the status-effect ADR? |
| OQ3 | GDD Q4 (Archer arrow ignores `PhysicalDamage`), Q6 (status-effect GDD/ADR), Q7 (clip timing), Q8 (class stat profiles) are still open. |
| OQ4 | Keep the old empty shells, or will you delete them once the ` (GDD)` assets are bound? |
