# Weekly change impact

- Range: `a8725a77d2` (`a8725a77d2`) .. `origin/sprint-23` (`077ebd31e3`)
- 40 commits (+12 merge commits), 223 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, MAP, PLY, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `58b3da35` | 2026-11-17 | Kay | update | 1 |
| `dc0b0fbf` | 2026-11-17 | Claude | docs(qa): NOTE-20261117-2 - BUG-118 deferred until after demo (owner) | 1 |
| `a1cea244` | 2026-11-17 | Claude | docs(qa): NOTE-20261117-1 - BUG-071 triage hint after owner fix | 1 |
| `0626dd59` | 2026-11-17 | Kay | update | 3 |
| `b5fb3581` | 2026-11-17 | Kay | miss fix bug 071 | 3 |
| `16f61da7` | 2026-11-17 | Kay | fix bug 071 | 7 |
| `f9ed9366` | 2026-11-17 | Kay | chore(standup): daily standup 2026-11-17 | 3 |
| `1d05475a` | 2026-11-16 | Claude | chore(abilities): finish ShootSpirit removal (BUG-073) | 12 |
| `e6c826e5` | 2026-11-16 | Claude | chore(data): remove ShootSpirit / SpiritBomd ability assets (BUG-073, BUG-090) | 11 |
| `b9263280` | 2026-11-16 | Claude | revert(weapon): drop the Shot payload from 073f2421, keep owner's finalDamage logic | 1 |
| `20cf6ca5` | 2026-11-16 | Claude | docs(weapons): ranged damage follows owner logic (finalDamage field, 3a2659ce) | 6 |
| `3a2659ce` | 2026-11-16 | Kay | logic damage for rangeweapon | 3 |
| `073f2421` | 2026-11-16 | Claude | fix(weapon): ranged basic attack scales with PhysicalDamage + crit (NOTE-20261104-4) | 3 |
| `cfb6a230` | 2026-11-16 | Kay | despawn source effect | 2 |
| `a7d48992` | 2026-11-16 | Claude | docs(qa): handoff note - update auto tests for sprint 23 dev changes (NOTE-20261116-4) | 2 |
| `aa045f79` | 2026-11-16 | Claude | feat(item): coin item effect asset + Create menu (BUG-123) | 3 |
| `8236f9d1` | 2026-11-16 | Kay | update 123 and 120 | 2 |
| `25d2d5e4` | 2026-11-16 | Claude | fix(data): rebind item effect assets to concrete scripts (BUG-120) | 3 |
| `25d869fb` | 2026-11-16 | Kay | update bug 95, 120, 123 and issue commit | 11 |
| `f528c087` | 2026-11-16 | Kay | bug 117, 120, 123, 108, 072, 095 | 3 |
| `041bf198` | 2026-11-16 | Claude | chore(data): delete retired Knight enemy assets (S23-07, BUG-108) | 36 |
| `042e6cc1` | 2026-11-16 | Claude | docs(inbox): NOTE-20261116-3 owner decision - BUG-106 player half by design, BUG-110 deferred | 1 |
| `b92b6f2a` | 2026-11-16 | Kay | bug-125 | 2 |
| `91c42d79` | 2026-11-16 | Kay | add dash state | 26 |
| `f39dd0b8` | 2026-11-16 | Kay | chore(standup): daily standup 2026-11-16 | 3 |
| `a0d52d73` | 2026-11-15 | Kay | chore(kickoff): duplicate firing skipped 2026-11-15 | 1 |
| `88140733` | 2026-11-15 | Kay | chore(kickoff): open sprint-23 2026-11-15 | 4 |
| `37b56d76` | 2026-11-15 | Kay | chore(doc-sync): routine log 2026-11-15 | 1 |
| `c7853589` | 2026-11-15 | Kay | docs(doc-sync): doc-truth pass 2026-11-15 - triage statuses BUG-063/084/109/115/124, rows BUG-125..128 | 11 |
| `063b5f87` | 2026-11-15 | Kay | chore(wrapup): weekly wrap-up 2026-11-15 | 2 |
| `b5e3238b` | 2026-11-15 | Kay | chore(wrapup): bug triage 2026-11-15 - BUG-125..128, 5 status updates | 14 |
| `ff7f3ecb` | 2026-11-15 | Kay | chore(doc-sync): routine log 2026-11-14 | 1 |
| `8a114568` | 2026-11-15 | Kay | docs(doc-sync): doc-truth pass 2026-11-14 - sprint-22 a8725a77, stat split, enemy sight/Defense, NavPath, asmdef tests | 16 |
| `cb5b51a9` | 2026-11-15 | Kay | chore(qa-auto): weekly QA run 20261114-2357 - 5 inbox notes 2026-11-15 | 2 |
| `174feb3a` | 2026-11-15 | Kay | test(qa): inbox proposals use the date of writing (NOTE-20261115-N), re-confirmed by run 20261114-2357 | 1 |
| `ca89c300` | 2026-11-15 | Kay | test(qa): weekly automation run 20261114-2357 on origin/sprint-22 - FAIL | 12 |
| `2c3fe106` | 2026-11-14 | Kay | test(qa): fix Rain Arrows and STATIC-05 test defects, add QA-STAT-07, map VIT-07/ITEM-03, run 20261114-2329 triage | 8 |
| `94f1acac` | 2026-11-14 | Kay | test(qa): weekly automation run 20261114-2329 on origin/sprint-22 - FAIL | 12 |
| `3b02df7d` | 2026-11-14 | Kay | coding need check | 39 |
| `f6921dc9` | 2026-11-13 | VIETUNION\kiet.ho | update doc GGD and add state dash for player | 25 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 11 | `Assets/Script/System/Abilities/Effects/SpawnEffectBase.cs`<br>`Assets/Script/System/Abilities/Runtime/ImpactMono/SourceEffect.cs`<br>`Assets/Script/System/Abilities/Runtime/SpawnMono/ProjectileBody.cs`<br>`Assets/SO/Skill/Enemy - Knight.asset`<br>... +7 |
| CMB | 8 | `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs`<br>`Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs`<br>`Assets/Script/Weapons/Weapon.cs`<br>`Assets/SO/Weapons/KnightWeapon.meta`<br>... +4 |
| DOCS | 57 | `.claude/rules/prototype-code.md`<br>`.claude/rules/scriptableobject-data.md`<br>`.claude/rules/test-standards.md`<br>`CLAUDE.md`<br>... +53 |
| ENT | 22 | `Assets/Script/Character/Entity/CoreComponent/EntityUIController.cs`<br>`Assets/Script/Character/Entity/Entity.cs`<br>`Assets/SO/Enemy/Enemy - Knight 1.asset`<br>`Assets/SO/Enemy/Enemy - Knight 1.asset.meta`<br>... +18 |
| ITEM | 9 | `Assets/Script/System/Item/ItemEffectDefinition/CoinModifierEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierCurrentStatsEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierMaxStatsEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/StatModifierEffect.cs`<br>... +5 |
| MAP | 7 | `Assets/Data/Json/Room/BossRoom_ThroneArena.json`<br>`Assets/Data/Json/Room/NormalRoom_20.json`<br>`Assets/Data/Json/Room/NormalRoom_20.json.meta`<br>`Assets/SO/Database/EnemyPrefab.prefab`<br>... +3 |
| OTHER | 2 | `tools/anim-viewer/character-animations.html`<br>`tools/anim-viewer/character-animations-standalone.html` |
| PLY | 15 | `Assets/Prefab/Player/PlayerTest.prefab`<br>`Assets/Script/Character/Base/CharacterData.cs`<br>`Assets/Script/Character/Base/CharacterInputBase.cs`<br>`Assets/Script/Character/Base/MovementBase.cs`<br>... +11 |
| QA | 32 | `Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`Assets/QA/Runtime/Suites/QAPlayerFlowSuite.cs`<br>`Assets/QA/Runtime/Suites/QASystemsSuite.cs`<br>... +28 |
| STAT | 2 | `Assets/Script/System/StatSystem/StatModifier.cs`<br>`Assets/Script/System/StatSystem/VitalModifier.cs` |
| STATIC | 22 | `Assets/Prefab/Bullet/SpiritProjcetile.prefab`<br>`Assets/Prefab/Bullet/SpiritProjcetile.prefab.meta`<br>`Assets/Prefab/Champion Monument/Champion_Monument_Archer.prefab`<br>`Assets/Prefab/Champion Monument/Champion_Monument_Mage.prefab`<br>... +18 |
| UI | 6 | `Assets/Script/UIFlow/Core/UIEvents.cs`<br>`Assets/Script/UIFlow/Gameplay/GameplayUIController.cs`<br>`Assets/Script/UIFlow/Gameplay/HUD/HUDPanel.cs`<br>`Assets/Script/UIFlow/Gameplay/HUD/InteractiveNotifi.cs`<br>... +2 |
| VISUAL | 27 | `Assets/Animation/Player/Paladin/Basic/Dash.meta`<br>`Assets/Animation/Player/Paladin/Basic/Dash/Paladin_Dash_dir0.anim`<br>`Assets/Animation/Player/Paladin/Basic/Dash/Paladin_Dash_dir0.anim.meta`<br>`Assets/Animation/Player/Paladin/Basic/Dash/Paladin_Dash_dir1.anim`<br>... +23 |
| VIT | 3 | `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Base/VitalStatsBase.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/CharacterData.cs` | Serialized by every PlayerData / EntityData asset — a rename without [FormerlySerializedAs] empties fields (BUG-105/108/121) | ENT, CMB, ABL, STATIC |
| `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Base/VitalStatsBase.cs` | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Entity/CoreComponent/EntityUIController.cs`<br>`Assets/Script/Character/Entity/Entity.cs` | Item drop on enemy death and the player pickup (QA-ITEM-02..04) run in the ENEMY scene | ITEM |
| `Assets/Script/System/Item/ItemEffectDefinition/CoinModifierEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierCurrentStatsEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierMaxStatsEffect.cs`<br>... +6 | ItemSpawner listens to ON_ENEMY_DEATH; QA-ITEM-02..04 live in the enemy suite | ENT |
| `Assets/Script/System/StatSystem/StatModifier.cs`<br>`Assets/Script/System/StatSystem/VitalModifier.cs` | Damage formulas, vitals max values and asset serialization all read the stat system | VIT, CMB, ENT, STATIC |
| `Assets/Script/UIFlow/Core/UIEvents.cs` | The Champion pick flow opens the confirm panel through UIEvents | PLY |
| `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs`<br>`Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs`<br>`Assets/Script/Weapons/Weapon.cs` | Enemies attack through the same Weapon classes; AttackSO field renames break assets | ENT, STATIC |
| `Assets/SO/Dungeon/Tile/Tile_Spawn_Bonfire.asset`<br>`Assets/SO/Enemy/Enemy - Knight 1.asset`<br>`Assets/SO/Enemy/Enemy - Knight 2.asset`<br>... +18 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (4)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/Character/Player/States/PlayerDashState.cs` - *no case yet*
- `Assets/Script/System/RoomGameSystem/BonfireController.cs` - *no case yet*
- `Assets/Script/System/RoomGameSystem/InteractiveObjectsRoom.cs` - *no case yet*
- `Assets/Script/UIFlow/Gameplay/HUD/InteractiveNotifi.cs` - *no case yet*

**New assets (3)**:

- `Assets/Prefab/Interactive/Bonfire.prefab`
- `Assets/SO/Dungeon/Tile/Tile_Spawn_Bonfire.asset`
- `Assets/SO/Item/Effect/Coin Effect.asset`

**Deleted (19)**:

- `Assets/AnimationController/2 Direction/Enemy/Knight_Controller.overrideController`
- `Assets/Prefab/Bullet/SpiritProjcetile.prefab`
- `Assets/Prefab/Weapon/KnightWeapon.prefab`
- `Assets/Prefab/Weapon/WeaponEntity.prefab`
- `Assets/SO/Database/EnemyPrefab.prefab`
- `Assets/SO/Enemy/Enemy - Knight 1.asset`
- `Assets/SO/Enemy/Enemy - Knight 2.asset`
- `Assets/SO/Enemy/Enemy - Knight 3.asset`
- `Assets/SO/Enemy/Enemy - Knight 4.asset`
- `Assets/SO/Enemy/Enemy - Knight.asset`
- `Assets/SO/Enemy/Knight/Knight.asset`
- `Assets/SO/Enemy/Weapon/EnemySnS_Stat.asset`
- `Assets/SO/Enemy/Weapon/EnemySword&Shiled.asset`
- `Assets/SO/Enemy/Weapon/Knight_Attack1.asset`
- `Assets/SO/Skill/Enemy - Knight.asset`
- `Assets/SO/Skill/ShootSpirit/MainEffect/ShootSpirit.asset`
- `Assets/SO/Skill/ShootSpirit/SpiritBomd.asset`
- `Assets/SO/Weapons/KnightWeapon/KnightSnS_Stat.asset`
- `Assets/SO/Weapons/KnightWeapon/KnightSnS_State1.asset`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
