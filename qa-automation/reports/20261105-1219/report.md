# Weekly QA automation report

- Run `20261105-1219` | sprint `origin/sprint-21` at `4097be4cce` | baseline `4b2133fb2d`
- Owner branch `sprint-21` | tree was clean
- Unity 2022.3.62f3 | priority <= P1 + deep P2 for: ENT | seed 20261031
- **Exit 1** - 2 unknown FAIL(s)

| PASS | FAIL | KNOWN | SKIP | OBSERVED | Total |
|---|---|---|---|---|---|
| 60 | 2 | 10 | 13 | 2 | 87 |

FAIL = unexpected, needs a look. KNOWN = fails because of an open bug file (listed). OBSERVED = recorded where the design is undecided. SKIP = not testable as built (reason in Details).

## 1. Compared with the previous run

Previous: run `20261105-1102` at `4b2133fb2d`.

### New FAIL (0)

- none

### Fixed since last run (0)

- none

### Still failing (2)

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ENT-04 | P0 | Lethal damage emits ON_ENEMY_DEATH once with the enemy as payload | **FAIL** |  | FAIL: emitted 2 times |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 6, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |

### Flaky suspects (0)

- none

## 2. Changed this week

Change range `4b2133fb2d`..`4097be4cce`: 5 commits, 30 files. Details: `weekly-change-impact.md`.

Cases of the touched systems (ABL, CMB, ENT, PLY, STAT, STATIC, UI, VIT): 45 PASS, 2 FAIL, 10 KNOWN.

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ENT-04 | P0 | Lethal damage emits ON_ENEMY_DEATH once with the enemy as payload | **FAIL** |  | FAIL: emitted 2 times |
| QA-CMB-01.Paladin | P0 | Paladin: melee hit damage = PhysicalDamage + stage damage | **KNOWN** | BUG-105 | FAIL: stage 0 attackDamage = 0 (stage damage lost) |
| QA-ABL-01.Paladin.Utility | P1 | Paladin Blessing: cast pays cost, starts cooldown, applies effect | **KNOWN** | BUG-114 | entered PlayerSkillWeaponState: True<br>effects: Paladin Buff - Debuff Stat Effect (BuffDebuffStatsForDuration), Paladin Recovery Reduction Stats Effect (RecoveryReductionStatsEffect)<br>FAIL: no stat change and no recovery within 2.5s |
| QA-CMB-04.Archer | P0 | Archer: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Archer | P1 | Archer: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Archer.Primary | P0 | Archer Hawk Eye: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerAttackState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Primary | P1 | Archer Hawk Eye: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Archer.Secondary | P1 | Archer Kick-back Trap: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-01.Archer.Utility | P1 | Archer Volley: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Utility | P1 | Archer Volley: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-PLY-05.Mage | P0 | Mage: Confirm applies the class | **OBSERVED** |  | OBSERVED: Mage has no default weapon in data |
| QA-ABL-01.Mage.Primary | P0 | Mage Fireball: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Primary | P1 | Mage Fireball: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Secondary | P1 | Mage Chain Lightning: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Secondary | P1 | Mage Chain Lightning: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Utility | P1 | Mage Frost Nova: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Utility | P1 | Mage Frost Nova: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Ultimate | P1 | Mage Meteor: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Ultimate | P1 | Mage Meteor: no recast while on cooldown | **OBSERVED** |  | OBSERVED: cooldown 1s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input |
| QA-STATIC-01 | P0 | No asset or prefab references a missing script | **KNOWN** | BUG-073, baseline debt (82) | FAIL: 83 finding(s)<br>Assets/Prefab/Bullet/BulletEnnemy.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/BulletPlayer.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/SpiritProjcetile.prefab: script guid 405e5c1b5d35de1428150d32c25b0bf1<br>Assets/Prefab/Core/CoreGame.prefab: script guid 1a76f0427ff553c419f560d0fc0d03fe<br>Assets/Prefab/Core/CoreGame.prefab: script guid 94c1121f7993220458a7cce8d02e297c |
| QA-STATIC-02 | P1 | No ScriptableObject under Assets/SO references a deleted asset | **KNOWN** | BUG-090, baseline debt (16) | FAIL: 19 finding(s)<br>Assets/SO/Database/Enemy/Assasin.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Boss.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/FastSwarm.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/RangeCaster.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Tank.asset: guid babc5d466b4a4854ca630ec23b670512 |
| QA-STATIC-03 | P1 | No serialized key left behind by a field rename (missing [FormerlySerializedAs]) | **KNOWN** | BUG-105, BUG-108, baseline debt (98) | FAIL: 111 finding(s)<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): maxHealth<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): movementVelocities<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): rangeCheckAttack<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): weaponSO<br>Assets/SO/Enemy/Enemy - Knight 2.asset (EntityData): maxHealth |
| QA-STATIC-04 | P0 | No runtime stat modifier saved into a stat asset | **KNOWN** | BUG-115 | FAIL: 2 finding(s)<br>Assets/SO/Stat/Player/ArcherStats.asset: 1 saved modifier(s)<br>Assets/SO/Stat/Player/MageStats.asset: 2 saved modifier(s) |
| QA-STATIC-05 | P0 | Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR | **KNOWN** | BUG-116 | FAIL: 1 finding(s)<br>Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4: using UnityEditor; |
| QA-VIT-05 | P1 | Recovery never pushes the value above max | **KNOWN** | BUG-119 | FAIL: HP 299 above max 294 after max-5 +10 |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 6, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |
| QA-VIT-08 | P1 | ON_PLAYER_DEATH is emitted once per death, not every frame | **SKIP** |  | SKIP: the player never emitted ON_PLAYER_DEATH (see QA-VIT-07) |

New scripts with no case yet: none

## 3. Hygiene

- No tracked asset was left modified by the run.

## 4. All results

### ENEMY

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ENT-01 | P0 | Pool spawns the enemy prefab with its core components and full HP | **PASS** |  |  |
| QA-ENT-02 | P0 | Spawned enemy has its default weapon equipped | **PASS** |  |  |
| QA-ENT-03 | P0 | Damage lowers HP by amount minus Defense | **PASS** |  |  |
| QA-ENT-04 | P0 | Lethal damage emits ON_ENEMY_DEATH once with the enemy as payload | **FAIL** |  | FAIL: emitted 2 times |
| QA-ENT-05 | P1 | Released then re-spawned enemy comes back with full HP | **PASS** |  |  |
| QA-ENT-06 | P2 | Enemy stat asset unchanged after spawn, damage and respawn | **PASS** |  |  |
| QA-ENT-07 | P1 | Spawned enemy reads move speed and attack range from its stats | **PASS** |  |  |

### PLAYER

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-PLY-01 | P0 | Player spawns once at the spawn point | **PASS** |  | data before pick: BaseData |
| QA-PLY-03.Paladin | P0 | Paladin: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Paladin | P1 | Paladin: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Paladin | P0 | Paladin: Confirm applies the class | **PASS** |  |  |
| QA-UI-01.Paladin | P1 | Paladin: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Paladin Blessed Slash Ability (1s, icon no), [2] Consecrate (1s, icon no), [3] Blessing (1s, icon no), [4] Avatar of Light (1s, icon no) |
| QA-CMB-01.Paladin | P0 | Paladin: melee hit damage = PhysicalDamage + stage damage | **KNOWN** | BUG-105 | FAIL: stage 0 attackDamage = 0 (stage damage lost) |
| QA-CMB-02.Paladin | P1 | Paladin: melee combo reaches every stage | **PASS** |  | hits: 3 |
| QA-CMB-03.Paladin | P1 | Paladin: melee out of range deals no damage | **PASS** |  |  |
| QA-ABL-01.Paladin.Primary | P0 | Paladin Paladin Blessed Slash Ability: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Paladin Spawn Slash Effect (SpawnProjectileEffect) |
| QA-ABL-02.Paladin.Primary | P1 | Paladin Paladin Blessed Slash Ability: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-01.Paladin.Secondary | P1 | Paladin Consecrate: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Paladin Spawn Consecrat Effect (SpawnSummonEffect) |
| QA-ABL-02.Paladin.Secondary | P1 | Paladin Consecrate: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-01.Paladin.Utility | P1 | Paladin Blessing: cast pays cost, starts cooldown, applies effect | **KNOWN** | BUG-114 | entered PlayerSkillWeaponState: True<br>effects: Paladin Buff - Debuff Stat Effect (BuffDebuffStatsForDuration), Paladin Recovery Reduction Stats Effect (RecoveryReductionStatsEffect)<br>FAIL: no stat change and no recovery within 2.5s |
| QA-ABL-02.Paladin.Utility | P1 | Paladin Blessing: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-01.Paladin.Ultimate | P1 | Paladin Avatar of Light: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Recovery Stat Effect (RecoveryReductionStatsEffect), Gain For Time Effect (BuffDebuffStatsForDuration) |
| QA-ABL-02.Paladin.Ultimate | P1 | Paladin Avatar of Light: no recast while on cooldown | **PASS** |  |  |
| QA-PLY-03.Archer | P0 | Archer: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Archer | P1 | Archer: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Archer | P0 | Archer: Confirm applies the class | **PASS** |  |  |
| QA-UI-01.Archer | P1 | Archer: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Hawk Eye (1s, icon no), [2] Kick-back Trap (1s, icon no), [3] Volley (1s, icon no), [4] Rain Arrows (1s, icon no) |
| QA-CMB-04.Archer | P0 | Archer: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Archer | P1 | Archer: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Archer.Primary | P0 | Archer Hawk Eye: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerAttackState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Primary | P1 | Archer Hawk Eye: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Archer.Secondary | P1 | Archer Kick-back Trap: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Secondary | P1 | Archer Kick-back Trap: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-01.Archer.Utility | P1 | Archer Volley: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Utility | P1 | Archer Volley: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Archer.Ultimate | P1 | Archer Rain Arrows: cast pays cost, starts cooldown, applies effect | **PASS** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>effects: Paladin Spawn Consecrat Effect (SpawnSummonEffect) |
| QA-ABL-02.Archer.Ultimate | P1 | Archer Rain Arrows: no recast while on cooldown | **PASS** |  |  |
| QA-PLY-03.Mage | P0 | Mage: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Mage | P1 | Mage: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Mage | P0 | Mage: Confirm applies the class | **OBSERVED** |  | OBSERVED: Mage has no default weapon in data |
| QA-UI-01.Mage | P1 | Mage: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Fireball (1s, icon no), [2] Chain Lightning (1s, icon no), [3] Frost Nova (1s, icon no), [4] Meteor (1s, icon no) |
| QA-CMB-06.Mage | P1 | Mage: attack with no weapon | **PASS** |  |  |
| QA-ABL-01.Mage.Primary | P0 | Mage Fireball: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Primary | P1 | Mage Fireball: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Secondary | P1 | Mage Chain Lightning: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Secondary | P1 | Mage Chain Lightning: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Utility | P1 | Mage Frost Nova: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Utility | P1 | Mage Frost Nova: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-01.Mage.Ultimate | P1 | Mage Meteor: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Ultimate | P1 | Mage Meteor: no recast while on cooldown | **OBSERVED** |  | OBSERVED: cooldown 1s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input |
| QA-STAT-10 | P0 | Class stat assets unchanged after a full player run (BUG-115) | **PASS** |  |  |

### STATIC

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-STATIC-01 | P0 | No asset or prefab references a missing script | **KNOWN** | BUG-073, baseline debt (82) | FAIL: 83 finding(s)<br>Assets/Prefab/Bullet/BulletEnnemy.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/BulletPlayer.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/SpiritProjcetile.prefab: script guid 405e5c1b5d35de1428150d32c25b0bf1<br>Assets/Prefab/Core/CoreGame.prefab: script guid 1a76f0427ff553c419f560d0fc0d03fe<br>Assets/Prefab/Core/CoreGame.prefab: script guid 94c1121f7993220458a7cce8d02e297c |
| QA-STATIC-02 | P1 | No ScriptableObject under Assets/SO references a deleted asset | **KNOWN** | BUG-090, baseline debt (16) | FAIL: 19 finding(s)<br>Assets/SO/Database/Enemy/Assasin.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Boss.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/FastSwarm.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/RangeCaster.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Tank.asset: guid babc5d466b4a4854ca630ec23b670512 |
| QA-STATIC-03 | P1 | No serialized key left behind by a field rename (missing [FormerlySerializedAs]) | **KNOWN** | BUG-105, BUG-108, baseline debt (98) | FAIL: 111 finding(s)<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): maxHealth<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): movementVelocities<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): rangeCheckAttack<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): weaponSO<br>Assets/SO/Enemy/Enemy - Knight 2.asset (EntityData): maxHealth |
| QA-STATIC-04 | P0 | No runtime stat modifier saved into a stat asset | **KNOWN** | BUG-115 | FAIL: 2 finding(s)<br>Assets/SO/Stat/Player/ArcherStats.asset: 1 saved modifier(s)<br>Assets/SO/Stat/Player/MageStats.asset: 2 saved modifier(s) |
| QA-STATIC-05 | P0 | Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR | **KNOWN** | BUG-116 | FAIL: 1 finding(s)<br>Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4: using UnityEditor; |
| QA-STATIC-06 | P0 | Every enabled build scene exists | **PASS** |  | build scenes: Assets/Scenes/Main/MainGamePlay.unity, Assets/Scenes/Main/Loading.unity, Assets/Scenes/Main/LoadRandomMap.unity, Assets/Scenes/Main/GameplayUI.unity, Assets/Scenes/Main/SetLevel.unity |
| QA-STATIC-07 | P1 | Every enemy role has a weapon, a move speed and a consistent attack range | **PASS** |  | AssasinEntityData: MoveSpeed 4.16, range 0-1.3, FOV 7<br>BossEntityData: MoveSpeed 2.21, range 0.5-1.6, FOV 10<br>BruteEntityData: MoveSpeed 2.03, range 0.6-2, FOV 4.5<br>EliteEntityData: MoveSpeed 2.82, range 0.5-2.3, FOV 7<br>FastSwarmEntityData: MoveSpeed 5.3, range 0.2-1.2, FOV 6.5<br>RangedCasterEntityData: MoveSpeed 2.56, range 3-6.1, FOV 8 |

### SYSTEMS

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-EVT-01 | P0 | Emit delivers the payload to a registered listener | **PASS** |  |  |
| QA-EVT-02 | P0 | UnResgister stops delivery | **PASS** |  |  |
| QA-EVT-03 | P1 | Two listeners on one event both receive it; Emit with no listener is safe | **PASS** |  |  |
| QA-STAT-01 | P0 | Flat modifier adds its value to FinalValue | **PASS** |  |  |
| QA-STAT-02 | P1 | PercentAdd modifiers sum before multiplying | **PASS** |  |  |
| QA-STAT-03 | P1 | PercentMult modifier multiplies | **PASS** |  |  |
| QA-STAT-04 | P0 | RemoveModifiersFromSource restores the value | **PASS** |  |  |
| QA-STAT-05 | P1 | Removing one source keeps another source's modifier | **PASS** |  |  |
| QA-STAT-06 | P0 | Working on an Instantiate copy leaves the stat asset unchanged | **PASS** |  |  |
| QA-POOL-01 | P0 | Spawn returns an active instance at the requested position | **PASS** |  |  |
| QA-POOL-02 | P0 | Release deactivates; the next Spawn reuses the same instance | **PASS** |  |  |
| QA-POOL-03 | P1 | Two spawns without release give two distinct instances | **PASS** |  |  |
| QA-MAP-01 | P0 | Room clear: ON_OPEN_DOOR fires once, when the last spawned enemy dies | **PASS** |  |  |
| QA-MAP-02 | P1 | Room clear counts an extra spawned enemy | **PASS** |  |  |
| QA-MAP-03 | P1 | MapModel.GetRandomRoom deals every room once per cycle | **PASS** |  |  |
| QA-MAP-04 | P1 | RoomModel.GetSpawnSet stays within weight budget + overflow (20 seeds per room) | **PASS** |  |  |
| QA-MAP-05 | P1 | Every room JSON parses with matching tiles / poses / layers counts | **PASS** |  | rooms: BossRoom_ThroneArena, BuffRoom_PowerShrine, CombatRoom_CentralIsland, CombatRoom_Checkerboard, CombatRoom_Cross, CombatRoom_DiagonalWalls, CombatRoom_EliteGuard, CombatRoom_FourPillars, CombatRoom_GrandArena, CombatRoom_HiddenCorner, CombatRoom_NarrowBridges, CombatRoom_PracticeYard, CombatRoom_RoundArena, CombatRoom_SmallMaze, CombatRoom_Spiral, CombatRoom_TwinCorridors, CombatRoom_TwoHalls, RestRoom_Campfire, ShopRoom_Merchant, StartRoom_Entrance |
| QA-PATH-01 | P0 | A* finds a path of walkable, adjacent waypoints to the farthest reachable tile | **PASS** |  | CombatRoom_GrandArena.json: 668 walkable tiles of 900 |
| QA-PATH-02 | P1 | A* to a wall tile fails cleanly | **PASS** |  |  |
| QA-PATH-03 | P1 | Same query twice gives the same path (search state cleared) | **PASS** |  |  |
| QA-ITEM-01 | P1 | DepotItem rolls only items from its table; same seed gives same roll | **PASS** |  |  |

### VITALS

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-VIT-01 | P0 | ReloadComponent fills HP and Mana to max | **PASS** |  |  |
| QA-VIT-02 | P0 | Reduction lowers the value by exactly the amount and raises CurrentStatsChanged | **PASS** |  |  |
| QA-VIT-03 | P1 | Reduction larger than the current value clamps at 0 | **PASS** |  |  |
| QA-VIT-04 | P1 | Recovery below max restores exactly the amount | **PASS** |  |  |
| QA-VIT-05 | P1 | Recovery never pushes the value above max | **KNOWN** | BUG-119 | FAIL: HP 299 above max 294 after max-5 +10 |
| QA-VIT-06 | P0 | Damage through the player hurtbox lowers HP, player survives | **PASS** |  |  |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 6, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |
| QA-VIT-08 | P1 | ON_PLAYER_DEATH is emitted once per death, not every frame | **SKIP** |  | SKIP: the player never emitted ON_PLAYER_DEATH (see QA-VIT-07) |

Manual cases (visual, feel, layout): `qa-automation/manual/manual-weekly-checklist.md`.
