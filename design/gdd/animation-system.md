# Animation System

> **Status**: In Design
> **Author**: Kiet + Claude
> **Last Updated**: 2026-05-19
> **Implements Pillar**: Foundation — enables responsive combat timing across all character systems

## Overview

The animation system is the timing bridge between Unity's Animator state machine and the gameplay logic layers. It translates animation clip events — fired at specific frames by the Unity Animator — into strongly-typed `AnimationEventId` signals that character state machines consume to execute gameplay actions (weapon damage, state exits, skill activations).

All character combat timing in this project depends on this system: a melee attack deals damage exactly when the animation's hit frame fires `AttactAnimation`; a state exits exactly when the clip's final frame fires `EndAnimation`. No gameplay system polls for timing independently — all timing is driven by animation events.

The system consists of three components: `AnimationEventManager` (the static event bus), `AnimationPlayerController` (the per-character event handler), and `AnimationEventId` (the strongly-typed event enum). Animator controller swapping at runtime enables skill abilities to override the active animation set on demand.

## Player Fantasy

Players never interact with the animation system directly — they feel what it enables. In Cult of the Lamb, every sword swing has a distinct hit-stop and impact frame that makes combat feel weighty; in Hades, the dash and attack animations snap instantly so the player always feels in control. This project aims for the same quality: the animation system is what makes the difference between "I hit the enemy" and "that hit felt good."

Concretely: a 3-hit melee combo should feel like three distinct, committed strikes — each one landing at the exact frame the animator intended, not a frame early or late. A Slash skill should feel like an instant release of energy the moment the player lets go of E. When this system works, players call the combat "snappy" and "responsive" without knowing why. When it breaks — event fires on the wrong frame, or the wrong event fires twice — every attack feels floaty and disconnected.

The animation system is invisible infrastructure: if it is well-built, players don't notice it; if it is broken, the entire combat feel collapses.

## Detailed Design

### Core Rules

1. All gameplay timing is driven by animation events, never by frame counts or coroutines.
2. Animation clips fire events by calling `AnimationEventManager.Emit(AnimationEventId, data)` via Unity Animation Event inspector entries — not from code.
3. `AnimationEventManager` broadcasts to all registered listeners via a static delegate dictionary.
4. Each character registers its event handlers in `OnEnable()` and unregisters in `OnDisable()` — no listener is active without a live `AnimationPlayerController`.
5. State machines receive events via flag properties (`isAnimationTrigger`, `isAnimationFinished`) set by callbacks, then act on those flags in `LogicUpdate()`.
6. Each flag is reset immediately after use — it is only `true` for one `LogicUpdate()` frame.
7. Skill abilities swap `Animator.runtimeAnimatorController` to the ability's own controller on `EnterAbility()` and restore the previous controller on `ExitAbility()`.
8. The 8-directional attack animations are provided by `AttackSO.directionAttackAnimatorOV` — a per-attack `AnimatorOverrideController` applied during `WeaponMelee.CheckCanAttack()`.

**AnimationEventId contract:**

| Event | Fires when | State machine action |
|-------|-----------|----------------------|
| `StartAnimation` | Clip begins playing | Reserved — currently unused |
| `MoveAnimation` | Locomotion phase starts | Play movement animation |
| `AttactAnimation` | Hit frame of attack clip | `isAnimationTrigger = true` → `Weapon.Attack()` |
| `DoSkillAnimation` | Active frame of skill clip | `isAnimationTrigger = true` → skill `Do()` phase |
| `EndAnimation` | Clip completes | `isAnimationFinished = true` → state exits |

**⚠️ Bug #9 — `AnimationPlayerController.OnEnable()` line 21:**
Currently registers `StartAnimation` twice; `EndAnimation` is never registered and never fires. Fix: change line 21 handler from `StartAnimation` to `EndAnimation`. Mirror the fix in `OnDisable()`.

**Correct registration table:**

| Line | EventId | Handler |
|------|---------|---------|
| 17 | `StartAnimation` | `StartAnimation()` |
| 18 | `MoveAnimation` | `Move()` |
| 19 | `AttactAnimation` | `Attack()` |
| 20 | `DoSkillAnimation` | `DoSkill()` |
| **21** | **`EndAnimation`** | **`EndAnimation()`** ← fix here |

**AnimationName constants (required — `AnimationName.cs` currently empty):**

| Constant | Value | Used by |
|----------|-------|---------|
| `IDLE` | `"Idle"` | `PlayerIdleState` |
| `MOVE` | `"Move"` | `PlayerMoveState` |
| `ATTACK` | `"Attack"` | `PlayerAttackState` |
| `SKILL` | `"Skill"` | `PlayerSkillWeaponState` |
| `TAKE_DAMAGE` | `"TakeDamage"` | `PlayerTakeDamageState` |
| `DEATH` | `"Death"` | Future death state |

---

### States and Transitions

| State | Enters animation via | Exits via | Flag consumed |
|-------|---------------------|-----------|---------------|
| `PlayerIdleState` | `Animator.SetBool(animBoolName, true)` | Input change | none |
| `PlayerMoveState` | `Animator.SetBool(animBoolName, true)` | Input change | none |
| `PlayerAttackState` | `AnimatorOverrideController` swap in `CheckCanAttack()` | `EndAnimation` event | `isAnimationFinished` |
| `PlayerSkillWeaponState` | `runtimeAnimatorController` swap in `EnterAbility()` | `EndAnimation` event | `isAnimationFinished` |
| `PlayerTakeDamageState` | `Animator.SetBool(animBoolName, true)` | `EndAnimation` event | `isAnimationFinished` |
| `EntityAttackState` | `AnimatorOverrideController` swap | `EndAnimation` event | `isAnimationFinished` |

---

### Interactions with Other Systems

| System | Consumes | Produces |
|--------|----------|---------|
| **Melee Combat** | `AttactAnimation` → fires `Weapon.Attack()` | Nothing |
| **Skill System** | `DoSkillAnimation` → fires skill `Do()` phase; `runtimeAnimatorController` swapped by `AbilityHolder` | Nothing |
| **Character States** | `EndAnimation` → all use-weapon states exit on this | `animBoolName` parameter to enter locomotion states |
| **Weapon System** | `AttackSO.directionAttackAnimatorOV` applied during `CheckCanAttack()` | Nothing |

## Formulas

This system has no mathematical formulas. It defines **timing contracts** — behavioral guarantees about when events fire and how many times.

```
AttactAnimation fires exactly once per attack state entry.
  → Weapon.Attack() called = 1 time per combo hit
  → isAnimationTrigger reset immediately after consumption

EndAnimation fires exactly once per clip completion.
  → State exits on the first EndAnimation received
  → isAnimationFinished reset immediately after consumption

runtimeAnimatorController swap:
  → On EnterAbility():  controller = ability.Animator
  → On ExitAbility():   controller = previous controller (weapon override or base)
  → Swap depth = 1 — no stacking; a skill cannot enter another skill mid-animation

AnimatorOverrideController depth:
  → WeaponMelee applies directionAttackAnimatorOV per attack hit (depth 1 over base)
  → AbilityHolder swaps runtimeAnimatorController entirely (replaces depth 0)
  → These two mechanisms do not stack — a skill swap replaces the weapon override
```

## Edge Cases

| Scenario | Correct Behaviour |
|----------|------------------|
| **Player takes damage mid-attack animation** | State machine transitions to `PlayerTakeDamageState` before `EndAnimation` fires — `isAnimationFinished` is never set. `PlayerTakeDamageState.Enter()` must reset both `isAnimationTrigger` and `isAnimationFinished` to `false`. |
| **`AttactAnimation` fires but weapon is null** | `PlayerAttackState.LogicUpdate()` calls `Weapon.Attack()` — if weapon is null → NullReferenceException. Guard: check `WeaponHolder.Weapon != null` before calling. |
| **`EndAnimation` fires twice on the same clip** | If the animator loops or a clip has a bug, `isAnimationFinished` could be set twice. Because the flag resets immediately after the first read, the second occurrence is a no-op — the state has already exited. No issue. |
| **Skill `ExitAbility()` does not restore controller** | Player is stuck with the skill's controller; subsequent attack animations will be wrong. `ExitAbility()` must always restore `runtimeAnimatorController` to the weapon base controller, even when the skill is interrupted. |
| **Bug #9 — `EndAnimation` never fires** | As documented in Detailed Design: `AnimationPlayerController.OnEnable()` line 21 registers the wrong event. Consequence: every use-weapon state never exits, player is permanently stuck. This fix is mandatory before demo. |
| **`isAnimationTrigger` still `true` when entering a new state** | If a state exits without resetting the flag, the next state receives a phantom event on its first frame. Every state `Enter()` must reset `isAnimationTrigger = false`. |

## Dependencies

| System | Relationship | Interface |
|--------|-------------|-----------|
| **Event Manager** (`EventManager.cs`) | Animation system mirrors the static bus pattern but is a separate bus | `AnimationEventManager` does not route through `EventManager` |
| **Character / Player Controller** | Downstream — `PlayerState` consumes `isAnimationTrigger` and `isAnimationFinished` flags | `AnimationPlayerController` registered per-character in `OnEnable()` |
| **Enemy AI** | Downstream — `EntityState` uses the same flag pattern for `EntityAttackState` and `EntityTakeDamageState` | Separate registration per entity, same contract |
| **Melee Combat / Weapon System** | Downstream — `PlayerAttackState` calls `Weapon.Attack()` on `AttactAnimation` event | `AttackSO.directionAttackAnimatorOV` is the controller override per-hit |
| **Skill & Ability System** | Downstream — `AbilityHolder.EnterAbility()` swaps `runtimeAnimatorController` | Skill SO holds `RuntimeAnimatorController` asset reference |
| **Input System** | No dependency — animation events are not input-driven | — |

## Tuning Knobs

All animation timing is configured in the Unity Animator inspector — not in code or ScriptableObjects.

| Parameter | Location | Effect | Notes |
|-----------|----------|--------|-------|
| **`AttactAnimation` hit frame** | Unity Animator → clip inspector → Animation Event position | Determines which frame damage is applied — earlier = aggressive, later = deliberate delay | Set in Inspector per clip |
| **`EndAnimation` exit frame** | Unity Animator → clip inspector | Determines combo window — earlier fire = faster combo, later fire = longer committed swing | Directly affects `durationNextAttack` calculation |
| **`directionAttackAnimatorOV`** | `AttackSO` SO asset | Controller override per combo hit — swaps controller to play the correct 8-directional animation | One asset per hit per 8 directions |
| **`ability.Animator`** | Each `ActivateSkill` SO asset | Controller used while skill is active — replaces the entire animation set | One asset per skill |
| **`animBoolName`** | Each `PlayerState` subclass | Animator parameter name used to enable/disable locomotion blend trees | Must match exactly the parameter name in the Animator Controller asset |

## Visual/Audio Requirements

[To be designed]

## UI Requirements

[To be designed]

## Acceptance Criteria

- [ ] **GIVEN** player presses LMB with weapon equipped, **WHEN** the attack animation reaches the hit frame, **THEN** `Weapon.Attack()` is called exactly once and damage is applied to enemies in range
- [ ] **GIVEN** player is in `PlayerAttackState`, **WHEN** the attack animation completes, **THEN** `EndAnimation` fires, `isAnimationFinished` becomes `true`, and the state transitions to Idle or Move within the same frame
- [ ] **GIVEN** player uses Slash skill (E key), **WHEN** the skill animation reaches the active frame, **THEN** `DoSkillAnimation` fires and the projectile spawns correctly
- [ ] **GIVEN** player equips a weapon with 3 attacks in the combo, **WHEN** each hit uses a different `directionAttackAnimatorOV`, **THEN** each hit plays the correct directional animation matching the mouse direction
- [ ] **GIVEN** player uses a skill, **WHEN** the skill exits (`ExitAbility()`), **THEN** `runtimeAnimatorController` is restored to the previous controller and subsequent attacks animate correctly
- [ ] **GIVEN** Bug #9 is fixed, **WHEN** any use-weapon state completes its animation, **THEN** `EndAnimation` fires exactly once and the state exits — no permanent state lock
- [ ] **GIVEN** player takes damage during an attack animation, **WHEN** `PlayerTakeDamageState.Enter()` is called, **THEN** `isAnimationTrigger` and `isAnimationFinished` are both reset to `false`
- [ ] **GIVEN** `AnimationName.cs` is populated, **WHEN** any state calls `Animator.SetBool(AnimationName.IDLE, true)`, **THEN** the correct Animator parameter is set with no magic string errors

## Open Questions

[To be designed]
