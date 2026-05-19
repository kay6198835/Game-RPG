---
status: reverse-documented
source: Assets/Script/Skill_Ability/
date: 2026-05-19
verified-by: Kiet
---

# Skill & Ability System Design

> **Note**: Reverse-engineered from existing implementation. Captures current behaviour
> and clarified design intent. Sections marked **[GAP]** describe intended design not yet
> implemented. Sections marked **[BUG]** identify known defects.

**Status**: In Design

---

## Overview

The skill and ability system governs all player-activated special actions beyond basic
weapon attacks. Every active skill is a `ActivateSkill` ScriptableObject with a 5-phase
lifecycle (Enter → Activate → Cast → Do → Exit) driven per-frame by `AbilityHolder`.

Skills are bound to weapons: each weapon SO carries two skill slots (`abilityWeapon` for
RMB and `skillWeapon` for E key). Equipping a weapon automatically makes its skills
available.

The same system also handles passive upgrades via `InternalSkillSO` — talent nodes
awarded after clearing a room.

**Demo target:** Two active skills functional — **Slash** (E key, weapon skill) and
**Block** (RMB hold, weapon ability).

---

## Player Fantasy

Skills punctuate the rhythm of combat. The player builds a combo with their weapon, then
unleashes a skill at the right moment — Slash to hit a distant enemy before they close
range, or Block to absorb an incoming strike and counterattack. Skills reward timing and
positioning, not just button mashing.

Passive upgrades from room clears let the player shape their run: a Strength node makes
attacks hit harder, a Dexterity node makes Slash faster. Each run feels different because
the upgrade path diverges.

---

## Detailed Rules

### Active Skill Lifecycle

Every `ActivateSkill` SO executes through five phases, driven by `AbilityHolder.SetStateAbility()` each frame:

```
Enter(player)        — store player ref, ability setup
    ↓
Activate()           — record startCastTime; begin cast window
    ↓
Cast()  [loop]       — update currentTime; wait for "Do" trigger
    ↓ (player input or DoNonCast type)
Do()    [once]       — main effect fires; set canUseAbility = false
    ↓
Exit()               — cleanup; start cooldown
```

**Skill type determines Cast phase behaviour:**

| Type | Cast phase | Use case |
|------|-----------|----------|
| `DoCast` | Loops until player releases the input key | Hold-and-release skills (charge-up, hold Block) |
| `DoNonCast` | Skips immediately to Do | Instant skills (Dash) |

**Cooldown:** Each concrete ability's `Exit()` calls `SetCanUseAbility(true)` after its
cooldown delay (implemented per-skill using `ability.cooldownTime`). While
`canUseAbility == false`, the player cannot re-enter the skill from `PlayerBasicState`.

### Skill Inputs (Demo)

| Input | Slot | Skill | Type |
|-------|------|-------|------|
| E key (hold/release) | `skillWeapon` on weapon SO | **Slash** | `DoCast` |
| RMB (hold) | `abilityWeapon` on weapon SO | **Block** | `DoCast` |

Skills are weapon-bound — changing weapons changes the available skills.

### Slash Skill (E key) — **[IMPLEMENTED]**

- **Type**: `DoCast`
- **Cast phase**: Player holds E while `Cast()` loops — brief charge window
- **Do phase**: Instantiates the `slashPrefab` projectile at `player.position + DirectionMouseVector × 3 units`, facing mouse direction
- **Projectile travel**: `Projectile.SetVelocity(DirectionMouseVector × speedSlash)`
- **Hit detection**: Projectile uses raycast → `INegativeReceiver.TakeDamage()`; `Spell.cs` variant also applies `IEffectable.ApplyEffect()` for status effects
- **Direction**: Mouse-aimed, 8-directional quantization inherited from `InputHandler`

### Block Skill (RMB hold) — **[GAP — BlockAbility is a stub]**

- **Type**: `DoCast`
- **Intended behaviour**: While RMB is held, `Cast()` loops — player is in a blocking stance
- **On hit during block**: Incoming damage reduced by `WeaponMeleeStats.blockDamage`
- **On release**: `Do()` fires any counter-effect; `Exit()` removes the stance
- **Current state**: `BlockAbility.Cast()` exists but contains no logic. Damage reduction
  is not applied anywhere. Needs implementation before demo.

### Dash Skill (Space) — **[IMPLEMENTED — reserved for future slot]**

- **Type**: `DoNonCast`
- **Do phase**: `rb.velocity = DirectionMouseVector × dashingPower` (instantaneous impulse)
- **Animation**: `animator.speed = 2f` during Enter, reset to `1f` in Exit
- **Note**: Dash is currently unbound in the weapon slot system — it is triggered by
  Space bar directly in `PlayerInputHandle`. Not yet wired to `abilityWeapon`/`skillWeapon`.

### DualAbility — **[PLANNED — being redesigned]**

`DualAbility` is intended as a multi-hit AoE ability. Implementation is on hold pending
design revision. When shipped it will extend `ActivateSkill` with:
- Multi-hit detection via `Physics2D.OverlapBoxAll`
- Complex animator state coordination
- Possibly a resource cost (mana or a per-run charge)

### Effect System

Skills can attach status effects to hit targets via `IEffectable.ApplyEffect()` (used by
`Spell.cs` projectiles). Effects are `EffectSkillSO` ScriptableObjects composed of
`EffectData` entries — each modifying one stat.

**Two effect types:**

| Type | Behaviour | Use case |
|------|-----------|----------|
| `EffectSkillDuringTime` | Applies `amount × deltaTime` per frame for `lifeTime` seconds | Poison, burn, regeneration, haste |
| `EffectSkillOneTime` | Applies full `amount` once; guard prevents double-application | Instant heal, one-shot stat boost |

**Effect stat targets:** `Health`, `SpeedMove`, `Armor` (maps to `EntityStatsSO` modifiers).

**Effect sign/type normalization** (at editor time via `OnValidate`):
```
if (isNegative)    → currentIncreaseAmount = -|effectIncreaseAmount|
if (isPercentage)  → currentIncreaseAmount ÷= 100
```
Values are pre-normalized — `DohEffect()` adds them directly without further processing.

### Passive Upgrades (Room-Clear Talent System)

After clearing a room, the player is offered **3 `InternalSkillSO` cards** to choose from.
Each node grants one or more passive stat bonuses from the `StatsTypes` pool:
`Strength`, `Dexterity`, `Intelligence`, `Charisma`.

Each node has:
- `skillPrerequisites` — nodes that must have been chosen previously (prerequisite graph)
- `skillTier` — depth in the tree (higher tier = more powerful, locked until prerequisites met)
- `cost` — resource cost (skill points or currency, TBD)
- `uppgradeData` — list of `{stat, amount, isPercentage}` bonuses applied on selection

**Room-clear flow [GAP — not implemented]:**
1. All enemies in room defeated → `ON_ROOM_CLEAR` event fires
2. `GameManager` pauses dungeon, shows 3 randomly-selected `InternalSkillSO` cards
3. Player selects one → `TalentManager.Unlock(node)` applies `uppgradeData` to `PlayerData`
4. Dungeon resumes; doors unlock

**Current state:** `TalentManager.cs` exists but hardcodes Str/Dex/Int/Cha values in
`Awake()` — not SO-driven. Room-clear UI does not exist.

---

## Formulas

```
# Cast window duration
periodCastTime = min(currentTime − startCastTime, maxCastTime)

# Dash impulse
dashVelocity = InputHandler.DirectionMouseVector × dashingPower

# Slash spawn position
slashSpawnPos = player.transform.position + InputHandler.DirectionMouseVector × 3.0f
slashRotation = Quaternion.Euler(0, 0, InputHandler.AngleRotationPlayer)

# Effect per frame (DuringTime)
statModifier += currentIncreaseAmount × Time.deltaTime   [each frame for lifeTime seconds]

# Effect instant (OneTime)
statModifier += currentIncreaseAmount                    [once on first DohEffect() call]

# Effect normalization (editor-time OnValidate)
currentIncreaseAmount = ±|effectIncreaseAmount|          [sign from isNegative]
if (isPercentage): currentIncreaseAmount ÷= 100

# Talent upgrade
playerStat += uppgradeData.skillIncreaseAmount           [flat]
playerStat += playerStat × (skillIncreaseAmount / 100)  [if isPercentage]
```

---

## Edge Cases

| Scenario | Current Behaviour | Correct Behaviour |
|----------|------------------|-------------------|
| Cooldown reset after skill use | **[BUG — unclear]** `SetCanUseAbility(false)` is set in Cast→Do transition but no concrete ability calls `SetCanUseAbility(true)` in `Exit()` — ability may be permanently locked after first use | Each `ActivateSkill.Exit()` override must call `player.Core.AbilityHolder.SetCanUseAbility(true)` after the cooldown delay |
| Block during no-weapon state | Player presses RMB with no weapon — `abilityWeapon` slot is null | `PlayerBasicState` must gate skill checks on `WeaponHolder.Weapon != null` |
| Slash while target is too close (within 3 units) | Slash spawns inside or behind the player | Spawn position should clamp to a minimum offset or use a fixed world-space position |
| `EffectSkillOneTime` reused without reset | `isActivate` guard prevents double-application ✓ | ✓ Correct |
| `EffectSkillDuringTime.RemoveEffect()` with `isRecover = false` | Total accumulated amount is NOT reversed — stat stays modified | Known design: `isRecover = false` effects are permanent (e.g., poison deals its damage and does not undo it) |
| `InternalSkillSO` prerequisites not met | No enforcement visible in current code | `TalentManager.CanUnlock(node)` must check `skillPrerequisites` are all already unlocked |
| `DashAbility` animator speed not reset on interrupted exit | If ability is interrupted before `Exit()`, speed stays at 2f | `Exit()` must run unconditionally; ensure no early-exit paths skip it |
| `BlockAbility` stub receives `Cast()` call | No-op — nothing happens | Block stance must be implemented before demo |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Weapons** (`WeaponMeleeStats`) | Carries `abilityWeapon` and `skillWeapon` SO refs; wires them to `AbilityHolder` on equip via `Weapon.SetAbility()` | Weapons → Skills |
| **Character** (`AbilityHolder`, `PlayerSkillWeaponState`) | `AbilityHolder` drives lifecycle each frame; `PlayerSkillWeaponState` calls `SetStateAbility()` on `AnimationTrigger` | Character → Skills |
| **Animation** (`AnimationEventManager`) | `ability.Animator` overrides the runtime controller; `AnimationTrigger` event starts skill execution | Skills → Animation |
| **Input** (`PlayerInputHandle`) | Provides `DirectionMouseVector`, `AngleRotationPlayer`, and `SkillState` enum to abilities | Input → Skills |
| **Interface** (`IEffectable`) | Projectile-delivered effects use `IEffectable.ApplyEffect(EffectSkillSO)` | Skills → Interface |
| **Room progression** (`RoomController`, `GameManager`) | `ON_ROOM_CLEAR` event triggers talent card selection **[GAP]** | Map → Skills |
| **Player data** (`PlayerData`, `TalentManager`) | Passive upgrades modify player stats via `TalentManager.Unlock()` **[GAP — not SO-driven yet]** | Skills → Player |

---

## Tuning Knobs

All values in ScriptableObject assets.

### Active Abilities

| Parameter | Field | SO | Effect |
|-----------|-------|-----|--------|
| Slash projectile speed | `speedSlash` | `SlashAbility` SO | Higher = faster travel |
| Slash spawn offset | hardcoded 3.0f | `SlashAbility.cs` **[GAP — move to SO]** | Distance ahead of player |
| Dash speed | `dashingPower` | `DashAbility` SO | Higher = farther dash |
| Skill cooldown | `cooldownTime` | Any `ActivateSkill` SO | Seconds between uses |
| Max cast window | `maxCastTime` | Any `ActivateSkill` SO | How long Cast phase can run |
| Block damage reduction | `blockDamage` | `WeaponMeleeStats` SO | Flat damage absorbed while blocking |

### Effects

| Parameter | Field | SO | Effect |
|-----------|-------|-----|--------|
| Effect amount | `effectIncreaseAmount` | `EffectData` in effect SO | Flat or % modifier |
| Effect duration | `lifeTime` | `EffectSkillSO` | Seconds continuous effect runs |
| Is negative | `isNegative` | `EffectData` | Damage vs. buff |
| Is percentage | `isPercentage` | `EffectData` | % of stat vs. flat value |
| Is recoverable | `isRecover` | `EffectData` | Whether stat reverts on end |

### Talent Upgrades

| Parameter | Field | SO | Effect |
|-----------|-------|-----|--------|
| Stat bonus amount | `skillIncreaseAmount` | `UppgradeData` in `InternalSkillSO` | Points or % added to stat |
| Unlock cost | `cost` | `InternalSkillSO` | Resource required to pick node |
| Tree tier | `skillTier` | `InternalSkillSO` | Depth; higher = more powerful |

---

## Acceptance Criteria

### Active Skills — Slash

- [ ] Holding E enters Skill state; movement freezes
- [ ] On release (Do phase): slash projectile spawns 3 units ahead in mouse direction
- [ ] Projectile travels and deals damage via `INegativeReceiver.TakeDamage()` on hit
- [ ] After `Exit()`: cooldown prevents re-use until `cooldownTime` elapses
- [ ] Slash works in all 8 movement directions

### Active Skills — Block **[all currently unimplemented]**

- [ ] Holding RMB enters Block stance (Cast phase loops)
- [ ] While in block stance, incoming melee damage is reduced by `blockDamage`
- [ ] Releasing RMB exits block stance (Do + Exit)
- [ ] Block has a cooldown preventing infinite blocking

### Ability Cooldown

- [ ] After using any skill, `canUseAbility` is false until `cooldownTime` elapses
- [ ] `SetCanUseAbility(true)` is called in each ability's `Exit()` after the delay
- [ ] UI indicator shows cooldown state **[depends on HUD implementation]**

### Passive Upgrades (Room-Clear)

- [ ] After all enemies in a room are defeated, 3 `InternalSkillSO` cards are shown
- [ ] Player selects one; its `uppgradeData` is applied to `PlayerData` via `TalentManager`
- [ ] Prerequisites enforce that higher-tier nodes require prior selections
- [ ] `TalentManager` reads from SO assets — no hardcoded values in `Awake()`
