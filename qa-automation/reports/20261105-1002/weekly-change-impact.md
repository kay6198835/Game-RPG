# Weekly change impact

- Range: `0a8f24c052` (`0a8f24c052`) .. `origin/sprint-21` (`04cfb53d78`)
- 23 commits (+6 merge commits), 1960 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, EVT, ITEM, MAP, PLY, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `7f590cec` | 2026-11-05 | Kay | docs(doc-sync): doc-truth pass 2026-11-05 - sprint-21 028fdb90, Abilities v1 deleted, EventID 25 | 18 |
| `028fdb90` | 2026-11-05 | Kay | chore(standup): daily standup 2026-11-05 | 3 |
| `1da76e9a` | 2026-11-04 | Kay | chore(process): pre-push gate blocks pushes to main only | 1 |
| `6094aa10` | 2026-11-04 | Kay | fix bug, add asset room, enemy, data | 1830 |
| `bcd4b12b` | 2026-11-04 | Kay | chore(standup): daily standup 2026-11-04 | 2 |
| `bf757c74` | 2026-11-03 | Kay | chore(standup): daily standup 2026-11-03 (evening update) | 3 |
| `01717ace` | 2026-11-03 | Claude | fix(process): static-check ignores fields moved into a base class | 1 |
| `c7fbd69e` | 2026-11-03 | Claude | chore(process): S21-09 gate checks pushes to main only | 5 |
| `1370136c` | 2026-11-03 | Kay | fix bug add skill | 47 |
| `3e3f6f10` | 2026-11-03 | Claude | chore(process): S21-09 process gate — pre-push static check + compile check | 7 |
| `ce1cbff9` | 2026-11-03 | Kay | chore(module-audit): monthly module quality audit 2026-11 | 3 |
| `f9d19cc7` | 2026-11-03 | Kay | chore(standup): daily standup 2026-11-03 | 3 |
| `6c61ccdb` | 2026-11-02 | Kay | feat(abilities): temporary Archer/Mage ability assets (Phase 2) | 32 |
| `ae0d6e82` | 2026-11-02 | Kay | design(gdd): Archer and Mage class abilities (Phase 1) | 2 |
| `03cd91db` | 2026-11-02 | Kay | chore(standup): daily standup 2026-11-02 | 2 |
| `f37d6202` | 2026-11-01 | Kay | chore(kickoff): open sprint-21 2026-11-01 | 4 |
| `ed2f984b` | 2026-11-01 | Kay | chore(wrapup): duplicate firing skipped 2026-11-01 | 1 |
| `9727bdcf` | 2026-11-01 | Kay | chore(qa-auto): weekly QA run 20261101-0221 - 0 inbox notes 2026-11-01 | 2 |
| `d0bd4c4b` | 2026-11-01 | Kay | test(qa): run 20261101-0221 part C (no new cases) and inbox evidence | 2 |
| `4e569ea0` | 2026-11-01 | Kay | test(qa): weekly automation run 20261101-0221 on origin/sprint-20 - FAIL | 11 |
| `2664dc64` | 2026-10-31 | Kay | chore(standup): sync owner's current branch into sprint-NN before standup | 1 |
| `4f1a03a6` | 2026-10-31 | Kay | test(qa): add earlier player-flow run reports | 2 |
| `79ad79b2` | 2026-10-31 | Kay | test(qa): add automated player-flow test scene and sprint-20 QA docs | 22 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 47 | `Assets/Script/Character/Base/AbilityHolderBase.cs`<br>`Assets/Script/System/Abilities/Effects/BuffDebuffStatsForDuration.cs`<br>`Assets/Script/System/Abilities/Effects/RecoveryReductionPerTimeForDuration.cs`<br>`Assets/Script/System/Abilities/Runtime/SpawnMono/DamagesInManyPos.cs`<br>... +43 |
| CMB | 10 | `Assets/Script/Character/Base/WeaponHolderBase.cs`<br>`Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs`<br>`Assets/Script/Weapons/Weapon.cs`<br>`Assets/SO/Weapons/ArcherWeapon/Range Weapon Stats.asset`<br>... +6 |
| DOCS | 41 | `.claude/skills/daily-standup/SKILL.md`<br>`CLAUDE.md`<br>`design/gdd/class-abilities-archer-mage.md`<br>`docs/changelog/tech-debt-register.CHANGELOG.md`<br>... +37 |
| ENT | 156 | `Assets/Prefab/Enemy/EnemyPrefab.prefab`<br>`Assets/Prefab/Enemy/Role.meta`<br>`Assets/Prefab/Enemy/Role/Assasin.meta`<br>`Assets/Prefab/Enemy/Role/Assasin/AssasinWeapon.prefab`<br>... +152 |
| EVT | 1 | `Assets/Script/Manager/EventManager.cs` |
| ITEM | 4 | `Assets/Script/System/Item/WeaponSO.cs`<br>`Assets/Script/System/Item/WeaponSO.cs.meta`<br>`Assets/SO/Item/Effect/New Stat Modifier Effect Definition - Copy.asset`<br>`Assets/SO/Item/Effect/New Stat Modifier Effect Definition.asset` |
| MAP | 31 | `Assets/Script/Database-SO/Modal/RoomModel.cs`<br>`Assets/Script/Map/Room/RoomCell.cs`<br>`Assets/Script/Map/Room/RoomGeneraterController.cs`<br>`Assets/Script/System/Enemy/EnemySpawner.cs`<br>... +27 |
| OTHER | 2 | `.githooks/pre-push`<br>`.githooks/static-check.sh` |
| PLY | 14 | `Assets/Script/Character/Base/CharacterBase.cs`<br>`Assets/Script/Character/Base/CharacterData.cs`<br>`Assets/Script/Character/Base/MovementBase.cs`<br>`Assets/Script/Character/Player/CoreComponent/PlayerMovement.cs`<br>... +10 |
| QA | 22 | `Assets/Script/FastTest/QAProbe.meta`<br>`Assets/Script/FastTest/QAProbe/Editor.meta`<br>`Assets/Script/FastTest/QAProbe/Editor/QAPlayerFlowRunner.cs`<br>`Assets/Script/FastTest/QAProbe/Editor/QAPlayerFlowRunner.cs.meta`<br>... +18 |
| STAT | 22 | `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/StatModifierGroup.cs`<br>`Assets/Script/System/StatSystem/StatType.cs`<br>`Assets/SO/Stat/Enemy/AssasinStats.asset`<br>... +18 |
| STATIC | 9 | `Assets/Editor/EnemyRoleScaffolder.cs`<br>`Assets/Editor/EnemyRoleScaffolder.cs.meta`<br>`Assets/Prefab/Bullet/Spell.prefab`<br>`Assets/Prefab/Weapon/KnightWeapon.prefab`<br>... +5 |
| VISUAL | 1594 | `Assets/Animation/Enemy/Role.meta`<br>`Assets/Animation/Enemy/Role/Assasin.meta`<br>`Assets/Animation/Enemy/Role/Assasin/Activate.meta`<br>`Assets/Animation/Enemy/Role/Assasin/Activate/Assasin_SpinAttack_State1_dir 0.anim`<br>... +1590 |
| VIT | 7 | `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Base/StatHandlerBase.cs`<br>`Assets/Script/Character/Base/VitalStatsBase.cs`<br>... +3 |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/AbilityHolderBase.cs` | Shared by the player and enemies; the UIFlow hotbar binds to AbilityCooldownStarted | PLY, ENT, UI |
| `Assets/Script/Character/Base/CharacterBase.cs`<br>`Assets/Script/Character/Base/CharacterData.cs` | Serialized by every PlayerData / EntityData asset — a rename without [FormerlySerializedAs] empties fields (BUG-105/108/121) | ENT, CMB, ABL, STATIC |
| `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Base/StatHandlerBase.cs`<br>... +1 | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Base/WeaponHolderBase.cs` | Shared by the player and enemies; default weapon equip happens in SetCharacterData (BUG-106) | PLY, ENT |
| `Assets/Script/Manager/EventManager.cs` | Room clear, death and HUD binding are all event-driven | MAP, VIT, ENT, UI |
| `Assets/Script/Map/Room/RoomCell.cs`<br>`Assets/Script/Map/Room/RoomGeneraterController.cs`<br>`Assets/Script/System/Enemy/EnemySpawner.cs` | Room clear depends on ON_ENEMY_DEATH from pooled enemies | ENT |
| `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/StatModifierGroup.cs`<br>`Assets/Script/System/StatSystem/StatType.cs` | Damage formulas, vitals max values and asset serialization all read the stat system | VIT, CMB, ENT, STATIC |
| `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs`<br>`Assets/Script/Weapons/Weapon.cs` | Enemies attack through the same Weapon classes; AttackSO field renames break assets | ENT, STATIC |
| `Assets/SO/Database/Map/MapModel.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_Boss.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_Roles.asset`<br>... +82 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (3)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/FastTest/QAProbe/QAInputDriver.cs` - *no case yet*
- `Assets/Script/FastTest/QAProbe/QAPlayerFlowProbe.cs` - covered
- `Assets/Script/FastTest/QAProbe/QAProbeTarget.cs` - *no case yet*

**New assets (67)**:

- `Assets/Prefab/Enemy/Role/Assasin/AssasinWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Boss/BossWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Brute/BruteWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Elite/EliteWeapon.prefab`
- `Assets/Prefab/Enemy/Role/FastSwarm/FastSwarmWeapon.prefab`
- `Assets/Prefab/Enemy/Role/RangedCaster/RangedCasterProjectile.prefab`
- `Assets/Prefab/Enemy/Role/RangedCaster/RangedCasterWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Scatter/ScatterProjectile.prefab`
- `Assets/Prefab/Enemy/Role/Scatter/ScatterWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Sniper/SniperProjectile.prefab`
- `Assets/Prefab/Enemy/Role/Sniper/SniperWeapon.prefab`
- `Assets/Prefab/Enemy/Role/Tank/TankWeapon.prefab`
- `Assets/Prefab/Enemy/Role/TrashMelee/TrashMeleeWeapon.prefab`
- `Assets/Prefab/Weapon/KnightWeapon.prefab`
- `Assets/SO/Database/Room/Role/RoomModel_Roles.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T1_Outpost.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T2_Ambush.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T2_FiringLine.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T3_HeavySiege.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T3_Warband.asset`
- `Assets/SO/Database/Room/Role/RoomModel_T4_ChampionGuard.asset`
- `Assets/SO/Enemy/Knight/Knight.asset`
- `Assets/SO/Enemy/Role/Assasin/AssasinEntityData.asset`
- `Assets/SO/Enemy/Role/Assasin/Weapon/AssasinWeaponStats.asset`
- `Assets/SO/Enemy/Role/Boss/BossEntityData.asset`
- `Assets/SO/Enemy/Role/Boss/Weapon/BossAttackStage1.asset`
- `Assets/SO/Enemy/Role/Boss/Weapon/BossWeaponStats.asset`
- `Assets/SO/Enemy/Role/Brute/BruteEntityData.asset`
- `Assets/SO/Enemy/Role/Brute/Weapon/BruteAttackStage1.asset`
- `Assets/SO/Enemy/Role/Brute/Weapon/BruteWeaponStats.asset`
- ... +37

**Deleted (23)**:

- `Assets/Prefab/Bullet/Spell.prefab`
- `Assets/Script/Character/Entity/CoreComponent/EntityEffectStats.cs`
- `Assets/Script/Character/Entity/CoreComponent/EntityWeapon.cs`
- `Assets/Script/Character/Player/Projectile/Spell.cs`
- `Assets/Script/Interface/IEffectable.cs`
- `Assets/Script/System/Skill_Ability/AbstractSkillSO.cs`
- `Assets/Script/System/Skill_Ability/ActivateSkill.cs`
- `Assets/Script/System/Skill_Ability/BlockAbility.cs`
- `Assets/Script/System/Skill_Ability/DashAbility.cs`
- `Assets/Script/System/Skill_Ability/DualAbility.cs`
- `Assets/Script/System/Skill_Ability/EffectSkillDuringTime.cs`
- `Assets/Script/System/Skill_Ability/EffectSkillOneTime.cs`
- `Assets/Script/System/Skill_Ability/EffectSkillSO.cs`
- `Assets/Script/System/Skill_Ability/InternalSkillSO.cs`
- `Assets/Script/System/Skill_Ability/SlashAbility.cs`
- `Assets/SO/Database/Room/RoomModel 2.asset`
- `Assets/SO/Database/Room/RoomModel 3.asset`
- `Assets/SO/Database/Room/RoomModel 4.asset`
- `Assets/SO/Skill/Dash Ability.asset`
- `Assets/SO/Skill/Dual Ability.asset`
- `Assets/SO/Skill/New Effect Skill During Time.asset`
- `Assets/SO/Skill/Slash Ability.asset`
- `Assets/SO/Skill/Test One Time.asset`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
