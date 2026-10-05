---
status: reverse-documented
source: Assets/Script/Weapons/
date: 2026-10-05
verified-by: Kiet (re-synced against HEAD 93ba6d8e by doc-sync)
---

# Weapons System Design

> 📜 Change log: [changelog/weapons-system.CHANGELOG.md](changelog/weapons-system.CHANGELOG.md)

> **Re-synced 2026-10-05 against HEAD `93ba6d8e`.** What changed since the 2026-09-11 version
> (full trail in the change log):
>
> - **Ranged projectiles rebuilt (2026-09-28).** `bullet.cs` and `BulletDataSO` are deleted. Every
>   projectile is a pooled `ProjectileBody` configured by a `ProjectileConfig` on the stage, and the
>   weapon receives hits through `IProjectilePayload.OnHit()`. Projectile abilities use the same body.
> - **Weapons no longer carry abilities.** `WeaponStats.AbilityWeapon` / `.SkillWeapon`,
>   `AttackSO.ability` and `Weapon.currentAbilitySO` are deleted. Abilities come from
>   `CharacterData.AbilityBindings` and run on keys `1`-`4` whether or not a weapon is equipped.
> - **Damage formula:** `PhysicalDamage + attackDamage (+ CritDamage)` computed by the holder, not the raw stage value.
> - **`attackDamege` → `attackDamage`** (2026-09-01). Old assets still carry the old key — **BUG-095**.
> - **Any character can hold a weapon** (`IWeaponHolder`, ADR-0005). Enemies attack only through a
>   weapon; `EntityAttack` / `EntityWeaponMelee` are deleted.
> - ✅ BUG-064 sub-item 7 closed — weapons are injected on equip.
> - ⚠️ **BUG-093:** the ranged `RecoveryTime` gate was dropped from `CanAttack()`, and ranged hits
>   ignore the computed damage.


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

Weapons are the primary source of damage in the dungeon. Since 2026-09-28 they are **independent
of abilities**: a character's abilities come from its `CharacterData.AbilityBindings`, not from the
weapon it holds. Any character (player or enemy) holds a weapon through `IWeaponHolder`.

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

- Each character spawns holding `CharacterData.DefaultWeapon` (a `WeaponSO` naming a prefab with a `Weapon`)
- Player equips a weapon by pressing **F** near a weapon pickup (via `PlayerIntertorState` → `Weapon.Interact()`)
- `WeaponHolderBase.Equid()` injects the weapon's `[Inject]` dependencies (VContainer `InjectGameObject`) on every equip
- Only **one weapon** can be equipped at a time; equipping a second drops the current
- Unequipping re-enables the weapon's collider and detaches it from the player
- An unequipped weapon remains in the world as a pickup
- Without a weapon, a character cannot attack (no fallback attack path on either side). Abilities still work

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
5. Animator fires `AnimationFinishTrigger` → `Weapon.OnDeactivate()`, then either chains (if input is held/buffered and `CanChain()`) or sets `Status = None` to exit to Idle/Move

`PlayerAttackState` never branches on `WeaponType`. Only `OnActivate()` and the use of the
aim direction differ between the two weapon families.

**Attack direction:** both families read `IAimProvider.AimDirection`, implemented by
`PlayerInputHandler` (mouse direction) and `EntityInput` (look direction).
Melee: hit center = `player.position + AimDirection × attackRange`.
Ranged: `firePoint = ownerPosition + AimDirection × spawnOffset`, computed in `OnAttackEnter()`.

### Melee Specifics [IMPLEMENTED]

`MeleeWeapon.OnActivate()` runs `Physics2D.OverlapCircleNonAlloc` against a cached
`Collider2D[]` buffer sized by `maxTargetsPerSwing`, and calls
`INegativeReceiver.TakeDamage(finalDamage, transform.position)` on every hit — multi-hit
AoE is intentional for the player. `finalDamage` is computed by
`WeaponHolderBase.CalculateCurrentDamage()` and passed into `OnActivate(finalDamage)`.

### Ranged Specifics [IMPLEMENTED]

`RangeAttackSO` extends `AttackSO` with the projectile payload: `ProjectilePrefab`,
`ProjectileCount`, `SpreadAngle`, `RecoveryTime` and a `ProjectileConfig`
(`speed`, `lifetime`, `targetMask`, `blockMask`).

`RangeWeapon : Weapon, IProjectilePayload`. `OnActivate()` spawns `ProjectileCount`
projectiles from `IObjecPoolService` (pooled — no `Instantiate` per shot), fanned across
`SpreadAngle` centred on the aim direction, and calls `ProjectileBody.Launch(direction, config,
payload: this)` on each. It then writes `nextFireTime = Time.time + RecoveryTime` —
⚠️ **which nothing reads (BUG-093)**, so the cooldown is not enforced today.

`ProjectileBody` (shared with projectile abilities) moves by `Rigidbody2D` velocity and handles
`OnTriggerEnter2D`: a `blockMask` layer releases it with no damage; a `targetMask` layer calls
`payload.OnHit(target, position)` and releases it; lifetime expiry releases it. A body that was not
spawned by the pool destroys itself instead. `RangeWeapon.OnHit()` resolves `INegativeReceiver` on
the hit collider and deals `currentStage.attackDamage` — ⚠️ the raw stage value, not the
`finalDamage` melee uses (BUG-093).

**Fire rate lives per stage** (`RangeAttackSO.RecoveryTime`), not on the weapon — a charged
shot and a quick shot on the same weapon need different recovery. The former weapon-level
`firerate` / `timeBtwShots` / `StartTimeBtwShots` fields are removed; `timeBtwShots` was a
runtime countdown stored in a shared SO asset, which persisted across play sessions.

### Weapon Skill Slots — REMOVED 2026-09-28

Weapons no longer carry ability references. `WeaponStats.AbilityWeapon` / `.SkillWeapon` and
`AttackSO.ability` are deleted, `Weapon.currentAbilitySO` is commented out, and
`Weapon.SetAbility()` is an empty stub ("fix later"). Abilities are bound per character in
`CharacterData.AbilityBindings` and triggered on keys `1`-`4` (Primary / Secondary / Utility /
Ultimate) — see `docs/systems/abilities/README.md`. The RMB block handler is commented out in
`PlayerInputHandler`. Only `EntityWeapon` still references Abilities v1 (`ActivateSkill`).

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
RangeWeapon.CanAttack() = base.CanAttack() && StatsRange != null && poolService != null
                          [⚠️ the `Time.time >= nextFireTime` term was dropped on 2026-09-28 — BUG-093]

# Melee damage (WeaponHolderBase.CalculateCurrentDamage)
finalDamage = PhysicalDamage + currentStage.attackDamage
if RollChance(CritChance): finalDamage += CritDamage
[stats read from the holder's IVitalComponent; the receiver then applies Mitigate()
 — enemies: finalDamage - Defense, clamped at 0]

# Ranged damage (RangeWeapon.OnHit) — ⚠️ BUG-093
rangedDamage = currentStage.attackDamage      [no PhysicalDamage, no crit]

# Ranged spread (ProjectileCount > 1)
step       = SpreadAngle ÷ (ProjectileCount - 1)
startAngle = aimAngle - (SpreadAngle ÷ 2)
angle[i]   = startAngle + step × i

# Ranged cooldown
nextFireTime = Time.time + RangeAttackSO.RecoveryTime

# Projectile travel (ProjectileBody)
velocity = direction.normalized × ProjectileConfig.speed
lifetime = ProjectileConfig.lifetime seconds, then released back to the pool
```

---

## Edge Cases

| Scenario | Behaviour |
|----------|-----------|
| Melee hit frame | `OverlapCircleNonAlloc` + `TakeDamage(finalDamage, transform.position)` on every hit collider ✓ |
| Projectile hits a `targetMask` layer | `ProjectileBody.OnTriggerEnter2D` → `RangeWeapon.OnHit()` → `TakeDamage(currentStage.attackDamage, pos)`, then released ✓ |
| Projectile hits a `blockMask` layer | Released to the pool, no damage ✓ |
| Projectile hits a layer in neither mask | Ignored — keeps flying ✓ |
| Projectile outlives `lifetime` | Released by the despawn coroutine ✓ |
| Projectile not spawned by the pool (hand-placed) | `Destroy(gameObject)` ✓ |
| Pooled projectile reused | `OnDisable` clears payload and velocity — no carry-over from the last shot ✓ |
| Stage index passes end of list | Resets to 0 ✓ |
| Attack input while no weapon equipped | `WeaponHolder.CanAttack()` returns false — `PlayerBasicState` never enters `PlayerAttackState` ✓ |
| Attack input while ranged weapon is on cooldown | ⚠️ **Not enforced** — `CanAttack()` no longer tests `nextFireTime` (BUG-093). Fire rate is bounded only by the attack animation |
| Attack finishes with no buffered input | `Status = None` → `PlayerUseWeaponState` exits to Idle/Move ✓ (previously the status stayed at `EndRangeTrigger` and the player was stuck in the attack state) |
| Attack input during TakeDamage state | `PlayerBasicState` transitions to TakeDamage before the attack check ✓ (TakeDamage > Attack) |
| Multiple enemies in melee hitbox | All are hit, bounded by `maxTargetsPerSwing` — intentional AoE for the player ✓ |
| Ranged stats SO wired onto a melee weapon (or vice versa) | `CanAttack()` returns false instead of throwing `InvalidCastException` ✓ |
| `attackDamage = 0` in an AttackSO | Melee hits for `PhysicalDamage` only; ranged hits for 0 — **[GAP]** no validator warns. ⚠️ This is the live state of `SnS_State1-3.asset` (BUG-095: old key `attackDamege` not migrated) |
| Character with no weapon | `WeaponHolderBase.MakeDamage()` returns early; no attack state entered ✓ |

---

## Dependencies

| System | Role | Direction |
|--------|------|-----------|
| **Character system** (`PlayerAttackState`, `WeaponHolder`) | Calls `Weapon.Attack()` on animation event; holds the equipped weapon reference | Character → Weapons |
| **Skill/Ability system** | No link since 2026-09-28 — abilities come from `CharacterData`. Shared piece: `ProjectileBody` / `IProjectilePayload` serve both ranged weapons and projectile abilities | Shared runtime |
| **Stats** (`IVitalComponent`) | `PhysicalDamage`, `CritChance`, `CritDamage` read by `WeaponHolderBase.CalculateCurrentDamage()` | Weapons → Stats |
| **Dependency injection** (VContainer) | `WeaponHolderBase.Equid()` injects the weapon; `RangeWeapon.Construct(IObjecPoolService)` | DI → Weapons |
| **Animation system** (`AnimationEventManager`) | `AnimationTrigger` event drives `Attack()` call; `directionAttackAnimatorOV` provides directional clips | Weapons → Animation |
| **Interface** (`INegativeReceiver`) | All damage application goes through this interface — weapons must never call `.health` directly | Weapons → Interface |
| **Pooling** (`ObjectPoolManager` / `Pool`) | Ranged weapons spawn every projectile through the pool; each `ProjectileBody` releases itself back | Weapons → Pooling |
| **Map/Room** (`RoomCell`) | Room clear counts enemies; weapons drive enemy death events | Weapons → Map (indirect) |

---

## Tuning Knobs

All values in ScriptableObject assets — never hardcode in MonoBehaviours.

### Per-stage tuning (`AttackSO` — both weapon families)

| Field | Effect | Demo target |
|-------|--------|-------------|
| `attackRange` | Melee hitbox radius / ranged muzzle offset (units) | 1.0 (light), 1.5 (heavy) |
| `attackDamage` | Stage damage, added to the holder's `PhysicalDamage` (was `attackDamege` until 2026-09-01) | 10 (light), 20 (heavy) |
| `directionAttackAnimatorOV` | Directional clip set for this stage | one per stage |

### Per-stage ranged tuning (`RangeAttackSO`)

| Field | Effect | Notes |
|-------|--------|-------|
| `ProjectilePrefab` | Prefab with a `ProjectileBody` (+ `Rigidbody2D`, a 2D collider) | required |
| `ProjectileConfig` | `speed` (1-60), `lifetime` (0.1-30 s), `targetMask`, `blockMask` | required |
| `ProjectileCount` | Projectiles per shot | 1 = single, >1 = shotgun fan |
| `SpreadAngle` | Total fan width in degrees | 0 for a single accurate shot |
| `RecoveryTime` | Cooldown before the next shot (seconds) | lower = faster |

### Per-weapon tuning (`WeaponStats`)

| Field | Effect | Notes |
|-------|--------|-------|
| `LayerMask` | Which layers the melee hitbox hits | set in Inspector |
| `AttackStages` | Stage list (`List<AttackSO>`) | 3 entries for melee, 1+ for ranged |
| `StatModifiers` | `StatModifierGroup` applied to the holder's stats on equip | per-weapon |
| `AutoFire` (ranged only) | Holding the trigger replays the stage list | true for pistols, false for a draw chain |

### Projectile tuning (`ProjectileConfig`, embedded in `RangeAttackSO`)

| Field | Effect | Notes |
|-------|--------|-------|
| `speed` | Projectile velocity (units/sec) | applied in `ProjectileBody.Launch()` |
| `lifetime` | Projectile range, indirectly (seconds) | released to the pool on expiry |
| `targetMask` | Layers that receive `OnHit()` | set in Inspector |
| `blockMask` | Layers that stop the projectile without damage | walls |

`BulletDataSO` (and its `dmg` field) was deleted on 2026-09-28; projectile damage now comes from the stage.

### Per-weapon-instance tuning (MonoBehaviour Inspector)

| Field | Effect | Default |
|-------|--------|---------|
| `maxTargetsPerSwing` (`MeleeWeapon`) | Hit buffer size — caps multi-hit AoE | 8 |
| `spawnOffset` (`RangeWeapon`) | Distance from the owner at which projectiles spawn | 0.5 |
| `firePoint` (`RangeWeapon`) | Serialized `Vector2`, **overwritten every shot** — runtime state, not a knob | — |

---

## Acceptance Criteria

### Melee — Player
- [ ] LMB advances through a 3-stage chain (light → light → heavy) with distinct animations per direction
- [ ] Each hit applies `PhysicalDamage + AttackSO.attackDamage` (+ crit) to all enemies within `attackRange` via `INegativeReceiver.TakeDamage()` — blocked by BUG-095 for the SnS stages
- [ ] Chain resets after the chain window expires or after the 3rd hit
- [ ] Missing the chain window (too slow between LMB presses) resets to stage 1
- [ ] No weapon equipped → LMB has no effect
- [ ] Attack state exits to Idle/Move when the animation finishes with no buffered input

### Melee — Enemy
- [ ] An enemy whose `EntityData.DefaultWeapon` names a melee weapon damages the player through the same `MeleeWeapon.OnActivate()` path
- [ ] An enemy with no weapon never attacks

### Ranged — Player
- [ ] LMB fires a projectile from `ownerPosition + AimDirection × spawnOffset` in the aim direction
- [ ] Projectile travels at `ProjectileConfig.speed` and returns to the pool after `lifetime` seconds
- [ ] Projectile hitting a `targetMask` collider applies damage via `INegativeReceiver.TakeDamage()`
- [ ] Projectile hitting a `blockMask` collider returns to the pool with no damage
- [ ] `RangeAttackSO.RecoveryTime` prevents rapid-fire spam — **fails today (BUG-093)**
- [ ] Projectiles are pooled — no `Instantiate` per shot after the pool warms up
- [ ] `ProjectileCount > 1` fans projectiles evenly across `SpreadAngle`

### Ranged — Stage chaining
- [ ] A 1-stage weapon with `AutoFire = true` repeats stage 0 while the trigger is held
- [ ] A 3-stage weapon with `AutoFire = false` runs the chain exactly like a melee combo, then exits
- [ ] No code path in `PlayerAttackState` branches on `WeaponType`

### Weapon Management
- [ ] Player can equip a weapon by pressing F near a pickup
- [ ] Unequipping clears `WeaponHolder.Weapon` so a different weapon can be picked up
- [ ] Equipping injects the weapon (a ranged weapon fires on its first attack after pickup)
- [ ] Abilities work with and without a weapon equipped
