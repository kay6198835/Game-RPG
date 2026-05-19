---
status: reverse-documented
source: Assets/Script/Weapons/
date: 2026-05-19
verified-by: Kiet
---

# Weapons System Design

> **Note**: Reverse-engineered from existing implementation. Captures current behaviour
> and clarified design intent. Sections marked **[GAP]** describe intended design not yet
> implemented. Sections marked **[BUG]** identify known defects.

**Status**: In Design

---

## Overview

The weapons system governs how players and enemies deal damage. Two weapon types exist:
**melee** (close-range directional attacks in combo chains) and **ranged** (projectile-based,
direction-agnostic). Each weapon is a pickable GameObject that the player equips via
interaction; enemies use a parallel EntityWeapon hierarchy.

Weapons are the primary source of damage in the dungeon. Every weapon carries two ability
slots — one for the block/ability input (RMB) and one for the skill input (E key) — linking
the weapon system directly to the skill system.

---

## Player Fantasy

Each weapon should feel distinct and rhythmic. Melee rewards commitment: you step in,
land a 3-hit combo, and back out before the enemy counters. Ranged rewards positioning:
you kite enemies at a distance, controlling the engagement range.

The directional attack system makes every attack feel intentional — swinging north vs east
has different visual feedback, grounding the action in space rather than just pressing a button.

---

## Detailed Rules

### Weapon Equip / Unequip

- Player equips a weapon by pressing **F** near a weapon pickup (via `PlayerIntertorState`)
- Only **one weapon** can be equipped at a time; equipping a second drops the current
- Unequipping re-enables the weapon's collider and detaches it from the player
- An unequipped weapon remains in the world as a pickup
- Without a weapon, the player cannot enter Attack or Skill states

### Melee Combat — 3-Hit Combo

Each melee weapon has a `List<AttackSO>` in its `WeaponMeleeStats` SO defining the combo
sequence. For the demo, all melee weapons use a **3-hit combo**: light → light → heavy.

**Combo rules:**
1. Each LMB press advances to the next `AttackSO` in the list
2. Pressing LMB before the combo window expires continues the chain
3. If the combo window expires OR the combo reaches the end, the index resets to 0
4. Each `AttackSO` has its own: hitbox range, damage value, and directional animator override

**Attack execution flow:**
1. `PlayerBasicState` checks `Weapon.CheckCanAttack(player)` each frame
2. On pass: `CheckCanAttack` loads the current `AttackSO`, swaps the Animator override, increments combo index, and records the combo window
3. Player transitions to `PlayerAttackState` — movement freezes
4. The Animator fires the `AnimationTrigger` event at the hit frame
5. `PlayerAttackState.LogicUpdate` calls `Weapon.Attack()` on that event
6. `Weapon.Attack()` runs `Physics2D.OverlapCircleAll` and applies damage **[BUG — currently empty; see Edge Cases]**
7. Animator fires `AnimationFinished` → player returns to Idle/Move

**Attack direction:** Hit center = `player.position + DirectionMouseVector × attackRange`
Direction is read from mouse position and quantized to 8 directions (45° bins).

### Ranged Combat **[GAP — incomplete, needs integration]**

Ranged weapons fire projectiles in the player-facing direction. The intended flow:

1. Player presses LMB with a ranged weapon equipped
2. `RangeWeapon.CheckCanAttack()` validates cooldown (`timeBtwShots`)
3. Player transitions to `PlayerAttackState`
4. On `AnimationTrigger`: `RangeWeapon.Attack()` calls `Shooting.shoot()`
5. `Shooting.shoot()` instantiates a bullet at `firePoint`, applies impulse force
6. Bullet travels until it hits a `BlockObject` layer (wall) or a damageable entity
7. On hit: `bullet.OnCollisionEnter2D` calls `INegativeReceiver.TakeDamage(BulletSO.dmg, transform.position)`

**Current status:**
- `RangeWeapon.Attack()` — **EMPTY STUB [BUG]**
- `RangeWeapon.CheckCanAttack()` — returns raw bool, no cooldown logic **[BUG]**
- `bullet.TakeDamage()` call — **commented out [BUG]**
- `Shooting.Update()` — reads `Input.GetMouseButton(0)` directly, bypasses state machine **[BUG]**

### Weapon Skill Slots

Every `WeaponMeleeStats` SO carries two ability references:

| Slot | Input | Field | Purpose |
|------|-------|-------|---------|
| `abilityWeapon` | RMB (held) | `WeaponStats.abilityWeapon` | Weapon-bound ability (e.g. block, special) |
| `skillWeapon` | E (held) | `WeaponStats.skillWeapon` | Weapon-bound skill (e.g. slash, dash-attack) |

`WeaponMelee.SetAbility()` reads the player's input enum and routes to the correct SO,
which is then registered with `AbilityHolder`. The ability lifecycle (Start→Cast→Do→Exit)
is driven by `AbilityHolder` per frame — not by the weapon itself.

### Block Mechanic
Out of scope for the demo. `blockDamage` and `shieldEra` fields in `WeaponMeleeStats` are
unused. The `BlockAbility` SO handles blocking when it is in scope.

---

## Formulas

```
# Melee hitbox center
hitCenter = player.transform.position + (DirectionMouseVector × currrentSA.attackRange)

# Combo window
comboOpen = (lastClickTime + durationNextAttack + deplayTime) > Time.time
            deplayTime        = 0.5f  (constant, defined in Weapon base class)
            durationNextAttack = animatorClipLength ÷ 8
            [NOTE: ÷ 8 accounts for 8 directional variants in the AnimatorOverride clip]

# Melee damage (reference: EntityWeaponMelee — working)
finalDamage = currrentSA.attackDamege
              [no multiplier, no armor reduction — raw value from AttackSO]

# Ranged bullet travel
bulletVelocity = firePoint.right × WeaponRangeStats.firerate   (impulse, not per-frame)
bulletLifetime = BulletDataSO.lifetime seconds, then auto-destroy

# Ranged cooldown
canShoot = timeBtwShots <= 0
           timeBtwShots decrements each frame; resets to StartTimeBtwShots on fire
```

---

## Edge Cases

| Scenario | Current Behaviour | Correct Behaviour |
|----------|------------------|-------------------|
| `WeaponMelee.Attack()` fires | **[BUG]** `OverlapCircleAll` runs, foreach body is empty — no damage applied | Mirror `EntityWeaponMelee.Attack()`: get `INegativeReceiver` from each hit collider, call `TakeDamage(attackDamege, transform.position)` |
| Bullet hits player/enemy | **[BUG]** Collision detected correctly but `TakeDamage()` call is commented out (line 30, bullet.cs) | Uncomment and call `INegativeReceiver.TakeDamage(BulletSO.dmg, transform.position)` |
| `RangeWeapon.Attack()` called | **[BUG]** Empty method — nothing happens | Must call `Shooting.shoot()` or equivalent |
| `Shooting.Update()` runs | **[BUG]** Reads `Input.GetMouseButton(0)` directly — fires outside state machine | Remove from Shooting.Update(); trigger only from `PlayerAttackState` on `AnimationTrigger` |
| `Weapon.SetWeaponHolder()` line 45 | **[BUG]** Code commented out — holder assignment broken | Uncomment or reroute weapon-to-holder binding |
| Combo index at end of list | Resets to 0 correctly | ✓ Correct |
| LMB while no weapon equipped | `WeaponHolder.Weapon == null` — `CheckCanAttack` should return false | Gate: `if (weapon == null) return false` in `CheckCanAttack` |
| LMB during TakeDamage state | `PlayerBasicState` transitions to TakeDamage before attack check | ✓ Priority ordering prevents this (TakeDamage > Attack) |
| Multiple enemies in hitbox | `WeaponMelee` uses `OverlapCircleAll` (hits all); `EntityWeaponMelee` uses `OverlapCircle` (first only) | Player melee should use `OverlapCircleAll` to enable multi-hit — intentional AoE for player |
| `attackDamege = 0` in default AttackSO | All attacks deal 0 damage until SO is configured | Validator: warn if `attackDamege == 0` on AttackSO assets |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Character system** (`PlayerAttackState`, `WeaponHolder`) | Calls `Weapon.Attack()` on animation event; holds the equipped weapon reference | Character → Weapons |
| **Skill/Ability system** (`ActivateSkill`, `AbilityHolder`) | Weapon SO carries ability references; `SetAbility()` registers them with `AbilityHolder` | Weapons → Skills |
| **Animation system** (`AnimationEventManager`) | `AnimationTrigger` event drives `Attack()` call; `directionAttackAnimatorOV` provides directional clips | Weapons → Animation |
| **Interface** (`INegativeReceiver`) | All damage application goes through this interface — weapons must never call `.health` directly | Weapons → Interface |
| **Pooling** (`ObjectPooling`) | Bullets should be pooled — currently `Instantiate` every shot **[GAP]** | Weapons → Pooling |
| **Map/Room** (`RoomController`) | Room clear counts enemies; weapons drive enemy death events | Weapons → Map (indirect) |

---

## Tuning Knobs

All values in ScriptableObject assets — never hardcode in MonoBehaviours.

### Per-attack tuning (`AttackSO`)

| Field | Effect | Demo target |
|-------|--------|-------------|
| `attackRange` | Hitbox radius (units) | 1.0 (light), 1.5 (heavy) |
| `attackDamege` | Raw damage dealt | 10 (light), 20 (heavy) |
| `directionAttackAnimatorOV` | Directional clip set for this attack | one per AttackSO |

### Per-weapon tuning (`WeaponMeleeStats`)

| Field | Effect | Notes |
|-------|--------|-------|
| `layerMask` | Which layers the hitbox hits | set in Inspector |
| `attackState` | Combo sequence (`List<AttackSO>`) | 3 entries for demo |
| `abilityWeapon` | RMB ability SO reference | per-weapon |
| `skillWeapon` | E key skill SO reference | per-weapon |

### Ranged tuning (`WeaponRangeStats` + `BulletDataSO`)

| Field | Effect | Notes |
|-------|--------|-------|
| `StartTimeBtwShots` | Fire rate cooldown (seconds) | lower = faster |
| `firerate` | Bullet impulse force | higher = faster bullet |
| `BulletDataSO.lifetime` | Bullet range (indirectly) | seconds until despawn |
| `BulletDataSO.dmg` | Bullet damage | raw, no reduction |

### Global timing (`Weapon` base class)

| Field | Effect | Default |
|-------|--------|---------|
| `deplayTime` | Min delay before first attack in a new combo | 0.5s |

---

## Acceptance Criteria

### Melee — Player
- [ ] LMB advances through a 3-hit combo (light → light → heavy) with distinct animations per direction
- [ ] Each hit applies `AttackSO.attackDamege` damage to all enemies within `attackRange` via `INegativeReceiver.TakeDamage()`
- [ ] Combo resets after the combo window expires or after the 3rd hit
- [ ] Missing combo window (too slow between LMB presses) resets to hit 1
- [ ] No weapon equipped → LMB has no effect

### Melee — Enemy
- [ ] `EntityWeaponMelee.Attack()` correctly deals damage to player (already functional — regression check only)

### Ranged — Player **[all currently unimplemented]**
- [ ] LMB fires a bullet from `firePoint` in the player-facing direction
- [ ] Bullet travels at `firerate` impulse speed and despawns after `lifetime` seconds
- [ ] Bullet collision with enemy applies `BulletDataSO.dmg` via `INegativeReceiver.TakeDamage()`
- [ ] Bullet collision with wall (`BlockObject` layer) destroys bullet with no damage
- [ ] `StartTimeBtwShots` cooldown prevents rapid-fire spam
- [ ] `Shooting.Update()` no longer reads input directly — fires only from state machine trigger
- [ ] Bullets are pooled — no `Instantiate` per shot at runtime

### Weapon Management
- [ ] Player can equip a weapon by pressing F near a pickup
- [ ] Equipping a second weapon drops the first
- [ ] Ability/skill slots on weapon SO are correctly registered with `AbilityHolder` on equip
