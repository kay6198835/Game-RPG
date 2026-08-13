---
status: reverse-documented
source: Assets/Script/Weapons/
date: 2026-08-13
verified-by: Kiet
---

# Weapons System Design

> **Note**: Reverse-engineered from existing implementation. Captures current behaviour
> and clarified design intent. Sections marked **[GAP]** describe intended design not yet
> implemented. Sections marked **[BUG]** identify known defects.

**Status**: Implemented — melee and ranged share one attack state

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

### Attack Stages — Shared By Both Weapon Types [IMPLEMENTED]

Every weapon owns a `List<AttackSO> AttackStages` on its `WeaponStats` SO. A stage is one
attack: its own hitbox range, damage, and directional animator override. The list is the
data; whether pressing again advances through it is a separate behavioural decision.

**Stage rules (identical for melee and ranged):**
1. Each attack input plays `AttackStages[CurrentStageIndex]`, then advances the index modulo `StageCount`
2. The index therefore wraps to 0 after the last stage and is always a valid index into the list
3. A zero index is the signal that the chain just completed — this is what `CanChain()` tests
4. If the chain window expires, the index resets to 0 so the next attack starts from stage 1
5. The chain window equals the current stage's animation length (`Utility.DurationNextAttack`)

**Chaining is decided by `Weapon.CanChain()`, not by the list:**

| Weapon | `StageCount` | `AutoFire` | Behaviour |
|--------|--------------|------------|-----------|
| Sword | 3 | — | 3-hit chain (light → light → heavy), then the state exits |
| Bow | 3 | `false` | 3-stage draw chain, identical structure to melee |
| Pistol | 1 | `true` | replays stage 0 at the fire-rate cadence |
| Shotgun | 2 | `true` | 2 distinct stages, then loops back to stage 0 |

A one-stage ranged weapon is therefore not a degenerate combo — the index resets to 0 on
every shot, and `AutoFire` keeps the chain alive while the trigger is held.

**Attack execution flow (weapon-agnostic):**
1. `PlayerBasicState` gates entry on `inputHandler.IsAttack && weaponHolder.CanAttack()`
2. Player transitions to `PlayerAttackState` — movement freezes
3. `Enter()` calls `WeaponHolder.Attack()` → `Weapon.OnAttackEnter(player)`, which picks the stage, swaps the animator override, and records the chain window
4. Animator fires `AnimationOnAction` at the hit frame → `Weapon.OnActivate()`
5. Animator fires `AnimtionFinishTrigger` → `Weapon.OnDeactivate()`, then either chains (if input is held/buffered and `CanChain()`) or sets `Status = None` to exit to Idle/Move

`PlayerAttackState` never branches on `WeaponType`. Only `OnActivate()` and the use of the
aim direction differ between the two weapon families.

**Attack direction:** both families read `IAimProvider.AimDirection`, implemented by
`PlayerInputHandler` (mouse direction) and `EntityInput` (look direction).
Melee: hit center = `player.position + AimDirection × attackRange`.
Ranged: `firePoint.right = AimDirection`.

### Melee Specifics [IMPLEMENTED]

`MeleeWeapon.OnActivate()` runs `Physics2D.OverlapCircleNonAlloc` against a cached
`Collider2D[]` buffer sized by `maxTargetsPerSwing`, and calls
`INegativeReceiver.TakeDamage(attackDamege, transform.position)` on every hit — multi-hit
AoE is intentional for the player.

### Ranged Specifics [IMPLEMENTED]

`RangeAttackSO` extends `AttackSO` with the projectile payload: `BulletPrefab`,
`BulletData`, `ProjectileCount`, `SpreadAngle`, `RecoveryTime`.

`RangeWeapon.OnActivate()` spawns `ProjectileCount` bullets from `ObjectPoolManager`
(pooled — no `Instantiate` per shot), fanned across `SpreadAngle` centred on the aim
direction, then sets `nextFireTime = Time.time + RecoveryTime`.

`bullet.cs` reads its speed and lifetime from `BulletDataSO`, applies damage through
`INegativeReceiver`, and returns itself to the pool on hit, on wall contact
(`blockMask`), or on lifetime expiry.

**Fire rate lives per stage** (`RangeAttackSO.RecoveryTime`), not on the weapon — a charged
shot and a quick shot on the same weapon need different recovery. The former weapon-level
`firerate` / `timeBtwShots` / `StartTimeBtwShots` fields are removed; `timeBtwShots` was a
runtime countdown stored in a shared SO asset, which persisted across play sessions.

### Weapon Skill Slots

Every `WeaponStats` SO carries two ability references:

| Slot | Input | Field | Purpose |
|------|-------|-------|---------|
| `abilityWeapon` | RMB (held) | `WeaponStats.abilityWeapon` | Weapon-bound ability (e.g. block, special) |
| `skillWeapon` | E (held) | `WeaponStats.skillWeapon` | Weapon-bound skill (e.g. slash, dash-attack) |

`Weapon.SetAbility()` reads the player's input enum and routes to the correct SO,
which is then registered with `AbilityHolder`. The ability lifecycle (Start→Cast→Do→Exit)
is driven by `AbilityHolder` per frame — not by the weapon itself.

### Block Mechanic
Out of scope for the demo. `blockDamage` and `shieldEra` fields in `MeleeWeaponStats` are
unused. The `BlockAbility` SO handles blocking when it is in scope.

---

## Formulas

```
# Melee hitbox center
hitCenter = player.transform.position + (AimDirection.normalized × currentStage.attackRange)

# Chain window (shared by both weapon families)
chainOpen  = (lastAttackTime + chainWindow) > Time.time
chainWindow = Utility.DurationNextAttack(overrideClips) ÷ player.Anim.speed
              [DurationNextAttack averages the 8 directional variants of the clip set]

# Stage selection (shared)
if (CurrentStageIndex >= StageCount || !chainOpen) CurrentStageIndex = 0
currentStage = AttackStages[CurrentStageIndex]
CurrentStageIndex = (CurrentStageIndex + 1) % StageCount

# Chain permission
Weapon.CanChain()      = CanAttack() && CurrentStageIndex != 0
RangeWeapon.CanChain() = CanAttack() && (AutoFire || CurrentStageIndex != 0)
RangeWeapon.CanAttack() = base.CanAttack() && Time.time >= nextFireTime

# Melee damage
finalDamage = currentStage.attackDamege
              [no multiplier, no armor reduction — raw value from AttackSO]

# Ranged spread (ProjectileCount > 1)
step       = SpreadAngle ÷ (ProjectileCount - 1)
startAngle = aimAngle - (SpreadAngle ÷ 2)
angle[i]   = startAngle + step × i

# Ranged cooldown
nextFireTime = Time.time + RangeAttackSO.RecoveryTime

# Bullet travel
bulletVelocity = transform.right × BulletDataSO.speed
bulletLifetime = BulletDataSO.lifetime seconds, then released back to the pool
```

---

## Edge Cases

| Scenario | Behaviour |
|----------|-----------|
| Melee hit frame | `OverlapCircleNonAlloc` + `TakeDamage(attackDamege, transform.position)` on every hit collider ✓ |
| Bullet hits player/enemy | `bullet.OnCollisionEnter2D` resolves `INegativeReceiver` and calls `TakeDamage(BulletSO.dmg, ...)`, then releases to the pool ✓ |
| Bullet hits a wall | `blockMask` match → released to the pool, no damage ✓ |
| Bullet outlives `lifetime` | Released to the pool by the `Update()` timer, never `Destroy`ed ✓ |
| Bullet has no `PoolMember` (hand-placed in a scene) | Falls back to `Destroy(gameObject)` ✓ |
| Stage index passes end of list | Resets to 0 ✓ |
| Attack input while no weapon equipped | `WeaponHolder.CanAttack()` returns false — `PlayerBasicState` never enters `PlayerAttackState` ✓ |
| Attack input while ranged weapon is on cooldown | `RangeWeapon.CanAttack()` returns false; the state is not re-entered, so the cooldown cannot be bypassed by leaving and re-entering ✓ |
| Attack finishes with no buffered input | `Status = None` → `PlayerUseWeaponState` exits to Idle/Move ✓ (previously the status stayed at `EndRangeTrigger` and the player was stuck in the attack state) |
| Attack input during TakeDamage state | `PlayerBasicState` transitions to TakeDamage before the attack check ✓ (TakeDamage > Attack) |
| Multiple enemies in melee hitbox | All are hit, bounded by `maxTargetsPerSwing` — intentional AoE for the player ✓ |
| Ranged stats SO wired onto a melee weapon (or vice versa) | `CanAttack()` returns false instead of throwing `InvalidCastException` ✓ |
| `attackDamege = 0` in an AttackSO | All attacks deal 0 damage until the SO is configured — **[GAP]** no validator warns about this yet |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Character system** (`PlayerAttackState`, `WeaponHolder`) | Calls `Weapon.Attack()` on animation event; holds the equipped weapon reference | Character → Weapons |
| **Skill/Ability system** (`ActivateSkill`, `AbilityHolder`) | Weapon SO carries ability references; `SetAbility()` registers them with `AbilityHolder` | Weapons → Skills |
| **Animation system** (`AnimationEventManager`) | `AnimationTrigger` event drives `Attack()` call; `directionAttackAnimatorOV` provides directional clips | Weapons → Animation |
| **Interface** (`INegativeReceiver`) | All damage application goes through this interface — weapons must never call `.health` directly | Weapons → Interface |
| **Pooling** (`ObjectPoolManager` / `Pool`) | Ranged weapons spawn every projectile through the pool; bullets release themselves back | Weapons → Pooling |
| **Map/Room** (`RoomController`) | Room clear counts enemies; weapons drive enemy death events | Weapons → Map (indirect) |

---

## Tuning Knobs

All values in ScriptableObject assets — never hardcode in MonoBehaviours.

### Per-stage tuning (`AttackSO` — both weapon families)

| Field | Effect | Demo target |
|-------|--------|-------------|
| `attackRange` | Melee hitbox radius / ranged muzzle offset (units) | 1.0 (light), 1.5 (heavy) |
| `attackDamege` | Raw damage dealt | 10 (light), 20 (heavy) |
| `directionAttackAnimatorOV` | Directional clip set for this stage | one per stage |

### Per-stage ranged tuning (`RangeAttackSO`)

| Field | Effect | Notes |
|-------|--------|-------|
| `BulletPrefab` | Which projectile to pool and spawn | required |
| `BulletData` | `BulletDataSO` carrying speed / lifetime / damage | required |
| `ProjectileCount` | Projectiles per shot | 1 = single, >1 = shotgun fan |
| `SpreadAngle` | Total fan width in degrees | 0 for a single accurate shot |
| `RecoveryTime` | Cooldown before the next shot (seconds) | lower = faster |

### Per-weapon tuning (`WeaponStats`)

| Field | Effect | Notes |
|-------|--------|-------|
| `LayerMask` | Which layers the melee hitbox hits | set in Inspector |
| `AttackStages` | Stage list (`List<AttackSO>`) | 3 entries for melee, 1+ for ranged |
| `AbilityWeapon` | RMB ability SO reference | per-weapon |
| `SkillWeapon` | E key skill SO reference | per-weapon |
| `AutoFire` (ranged only) | Holding the trigger replays the stage list | true for pistols, false for a draw chain |

### Bullet tuning (`BulletDataSO`)

| Field | Effect | Notes |
|-------|--------|-------|
| `speed` | Projectile velocity (units/sec) | set on the SO, applied on spawn |
| `lifetime` | Bullet range, indirectly (seconds) | released to the pool on expiry |
| `dmg` | Bullet damage | raw, no reduction |
| `targetMask` | Which layers the bullet damages | set in Inspector |

### Per-weapon-instance tuning (MonoBehaviour Inspector)

| Field | Effect | Default |
|-------|--------|---------|
| `maxTargetsPerSwing` (`MeleeWeapon`) | Hit buffer size — caps multi-hit AoE | 8 |
| `firePoint` (`RangeWeapon`) | Muzzle transform, rotated to the aim direction | required |
| `blockMask` (`bullet`) | Layers that stop a projectile without damage | walls |

---

## Acceptance Criteria

### Melee — Player
- [ ] LMB advances through a 3-stage chain (light → light → heavy) with distinct animations per direction
- [ ] Each hit applies `AttackSO.attackDamege` damage to all enemies within `attackRange` via `INegativeReceiver.TakeDamage()`
- [ ] Chain resets after the chain window expires or after the 3rd hit
- [ ] Missing the chain window (too slow between LMB presses) resets to stage 1
- [ ] No weapon equipped → LMB has no effect
- [ ] Attack state exits to Idle/Move when the animation finishes with no buffered input

### Melee — Enemy
- [ ] `EntityWeaponMelee.Attack()` correctly deals damage to player (regression check only)

### Ranged — Player
- [ ] LMB fires a bullet from `firePoint` in the aim direction
- [ ] Bullet travels at `BulletDataSO.speed` and returns to the pool after `lifetime` seconds
- [ ] Bullet collision with an enemy applies `BulletDataSO.dmg` via `INegativeReceiver.TakeDamage()`
- [ ] Bullet collision with a `blockMask` layer returns it to the pool with no damage
- [ ] `RangeAttackSO.RecoveryTime` prevents rapid-fire spam, and cannot be bypassed by leaving and re-entering the attack state
- [ ] Bullets are pooled — no `Instantiate` per shot after the pool warms up
- [ ] `ProjectileCount > 1` fans projectiles evenly across `SpreadAngle`

### Ranged — Stage chaining
- [ ] A 1-stage weapon with `AutoFire = true` repeats stage 0 while the trigger is held
- [ ] A 3-stage weapon with `AutoFire = false` runs the chain exactly like a melee combo, then exits
- [ ] No code path in `PlayerAttackState` branches on `WeaponType`

### Weapon Management
- [ ] Player can equip a weapon by pressing F near a pickup
- [ ] Unequipping clears `WeaponHolder.Weapon` so a different weapon can be picked up
- [ ] Ability/skill slots on weapon SO are correctly registered with `AbilityHolder` on equip
