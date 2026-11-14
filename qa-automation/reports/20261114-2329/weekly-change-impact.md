# Weekly change impact

- Range: `5a3256e7dc` (`5a3256e7dc`) .. `origin/sprint-22` (`a8725a77d2`)
- 29 commits (+12 merge commits), 343 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, MAP, PATH, PLY, POOL, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `4acd62ef` | 2026-11-13 | Kay | chore(standup): daily standup 2026-11-13 | 4 |
| `4ee57687` | 2026-11-12 | Claude | docs(qa): add the local verification checklist for the EditMode tests | 1 |
| `b7bac279` | 2026-11-12 | Claude | docs(qa): add the Unit mirror column to the case catalog | 1 |
| `47f66ddb` | 2026-11-12 | Claude | test(editmode): mirror the logic-only QA cases as NUnit EditMode tests | 18 |
| `e7fb161e` | 2026-11-12 | Kay | test(qa): run EditMode unit tests in the weekly run | 3 |
| `d9776bb8` | 2026-11-12 | Claude | fix(build): rename the game's Path class to NavPath | 9 |
| `e555e9da` | 2026-11-12 | Claude | fix(build): give DOTween Modules their own assembly | 2 |
| `9c11c672` | 2026-11-12 | Claude | chore(test): add assembly definitions and the first EditMode test | 19 |
| `285e6900` | 2026-11-12 | Claude | fix(ability): wire Archer Rain Arrows to RainArrows.prefab and despawn it at clip end | 11 |
| `c35cdf3c` | 2026-11-12 | Claude | test(qa): enemy state, effect receiver, item drop and pickup cases | 10 |
| `4c7c611c` | 2026-11-12 | Kay | chore(standup): daily standup 2026-11-12 | 3 |
| `e5c4e1d8` | 2026-11-11 | Kay | chore(standup): daily standup 2026-11-11 | 3 |
| `5e30cf72` | 2026-11-10 | Kay | chore(standup): daily standup 2026-11-10 | 3 |
| `4e14862b` | 2026-11-09 | Kay | docs(plan): weekly to-do for 2026-11-09 (death/replay, rooms, boss, bonfire, dash, animation) | 1 |
| `094ec9df` | 2026-11-09 | Kay | fix(data): migrate renamed ItemSO fields (itemSprite, nameItem) and add key audit script | 10 |
| `b47f7a62` | 2026-11-09 | Kay | chore(tools): character animation viewer build scripts | 3 |
| `b33dedf9` | 2026-11-09 | Kay | fix(respawn): reset pooled enemy state on reuse; percent Defense mitigation | 6 |
| `a0e250fb` | 2026-11-09 | Kay | refactor(stats): split BaseStatsSO into data-only asset + StatSheet calculator | 48 |
| `70067482` | 2026-11-09 | Kay | chore(standup): fix-survival result 2026-11-09 | 1 |
| `7fdc4a48` | 2026-11-09 | Kay | chore(standup): daily standup 2026-11-09 | 2 |
| `bd75d17c` | 2026-11-08 | Kay | save asset | 13 |
| `59218ef9` | 2026-11-08 | Kay | chore(kickoff): open sprint-22 2026-11-08 | 3 |
| `0f3644bd` | 2026-11-08 | Kay | animation and add weapon attack mage | 32 |
| `38d40925` | 2026-11-08 | Kay | paladin combo attackcc | 138 |
| `69bb7d63` | 2026-11-07 | Kay | docs(doc-sync): doc-truth pass 2026-11-07 - sprint-21 c6f3ccf2, death input flag, triage statuses | 13 |
| `c6f3ccf2` | 2026-11-07 | Kay | chore(wrapup): weekly wrap-up 2026-11-07 | 2 |
| `5e77ef55` | 2026-11-07 | Kay | chore(wrapup): bug triage 2026-11-07 - BUG-122..124, 7 status updates | 14 |
| `80e025fb` | 2026-11-07 | Kay | test(qa): weekly automation run 20261107-1437 on origin/sprint-21 - FAIL | 11 |
| `d2d45973` | 2026-11-07 | Kay | coding need check | 4 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 4 | `Assets/Script/Character/Base/AbilityHolderBase.cs`<br>`Assets/Script/System/Abilities/Runtime/SpawnMono/DamagesInManyPos.cs`<br>`Assets/SO/Skill/Archer/Ability/Rain Arrow/Effect/Archer Spawn Rain Arrows Effect.asset`<br>`Assets/SO/Skill/Archer/Ability/Rain Arrow/Rain Arrows.asset` |
| CMB | 14 | `Assets/Script/Character/Base/WeaponHolderBase.cs`<br>`Assets/Script/Weapons/Weapon.cs`<br>`Assets/SO/Weapons/MageWeapon.meta`<br>`Assets/SO/Weapons/MageWeapon/Mage Attack SO State 1.asset`<br>... +10 |
| DOCS | 37 | `.claude/launch.json`<br>`.claude/rules/test-standards.md`<br>`CLAUDE.md`<br>`docs/CHANGELOG-DOCS.md`<br>... +33 |
| ENT | 28 | `Assets/Prefab/Enemy/EnemyPrefab.prefab`<br>`Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityInput.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs`<br>... +24 |
| ITEM | 6 | `Assets/SO/Item/PlatiumOre 0.asset`<br>`Assets/SO/Item/PlatiumOre 1.asset`<br>`Assets/SO/Item/PlatiumOre 2.asset`<br>`Assets/SO/Item/PlatiumOre 3.asset`<br>... +2 |
| MAP | 3 | `Assets/Script/System/Enemy/EnemySpawner.cs`<br>`Assets/SO/Dungeon/GameRPG.asmref`<br>`Assets/SO/Dungeon/GameRPG.asmref.meta` |
| OTHER | 3 | `tools/anim-viewer/build-character-viewer.js`<br>`tools/anim-viewer/pack_standalone.py`<br>`tools/anim-viewer/viewer-template.html` |
| PATH | 5 | `Assets/Script/System/Pathfinding/Algorithm/AStar.cs`<br>`Assets/Script/System/Pathfinding/Data/NavPath.cs`<br>`Assets/Script/System/Pathfinding/Data/NavPath.cs.meta`<br>`Assets/Script/System/Pathfinding/Data/PathRequest.cs`<br>... +1 |
| PLY | 7 | `Assets/Script/Character/Base/CharacterData.cs`<br>`Assets/Script/Character/Base/CharacterInputBase.cs`<br>`Assets/Script/Character/Base/MovementBase.cs`<br>`Assets/Script/Character/Player/CoreComponent/PlayerInputHandle.cs`<br>... +3 |
| POOL | 1 | `Assets/Script/System/PoolableService/Pool.cs` |
| QA | 37 | `Assets/QA/Editor/QASceneBuilder.cs`<br>`Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/QAEmptyPathGrid.cs`<br>`Assets/QA/Runtime/QAFloorPathGrid.cs`<br>... +33 |
| STAT | 23 | `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/DerivedStatFormula.cs`<br>`Assets/Script/System/StatSystem/PrimaryStatEntry.cs`<br>`Assets/Script/System/StatSystem/PrimaryStatEntry.cs.meta`<br>... +19 |
| STATIC | 37 | `Assets/Editor/BaseStatsSOEditor.cs`<br>`Assets/Editor/BaseStatsSOEditor.cs.meta`<br>`Assets/Editor/EnemyRoleScaffolder.cs`<br>`Assets/Editor/EntityDataEditor.cs`<br>... +33 |
| UI | 3 | `Assets/Script/UIFlow/Editor/GameRPG.UIFlow.Editor.asmdef`<br>`Assets/Script/UIFlow/Editor/GameRPG.UIFlow.Editor.asmdef.meta`<br>`Assets/Script/UIFlow/Services/Real/RealPlayerDataProvider.cs` |
| VISUAL | 133 | `Assets/Animation/Enemy/Golem Phase 3/GolemP3_Attack_1_Projectile.anim`<br>`Assets/Animation/Enemy/Rat/Rat_Attack.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Attack/Assasin_Attack_dir 0.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Attack/Assasin_Attack_dir 1.anim`<br>... +129 |
| VIT | 2 | `Assets/Script/Character/Base/StatHandlerBase.cs`<br>`Assets/Script/Character/Player/CoreComponent/StatHandler.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/AbilityHolderBase.cs` | Shared by the player and enemies; the UIFlow hotbar binds to AbilityCooldownStarted | PLY, ENT, UI |
| `Assets/Script/Character/Base/CharacterData.cs` | Serialized by every PlayerData / EntityData asset — a rename without [FormerlySerializedAs] empties fields (BUG-105/108/121) | ENT, CMB, ABL, STATIC |
| `Assets/Script/Character/Base/StatHandlerBase.cs` | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Base/WeaponHolderBase.cs` | Shared by the player and enemies; default weapon equip happens in SetCharacterData (BUG-106) | PLY, ENT |
| `Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityInput.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs`<br>... +7 | Item drop on enemy death and the player pickup (QA-ITEM-02..04) run in the ENEMY scene | ITEM |
| `Assets/Script/System/Enemy/EnemySpawner.cs` | Room clear depends on ON_ENEMY_DEATH from pooled enemies | ENT |
| `Assets/Script/System/Pathfinding/Algorithm/AStar.cs`<br>`Assets/Script/System/Pathfinding/Data/NavPath.cs`<br>`Assets/Script/System/Pathfinding/Data/NavPath.cs.meta`<br>... +2 | Enemy wander and chase (QA-ENT-10 / QA-ENT-15) path on a PathfindingGrid | ENT |
| `Assets/Script/System/PoolableService/Pool.cs` | Enemies, projectiles and ability objects are all pooled and injected on spawn | ENT, CMB, ABL |
| `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/DerivedStatFormula.cs`<br>`Assets/Script/System/StatSystem/PrimaryStatEntry.cs`<br>... +5 | Damage formulas, vitals max values and asset serialization all read the stat system | VIT, CMB, ENT, STATIC |
| `Assets/Script/Weapons/Weapon.cs` | Enemies attack through the same Weapon classes; AttackSO field renames break assets | ENT, STATIC |
| `Assets/SO/Enemy/Enemy - Knight 1.asset`<br>`Assets/SO/Enemy/Enemy - Knight 2.asset`<br>`Assets/SO/Enemy/Enemy - Knight 3.asset`<br>... +45 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |
| `Assets/SO/Item/PlatiumOre 0.asset`<br>`Assets/SO/Item/PlatiumOre 1.asset`<br>`Assets/SO/Item/PlatiumOre 2.asset`<br>... +3 | ItemSpawner listens to ON_ENEMY_DEATH; QA-ITEM-02..04 live in the enemy suite | ENT |

## 4. New and deleted

**New runtime scripts (3)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/FastTest/AttackFlowEventProbe.cs` - *no case yet*
- `Assets/Script/System/StatSystem/PrimaryStatEntry.cs` - *no case yet*
- `Assets/Script/System/StatSystem/StatSheet.cs` - covered

**New assets (5)**:

- `Assets/Prefab/Particle Effect/Attack Projectile/MagicBolt.prefab`
- `Assets/SO/Weapons/MageWeapon/Mage Attack SO State 1.asset`
- `Assets/SO/Weapons/MageWeapon/Mage Attack SO State 2.asset`
- `Assets/SO/Weapons/MageWeapon/Mage Attack SO State 3.asset`
- `Assets/SO/Weapons/MageWeapon/Mage Weapon Stats.asset`

**Deleted (1)**:

- `Assets/QA/Runtime/QAEmptyPathGrid.cs`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
