# Weekly QA automation report

- Run `20261114-2329` | sprint `origin/sprint-22` at `a8725a77d2` | baseline `5a3256e7dc`
- Owner branch `docs/gdd-pathfinding-ability` | auto-commit `3b02df7d` (coding need check)
- Unity 2022.3.62f3 | priority <= P1 + deep P2 for: ABL,CMB,ENT,ITEM,MAP,PATH,PLY,POOL,STAT,STATIC,UI,VIT | seed 20261031
- **Exit 1** - 7 unknown FAIL(s)

| PASS | FAIL | KNOWN | SKIP | OBSERVED | Total |
|---|---|---|---|---|---|
| 149 | 7 | 8 | 20 | 3 | 187 |

FAIL = unexpected, needs a look. KNOWN = fails because of an open bug file (listed). OBSERVED = recorded where the design is undecided. SKIP = not testable as built (reason in Details).

## 1. Compared with the previous run

Previous: run `20261107-1437` at `5a3256e7dc`.

### New FAIL (5)

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ITEM-03 | P0 | Player picks up the dropped item with Q | **FAIL** |  | item BaseOrds: 1 effect(s), 1 did not load (effect asset bound to an abstract script)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39) |
| QA-CMB-01.Paladin | P0 | Paladin: melee hit damage = PhysicalDamage + stage damage | **FAIL** |  | _mapped to BUG-105 whose file says fixed - regression?_<br>FAIL: damage 72.4, expected 64.4 (or 222.4 on crit) |
| QA-ABL-01.Archer.Ultimate | P1 | Archer Rain Arrows: cast pays cost, starts cooldown, applies effect | **FAIL** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>effects: Archer Spawn Rain Arrows Effect (SpawnSummonEffect)<br>FAIL: spawn effect dealt no damage to a target 2.5 units ahead within 2.5s |
| QA-STATIC-03 | P1 | No serialized key left behind by a field rename (missing [FormerlySerializedAs]) | **FAIL** |  | _58 new finding(s) not covered by a known bug or the baseline (62 baseline)_<br>FAIL: 128 finding(s)<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): idleDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): maxHealth<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): moveDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): movementVelocities<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): rangeCheckAttack |
| QA-STATIC-05 | P0 | Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR | **FAIL** |  | _2 new finding(s) not covered by a known bug or the baseline (0 baseline)_<br>FAIL: 2 finding(s)<br>Assets/Tests/EditMode/TestSupport.cs:5: using UnityEditor;<br>Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4: using UnityEditor; |

### Fixed since last run (1)

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-STATIC-04 | P0 | No runtime stat modifier saved into a stat asset | **PASS** |  |  |

### Still failing (2)

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-STATIC-01 | P0 | No asset or prefab references a missing script | **FAIL** |  | _18 new finding(s) not covered by a known bug or the baseline (82 baseline)_<br>FAIL: 101 finding(s)<br>Assets/Prefab/Bullet/BulletEnnemy.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/BulletPlayer.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/SpiritProjcetile.prefab: script guid 405e5c1b5d35de1428150d32c25b0bf1<br>Assets/Prefab/Core/CoreGame.prefab: script guid 1a76f0427ff553c419f560d0fc0d03fe<br>Assets/Prefab/Core/CoreGame.prefab: script guid 94c1121f7993220458a7cce8d02e297c |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 0, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |

### Flaky suspects (0)

- none

Not run this week (P2 of an unchanged system, or removed): 1 case(s).

## 2. Changed this week

Change range `5a3256e7dc`..`a8725a77d2`: 29 commits, 343 files. Details: `weekly-change-impact.md`.

Cases of the touched systems (ABL, CMB, ENT, ITEM, MAP, PATH, PLY, POOL, STAT, STATIC, UI, VIT): 82 PASS, 7 FAIL, 8 KNOWN.

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ENT-14 | P1 | Effect receiver: vital modifiers heal and damage current HP | **KNOWN** | BUG-117 | FAIL: a -30.4 HP vital modifier changed HP 121.6 -> 152, expected 91.2 (a negative vital modifier does not damage) |
| QA-ITEM-03 | P0 | Player picks up the dropped item with Q | **FAIL** |  | item BaseOrds: 1 effect(s), 1 did not load (effect asset bound to an abstract script)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39) |
| QA-PLY-02 | P2 | Before a class is picked: attack and ability input | **OBSERVED** |  | OBSERVED: design undecided: is an unpicked player meant to be unarmed?<br>weapon before pick: none<br>LMB entered PlayerAttackState: False<br>key 1 entered PlayerSkillWeaponState: False<br>HP 270 -> 270, Mana 170 -> 170 |
| QA-CMB-01.Paladin | P0 | Paladin: melee hit damage = PhysicalDamage + stage damage | **FAIL** |  | _mapped to BUG-105 whose file says fixed - regression?_<br>FAIL: damage 72.4, expected 64.4 (or 222.4 on crit) |
| QA-ABL-03.Paladin.Ultimate | P2 | Paladin Avatar of Light: refused when the cost cannot be paid | **OBSERVED** |  | OBSERVED: HP set to 39 below its 40 cost (design open: should an HP cost be refused?) |
| QA-CMB-04.Archer | P0 | Archer: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Archer | P1 | Archer: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Archer.Primary | P0 | Archer Hawk Eye: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerAttackState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Primary | P1 | Archer Hawk Eye: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Archer.Primary | P2 | Archer Hawk Eye: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Secondary | P1 | Archer Kick-back Trap: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-03.Archer.Secondary | P2 | Archer Kick-back Trap: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Utility | P1 | Archer Volley: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Utility | P1 | Archer Volley: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Archer.Utility | P2 | Archer Volley: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Ultimate | P1 | Archer Rain Arrows: cast pays cost, starts cooldown, applies effect | **FAIL** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>effects: Archer Spawn Rain Arrows Effect (SpawnSummonEffect)<br>FAIL: spawn effect dealt no damage to a target 2.5 units ahead within 2.5s |
| QA-CMB-04.Mage | P0 | Mage: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Mage | P1 | Mage: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Mage.Primary | P0 | Mage Fireball: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Primary | P1 | Mage Fireball: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Primary | P2 | Mage Fireball: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Secondary | P1 | Mage Chain Lightning: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Secondary | P1 | Mage Chain Lightning: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Secondary | P2 | Mage Chain Lightning: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Utility | P1 | Mage Frost Nova: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Utility | P1 | Mage Frost Nova: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Utility | P2 | Mage Frost Nova: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Ultimate | P1 | Mage Meteor: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Ultimate | P1 | Mage Meteor: no recast while on cooldown | **OBSERVED** |  | OBSERVED: cooldown 1s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input |
| QA-ABL-03.Mage.Ultimate | P2 | Mage Meteor: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-STATIC-01 | P0 | No asset or prefab references a missing script | **FAIL** |  | _18 new finding(s) not covered by a known bug or the baseline (82 baseline)_<br>FAIL: 101 finding(s)<br>Assets/Prefab/Bullet/BulletEnnemy.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/BulletPlayer.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/SpiritProjcetile.prefab: script guid 405e5c1b5d35de1428150d32c25b0bf1<br>Assets/Prefab/Core/CoreGame.prefab: script guid 1a76f0427ff553c419f560d0fc0d03fe<br>Assets/Prefab/Core/CoreGame.prefab: script guid 94c1121f7993220458a7cce8d02e297c |
| QA-STATIC-02 | P1 | No ScriptableObject under Assets/SO references a deleted asset | **KNOWN** | BUG-090, baseline debt (16) | FAIL: 19 finding(s)<br>Assets/SO/Database/Enemy/Assasin.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Boss.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/FastSwarm.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/RangeCaster.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Tank.asset: guid babc5d466b4a4854ca630ec23b670512 |
| QA-STATIC-03 | P1 | No serialized key left behind by a field rename (missing [FormerlySerializedAs]) | **FAIL** |  | _58 new finding(s) not covered by a known bug or the baseline (62 baseline)_<br>FAIL: 128 finding(s)<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): idleDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): maxHealth<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): moveDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): movementVelocities<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): rangeCheckAttack |
| QA-STATIC-05 | P0 | Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR | **FAIL** |  | _2 new finding(s) not covered by a known bug or the baseline (0 baseline)_<br>FAIL: 2 finding(s)<br>Assets/Tests/EditMode/TestSupport.cs:5: using UnityEditor;<br>Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4: using UnityEditor; |
| QA-MAP-06 | P2 | Combat rooms carry a spawn marker; other room types report theirs | **KNOWN** | BUG-096 | FAIL: BossRoom_ThroneArena: no Tile_Spawn_Enemy marker<br>BuffRoom_PowerShrine: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>RestRoom_Campfire: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>ShopRoom_Merchant: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>StartRoom_Entrance: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096) |
| QA-VIT-05 | P1 | Recovery never pushes the value above max | **KNOWN** | BUG-119 | FAIL: HP 279 above max 274 after max-5 +10 |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 0, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |
| QA-VIT-08 | P1 | ON_PLAYER_DEATH is emitted once per death, not every frame | **SKIP** |  | SKIP: the player never emitted ON_PLAYER_DEATH (see QA-VIT-07) |

New scripts with no case yet: `Assets/Script/FastTest/AttackFlowEventProbe.cs`, `Assets/Script/System/StatSystem/PrimaryStatEntry.cs`

## 3. Hygiene

- No tracked asset was left modified by the run.

## 4. All results

### ENEMY

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-ENT-01 | P0 | Pool spawns the enemy prefab with its core components and full HP | **PASS** |  |  |
| QA-ENT-02 | P0 | Spawned enemy has its default weapon equipped | **PASS** |  |  |
| QA-ENT-03 | P0 | Damage lowers HP by amount x 100 / (100 + Defense) | **PASS** |  |  |
| QA-ENT-04 | P0 | Lethal damage emits ON_ENEMY_DEATH once with the enemy as payload | **PASS** |  |  |
| QA-ENT-05 | P1 | Released then re-spawned enemy comes back with full HP | **PASS** |  |  |
| QA-ENT-06 | P2 | Enemy stat asset unchanged after spawn, damage and respawn | **PASS** |  |  |
| QA-ENT-07 | P1 | Spawned enemy reads move speed and attack range from its stats | **PASS** |  |  |
| QA-ENT-09 | P0 | Idle: a spawned enemy with no target stands still, then leaves for Move | **PASS** |  |  |
| QA-ENT-10 | P0 | Move: with no target the enemy wanders at its MoveSpeed | **PASS** |  | farthest point from its spawn point: 1.28 |
| QA-ENT-11 | P0 | TakeDamage: a non-lethal hit enters the take-damage state and leaves it | **PASS** |  |  |
| QA-ENT-12 | P0 | Death: a lethal hit enters the death state from Idle and from TakeDamage | **PASS** |  |  |
| QA-ENT-13 | P1 | Effect receiver: a max-stat buff applies, is removed by source and expires | **PASS** |  |  |
| QA-ENT-14 | P1 | Effect receiver: vital modifiers heal and damage current HP | **KNOWN** | BUG-117 | FAIL: a -30.4 HP vital modifier changed HP 121.6 -> 152, expected 91.2 (a negative vital modifier does not damage) |
| QA-ITEM-02 | P0 | Enemy death drops the rolled item at the death position | **PASS** |  | Depot Item 1: drop rate 100%<br>the drop table always drops: a roll that drops nothing cannot be tested |
| QA-ITEM-03 | P0 | Player picks up the dropped item with Q | **FAIL** |  | item BaseOrds: 1 effect(s), 1 did not load (effect asset bound to an abstract script)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39)<br>FAIL: Exception: NullReferenceException: Object reference not set to an instance of an object @ ItemController.Interact (Interact interactor) (at Assets/Script/System/Item/ItemController.cs:39) |
| QA-ITEM-04 | P1 | Q with no item in reach does nothing | **PASS** |  |  |
| QA-ENT-15 | P0 | Move: the enemy locks a player in view, chases into attack range and attacks | **PASS** |  |  |
| QA-ENT-08.Assasin | P0 | Assasin: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | AssasinEntityData: weapon AssasinWeapon(Clone) (MeleeWP), range 0-1.3, player held at 0.65, min attackRate 1s<br>attack clip Assasin_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.Boss | P0 | Boss: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | BossEntityData: weapon BossWeapon(Clone) (MeleeWP), range 0.5-1.6, player held at 1.05, min attackRate 1s<br>attack clip Boss_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.Brute | P0 | Brute: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | BruteEntityData: weapon BruteWeapon(Clone) (MeleeWP), range 0.6-2, player held at 1.3, min attackRate 2.2s<br>attack clip Brute_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.Elite | P0 | Elite: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | EliteEntityData: weapon EliteWeapon(Clone) (MeleeWP), range 0.5-2.3, player held at 1.4, min attackRate 1.1s<br>attack clip Elite_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.FastSwarm | P0 | FastSwarm: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | FastSwarmEntityData: weapon FastSwarmWeapon(Clone) (MeleeWP), range 0.2-1.2, player held at 0.7, min attackRate 1s<br>attack clip FastSwarm_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.RangedCaster | P0 | RangedCaster: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | RangedCasterEntityData: weapon RangedCasterWeapon(Clone) (RangeWP), range 3-6.1, player held at 4.55, min attackRate 1s<br>attack clip RangedCaster_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction)<br>1 projectile(s) seen; closest 0.93 from the player at (0.38, 0.00) (player at (-0.55, 0.00), enemy at (4.00, 0.00)) |
| QA-ENT-08.Scatter | P0 | Scatter: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | ScatterEntityData: weapon ScatterWeapon(Clone) (RangeWP), range 2-4.52, player held at 3.26, min attackRate 1.8s<br>attack clip Scatter_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction)<br>3 projectile(s) seen; closest 0.96 from the player at (1.70, 0.00) (player at (0.74, 0.00), enemy at (4.00, 0.00)) |
| QA-ENT-08.Sniper | P0 | Sniper: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | SniperEntityData: weapon SniperWeapon(Clone) (RangeWP), range 4.5-8.04, player held at 6.27, min attackRate 2.5s<br>attack clip Sniper_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction)<br>3 projectile(s) seen; closest 0.97 from the player at (-1.30, 0.00) (player at (-2.27, 0.00), enemy at (4.00, 0.00)) |
| QA-ENT-08.Tank | P0 | Tank: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | TankEntityData: weapon TankWeapon(Clone) (MeleeWP), range 0.3-1.4, player held at 0.85, min attackRate 1s<br>attack clip Tank_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |
| QA-ENT-08.TrashMelee | P0 | TrashMelee: enemy attacks the player in range, player loses HP, attacks respect attackRate | **PASS** |  | TrashMeleeEntityData: weapon TrashMeleeWeapon(Clone) (MeleeWP), range 0.4-1.5, player held at 0.95, min attackRate 1s<br>attack clip TrashMelee_Attack_dir 1 [events: AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd] (EntityAttackState deals damage on AnimationOnAction) |

### PLAYER

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-PLY-01 | P0 | Player spawns once at the spawn point | **PASS** |  | data before pick: BaseData |
| QA-PLY-02 | P2 | Before a class is picked: attack and ability input | **OBSERVED** |  | OBSERVED: design undecided: is an unpicked player meant to be unarmed?<br>weapon before pick: none<br>LMB entered PlayerAttackState: False<br>key 1 entered PlayerSkillWeaponState: False<br>HP 270 -> 270, Mana 170 -> 170 |
| QA-PLY-03.Paladin | P0 | Paladin: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Paladin | P1 | Paladin: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Paladin | P0 | Paladin: Confirm applies the class | **PASS** |  |  |
| QA-UI-01.Paladin | P1 | Paladin: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Paladin Blessed Slash Ability (1s, icon no), [2] Consecrate (1s, icon no), [3] Blessing (1s, icon no), [4] Avatar of Light (1s, icon no) |
| QA-CMB-01.Paladin | P0 | Paladin: melee hit damage = PhysicalDamage + stage damage | **FAIL** |  | _mapped to BUG-105 whose file says fixed - regression?_<br>FAIL: damage 72.4, expected 64.4 (or 222.4 on crit) |
| QA-CMB-02.Paladin | P1 | Paladin: melee combo reaches every stage | **PASS** |  | hits: 2 |
| QA-CMB-03.Paladin | P1 | Paladin: melee out of range deals no damage | **PASS** |  |  |
| QA-ABL-01.Paladin.Primary | P0 | Paladin Paladin Blessed Slash Ability: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Paladin Spawn Slash Effect (SpawnProjectileEffect) |
| QA-ABL-02.Paladin.Primary | P1 | Paladin Paladin Blessed Slash Ability: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Paladin.Primary | P2 | Paladin Paladin Blessed Slash Ability: refused when the cost cannot be paid | **PASS** |  |  |
| QA-ABL-01.Paladin.Secondary | P1 | Paladin Consecrate: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Paladin Spawn Consecrat Effect (SpawnSummonEffect) |
| QA-ABL-02.Paladin.Secondary | P1 | Paladin Consecrate: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Paladin.Secondary | P2 | Paladin Consecrate: refused when the cost cannot be paid | **PASS** |  |  |
| QA-ABL-01.Paladin.Utility | P1 | Paladin Blessing: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Paladin Buff - Debuff Stat Effect (BuffDebuffStatsForDuration), Paladin Recovery Reduction Stats Effect (RecoveryReductionStatsEffect) |
| QA-ABL-02.Paladin.Utility | P1 | Paladin Blessing: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Paladin.Utility | P2 | Paladin Blessing: refused when the cost cannot be paid | **PASS** |  |  |
| QA-ABL-01.Paladin.Ultimate | P1 | Paladin Avatar of Light: cast pays cost, starts cooldown, applies effect | **PASS** |  | entered PlayerSkillWeaponState: True<br>effects: Recovery Stat Effect (RecoveryReductionStatsEffect), Gain For Time Effect (BuffDebuffStatsForDuration) |
| QA-ABL-02.Paladin.Ultimate | P1 | Paladin Avatar of Light: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Paladin.Ultimate | P2 | Paladin Avatar of Light: refused when the cost cannot be paid | **OBSERVED** |  | OBSERVED: HP set to 39 below its 40 cost (design open: should an HP cost be refused?) |
| QA-PLY-03.Archer | P0 | Archer: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Archer | P1 | Archer: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Archer | P0 | Archer: Confirm applies the class | **PASS** |  |  |
| QA-UI-01.Archer | P1 | Archer: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Hawk Eye (1s, icon no), [2] Kick-back Trap (1s, icon no), [3] Volley (1s, icon no), [4] Rain Arrows (1s, icon no) |
| QA-CMB-04.Archer | P0 | Archer: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Archer | P1 | Archer: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Archer.Primary | P0 | Archer Hawk Eye: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerAttackState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Primary | P1 | Archer Hawk Eye: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Archer.Primary | P2 | Archer Hawk Eye: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Secondary | P1 | Archer Kick-back Trap: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Secondary | P1 | Archer Kick-back Trap: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Archer.Secondary | P2 | Archer Kick-back Trap: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Utility | P1 | Archer Volley: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>not judged: no cooldown event — the ability never reached Do<br>(carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Archer.Utility | P1 | Archer Volley: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Archer.Utility | P2 | Archer Volley: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Archer.Ultimate | P1 | Archer Rain Arrows: cast pays cost, starts cooldown, applies effect | **FAIL** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>effects: Archer Spawn Rain Arrows Effect (SpawnSummonEffect)<br>FAIL: spawn effect dealt no damage to a target 2.5 units ahead within 2.5s |
| QA-ABL-02.Archer.Ultimate | P1 | Archer Rain Arrows: no recast while on cooldown | **PASS** |  |  |
| QA-ABL-03.Archer.Ultimate | P2 | Archer Rain Arrows: refused when the cost cannot be paid | **PASS** |  |  |
| QA-PLY-03.Mage | P0 | Mage: G at the monument opens the confirm panel | **PASS** |  | panel data: confirm='Xác nhận', cancel='Hủy' |
| QA-PLY-04.Mage | P1 | Mage: nothing changes before Confirm | **PASS** |  | the Cancel button itself is a manual case (MAN-PLY-03) |
| QA-PLY-05.Mage | P0 | Mage: Confirm applies the class | **PASS** |  |  |
| QA-UI-01.Mage | P1 | Mage: hotbar shows every bound ability | **PASS** |  | hotbar: [1] Fireball (1s, icon no), [2] Chain Lightning (1s, icon no), [3] Frost Nova (1s, icon no), [4] Meteor (1s, icon no) |
| QA-CMB-04.Mage | P0 | Mage: arrow hits and despawns | **KNOWN** | BUG-095 | FAIL: no arrow reached the target within 2.5s<br>FAIL: 1 projectiles still active 2s later |
| QA-CMB-05.Mage | P1 | Mage: shots are at least RecoveryTime apart | **KNOWN** | BUG-103 | FAIL: only 0 shot(s) landed in ~2s of clicking |
| QA-ABL-01.Mage.Primary | P0 | Mage Fireball: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Primary | P1 | Mage Fireball: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Primary | P2 | Mage Fireball: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Secondary | P1 | Mage Chain Lightning: cast pays cost, starts cooldown, applies effect | **SKIP** |  | entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Secondary | P1 | Mage Chain Lightning: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Secondary | P2 | Mage Chain Lightning: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Utility | P1 | Mage Frost Nova: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Utility | P1 | Mage Frost Nova: no recast while on cooldown | **SKIP** |  | SKIP: first cast never started a cooldown (see QA-ABL-01) |
| QA-ABL-03.Mage.Utility | P2 | Mage Frost Nova: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-ABL-01.Mage.Ultimate | P1 | Mage Meteor: cast pays cost, starts cooldown, applies effect | **SKIP** |  | (carried over) player stuck in PlayerSkillWeaponState for 4s after the action<br>entered PlayerSkillWeaponState: True<br>SKIP: Effects list is empty — ability not implemented yet; the notes above are information only |
| QA-ABL-02.Mage.Ultimate | P1 | Mage Meteor: no recast while on cooldown | **OBSERVED** |  | OBSERVED: cooldown 1s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input |
| QA-ABL-03.Mage.Ultimate | P2 | Mage Meteor: refused when the cost cannot be paid | **SKIP** |  | SKIP: ability has no cost |
| QA-STAT-10 | P0 | Class stat assets unchanged after a full player run (BUG-115) | **PASS** |  |  |

### STATIC

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-STATIC-01 | P0 | No asset or prefab references a missing script | **FAIL** |  | _18 new finding(s) not covered by a known bug or the baseline (82 baseline)_<br>FAIL: 101 finding(s)<br>Assets/Prefab/Bullet/BulletEnnemy.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/BulletPlayer.prefab: script guid 92b59311105d6b640bbae19ea60be7c2<br>Assets/Prefab/Bullet/SpiritProjcetile.prefab: script guid 405e5c1b5d35de1428150d32c25b0bf1<br>Assets/Prefab/Core/CoreGame.prefab: script guid 1a76f0427ff553c419f560d0fc0d03fe<br>Assets/Prefab/Core/CoreGame.prefab: script guid 94c1121f7993220458a7cce8d02e297c |
| QA-STATIC-02 | P1 | No ScriptableObject under Assets/SO references a deleted asset | **KNOWN** | BUG-090, baseline debt (16) | FAIL: 19 finding(s)<br>Assets/SO/Database/Enemy/Assasin.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Boss.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/FastSwarm.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/RangeCaster.asset: guid babc5d466b4a4854ca630ec23b670512<br>Assets/SO/Database/Enemy/Tank.asset: guid babc5d466b4a4854ca630ec23b670512 |
| QA-STATIC-03 | P1 | No serialized key left behind by a field rename (missing [FormerlySerializedAs]) | **FAIL** |  | _58 new finding(s) not covered by a known bug or the baseline (62 baseline)_<br>FAIL: 128 finding(s)<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): idleDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): maxHealth<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): moveDurationTime<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): movementVelocities<br>Assets/SO/Enemy/Enemy - Knight 1.asset (EntityData): rangeCheckAttack |
| QA-STATIC-04 | P0 | No runtime stat modifier saved into a stat asset | **PASS** |  |  |
| QA-STATIC-05 | P0 | Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR | **FAIL** |  | _2 new finding(s) not covered by a known bug or the baseline (0 baseline)_<br>FAIL: 2 finding(s)<br>Assets/Tests/EditMode/TestSupport.cs:5: using UnityEditor;<br>Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4: using UnityEditor; |
| QA-STATIC-06 | P0 | Every enabled build scene exists | **PASS** |  | build scenes: Assets/Scenes/Main/MainGamePlay.unity, Assets/Scenes/Main/Loading.unity, Assets/Scenes/Main/LoadRandomMap.unity, Assets/Scenes/Main/GameplayUI.unity, Assets/Scenes/Main/SetLevel.unity |
| QA-STATIC-07 | P1 | Every enemy role has a weapon, a move speed and a consistent attack range | **PASS** |  | AssasinEntityData: MoveSpeed 4.16, range 0-1.3, FOV 7.02<br>BossEntityData: MoveSpeed 2.21, range 0.5-1.6, FOV 10<br>BruteEntityData: MoveSpeed 2.03, range 0.6-2, FOV 4.5<br>EliteEntityData: MoveSpeed 2.82, range 0.5-2.3, FOV 6.99<br>FastSwarmEntityData: MoveSpeed 5.3, range 0.2-1.2, FOV 6.48<br>RangedCasterEntityData: MoveSpeed 2.56, range 3-6.1, FOV 7.93 |
| QA-STATIC-08 | P1 | Every enemy role weapon can hit the player: attack clips carry the damage event, masks include the player hurtbox | **PASS** |  | player hurtbox layer Player (7)<br>AssasinEntityData: AssasinWeaponStats stage 0 (AssasinAttackStage1): 32 attack clip(s) checked in Assasin_AttackStage1<br>BossEntityData: BossWeaponStats stage 0 (BossAttackStage1): 32 attack clip(s) checked in Boss_AttackStage1<br>BruteEntityData: BruteWeaponStats stage 0 (BruteAttackStage1): 32 attack clip(s) checked in Brute_AttackStage1<br>EliteEntityData: EliteWeaponStats stage 0 (EliteAttackStage1): 32 attack clip(s) checked in Elite_AttackStage1<br>FastSwarmEntityData: FastSwarmWeaponStats stage 0 (FastSwarmAttackStage1): 32 attack clip(s) checked in FastSwarm_AttackStage1 |

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
| QA-MAP-06 | P2 | Combat rooms carry a spawn marker; other room types report theirs | **KNOWN** | BUG-096 | FAIL: BossRoom_ThroneArena: no Tile_Spawn_Enemy marker<br>BuffRoom_PowerShrine: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>RestRoom_Campfire: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>ShopRoom_Merchant: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)<br>StartRoom_Entrance: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096) |
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
| QA-VIT-05 | P1 | Recovery never pushes the value above max | **KNOWN** | BUG-119 | FAIL: HP 279 above max 274 after max-5 +10 |
| QA-VIT-06 | P0 | Damage through the player hurtbox lowers HP, player survives | **PASS** |  |  |
| QA-VIT-07 | P0 | Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH | **FAIL** |  | FAIL: ON_PLAYER_DEATH not emitted within 4s of death<br>death clip Knight_Die_dir 0, events: AnimationOnAction (PlayerDeathState emits on StatusAnimation.EndRangeTrigger) |
| QA-VIT-08 | P1 | ON_PLAYER_DEATH is emitted once per death, not every frame | **SKIP** |  | SKIP: the player never emitted ON_PLAYER_DEATH (see QA-VIT-07) |

### UNIT

| ID | Pri | Case | Result | Bug | Details |
|---|---|---|---|---|---|
| QA-UNIT.AStarTests.CommittedRoom_PathToAWallTile_FailsCleanly | P0 | EditMode: AStarTests.CommittedRoom_PathToAWallTile_FailsCleanly | **PASS** |  |  |
| QA-UNIT.AStarTests.CommittedRoom_PathToFarthestReachableTile_IsWalkableAdjacentAndEndsAtTarget | P0 | EditMode: AStarTests.CommittedRoom_PathToFarthestReachableTile_IsWalkableAdjacentAndEndsAtTarget | **PASS** |  |  |
| QA-UNIT.AStarTests.CommittedRoom_SameQueryTwice_GivesTheSamePath | P0 | EditMode: AStarTests.CommittedRoom_SameQueryTwice_GivesTheSamePath | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_AroundAWall_TakesTheOnlyShortestDetour | P0 | EditMode: AStarTests.FindPath_AroundAWall_TakesTheOnlyShortestDetour | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_DiagonalBetweenTwoWalls_IsNotAllowed | P0 | EditMode: AStarTests.FindPath_DiagonalBetweenTwoWalls_IsNotAllowed | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_FromAWallTile_Fails | P0 | EditMode: AStarTests.FindPath_FromAWallTile_Fails | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_OnALargerGrid_WaypointsAreWalkableAndAdjacent | P0 | EditMode: AStarTests.FindPath_OnALargerGrid_WaypointsAreWalkableAndAdjacent | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_OpenDiagonal_StepsDiagonally | P0 | EditMode: AStarTests.FindPath_OpenDiagonal_StepsDiagonally | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_SameQueryAfterAnotherSearch_GivesTheSamePath | P0 | EditMode: AStarTests.FindPath_SameQueryAfterAnotherSearch_GivesTheSamePath | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_StartEqualsTarget_ReturnsASingleWaypoint | P0 | EditMode: AStarTests.FindPath_StartEqualsTarget_ReturnsASingleWaypoint | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_StraightCorridor_ReturnsEveryTileInOrder | P0 | EditMode: AStarTests.FindPath_StraightCorridor_ReturnsEveryTileInOrder | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_TargetOutsideTheGrid_Fails | P0 | EditMode: AStarTests.FindPath_TargetOutsideTheGrid_Fails | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_TargetWalledOff_Fails | P0 | EditMode: AStarTests.FindPath_TargetWalledOff_Fails | **PASS** |  |  |
| QA-UNIT.AStarTests.FindPath_ToAWallTile_FailsCleanly | P0 | EditMode: AStarTests.FindPath_ToAWallTile_FailsCleanly | **PASS** |  |  |
| QA-UNIT.DepotItemTests.CommittedDepot_RollsOnlyItemsOfItsTableAndIsDeterministicPerSeed | P0 | EditMode: DepotItemTests.CommittedDepot_RollsOnlyItemsOfItsTableAndIsDeterministicPerSeed | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TierWithoutAnyItem_IsExcludedFromTheDropRate | P0 | EditMode: DepotItemTests.TierWithoutAnyItem_IsExcludedFromTheDropRate | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_EmptyDepot_NeverDrops | P0 | EditMode: DepotItemTests.TryRollItem_EmptyDepot_NeverDrops | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_ReportsTheTierOfTheItemItReturns | P0 | EditMode: DepotItemTests.TryRollItem_ReportsTheTierOfTheItemItReturns | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_SameSeed_GivesTheSameItem | P0 | EditMode: DepotItemTests.TryRollItem_SameSeed_GivesTheSameItem | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_ThirtyPercentTier_DropsAboutThirtyPercentOfRolls | P0 | EditMode: DepotItemTests.TryRollItem_ThirtyPercentTier_DropsAboutThirtyPercentOfRolls | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_TierAtHundredPercent_AlwaysDropsAnItemOfTheTable | P0 | EditMode: DepotItemTests.TryRollItem_TierAtHundredPercent_AlwaysDropsAnItemOfTheTable | **PASS** |  |  |
| QA-UNIT.DepotItemTests.TryRollItem_TierWithZeroRate_NeverDrops | P0 | EditMode: DepotItemTests.TryRollItem_TierWithZeroRate_NeverDrops | **PASS** |  |  |
| QA-UNIT.EventManagerTests.Emit_AfterEveryListenerRemoved_DoesNotThrow | P0 | EditMode: EventManagerTests.Emit_AfterEveryListenerRemoved_DoesNotThrow | **PASS** |  |  |
| QA-UNIT.EventManagerTests.Emit_CallsEveryListenerOfTheEventOnce | P0 | EditMode: EventManagerTests.Emit_CallsEveryListenerOfTheEventOnce | **PASS** |  |  |
| QA-UNIT.EventManagerTests.Emit_DeliversPayloadOnceToRegisteredListener | P0 | EditMode: EventManagerTests.Emit_DeliversPayloadOnceToRegisteredListener | **PASS** |  |  |
| QA-UNIT.EventManagerTests.Emit_DoesNotReachListenersOfAnotherEvent | P0 | EditMode: EventManagerTests.Emit_DoesNotReachListenersOfAnotherEvent | **PASS** |  |  |
| QA-UNIT.EventManagerTests.Emit_WithoutPayload_DeliversNull | P0 | EditMode: EventManagerTests.Emit_WithoutPayload_DeliversNull | **PASS** |  |  |
| QA-UNIT.EventManagerTests.UnResgister_OneListener_KeepsTheOther | P0 | EditMode: EventManagerTests.UnResgister_OneListener_KeepsTheOther | **PASS** |  |  |
| QA-UNIT.EventManagerTests.UnResgister_StopsDelivery | P0 | EditMode: EventManagerTests.UnResgister_StopsDelivery | **PASS** |  |  |
| QA-UNIT.MapModelTests.CommittedMapModel_DealsEveryRoomOncePerCycle | P0 | EditMode: MapModelTests.CommittedMapModel_DealsEveryRoomOncePerCycle | **PASS** |  |  |
| QA-UNIT.MapModelTests.GetRandomRoom_DealsEveryRoomOncePerCycle | P0 | EditMode: MapModelTests.GetRandomRoom_DealsEveryRoomOncePerCycle | **PASS** |  |  |
| QA-UNIT.MapModelTests.GetRandomRoom_SkipsNullEntries | P0 | EditMode: MapModelTests.GetRandomRoom_SkipsNullEntries | **PASS** |  |  |
| QA-UNIT.MapModelTests.GetRandomRoom_StartsANewCycleWhenTheBagIsEmpty | P0 | EditMode: MapModelTests.GetRandomRoom_StartsANewCycleWhenTheBagIsEmpty | **PASS** |  |  |
| QA-UNIT.MapModelTests.GetRandomRoom_WithNoRoom_ReturnsNull | P0 | EditMode: MapModelTests.GetRandomRoom_WithNoRoom_ReturnsNull | **PASS** |  |  |
| QA-UNIT.MapModelTests.GetRandomRoom_WithOnlyNullEntries_ReturnsNull | P0 | EditMode: MapModelTests.GetRandomRoom_WithOnlyNullEntries_ReturnsNull | **PASS** |  |  |
| QA-UNIT.ObjectPoolManagerTests.Release_DeactivatesAndNextSpawnReusesTheSameInstance | P0 | EditMode: ObjectPoolManagerTests.Release_DeactivatesAndNextSpawnReusesTheSameInstance | **PASS** |  |  |
| QA-UNIT.ObjectPoolManagerTests.Release_ObjectThatNeverCameFromThePool_IsIgnored | P0 | EditMode: ObjectPoolManagerTests.Release_ObjectThatNeverCameFromThePool_IsIgnored | **PASS** |  |  |
| QA-UNIT.ObjectPoolManagerTests.Release_Twice_DoesNotQueueTheInstanceTwice | P0 | EditMode: ObjectPoolManagerTests.Release_Twice_DoesNotQueueTheInstanceTwice | **PASS** |  |  |
| QA-UNIT.ObjectPoolManagerTests.Spawn_ReturnsActiveInstanceAtRequestedPosition | P0 | EditMode: ObjectPoolManagerTests.Spawn_ReturnsActiveInstanceAtRequestedPosition | **PASS** |  |  |
| QA-UNIT.ObjectPoolManagerTests.Spawn_TwiceWithoutRelease_GivesTwoDistinctInstances | P0 | EditMode: ObjectPoolManagerTests.Spawn_TwiceWithoutRelease_GivesTwoDistinctInstances | **PASS** |  |  |
| QA-UNIT.RoomJsonTests.EveryRoomJson_HasAtLeastOneFloorTile | P0 | EditMode: RoomJsonTests.EveryRoomJson_HasAtLeastOneFloorTile | **PASS** |  |  |
| QA-UNIT.RoomJsonTests.EveryRoomJson_ParsesWithMatchingTilesPosesAndLayers | P0 | EditMode: RoomJsonTests.EveryRoomJson_ParsesWithMatchingTilesPosesAndLayers | **PASS** |  |  |
| QA-UNIT.RoomJsonTests.RoomFolder_ContainsRoomFiles | P0 | EditMode: RoomJsonTests.RoomFolder_ContainsRoomFiles | **PASS** |  |  |
| QA-UNIT.RoomModelTests.CommittedRooms_SpawnSetStaysWithinBudgetPlusOverflow | P0 | EditMode: RoomModelTests.CommittedRooms_SpawnSetStaysWithinBudgetPlusOverflow | **PASS** |  |  |
| QA-UNIT.RoomModelTests.GetSpawnSet_GroupsRepeatedPicksIntoOneEntryPerEnemyType | P0 | EditMode: RoomModelTests.GetSpawnSet_GroupsRepeatedPicksIntoOneEntryPerEnemyType | **PASS** |  |  |
| QA-UNIT.RoomModelTests.GetSpawnSet_SameSeed_GivesTheSameSet | P0 | EditMode: RoomModelTests.GetSpawnSet_SameSeed_GivesTheSameSet | **PASS** |  |  |
| QA-UNIT.RoomModelTests.GetSpawnSet_SingleEnemyType_SpendsBudgetDownToTenPercent | P0 | EditMode: RoomModelTests.GetSpawnSet_SingleEnemyType_SpendsBudgetDownToTenPercent | **PASS** |  |  |
| QA-UNIT.RoomModelTests.GetSpawnSet_WithNoEnemyType_ReturnsNull | P0 | EditMode: RoomModelTests.GetSpawnSet_WithNoEnemyType_ReturnsNull | **PASS** |  |  |
| QA-UNIT.RoomModelTests.GetSpawnSet_WithSmallWeights_SpendsBetweenNinetyAndHundredPercentOfBudget | P0 | EditMode: RoomModelTests.GetSpawnSet_WithSmallWeights_SpendsBetweenNinetyAndHundredPercentOfBudget | **PASS** |  |  |
| QA-UNIT.StatSheetTests.AddPrimaryPoint_OnDerivedStat_IsRefused | P0 | EditMode: StatSheetTests.AddPrimaryPoint_OnDerivedStat_IsRefused | **PASS** |  |  |
| QA-UNIT.StatSheetTests.AddPrimaryPoint_RaisesChangedOnceForThatStat | P0 | EditMode: StatSheetTests.AddPrimaryPoint_RaisesChangedOnceForThatStat | **PASS** |  |  |
| QA-UNIT.StatSheetTests.CommittedStatAssets_AreUnchangedAfterASheetIsBuffed | P0 | EditMode: StatSheetTests.CommittedStatAssets_AreUnchangedAfterASheetIsBuffed | **PASS** |  |  |
| QA-UNIT.StatSheetTests.FlatModifier_AddsItsValueToFinalValue | P0 | EditMode: StatSheetTests.FlatModifier_AddsItsValueToFinalValue | **PASS** |  |  |
| QA-UNIT.StatSheetTests.ModifierOnPrimaryStat_ReachesDerivedStatAsPrimaryBonus | P0 | EditMode: StatSheetTests.ModifierOnPrimaryStat_ReachesDerivedStatAsPrimaryBonus | **PASS** |  |  |
| QA-UNIT.StatSheetTests.Modifiers_ApplyFlatThenPercentAddThenPercentMult | P0 | EditMode: StatSheetTests.Modifiers_ApplyFlatThenPercentAddThenPercentMult | **PASS** |  |  |
| QA-UNIT.StatSheetTests.NewSheet_FromProfile_ComputesPrimaryBaseAndDerivedFormula | P0 | EditMode: StatSheetTests.NewSheet_FromProfile_ComputesPrimaryBaseAndDerivedFormula | **PASS** |  |  |
| QA-UNIT.StatSheetTests.NewSheet_WithoutProfile_StartsAtZero | P0 | EditMode: StatSheetTests.NewSheet_WithoutProfile_StartsAtZero | **PASS** |  |  |
| QA-UNIT.StatSheetTests.PercentAddModifiers_SumBeforeMultiplyingOnce | P0 | EditMode: StatSheetTests.PercentAddModifiers_SumBeforeMultiplyingOnce | **PASS** |  |  |
| QA-UNIT.StatSheetTests.PercentMultModifier_MultipliesAfterFlat | P0 | EditMode: StatSheetTests.PercentMultModifier_MultipliesAfterFlat | **PASS** |  |  |
| QA-UNIT.StatSheetTests.PrimaryPoint_RaisesDerivedStatThroughItsCoefficient | P0 | EditMode: StatSheetTests.PrimaryPoint_RaisesDerivedStatThroughItsCoefficient | **PASS** |  |  |
| QA-UNIT.StatSheetTests.RemoveModifiersFromSource_KeepsOtherSources | P0 | EditMode: StatSheetTests.RemoveModifiersFromSource_KeepsOtherSources | **PASS** |  |  |
| QA-UNIT.StatSheetTests.RemoveModifiersFromSource_OnProfileSheet_KeepsOtherSourceModifier | P0 | EditMode: StatSheetTests.RemoveModifiersFromSource_OnProfileSheet_KeepsOtherSourceModifier | **PASS** |  |  |
| QA-UNIT.StatSheetTests.RemoveModifiersFromSource_RestoresValueAndLeavesNoModifier | P0 | EditMode: StatSheetTests.RemoveModifiersFromSource_RestoresValueAndLeavesNoModifier | **PASS** |  |  |
| QA-UNIT.StatSheetTests.TwoSheetsFromOneProfile_DoNotShareModifiers | P0 | EditMode: StatSheetTests.TwoSheetsFromOneProfile_DoNotShareModifiers | **PASS** |  |  |

Manual cases (visual, feel, layout): `qa-automation/manual/manual-weekly-checklist.md`.
