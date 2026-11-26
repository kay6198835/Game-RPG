# Weekly change impact

- Range: `2b3028a0ec` (`2b3028a0ec`) .. `origin/sprint-23` (`76a60d2fda`)
- 17 commits (+9 merge commits), 2224 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, MAP, PLY, POOL, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `8005d27e` | 2026-11-21 | Kay | update animation, asset | 1077 |
| `4df28a08` | 2026-11-21 | Kay | chore(qa-auto): weekly QA run 20261121-0711 - 0 inbox notes 2026-11-21 | 1 |
| `11b460a9` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-0711 - RUNNER_ERROR | 1 |
| `be973605` | 2026-11-21 | Kay | coding need check | 1286 |
| `8f3f37ff` | 2026-11-20 | Kay | chore(wrapup): weekly wrap-up 2026-11-20 | 2 |
| `4daee1e3` | 2026-11-20 | Kay | chore(wrapup): bug triage 2026-11-20 - BUG-129..134, 13 status updates | 27 |
| `633b7647` | 2026-11-20 | Claude | test(qa): coverage review 2026-11-20 - item effect, vitals, event, room, static and EditMode cases | 17 |
| `19540dff` | 2026-11-20 | Kay | knockback, update UI confirm | 6 |
| `b9cdc962` | 2026-11-20 | Claude | docs(qa): list animation clips with a real sprite rate above 30 FPS | 1 |
| `3eb374ed` | 2026-11-20 | Kay | chore(standup): daily standup 2026-11-20 | 4 |
| `69fc12b3` | 2026-11-19 | Kay | chore(qa-auto): weekly QA run 20261119-1616 - 0 inbox notes 2026-11-19 | 2 |
| `5e0e0f27` | 2026-11-19 | Kay | test(qa): weekly automation run 20261119-1638 on origin/sprint-23 - FAIL | 12 |
| `6c289ac9` | 2026-11-19 | Kay | test(qa): inbox proposals for run 20261119-1616 (evidence for NOTE-20261117-3) | 1 |
| `32ad25bd` | 2026-11-19 | Kay | test(qa): part C for run 20261119-1616 - QA-PATH-04..06, QA-MAP-07, manual MAN-ENT-11..13, MAN-MAP-06..07 | 4 |
| `06adc2a0` | 2026-11-19 | Kay | update syntax and logic | 16 |
| `abdbb332` | 2026-11-19 | Kay | feat(ui): return-to-main-menu request for boss exit; document death flows | 6 |
| `49c58e35` | 2026-11-19 | Kay | fix death flow | 4 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 3 | `Assets/Script/System/Abilities/Runtime/BaseController/SpawnMono.cs`<br>`Assets/Script/System/Abilities/Runtime/ImpactMono/SourceEffect.cs`<br>`Assets/Script/System/Abilities/Runtime/SpawnMono/ProjectileBody.cs` |
| CMB | 1 | `Assets/Script/Character/Base/WeaponHolderBase.cs` |
| DOCS | 35 | `CLAUDE.md`<br>`docs/CHANGELOG-DOCS.md`<br>`docs/ui/changelog/ui-ux-flow.CHANGELOG.md`<br>`docs/ui/ui-ux-flow.md`<br>... +31 |
| ITEM | 1 | `Assets/Script/System/Item/ItemSpawner.cs` |
| MAP | 1 | `Assets/Script/System/Enemy/EnemySpawner.cs` |
| OTHER | 147 | `tools/anim-viewer/_old/erika_crouch_dir0.png`<br>`tools/anim-viewer/_old/erika_crouch_dir1.png`<br>`tools/anim-viewer/_old/erika_crouch_dir2.png`<br>`tools/anim-viewer/_old/erika_crouch_dir3.png`<br>... +143 |
| PLY | 9 | `Assets/Script/Character/Base/CharacterInputBase.cs`<br>`Assets/Script/Character/Player/States/PlayerAttackState.cs`<br>`Assets/Script/Character/Player/States/PlayerBasicState.cs`<br>`Assets/Script/Character/Player/States/PlayerDashState.cs`<br>... +5 |
| POOL | 1 | `Assets/Script/System/PoolableService/ObjectPoolManager.cs` |
| QA | 29 | `Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/QAPlayerRig.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>... +25 |
| STATIC | 8 | `Assets/Script/System/RoomGameSystem/PedestalChampionController.cs`<br>`Assets/Tests/EditMode/AbilityDefinitionTests.cs`<br>`Assets/Tests/EditMode/AbilityDefinitionTests.cs.meta`<br>`Assets/Tests/EditMode/ItemEffectDataTests.cs`<br>... +4 |
| UI | 5 | `Assets/Script/UI/StatsUIController.cs`<br>`Assets/Script/UIFlow/Core/UIEvents.cs`<br>`Assets/Script/UIFlow/Gameplay/GameplayUIController.cs`<br>`Assets/Script/UIFlow/Gameplay/Windows/ConfirnPanel.cs`<br>... +1 |
| VISUAL | 1983 | `Assets/Animation/Enemy/Bat/Bat_Attack.anim`<br>`Assets/Animation/Enemy/Bat/Bat_Death.anim`<br>`Assets/Animation/Enemy/Bat/Bat_Hit.anim`<br>`Assets/Animation/Enemy/Bat/Bat_Idle.anim`<br>... +1979 |
| VIT | 1 | `Assets/Script/Character/Base/NegativeReceiverBase.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/NegativeReceiverBase.cs` | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Base/WeaponHolderBase.cs` | Shared by the player and enemies; default weapon equip happens in SetCharacterData (BUG-106) | PLY, ENT |
| `Assets/Script/System/Enemy/EnemySpawner.cs` | Room clear depends on ON_ENEMY_DEATH from pooled enemies | ENT |
| `Assets/Script/System/Item/ItemSpawner.cs` | ItemSpawner listens to ON_ENEMY_DEATH; QA-ITEM-02..04 live in the enemy suite | ENT |
| `Assets/Script/System/LifetimeScope/Interface/IObjecPoolService.cs`<br>`Assets/Script/System/PlayerSystem/PlayerManager.cs` | Injection of the whole player chain | VIT, CMB, ABL, ENT |
| `Assets/Script/System/PoolableService/ObjectPoolManager.cs` | Enemies, projectiles and ability objects are all pooled and injected on spawn | ENT, CMB, ABL |
| `Assets/Script/UIFlow/Core/UIEvents.cs`<br>`Assets/Script/UIFlow/Gameplay/Windows/ConfirnPanel.cs` | The Champion pick flow opens the confirm panel through UIEvents | PLY |

## 4. New and deleted

**New runtime scripts (0)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- none

**New assets (0)**:

- none

**Deleted (249)**:

- `Assets/Sprite/Erika/Animations2D/erika_arrow_8dir_controller.controller`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir0.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir1.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir2.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir3.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir4.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir5.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir6.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_arrow_dir7.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir0.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir1.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir2.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir3.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir4.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir5.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir6.anim`
- `Assets/Sprite/Erika/Animations2D/erika_attack_up45_arrow_dir7.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir0.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir1.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir2.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir3.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir4.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir5.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir6.anim`
- `Assets/Sprite/Erika/Animations2D/erika_crouch_arrow_dir7.anim`
- `Assets/Sprite/Erika/Animations2D/erika_death_arrow_dir0.anim`
- `Assets/Sprite/Erika/Animations2D/erika_death_arrow_dir1.anim`
- `Assets/Sprite/Erika/Animations2D/erika_death_arrow_dir2.anim`
- `Assets/Sprite/Erika/Animations2D/erika_death_arrow_dir3.anim`
- `Assets/Sprite/Erika/Animations2D/erika_death_arrow_dir4.anim`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
