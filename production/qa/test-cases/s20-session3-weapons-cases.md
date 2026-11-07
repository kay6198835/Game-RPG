# Sprint 20 — Session 3: Weapons — Manual Test Cases

**Stories**: S20-04 (BUG-106, BUG-107, BUG-110), S20-06 (BUG-105, BUG-095, BUG-072)
**Story type**: S20-04 Integration (BLOCKING, documented playtest; BUG-107 guard has no automated test — BUG-084); S20-06 Config/Data (ADVISORY)
**Evidence**: this sheet; `git diff` for S20-06 data changes

## Common preconditions

- Sessions 1-2 PASS on the same commit (S20-02 must be in — Knights need stats). Commit: `________`
- Start from `MainGamePlay` → New Game unless stated. `CombatTest.unity` may be used for damage reads if it spawns the same enemy prefabs; record the scene used.
- Damage reads: note the enemy HP before and after a hit (HP bar or `EntityVitalStats` in the Inspector, Debug mode). To exclude crits, discard any hit that shows a crit, or temporarily set `CritChance` on a runtime copy only — never on a committed asset.

---

### S3-01 — Enemies attack the player
- **Story**: S20-04 (BUG-106)
- **Preconditions**: Combat room with live enemies; champion picked.
- **Steps**:
  1. Stand still next to an enemy for 10 s without attacking.
- **Expected Result**: Enemy plays its attack animation and the player HP bar drops at least once; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S3-02 — Pooled enemies keep attacking on re-spawn
- **Story**: S20-04 (BUG-106)
- **Preconditions**: S3-01 PASS; at least one enemy killed so its pooled instance is reused in a later room.
- **Steps**:
  1. Clear a room; move to the next combat room.
  2. Stand next to an enemy for 10 s.
- **Expected Result**: Enemy attacks and deals damage; enemy carries exactly one weapon object (Hierarchy), not a duplicate.
- **Actual Result**:
- **Pass/Fail**:

### S3-03 — Player armed before champion pick
- **Story**: S20-04 (BUG-106)
- **Preconditions**: New run; champion not yet picked.
- **Steps**:
  1. Press LMB.
  2. Press `1`-`4`.
- **Expected Result**: LMB swings the default weapon; ability keys either cast default bindings or do nothing — 0 exceptions in both cases. Record which.
- **Actual Result**:
- **Pass/Fail**:

### S3-04 — Re-pick champion replaces the weapon
- **Story**: S20-04 (BUG-107 item 1, regression)
- **Preconditions**: Paladin picked.
- **Steps**:
  1. Interact with the Archer monument and confirm.
  2. Inspect the player hierarchy for weapon objects.
  3. Attack with LMB.
- **Expected Result**: 0 exceptions; exactly one weapon under the player (old one released to the pool); the Archer weapon attacks.
- **Actual Result**:
- **Pass/Fail**:

### S3-05 — Mage confirm does not throw
- **Story**: S20-04 (BUG-107 item 2)
- **Preconditions**: New run.
- **Steps**:
  1. Interact with the Mage monument; confirm.
  2. Press LMB.
- **Expected Result**: 0 exceptions on confirm (no NRE in `WeaponHolderBase.EquipDefaultWeapon`); LMB either attacks with the Mage weapon or does nothing (explicit unarmed case). Record which.
- **Actual Result**:
- **Pass/Fail**:

### S3-06 — Dropped weapon no longer attacks
- **Story**: S20-04 (BUG-110)
- **Preconditions**: Melee weapon equipped; no weapon on the floor nearby.
- **Steps**:
  1. Press F (drop) away from any floor weapon.
  2. Press LMB next to an enemy 3 times.
- **Expected Result**: Weapon lies on the floor; no attack animation and no damage on LMB.
- **Actual Result**:
- **Pass/Fail**:

### S3-07 — Picked-up melee weapon is the one that attacks
- **Story**: S20-04 (BUG-110)
- **Preconditions**: A melee weapon on the floor different from the equipped one (or the one dropped in S3-06). Record its stage-1 `attackDamage`.
- **Steps**:
  1. Stand on the floor weapon; press F (pick up).
  2. Hit an enemy once with LMB; record HP drop.
- **Expected Result**: Weapon is parented at the player's position; HP drop = PhysicalDamage + picked weapon stage-1 damage − target Defense (non-crit); the previous weapon is not still attacking.
- **Actual Result**:
- **Pass/Fail**:

### S3-08 — Picked-up ranged weapon fires and hits
- **Story**: S20-04 (BUG-110)
- **Preconditions**: A ranged weapon on the floor.
- **Steps**:
  1. Pick it up (F).
  2. Aim at an enemy; LMB.
- **Expected Result**: Arrow spawns, travels, hits; enemy HP drops. (Fails if `CanAttack()` stays false — weapon not injected.)
- **Actual Result**:
- **Pass/Fail**:

### S3-09 — Sword stage data migrated
- **Story**: S20-06 (BUG-105)
- **Preconditions**: Fix committed.
- **Steps**:
  1. Open `Assets/SO/Weapons/MeleeWeapons/SnS_State1.asset`, `SnS_State2.asset`, `SnS_State3.asset` in the Inspector.
  2. Check `git diff` / YAML.
- **Expected Result**: `Attack Damage` = 55 on State1 (and the original values on State2/3); YAML key `attackDamage:`; `AttackSO.cs` carries `[FormerlySerializedAs("attackDamege")]`.
- **Actual Result**:
- **Pass/Fail**:

### S3-10 — Sword hit deals 55 + PhysicalDamage
- **Story**: S20-06 (BUG-105)
- **Preconditions**: Paladin (sword) equipped; record PhysicalDamage from the stats panel (C) and the target's Defense from its `EnemyStatSO`.
- **Steps**:
  1. Hit one enemy with combo stage 1; record HP drop (non-crit).
  2. Repeat for stage 2 and stage 3.
- **Expected Result**: Stage 1 HP drop = 55 + PhysicalDamage − Defense (min 0); stages 2/3 = their `attackDamage` + PhysicalDamage − Defense. Not PhysicalDamage alone.
- **Actual Result**:
- **Pass/Fail**:

### S3-11 — Arrow prefab has a trigger collider
- **Story**: S20-06 (BUG-095)
- **Preconditions**: Fix committed.
- **Steps**:
  1. Open `Assets/Prefab/Particle Effect/Attack Projectile/Arrow.prefab`.
- **Expected Result**: Root has a `Collider2D` (Circle/Box) plus `Rigidbody2D` and `ProjectileBody`; no Missing Script.
- **Actual Result**:
- **Pass/Fail**:

### S3-12 — Arrow hit registers and arrow returns to pool
- **Story**: S20-06 (BUG-095)
- **Preconditions**: Ranged weapon equipped (Archer).
- **Steps**:
  1. Fire 10 arrows at an enemy.
  2. Watch the pool hierarchy count under `ObjectPoolManager`.
- **Expected Result**: Each arrow that touches the enemy reduces its HP and disappears; pooled arrow count stops growing after the first few shots.
- **Actual Result**:
- **Pass/Fail**:

### S3-13 — Lightning layerMask is set
- **Story**: S20-06 (BUG-072)
- **Preconditions**: None.
- **Steps**:
  1. Open `Assets/Prefab/Particle Effect/Lightning.prefab` (or its renamed successor on `sprint-20`); read `layerMask`.
- **Expected Result**: `layerMask` includes the enemy hurtbox layer; is not `Nothing`.
- **Actual Result**:
- **Pass/Fail**:

### S3-14 — Consecrate Lightning damages enemies in radius
- **Story**: S20-06 (BUG-072)
- **Preconditions**: Paladin picked; 2+ enemies close together; Mana full.
- **Steps**:
  1. Cast Consecrate so the strike lands on the enemy group; record HP before/after for each enemy inside and one enemy clearly outside the radius.
- **Expected Result**: Every enemy inside the radius loses HP; the enemy outside does not; player HP unchanged.
- **Actual Result**:
- **Pass/Fail**:
