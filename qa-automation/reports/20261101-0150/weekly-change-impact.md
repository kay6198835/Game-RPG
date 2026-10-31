# Weekly change impact

- Range: `origin/sprint-19` (`566a684d47`) .. `origin/sprint-20` (`09ca5d1905`)
- 20 commits (+7 merge commits), 182 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, PLY, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `86b72074` | 2026-10-31 | Kay | docs(doc-sync): doc-truth pass 2026-10-31 - sprint-20 renames, BUG-110..121 rows, 4 inbox notes | 24 |
| `e8baf251` | 2026-10-31 | Kay | chore(wrapup): weekly wrap-up 2026-10-31 | 2 |
| `b63b4a40` | 2026-10-31 | Kay | chore(wrapup): triage 2026-10-31 - BUG-116..121, inbox emptied | 9 |
| `343a3830` | 2026-10-29 | Kay | coding | 47 |
| `a787f234` | 2026-10-29 | Claude | fix(test): make CombatTestEnemySpawner registration optional | 1 |
| `ba8bcabc` | 2026-10-29 | Kay | fix(anim): retarget Archer Hawk Eye clips to the erika_attack sheet | 27 |
| `fc98fa2a` | 2026-10-29 | Claude | feat(test): real enemy in CombatTest, pinned in place; fix BUG-108 stats key | 5 |
| `0b05f72d` | 2026-10-29 | Kay | chore(standup): daily standup 2026-10-29 | 3 |
| `6f9d0971` | 2026-10-28 | Claude | fix(test): qualify System.IO.Path in CombatTestSceneBuilder | 1 |
| `87e053b4` | 2026-10-28 | Claude | feat(test): add CombatTest scene builder with class pick and training dummy | 10 |
| `5b8789a1` | 2026-10-28 | Kay | chore(standup): daily standup 2026-10-28 | 2 |
| `3e90f323` | 2026-10-27 | Claude | docs: log effect source plan in CHANGELOG-DOCS | 1 |
| `ef0d9c6f` | 2026-10-27 | Claude | docs: add effect source plan (SourceEffect per applied effect) | 1 |
| `26c5b4e1` | 2026-10-27 | Claude | docs(architecture): add core component access plan (public vs internal interfaces) | 3 |
| `6e484f0c` | 2026-10-27 | Kay | update logic effect | 15 |
| `8f17c977` | 2026-10-26 | Kay | chore(standup): daily standup 2026-10-26 | 3 |
| `e156bdf4` | 2026-10-25 | Kay | fix conflict | 17 |
| `041292ce` | 2026-10-25 | Kay | done flow pick champion | 40 |
| `7663aee7` | 2026-10-25 | Kay | chore(kickoff): open sprint-20 2026-10-25 | 4 |
| `786c031b` | 2026-10-25 | VIETUNION\kiet.ho | add effec component | 21 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 18 | `Assets/Script/Character/Base/AbilityHolderBase.cs`<br>`Assets/Script/System/Abilities/Core/AbilityContext.cs`<br>`Assets/Script/System/Abilities/Core/AbilityDefinition.cs`<br>`Assets/Script/System/Abilities/Core/AbilityEffectDefinition.cs`<br>... +14 |
| CMB | 2 | `Assets/Script/Character/Base/WeaponHolderBase.cs`<br>`Assets/Script/Weapons/Weapon.cs` |
| DOCS | 40 | `.claude/launch.json`<br>`.claude/settings.local.json`<br>`CLAUDE.md`<br>`docs/architecture/changelog/core-component-access-plan.CHANGELOG.md`<br>... +36 |
| ENT | 5 | `Assets/Script/Character/Entity/CoreComponent/EntityEffectReceiver.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityEffectReceiver.cs.meta`<br>`Assets/Script/Character/Entity/CoreComponent/EntityNegativeReciver.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityVitalStats.cs`<br>... +1 |
| ITEM | 8 | `Assets/Script/System/Item/ItemEffectDefinition/CoinModifierEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierCurrentStatsEffect.cs`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierCurrentStatsEffect.cs.meta`<br>`Assets/Script/System/Item/ItemEffectDefinition/ModifierMaxStatsEffect.cs`<br>... +4 |
| OTHER | 3 | `tools/anim-viewer/archer-animations.html`<br>`tools/anim-viewer/build-archer-viewer.js`<br>`tools/anim-viewer/viewer-template.html` |
| PLY | 16 | `Assets/Prefab/Player/PlayerTest.prefab`<br>`Assets/Script/Character/Base/CharacterBase.cs`<br>`Assets/Script/Character/Base/CharacterData.cs`<br>`Assets/Script/Character/Base/Interface/ICharacter.cs`<br>... +12 |
| QA | 14 | `Assets/Script/FastTest/AttackFlowDebugger.cs`<br>`Assets/Script/FastTest/AttackFlowDebugger.cs.meta`<br>`Assets/Script/FastTest/CombatTest.meta`<br>`Assets/Script/FastTest/CombatTest/CombatTestBootstrap.cs`<br>... +10 |
| STAT | 8 | `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/Stat.cs`<br>`Assets/Script/System/StatSystem/StatModifier.cs`<br>`Assets/Script/System/StatSystem/StatModifierGroup.cs`<br>... +4 |
| STATIC | 7 | `Assets/Prefab/Particle Effect/Slash-VFX-GO.prefab`<br>`Assets/Prefab/Particle Effect/SourceEffect.prefab`<br>`Assets/Prefab/Particle Effect/SourceEffect.prefab.meta`<br>`Assets/Scenes/Main/LoadRandomMap.unity`<br>... +3 |
| UI | 1 | `Assets/Script/UIFlow/Services/Real/RealPlayerDataProvider.cs` |
| VISUAL | 53 | `Assets/Animation/Player/Archer/Ability-Skill/Hawk Eye/Archer Hawk Eye State 1/erika_attack_arrow_State1_dir0.anim`<br>`Assets/Animation/Player/Archer/Ability-Skill/Hawk Eye/Archer Hawk Eye State 1/erika_attack_arrow_State1_dir1.anim`<br>`Assets/Animation/Player/Archer/Ability-Skill/Hawk Eye/Archer Hawk Eye State 1/erika_attack_arrow_State1_dir2.anim`<br>`Assets/Animation/Player/Archer/Ability-Skill/Hawk Eye/Archer Hawk Eye State 1/erika_attack_arrow_State1_dir3.anim`<br>... +49 |
| VIT | 7 | `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/EffectReceiverBase.cs.meta`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Base/StatHandlerBase.cs`<br>... +3 |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/AbilityHolderBase.cs` | Shared by the player and enemies; the UIFlow hotbar binds to AbilityCooldownStarted | PLY, ENT, UI |
| `Assets/Script/Character/Base/CharacterBase.cs`<br>`Assets/Script/Character/Base/CharacterData.cs` | Serialized by every PlayerData / EntityData asset — a rename without [FormerlySerializedAs] empties fields (BUG-105/108/121) | ENT, CMB, ABL, STATIC |
| `Assets/Script/Character/Base/EffectReceiverBase.cs`<br>`Assets/Script/Character/Base/EffectReceiverBase.cs.meta`<br>`Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>... +2 | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Base/WeaponHolderBase.cs` | Shared by the player and enemies; default weapon equip happens in SetCharacterData (BUG-106) | PLY, ENT |
| `Assets/Script/System/Abilities/Core/AbilityContext.cs`<br>`Assets/Script/System/Abilities/Core/AbilityDefinition.cs`<br>`Assets/Script/System/Abilities/Core/AbilityEffectDefinition.cs`<br>... +2 | Cast gating and cooldowns drive the player skill state, enemy abilities and the hotbar | PLY, ENT, UI |
| `Assets/Script/System/StatSystem/BaseStatsSO.cs`<br>`Assets/Script/System/StatSystem/Stat.cs`<br>`Assets/Script/System/StatSystem/StatModifier.cs`<br>... +3 | Damage formulas, vitals max values and asset serialization all read the stat system | VIT, CMB, ENT, STATIC |
| `Assets/Script/Weapons/Weapon.cs` | Enemies attack through the same Weapon classes; AttackSO field renames break assets | ENT, STATIC |
| `Assets/SO/Player/Archer/Archer.asset`<br>`Assets/SO/Player/Mage/Mage.asset`<br>`Assets/SO/Player/Paladin/Paladin.asset`<br>... +6 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (14)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/Character/Base/EffectReceiverBase.cs` - *no case yet*
- `Assets/Script/Character/Entity/CoreComponent/EntityEffectReceiver.cs` - *no case yet*
- `Assets/Script/Character/Player/CoreComponent/PlayerEffectReceiver.cs` - *no case yet*
- `Assets/Script/FastTest/AttackFlowDebugger.cs` - *no case yet*
- `Assets/Script/FastTest/CombatTest/CombatTestBootstrap.cs` - *no case yet*
- `Assets/Script/FastTest/CombatTest/CombatTestEnemySpawner.cs` - *no case yet*
- `Assets/Script/FastTest/CombatTest/CombatTestLifetimeScope.cs` - *no case yet*
- `Assets/Script/FastTest/CombatTest/TrainingDummy.cs` - *no case yet*
- `Assets/Script/Interface/IEffectReceiver.cs` - *no case yet*
- `Assets/Script/System/Abilities/Runtime/ImpactMono/SourceEffect.cs` - *no case yet*
- `Assets/Script/System/Item/ItemEffectDefinition/ModifierCurrentStatsEffect.cs` - *no case yet*
- `Assets/Script/System/Item/ItemEffectDefinition/ModifierMaxStatsEffect.cs` - *no case yet*
- `Assets/Script/System/Item/ItemEffectDefinition/StatModifierEffect.cs` - *no case yet*
- `Assets/Script/System/StatSystem/VitalModifier.cs` - *no case yet*

**New assets (1)**:

- `Assets/Prefab/Particle Effect/SourceEffect.prefab`

**Deleted (1)**:

- `Assets/Script/System/Item/ItemEffectDefinition/StatModifierEffectDefinition.cs`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
