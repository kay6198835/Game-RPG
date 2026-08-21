# Animation System

> **Status**: In Design
> **Author**: Kiet + Claude
> **Last Updated**: 2026-08-21 (rewritten against the shipped `StatusAnimation` enum)

> **✅ Rewritten against `StatusAnimation` on 2026-08-21** (owner decision C5). This document
> previously specified a **flag-based** handoff (`isAnimationTrigger` / `isAnimationFinished`,
> reset after one frame). Neither flag has ever existed in the codebase; the shipped mechanism is
> the `StatusAnimation` enum set through `SetAnimationStatus()`. The owner chose to follow the
> code, so the contract sections below now describe the enum model. Player Fantasy and Tuning
> Knobs are unchanged.
>
> **A second correction in the same pass:** `AnimationEventManager` and `AnimationEventId` — which
> earlier revisions of this document described as the core mechanism — are **dead code**.
> `AnimationEventManager.Emit()` has zero callers anywhere in the repository, so the bus never
> fires. See "The dead parallel bus" below.

> **Implements Pillar**: Foundation — enables responsive combat timing across all character systems

## Overview

The animation system is the timing bridge between Unity's Animator and the gameplay logic layers.
Animation clips carry Unity **Animation Events** that call methods by name on the character
MonoBehaviour (`Player` / `Entity`); each of those methods writes a value into the current state's
`StatusAnimation`. States read that value in `LogicUpdate()` and act on it.

All character combat timing depends on this: a melee attack deals damage exactly when the hit frame
sets `OnActivate`; an attack chains or exits exactly when the final frame sets `EndRangeTrigger`. No
gameplay system polls for timing independently — all timing originates in the animation clip.

The system is three pieces:

| Piece | Where | Role |
|---|---|---|
| `StatusAnimation` enum | `Character/Base/StatusAnimation.cs` | The eight values a state can be in |
| Animation-event methods | `Player.cs:70-75`, `Entity.cs:65-68` | Called by name from the clip; each sets one value |
| `SetAnimationStatus()` | `PlayerState` / `EntityState` | Writes `Status`; states branch on it |

Animator controller swapping at runtime lets weapons and skills override the active animation set —
`AttackSO.directionAttackAnimatorOV` per combo stage, `ActivateSkill.Animator` per skill.

## Player Fantasy

Players never interact with the animation system directly — they feel what it enables. In Cult of the Lamb, every sword swing has a distinct hit-stop and impact frame that makes combat feel weighty; in Hades, the dash and attack animations snap instantly so the player always feels in control. This project aims for the same quality: the animation system is what makes the difference between "I hit the enemy" and "that hit felt good."

Concretely: a 3-hit melee combo should feel like three distinct, committed strikes — each one landing at the exact frame the animator intended, not a frame early or late. A Slash skill should feel like an instant release of energy the moment the player lets go of E. When this system works, players call the combat "snappy" and "responsive" without knowing why. When it breaks — event fires on the wrong frame, or the wrong event fires twice — every attack feels floaty and disconnected.

The animation system is invisible infrastructure: if it is well-built, players don't notice it; if it is broken, the entire combat feel collapses.

## Detailed Design

### Core Rules

1. All gameplay timing is driven by animation events, never by frame counts or coroutines.
2. Animation clips fire events through Unity's **Animation Event** feature, which invokes a method
   **by name** on a component of the animated GameObject. The clip does not reference any manager.
3. Each of those methods does exactly one thing: `stateMachine.CurrentState.SetAnimationStatus(X)`.
   No gameplay logic lives in them.
4. `Status` is **durable state, not a one-frame pulse.** It holds its value until something writes
   a new one. A state may therefore read the same `Status` on several consecutive `LogicUpdate()`
   calls, and must write `Status` itself if it wants to consume a value once — see
   `PlayerAttackState.cs:20-63`, which sets `Status = OffActivate` immediately after acting on
   `OnActivate`, and `Status = None` when the chain ends.
5. `PlayerState.Enter()` sets `Status = Start`; `PlayerState.Exit()` sets `Status = End`. A state
   therefore always begins from a known value and never inherits the previous state's.
6. Skill abilities swap `Animator.runtimeAnimatorController` to the ability's own controller on
   `EnterAbility()` and restore the previous controller on `ExitAbility()`.
7. The 8-directional attack animations come from `AttackSO.directionAttackAnimatorOV`, applied per
   combo stage in `Weapon.OnAttackEnter()`.

**`StatusAnimation` contract:**

| Value | Set by (method on `Player` / `Entity`) | State machine action |
|---|---|---|
| `None` | states, to mean "consumed / idle" | No action; used by `PlayerAttackState` to end a chain |
| `Start` | `AnimationStart()`, and `PlayerState.Enter()` | Clip is beginning; attack state clears its input buffer here |
| `Animaing` | *nothing* | ⚠️ Declared in the enum, never written or read anywhere. Dead value |
| `StartRangeTrigger` | `AnimationTrigger()` | Start of the active window |
| `OnActivate` | `AnimationOnAction()` | **Hit frame** → `WeaponHolder.MakeDamage()` |
| `OffActivate` | `AnimationOffAction()` | End of the active window → `WeaponHolder.EndDamage()` |
| `EndRangeTrigger` | `AnimtionFinishTrigger()` *(typo intentional)* | Chain to the next stage, or fall through to exit |
| `End` | `AnimationEnd()`, and `PlayerState.Exit()` | Clip finished; use-weapon states leave to Idle/Move |

**Player vs Entity coverage.** `Player.cs:70-75` implements all six event methods.
`Entity.cs:65-68` implements only four — it has **no** `AnimationStart()` and **no**
`AnimationEnd()`. Enemy states therefore only ever see `Start`/`End` from `Enter()`/`Exit()`, never
from a clip. Any enemy behaviour that needs a clip-driven start or end frame has to be added.

### The dead parallel bus

`AnimationEventManager` (`Manager/AnimationEventManager.cs`) is a static
`Dictionary<AnimationEventId, Action<object>>` with `Resgister` / `UnResgister` / `Emit`. It is
**not** used:

- `AnimationEventManager.Emit()` has **zero callers** in the entire repository. Nothing ever fires
  the bus.
- `AnimationPlayerController` registers five handlers to it in `OnEnable()` and unregisters them in
  `OnDisable()` — correctly balanced, but to a bus that never fires.
- Four of those five handlers (`StartAnimation`, `EndAnimation`, `Attack`, `DoSkill`) have **empty
  bodies**. Only `Move()` contains anything, and it is unreachable.

Bug #9 (registration on line 21 pointing at the wrong event id) is genuinely fixed, but the fix is
close to moot given the above: correct registration on a dead bus.

**Design decision needed:** either delete `AnimationEventManager` / `AnimationEventId` /
`AnimationPlayerController`, or give them a purpose. Leaving them is actively misleading — they
read as the system's core, which is how earlier revisions of this document described them. Not
resolved here.

### AnimationName constants

`AnimationName.cs` is **not** a constants file — it is an unrelated empty `ScriptableObject` stub
with a `[CreateAssetMenu]` attribute and should be deleted (TD-016). The real constants live in
`GameConstants.AnimationName` and are in use (e.g. `EntityBasicState.cs:27` reads
`GameConstants.AnimationName.Parameter.DIRECTION`). `IDLE`, `MOVE`, `ATTACK`, `EQUIP_UNEQUIP`,
`INTERACTOR`, `ABILITY` and `TAKE_DAMAGE` all exist there. Still missing, and still required by
this design:

| Constant | Value | Used by |
|----------|-------|---------|
| `SKILL` | `"Skill"` | `PlayerSkillWeaponState` |
| `DEATH` | `"Death"` | `PlayerDeathState` (exists but is never constructed — BUG-044) |

---

### States and Transitions

| State | Enters animation via | Exits via | Status branch |
|-------|---------------------|-----------|---------------|
| `PlayerIdleState` | `Animator.SetBool(animBoolName, true)` | Input change | none |
| `PlayerMoveState` | `Animator.SetBool(animBoolName, true)` | Input change | none |
| `PlayerAttackState` | `AnimatorOverrideController` swap in `Weapon.OnAttackEnter()` | `Status == End` | `OnActivate` → damage; `EndRangeTrigger` → chain or `None` |
| `PlayerSkillWeaponState` | `runtimeAnimatorController` swap in `EnterAbility()` | `Status == End` | drives `AbilityHolder.SetStateAbility()` per frame |
| `PlayerTakeDamageState` | `Animator.SetBool(animBoolName, true)` | `Status == End` | — |
| `EntityAttackState` | `AnimatorOverrideController` swap | `Status == End` (from `Exit()`, not a clip) | `OnActivate` → weapon `Attack()` |
| `EntityDeathState` | death clip | never exits | `EndRangeTrigger` → emit `ON_ENEMY_DEATH` |

---

### Interactions with Other Systems

| System | Consumes | Produces |
|--------|----------|---------|
| **Melee Combat** | `OnActivate` → `WeaponHolder.MakeDamage()` → `Weapon.OnActivate()` | Nothing |
| **Skill System** | `PlayerSkillWeaponState` drives `AbilityHolder` each frame; `runtimeAnimatorController` swapped by `AbilityHolder` | Nothing |
| **Character States** | `End` → all use-weapon states exit on this | `animBoolName` parameter to enter locomotion states |
| **Weapon System** | `AttackSO.directionAttackAnimatorOV` applied in `Weapon.OnAttackEnter()` | Nothing |

## Formulas

This system has no mathematical formulas. It defines **timing contracts** — behavioural guarantees
about what `Status` holds and who may write it.

```
Status is DURABLE STATE, not a one-frame pulse.
  → It keeps its value until something writes a new one.
  → A state may read the same value on several consecutive LogicUpdate() calls.
  → To consume a value once, the state writes Status itself:
        PlayerAttackState: OnActivate  -> MakeDamage(), then Status = OffActivate
        PlayerAttackState: EndRangeTrigger -> chain (Status = Start) or exit (Status = None)

Status lifecycle per state:
  → PlayerState.Enter() sets Status = Start
  → PlayerState.Exit()  sets Status = End
  → so a state never inherits the previous state's Status

OnActivate fires once per attack stage.
  → WeaponHolder.MakeDamage() called = 1 time per combo hit
  → the state immediately writes OffActivate, so a repeated read is a no-op

runtimeAnimatorController swap:
  → On EnterAbility():  controller = ability.Animator
  → On ExitAbility():   controller = previous controller (weapon override or base)
  → Swap depth = 1 — no stacking; a skill cannot enter another skill mid-animation

AnimatorOverrideController depth:
  → Weapon.OnAttackEnter() applies directionAttackAnimatorOV per stage (depth 1 over base)
  → AbilityHolder swaps runtimeAnimatorController entirely (replaces depth 0)
  → These two do not stack — a skill swap replaces the weapon override
```

## Edge Cases

| Scenario | Correct Behaviour |
|----------|------------------|
| **Player takes damage mid-attack animation** | The state machine transitions to `PlayerTakeDamageState` before the attack clip reaches its end frame. `PlayerState.Exit()` sets `Status = End` on the outgoing state and `Enter()` sets `Status = Start` on the incoming one, so no stale value carries over. This is handled by the base class — individual states need no reset code. |
| **Hit frame fires but no weapon is equipped** | `WeaponHolder.MakeDamage()` early-returns on `weapon == null` (`WeaponHolder.cs:41-45`), so this is safe. `PlayerBasicState` also gates entry into `PlayerAttackState` on `weaponHolder.Weapon != null`. |
| **A clip sets `OnActivate` twice** | `PlayerAttackState` writes `Status = OffActivate` in the same frame it acts, so a second `OnActivate` from the clip would re-trigger damage. Unlike the old flag model there is no automatic one-shot guard — **the state is responsible for consuming the value**. Author one `AnimationOnAction` event per clip. |
| **`EndRangeTrigger` arrives with input buffered** | `PlayerAttackState` chains: calls `weaponHolder.Attack()`, replays the animator state from 0, sets `Status = Start`. If `CanChain()` is false it sets `Status = None` and the state falls through to exit. |
| **Skill `ExitAbility()` does not restore controller** | Player is stuck with the skill's controller; subsequent attack animations will be wrong. `ExitAbility()` must always restore `runtimeAnimatorController` to the weapon base controller, even when the skill is interrupted. |
| **Enemy clip has no start/end event** | Expected — `Entity` implements only four of the six event methods (no `AnimationStart`, no `AnimationEnd`). Enemy states see `Start`/`End` only from `Enter()`/`Exit()`. |
| **Bug #9 — `EndAnimation` never fires** | ✅ **RESOLVED.** `AnimationPlayerController.cs:21` registers `EndAnimation` correctly. Kept for history because several other documents still cite it as open — and note it is registration on a bus that never fires (see "The dead parallel bus"). |

## Dependencies

| System | Relationship | Interface |
|--------|-------------|-----------|
| **Character / Player Controller** | Downstream — states read `Status` in `LogicUpdate()` | `SetAnimationStatus(StatusAnimation)` on `PlayerState` / `EntityState` |
| **Enemy AI** | Downstream — same enum, but only four of the six event methods exist on `Entity` | `Entity.cs:65-68` |
| **Melee Combat / Weapon System** | Downstream — `OnActivate` drives `WeaponHolder.MakeDamage()` | `AttackSO.directionAttackAnimatorOV` is the per-stage controller override |
| **Skill & Ability System** | Downstream — `AbilityHolder.EnterAbility()` swaps `runtimeAnimatorController` | Skill SO holds a `RuntimeAnimatorController` reference |
| **Event Manager** (`EventManager.cs`) | **No dependency.** Animation status never routes through the `EventID` bus | — |
| **`AnimationEventManager`** | ⚠️ **Dead.** A separate static dict with zero `Emit()` callers. `.claude/rules/manager-event-code.md` claims it "fires through `EventManager`" — it does not; it is its own bus, and an unused one | — |
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

- [x] **GIVEN** player presses LMB with a weapon equipped, **WHEN** the attack clip reaches its hit frame, **THEN** `AnimationOnAction()` sets `Status = OnActivate` and `PlayerAttackState` calls `WeaponHolder.MakeDamage()` exactly once *(implemented — `PlayerAttackState.cs:31-34`)*
- [x] **GIVEN** player is in `PlayerAttackState`, **WHEN** the clip's final frame fires, **THEN** `Status` reaches `End` and the state transitions to Idle or Move *(implemented via `PlayerUseWeaponState`)*
- [x] **GIVEN** input is buffered at `EndRangeTrigger` and `CanChain()` is true, **WHEN** the trigger is read, **THEN** the next stage starts and `Status` returns to `Start` *(implemented — `PlayerAttackState.cs:39-52`)*
- [ ] **GIVEN** player uses Slash skill (E key), **WHEN** the skill animation reaches its active frame, **THEN** the projectile spawns correctly
- [ ] **GIVEN** player equips a weapon with 3 combo stages, **WHEN** each stage uses a different `directionAttackAnimatorOV`, **THEN** each hit plays the correct directional animation matching the mouse direction
- [ ] **GIVEN** player uses a skill, **WHEN** the skill exits (`ExitAbility()`), **THEN** `runtimeAnimatorController` is restored and subsequent attacks animate correctly
- [ ] **GIVEN** an enemy attack clip, **WHEN** it needs a clip-driven start or end frame, **THEN** `Entity` implements `AnimationStart()` / `AnimationEnd()` *(currently missing — only four of six methods exist)*
- [ ] **GIVEN** `GameConstants.AnimationName` is complete (`SKILL` and `DEATH` still missing), **WHEN** any state calls `Animator.SetBool(GameConstants.AnimationName.IDLE, true)`, **THEN** the correct Animator parameter is set with no magic string errors
- [ ] **GIVEN** the dead `AnimationEventManager` bus, **WHEN** the owner decides its fate, **THEN** it is either deleted or given a real emitter — it must not stay as unreachable code that reads like the core mechanism

## Open Questions

1. **What happens to `AnimationEventManager` / `AnimationEventId` / `AnimationPlayerController`?**
   Zero emitters, four of five handlers empty. Delete, or wire up? Raised 2026-08-21.
2. **Should `Entity` gain `AnimationStart()` / `AnimationEnd()`** so enemy clips can drive start and
   end frames the way player clips do? Currently enemy states only see `Start`/`End` from
   `Enter()`/`Exit()`.
3. **`StatusAnimation.Animaing`** is declared but never written or read. Remove, or was it intended
   for a "clip in progress" state that was never built?
