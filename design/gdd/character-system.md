---
status: reverse-documented
source: Assets/Script/Character/
date: 2026-05-19
verified-by: Kiet
---

# Character System Design

> **Note**: Reverse-engineered from existing implementation. Captures current behaviour
> and clarified design intent. Sections marked **[GAP]** describe intended design not yet
> implemented. Sections marked **[BUG]** identify known defects.

**Status**: In Design

---

## Overview

The character system defines movement, combat, and AI behaviour for all living entities in
the dungeon. Two subsystems share the same hierarchical state machine pattern: **Player**
(human-controlled) and **Entity** (AI-controlled enemies). Both use a Core component hub
architecture, ScriptableObject-driven stats, and animation-event-driven combat transitions.

All damage, death, and health changes flow through the `INegativeReceiver` interface.
No MonoBehaviour may directly mutate another entity's health field.

---

## Player Fantasy

The player feels agile and in control — crisp directional movement, weapon attacks that
commit on animation, and skills with distinct charge windows. Combat rewards positioning
and timing, not button mashing: a well-timed dodge or skill cast beats spamming attacks.

Enemies feel dangerous and reactive. They patrol, detect the player, and close the distance
relentlessly. Standing still is punished; fighting multiple enemies at once is risky.
Every enemy encounter should feel like a small puzzle: when to attack, when to dodge,
when to use a skill.

---

## Detailed Rules

### Player States

| State | Entry Condition | Exit Condition | Movement |
|-------|----------------|----------------|----------|
| **Idle** | Default; MoveVector = 0 | MoveVector ≠ 0 | velocity = 0 (deceleration to stop — see Formulas) |
| **Move** | MoveVector ≠ 0 | MoveVector = 0 | velocity = MoveVector × speed |
| **Attack** | LMB + weapon equipped + `CanAttack` passes | `animationFinished` event | frozen (velocity = 0) |
| **Skill** | E or RMB + `CanUseAbility` | `animationFinished` event | frozen (velocity = 0) |
| **TakeDamage** | `INegativeReceiver.TakeDamage()` called | `animationFinished` event | frozen; knockback dir set |
| **EquipUnequip** | F key + weapon nearby | `animationFinished` event | frozen |
| **Interact** | G key + interactable nearby | `animationFinished` event | frozen |

**Transition priority (highest wins when multiple conditions are true):**
`TakeDamage > EquipUnequip > Interact > Attack > Skill > Move > Idle`

**Attack gate:** `PlayerBasicState` checks `IsAttack && WeaponHolder.Weapon != null && Weapon.CheckCanAttack(player)` each frame. The weapon's `CheckCanAttack` manages the attack cooldown internally.

**Skill lifecycle** (driven by `AbilityHolder` each frame):
```
Enter(player) → Activate() → Cast() [held] → Do() [released or DoNonCast type] → Exit()
```
- `DoNonCast` skills skip the Cast phase and execute immediately
- `Cast` skills wait for the button release before triggering `Do()`
- Cooldown reset occurs in the skill's own `Exit()` implementation

### Enemy (Entity) States

| State | Entry Condition | Exit Condition | Movement |
|-------|----------------|----------------|----------|
| **Idle** | Default; timer expires or enters first | Target detected OR `idleDurationTime` elapsed | random 45° wander |
| **Move** | From Idle; target detected or wander timer | `moveDurationTime` elapsed (no target) OR attack range reached | towards target or random |
| **Attack** | Within `rangeCheckAttack` + `CanAttack` passes | `animationFinished` event | frozen |
| **TakeDamage** | `INegativeReceiver.TakeDamage()` called, health > 0 | `animationFinished` event | frozen; knockback dir set |
| **Death** | health ≤ 0 **[BUG — not triggered; see Edge Cases]** | despawn | frozen |

**Target detection:** `EntityInput.Update()` runs `Physics2D.OverlapCircle` every frame with `rangeCheckFieldOfView` radius. Player is automatically detected — no manual assignment needed.

**Obstacle avoidance:** When moving without a target and a wall is detected via raycast,
the entity turns 90° left or right plus a random bonus angle (45–90°) and continues.

**Chase range [GAP]:** Currently hardcoded as `10f` in `EntityMoveState`. This should be
read from `EntityData.rangeCheckChase` (field does not yet exist — needs adding to `EntityData` SO).

### Damage Rule

```
INegativeReceiver.TakeDamage(int amountDamage, Vector2 attackPosition)
```

- Called by weapons (`Weapon.Attack()`) and projectiles (`Projectile.CheckCollisions()`)
- Implementors: `Core` (Player) and `EntityCore` (Entity)
- `attackPosition` is used only to compute knockback direction — it does not affect damage amount
- After health reaches 0, further `TakeDamage` calls are no-ops

---

## Formulas

```
# Movement
playerVelocity  = MoveVector.normalized × movementVelocities
                  [GAP: smooth decel target — velocity decays to 0 over ~0.1s on release]
                  [CURRENT: instant stop — rb.velocity = Vector2.zero]

# Entity stats (base + modifier architecture)
entityHealth    = baseHealth + modifiersHealth
entityVelocity  = baseVelocities + modifiersVelocities
entityArmor     = baseArmor + modifiersArmor           [defined, NOT applied in damage calc]

# Damage
finalDamage     = rawDamage                            [CURRENT — no armor reduction]
                  [PLANNED: finalDamage = rawDamage - target.entityArmor]

# Knockback direction
knockbackDir    = Atan2((attackPos - entityPos).x, (attackPos - entityPos).y)
                  → converted to degrees → quantized to 8 directions (45° bins, 0=NE…7=N)
```

---

## Edge Cases

| Scenario | Current Behaviour | Correct Behaviour |
|----------|------------------|-------------------|
| Enemy loses target mid-chase | **[BUG]** `EntityMoveState` dereferences `Target.transform.position` before the null check on line 30 → NullReferenceException | Guard: `if (Target == null) → Idle` at top of `LogicUpdate()` |
| Player health ≤ 0 | `Core.TakeDamage` stops further damage but no death event fires, no restart | Should emit `ON_PLAYER_DEATH`; `GameManager` calls `PlayerData.Reborn()` + reload |
| Entity health ≤ 0 | `EntityBasicState` health ≤ 0 block is empty — entity freezes | Transition to `EntityDeathState`; play death anim; despawn |
| `EntityDeathState` called | **[BUG]** Extends `MonoBehaviour` instead of `EntityState` — not in state machine | Rewrite to extend `EntityState` |
| `EntityStatsSO.ModifiersAmor` read | **[BUG]** Property getter calls itself → stack overflow | Fix getter: `return modifiersAmor` (lowercase field) |
| Skill used without weapon | `AbilityHolder` may invoke skill while `WeaponHolder.Weapon == null` — unchecked | `PlayerBasicState` should gate skill check on `WeaponHolder.Weapon != null` |
| Enemy attack at edge of chase range | Possible: entity attacks at range 10f then exits attack but re-enters immediately | `rangeCheckAttack` must be ≤ chase range; gate checked each frame |
| `AnimationPlayerController.OnEnable` | **[BUG]** `StartAnimation` callback registered twice; `EndAnimation` callback never fires | Line 21: second registration should be `EndAnimation` |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Weapons** (`Assets/Script/Weapons/`) | `Weapon.Attack()` and `Weapon.CheckCanAttack()` are called by attack states | Character → Weapons |
| **Skill/Ability** (`Assets/Script/Skill_Ability/`) | `ActivateSkill` SO provides ability lifecycle; `AbilityHolder` drives it | Character → Skills |
| **Event Manager** (`EventManager.cs`) | `ON_PLAYER_DEATH`, `ON_PLAYER_TAKE_DAMAGE`, `ON_ENEMY_DEATH` events needed — not yet in `EventID` enum | Character → EventManager |
| **Animation** (`AnimationEventManager.cs`) | `AnimationTrigger` fires weapon/skill; `AnimationFinished` exits states | Character → Animation |
| **Input** (`PlayerInputHandle.cs`) | Provides `MoveVector`, `DirectionMouse`, `IsAttack`, `IsSkill`, `IsTakeDamage` | Input → Character |
| **Map** (`RoomController`) | Enemies must be registered with `RoomController` for room-clear tracking | Character → Map |

---

## Tuning Knobs

All values live in ScriptableObject assets — never hardcode in state classes.

| Parameter | SO Asset | Field | Default | Effect |
|-----------|----------|-------|---------|--------|
| Player max health | `PlayerData` | `maxHealth` | 100 | Total health pool |
| Player move speed | `PlayerData` | `movementVelocities` | 10f | Units/frame at full input |
| Enemy health | `EntityStatsSO` | `baseHealth` | varies | Total health before modifiers |
| Enemy base speed | `EntityStatsSO` | `baseVelocities` | 10f | Movement speed |
| Enemy armor | `EntityStatsSO` | `baseArmor` | 0 | Damage reduction **[PLANNED]** |
| Enemy FOV range | `EntityData` | `rangeCheckFieldOfView` | varies | Player detection radius |
| Enemy attack range | `EntityData` | `rangeCheckAttack` | varies | Attack trigger radius |
| Enemy idle duration | `EntityData` | `idleDurationTime` | varies | Seconds before wandering |
| Enemy move duration | `EntityData` | `moveDurationTime` | varies | Seconds before idle |
| Enemy chase range | `EntityData` | `rangeCheckChase` **[GAP — field missing]** | 10f | Max pursuit distance |
| Ability cooldown | per `ActivateSkill` SO | per-skill field | varies | Set in `Exit()` phase |

---

## Acceptance Criteria

### Player Movement
- [ ] Player moves in 8 directions at `PlayerData.movementVelocities` speed
- [ ] Releasing WASD decelerates player to a stop over ~0.1s (not instant)
- [ ] Direction-facing updates from mouse position for attack/skill aiming

### Player Combat
- [ ] LMB with weapon equipped triggers Attack state; movement freezes
- [ ] Damage applies to enemy via `INegativeReceiver.TakeDamage()` on animation trigger
- [ ] Player returns to Idle/Move on `animationFinished`

### Player Skills
- [ ] E/RMB triggers Skill state; `AbilityHolder` drives Start → Cast → Do → Exit
- [ ] `DoNonCast` skills execute immediately without Cast phase
- [ ] Cooldown defined in skill SO prevents immediate re-use after `Exit()`

### Player Damage & Death
- [ ] `INegativeReceiver.TakeDamage()` decrements `PlayerData.currentHealth`
- [ ] Knockback direction in TakeDamage animation matches attacker position
- [ ] `currentHealth ≤ 0` emits `ON_PLAYER_DEATH`; scene reloads to StartScene

### Enemy AI
- [ ] Enemy detects player within `rangeCheckFieldOfView` and pursues
- [ ] Enemy loses target and transitions to Idle after `moveDurationTime`
- [ ] Enemy attacks when within `rangeCheckAttack`; player health decrements
- [ ] Enemy avoids walls during random wander
- [ ] Enemy null-dereference on target loss is resolved (no NullRef in console)

### Enemy Damage & Death
- [ ] `INegativeReceiver.TakeDamage()` decrements entity health via `EntityStatsSO.ModifiersHealth`
- [ ] TakeDamage state plays stun + directional knockback animation
- [ ] `health ≤ 0` transitions to `EntityDeathState`; entity despawns
- [ ] `EntityDeathState` extends `EntityState` (not MonoBehaviour)
