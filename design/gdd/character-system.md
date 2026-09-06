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

**Status**: In Design (implementation-status claims corrected 2026-08-20)

> **Audit note (2026-08-20).** Design intent below is unchanged. Only *descriptive* claims —
> class names, API names, and `[BUG]` markers — were corrected against source. Where a
> `[BUG]` has been fixed it is marked ✅ and kept for history, because other documents still
> cite several of them as open.

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

**Attack gate:** `PlayerBasicState` checks `inputHandler.IsAttack && weaponHolder.Weapon != null && weaponHolder.CanAttack()` each frame (`PlayerBasicState.cs:43-49`). Corrected 2026-08-20 — `CheckCanAttack()` no longer exists. The current weapon API is `CanAttack()` / `CanChain()` / `OnAttackEnter(Player)` / `OnActivate()` / `OnDeactivate()`; cooldown and stage selection live in `Weapon.OnAttackEnter()`.

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
| **Death** | health ≤ 0 **[BUG — unreachable; see Edge Cases]** | despawn | frozen |

**Target detection [BUG — disabled]:** the design is that `EntityInput.Update()` resolves a target within `rangeCheckFieldOfView` every frame. In source, the call is commented out — `EntityInput.cs:67` reads `//GetTargetInRange();`, and that method is the only writer of `targetTransform`. `TargetTransform` is therefore permanently `null`: enemies never detect the player, `EntityMoveState` always takes the no-target branch, and `EntityAttackState.cs:24` would NullRef if reached. Restore it **with** a null guard and `OverlapCircleNonAlloc` (TD-037 / TD-005).

**Obstacle avoidance:** When moving without a target and a wall is detected via raycast,
the entity turns 90° left or right plus a random bonus angle (45–90°) and continues.

**Chase range [GAP]:** `EntityData.rangeCheckChase` still does not exist. Corrected
2026-08-20: after the `EntityMoveState` rewrite the literal `10` at line 27 is a **no-target
timeout in seconds**, not a chase distance — so there are now two gaps, a magic timer and a
missing chase-range field (TD-019).

### Damage Rule

```
INegativeReceiver.TakeDamage(int amountDamage, Vector2 attackPosition)
```

- Called by weapons (`Weapon.OnActivate()`) and projectiles (`Projectile.CheckCollisions()`)
- **Implementors, corrected 2026-08-20** — the design is one implementer per character. Source has:
  - Player: `NegativeReciver` (a `CoreComponent<Core>`, *not* `Core` itself) — implemented, emits `ON_PLAYER_DEATH`
  - Enemy: **two** implementers, which is the defect. `EntityCore.TakeDamage()` throws `NotImplementedException` (BUG-042), while `EntityNegativeReciver.TakeDamage()` decrements its own `currentHealth`, resolves `PlayerInputHandler` off an `EntityCore` (→ NRE) and emits `ON_PLAYER_DEATH` on an *enemy* death (BUG-053)
  - Target shape per story S10-01: `EntityCore` is the sole implementer, health routed through `EntityStatsSO`, `EntityNegativeReciver.cs` deleted
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
| Enemy loses target mid-chase | ✅ **RESOLVED** — `EntityMoveState.LogicUpdate()` now opens with `if (!entityInput.TargetTransform)` and falls back to a 10s timeout → `IdleState` (`EntityMoveState.cs:24-33`) | Guard at top of `LogicUpdate()` — done |
| Player health ≤ 0 | **[PARTIAL]** `NegativeReciver.TakeDamage()` guards at zero and emits `ON_PLAYER_DEATH` (the event now exists in `EventID`). Still missing: it writes its own `currentHealth`, never `PlayerData.currentHealth`; `PlayerDeathState.LogicUpdate()` is commented out and the state is never constructed in `Player.Awake()` (BUG-044); no `GameManager` exists, so `PlayerData.Reborn()` has no caller | Write through to `PlayerData`; construct and restore `PlayerDeathState`; add a `GameManager` that calls `Reborn()` and reloads `StartScene` |
| Entity health ≤ 0 | ✅ transition implemented (`EntityBasicState.cs:30-34`) but **unreachable**: it reads `entity.Data.StatsSO.Health`, which nothing decrements. Damage lands on `EntityNegativeReciver.currentHealth` instead — two disconnected stores, so enemies cannot die (TD-036) | One health store, owned by `EntityStatsSO` and written through the Core hub |
| `EntityDeathState` called | ✅ **RESOLVED** — extends `EntityBasicState`, constructed in `Entity.LoadState()`, emits `ON_ENEMY_DEATH` on `EndRangeTrigger` (`EntityDeathState.cs:1,13-19`) | — |
| `EntityStatsSO.ModifiersAmor` read | **[BUG — still open, 12 weeks]** getter is `get => ModifiersAmor;` and the setter also reads and writes itself → `StackOverflowException`, which is uncatchable and kills the Editor rather than logging. Latent only because armour is not applied in the damage formula yet (`EntityStatsSO.cs:45-56`, TD-011) | Fix getter/setter to use the lowercase `modifiersAmor` field |
| Skill used without weapon | `AbilityHolder` may invoke skill while `WeaponHolder.Weapon == null` — unchecked | `PlayerBasicState` should gate skill check on `WeaponHolder.Weapon != null` |
| Enemy attack at edge of chase range | Possible: entity attacks at range 10f then exits attack but re-enters immediately | `rangeCheckAttack` must be ≤ chase range; gate checked each frame |
| `AnimationPlayerController.OnEnable` | ✅ **RESOLVED** — line 21 registers `EndAnimation`, mirrored at line 29 (Bug #9) | — |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Weapons** (`Assets/Script/Weapons/`) | `WeaponHolder.Attack()` → `Weapon.OnAttackEnter()` on state entry; `WeaponHolder.MakeDamage()` → `Weapon.OnActivate()` on the animation hit frame | Character → Weapons |
| **Skill/Ability** (`Assets/Script/Skill_Ability/`) | `ActivateSkill` SO provides ability lifecycle; `AbilityHolder` drives it | Character → Skills |
| **Event Manager** (`EventManager.cs`) | Corrected 2026-08-20 — `ON_PLAYER_DEATH` and `ON_ENEMY_DEATH` **now exist** (the enum has 20 values). Only `ON_PLAYER_TAKE_DAMAGE` is still absent, and `.claude/rules/ui-code.md` tells the health bar to bind to it | Character → EventManager |
| **Animation** (`AnimationEventManager.cs`) | `AnimationTrigger` fires weapon/skill; `AnimationFinished` exits states | Character → Animation |
| **Input** (`PlayerInputHandle.cs`, class `PlayerInputHandler`) | Provides `MoveVector`, `DirectionMouse`, `IsAttack`, `IsSkill`, `IsTakeDamage`, plus `BufferIsAttack` for combo buffering; also implements `IAimProvider` so weapons read aim direction through the interface rather than the concrete type | Input → Character |
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
| Enemy chase range | `EntityData` | `rangeCheckChase` **[GAP — field still missing]** | — | Max pursuit distance. Note: the literal `10` at `EntityMoveState.cs:27` is a no-target **timeout in seconds** after the rewrite, not a chase range (TD-019) |
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
- [ ] Enemy detects player within `rangeCheckFieldOfView` and pursues — **blocked**: `GetTargetInRange()` is commented out (TD-037)
- [ ] Enemy loses target and transitions to Idle after `moveDurationTime`
- [ ] Enemy attacks when within `rangeCheckAttack`; player health decrements
- [ ] Enemy avoids walls during random wander
- [x] Enemy null-dereference on target loss is resolved (no NullRef in console) — done

### Enemy Damage & Death
- [ ] `INegativeReceiver.TakeDamage()` decrements entity health via `EntityStatsSO.ModifiersHealth` — **currently false**: damage goes to `EntityNegativeReciver.currentHealth` instead (TD-036)
- [ ] TakeDamage state plays stun + directional knockback animation
- [ ] `health ≤ 0` transitions to `EntityDeathState`; entity despawns
- [x] `EntityDeathState` extends `EntityState` (not MonoBehaviour) — done
