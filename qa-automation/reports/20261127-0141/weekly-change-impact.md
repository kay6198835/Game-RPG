# Weekly change impact

- Range: `bcedaebb88` (`bcedaebb88`) .. `origin/sprint-24` (`075711d9ad`)
- 37 commits (+10 merge commits), 806 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, MAP, PLY, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `075711d9` | 2026-11-27 | Kay | chore(qa-auto): weekly QA run 20261127-0134 - 0 inbox notes 2026-11-27 | 1 |
| `d3ef66da` | 2026-11-27 | Kay | chore(standup): daily standup 2026-11-27 | 4 |
| `fa206f82` | 2026-11-26 | Claude | fix(anim): delete the Exit transition from the 4 spawn effect controllers | 6 |
| `6dd66791` | 2026-11-26 | Claude | fix(anim): spawn effect clips play once and despawn (Exit transition off) | 6 |
| `43bb14be` | 2026-11-26 | Claude | fix(anim): Consecrate player clips play once with a padded end; Activate -> Cast exit time 1 | 19 |
| `e52431d6` | 2026-11-26 | Claude | fix(abilities): summon spawns once; RainArrows clips hold the last frame; docs for A1 | 14 |
| `83800891` | 2026-11-26 | Claude | feat(stats): StatModifier value is a magnitude, impactType serialized (A1) | 4 |
| `f75573c8` | 2026-11-26 | Kay | update miss | 3 |
| `a9398fc5` | 2026-11-26 | Kay | fix rain arrows skill | 2 |
| `c973a8fb` | 2026-11-26 | Kay | fix | 7 |
| `8377ebaf` | 2026-11-26 | Kay | coding | 12 |
| `85899a52` | 2026-11-26 | Claude | feat(abilities): Delivery / Payload split, SelfApplyEffect, dead guard | 27 |
| `b2202215` | 2026-11-26 | Kay | chore(standup): daily standup 2026-11-26 | 3 |
| `b001d78e` | 2026-11-25 | Claude | feat(abilities): DamageEffect — ability damage through IEffectReceiver | 42 |
| `f2a580b2` | 2026-11-25 | Kay | add trap object, update effect logic sub effect | 34 |
| `da45d8c1` | 2026-11-25 | Kay | chore(standup): daily standup 2026-11-25 | 3 |
| `e47f5c38` | 2026-11-24 | Kay | fix bug | 9 |
| `070bc0a5` | 2026-11-24 | Kay | chore(build): remove test leftovers and TMP examples blocking Player build | 273 |
| `7a41d7c4` | 2026-11-24 | Kay | fix(map): load room JSON from TextAsset so rooms work in Player builds | 5 |
| `b0cfe539` | 2026-11-24 | Kay | feat(anim): wire dash state into Paladin, Mage, Archer controllers | 3 |
| `d32edffd` | 2026-11-24 | Kay | chore(standup): daily standup 2026-11-24 | 2 |
| `39280c8d` | 2026-11-23 | Kay | chore(standup): daily standup 2026-11-23 | 3 |
| `df9e98fe` | 2026-11-23 | Kay | add status effect | 22 |
| `c40da7b8` | 2026-11-23 | Kay | chore: stop tracking Claude Code config, add graphify ignore | 306 |
| `b833f907` | 2026-11-22 | Kay | chore(kickoff): open sprint-24 2026-11-22 | 4 |
| `28301158` | 2026-11-22 | Kay | docs(doc-sync): doc-truth pass 2026-11-22 - BUG-135 receiver split, pooled player spawn, enemy investigate, grid rules | 16 |
| `1e5afb3c` | 2026-11-22 | Kay | add effect dash | 21 |
| `77beaa2d` | 2026-11-21 | Kay | chore(wrapup): run-log row for duplicate firing 2026-11-21 (SKIPPED) | 1 |
| `6551d0d0` | 2026-11-21 | Kay | chore(wrapup): weekly wrap-up 2026-11-21 | 2 |
| `1107d0bb` | 2026-11-21 | Kay | chore(wrapup): triage 2026-11-21 - BUG-135 (S1 hurtbox interface), BUG-129 FIXED, BUG-130 evidence | 7 |
| `191087ad` | 2026-11-21 | Kay | chore(qa-auto): weekly QA run 20261121-2106 - 1 inbox notes 2026-11-21 | 2 |
| `36d0b0db` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-2106 on origin/sprint-23 - FAIL | 12 |
| `f96b86d8` | 2026-11-21 | Kay | test(qa): part C for run 20261121-2050 - QA-VIT-15..16, hurtbox guards, known-issues and static baseline, 5 manual rows | 7 |
| `14f69eaf` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-2050 on origin/sprint-23 - FAIL | 12 |
| `d4f762ca` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-2050 - RUNNER_ERROR | 1 |
| `501b941a` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-2029 - RUNNER_ERROR | 1 |
| `a9943aca` | 2026-11-21 | Kay | test(qa): weekly automation run 20261121-1931 on origin/sprint-23 - FAIL | 12 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 78 | `Assets/Script/System/Abilities/Core/AbilityContext.cs`<br>`Assets/Script/System/Abilities/Core/AbilityEffectDefinition.cs`<br>`Assets/Script/System/Abilities/Core/DeliveryEffectBase.cs`<br>`Assets/Script/System/Abilities/Core/DeliveryEffectBase.cs.meta`<br>... +74 |
| DOCS | 335 | `.claude/agent-memory/lead-programmer/MEMORY.md`<br>`.claude/agents/accessibility-specialist.md`<br>`.claude/agents/ai-programmer.md`<br>`.claude/agents/analytics-engineer.md`<br>... +331 |
| ENT | 2 | `Assets/Prefab/Enemy/EnemyPrefab.prefab`<br>`Assets/Script/Character/Entity/CoreComponent/EntityEffectReceiver.cs` |
| MAP | 10 | `Assets/Script/LevelEdit/LevelEditorTool.cs`<br>`Assets/Script/Map/Room/RoomGeneraterController.cs`<br>`Assets/SO/Database/Enemy/Prefab/Assasin.prefab`<br>`Assets/SO/Database/Enemy/Prefab/Boss.prefab`<br>... +6 |
| OTHER | 2 | `.gitignore`<br>`.graphifyignore` |
| PLY | 6 | `Assets/Prefab/Player/PlayerTest.prefab`<br>`Assets/Script/Character/Base/StatusBase.cs`<br>`Assets/Script/Character/Player/CoreComponent/PlayerMovement.cs`<br>`Assets/Script/Character/Player/States/PlayerDashState.cs`<br>... +2 |
| QA | 54 | `Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.Items.cs`<br>`Assets/QA/Runtime/Suites/QASystemsSuite.cs`<br>... +50 |
| STAT | 5 | `Assets/Script/System/StatSystem/StatModifier.cs`<br>`Assets/Script/System/StatSystem/StatModifierGroup.cs`<br>`Assets/Script/System/StatSystem/StatModifierTester.cs`<br>`Assets/Script/System/StatSystem/StatSheet.cs`<br>... +1 |
| STATIC | 267 | `Assets/Prefab/Interactable Object/Trap.prefab`<br>`Assets/Prefab/Interactable Object/Trap.prefab.meta`<br>`Assets/Prefab/Particle Effect/CircleSummon.prefab`<br>`Assets/Prefab/Particle Effect/DashSmoke.prefab`<br>... +263 |
| VISUAL | 46 | `Assets/Animation/Particle Effect/DashEffect.anim`<br>`Assets/Animation/Particle Effect/DashEffect.anim.meta`<br>`Assets/Animation/Particle Effect/RainArrows/RainArrows_dir 0.anim`<br>`Assets/Animation/Particle Effect/RainArrows/RainArrows_dir 1.anim`<br>... +42 |
| VIT | 1 | `Assets/Script/Character/Base/EffectReceiverBase.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/EffectReceiverBase.cs` | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |
| `Assets/Script/Character/Entity/CoreComponent/EntityEffectReceiver.cs` | Item drop on enemy death and the player pickup (QA-ITEM-02..04) run in the ENEMY scene | ITEM |
| `Assets/Script/Map/Room/RoomGeneraterController.cs` | Room clear depends on ON_ENEMY_DEATH from pooled enemies | ENT |
| `Assets/Script/System/Abilities/Core/AbilityContext.cs`<br>`Assets/Script/System/Abilities/Core/AbilityEffectDefinition.cs`<br>`Assets/Script/System/Abilities/Core/DeliveryEffectBase.cs`<br>... +3 | Cast gating and cooldowns drive the player skill state, enemy abilities and the hotbar | PLY, ENT, UI |
| `Assets/Script/System/StatSystem/StatModifier.cs`<br>`Assets/Script/System/StatSystem/StatModifierGroup.cs`<br>`Assets/Script/System/StatSystem/StatModifierTester.cs`<br>... +2 | Damage formulas, vitals max values and asset serialization all read the stat system | VIT, CMB, ENT, STATIC |
| `Assets/SO/Config/Status Status Config.asset`<br>`Assets/SO/Dungeon/Maze_Storage.asset`<br>`Assets/SO/Skill/Archer/Ability/Hawk Eye (GDD)/Effect/Archer Hawk Eye Damage Effect.asset`<br>... +34 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (7)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/Config/StatusStatsEffect.cs` - *no case yet*
- `Assets/Script/System/Abilities/Core/DeliveryEffectBase.cs` - *no case yet*
- `Assets/Script/System/Abilities/Core/PayloadEffectBase.cs` - *no case yet*
- `Assets/Script/System/Abilities/Effects/DamageEffect.cs` - *no case yet*
- `Assets/Script/System/Abilities/Effects/SelfApplyEffect.cs` - *no case yet*
- `Assets/Script/System/Abilities/Runtime/BaseController/SpawnEffectMono.cs` - *no case yet*
- `Assets/Script/System/Abilities/Runtime/SpawnMono/Trap.cs` - *no case yet*

**New assets (13)**:

- `Assets/Prefab/Interactable Object/Trap.prefab`
- `Assets/Prefab/Particle Effect/DashSmoke.prefab`
- `Assets/SO/Config/Status Status Config.asset`
- `Assets/SO/Skill/Archer/Ability/Hawk Eye (GDD)/Effect/Archer Hawk Eye Damage Effect.asset`
- `Assets/SO/Skill/Archer/Ability/Rain Arrow/Effect/Archer Rain Arrow Damage Effect.asset`
- `Assets/SO/Skill/Archer/Ability/Rain Arrows (GDD)/Effect/Archer Rain Arrows Damage Effect.asset`
- `Assets/SO/Skill/Mage/Ability/Chain Lightning (GDD)/Effect/Mage Chain Lightning Damage Effect.asset`
- `Assets/SO/Skill/Mage/Ability/Fireball (GDD)/Effect/Mage Fireball Damage Effect.asset`
- `Assets/SO/Skill/Mage/Ability/Meteor (GDD)/Effect/Mage Meteor Damage Effect.asset`
- `Assets/SO/Skill/Paladin/Ability/Avatar of Light/Effect/Avatar of Light Self Apply Effect.asset`
- `Assets/SO/Skill/Paladin/Ability/Blessed Slash/Effect/Paladin Blessed Slash Damage Effect.asset`
- `Assets/SO/Skill/Paladin/Ability/Blessing/Effect/Paladin Blessing Self Apply Effect.asset`
- `Assets/SO/Skill/Paladin/Ability/Consecrate/Effect/Paladin Consecrate Damage Effect.asset`

**Deleted (120)**:

- `Assets/Script/FastTest/FasTestEnemyDeath.cs`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Anton OFL.txt`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Anton.ttf`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Bangers - OFL.txt`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Bangers.ttf`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Electronic Highway Sign.TTF`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Oswald-Bold - OFL.txt`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Oswald-Bold.ttf`
- `Assets/TextMesh Pro/Examples & Extras/Fonts/Roboto-Bold.ttf`
- `Assets/TextMesh Pro/Examples & Extras/Materials/Crate - Surface Shader Scene.mat`
- `Assets/TextMesh Pro/Examples & Extras/Materials/Ground - Logo Scene.mat`
- `Assets/TextMesh Pro/Examples & Extras/Materials/Ground - Surface Shader Scene.mat`
- `Assets/TextMesh Pro/Examples & Extras/Materials/Small Crate_diffuse.mat`
- `Assets/TextMesh Pro/Examples & Extras/Prefabs/Text Popup.prefab`
- `Assets/TextMesh Pro/Examples & Extras/Prefabs/TextMeshPro - Prefab 1.prefab`
- `Assets/TextMesh Pro/Examples & Extras/Prefabs/TextMeshPro - Prefab 2.prefab`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Color Gradient Presets/Blue to Purple - Vertical.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Color Gradient Presets/Dark to Light Green - Vertical.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Color Gradient Presets/Light to Dark Green - Vertical.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Color Gradient Presets/Yellow to Orange - Vertical.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Anton SDF - Drop Shadow.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Anton SDF - Outline.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Anton SDF - Sunny Days.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Anton SDF.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Bangers SDF - Drop Shadow.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Bangers SDF - Outline.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Bangers SDF Glow.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Bangers SDF Logo.mat`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Bangers SDF.asset`
- `Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Electronic Highway Sign SDF.asset`

## 5. Suggested new cases

Filled in by the Friday QA task on 2026-11-27.

| Script / change | Behaviour | Kind | Case |
|---|---|---|---|
| `Assets/Script/System/Abilities/Effects/DamageEffect.cs`, `PayloadEffectBase.cs` | Ability hit damage through `IEffectReceiver.ApplyDamage` on `ctx.Target`, Defense-mitigated; no target, no damage | AUTO | **QA-ENT-17** (new, `QAEnemySuite`). Also `QAProbeTarget` now implements `IEffectReceiver`, so QA-ABL-01 judges `DamageEffect` hits again |
| `DeliveryEffectBase.cs`, `SpawnEffectMono.cs` | Spawned objects hit once and despawn; hit reaction on the enemy | AUTO (existing QA-ABL-01 spawn check) + MANUAL | MAN-ABL-06, MAN-ABL-07 |
| `SelfApplyEffect.cs`, `Config/StatusStatsEffect.cs` | Caster buffs / status effects (Blessing, Avatar of Light) | AUTO (existing QA-ABL-01 stat check, Paladin Utility KNOWN BUG-114) + MANUAL | MAN-ABL-10 |
| `Runtime/SpawnMono/Trap.cs`, `Trap.prefab` | Trap fires on an enemy that enters it | MANUAL (no scene places a trap yet; inbox NOTE-20261126-1 predicts it never fires) | MAN-ABL-11 |
| Spawn / cast clips (`43bb14be`, `6dd66791`, `fa206f82`, `e52431d6`) | Effects play once and despawn; Consecrate cast plays once; Rain Arrows hold the last frame | MANUAL | MAN-ABL-07, MAN-ABL-08, MAN-ABL-09 |
| Dash (`1e5afb3c` DashSmoke, `b0cfe539` dash on every class) | Smoke puff, dash clip on Archer / Mage | MANUAL (distance is AUTO QA-PLY-06, KNOWN BUG-130) | MAN-PLY-10 |
| Room JSON from TextAsset (`7a41d7c4`), build blockers removed (`070bc0a5`) | Rooms load in a Player build | MANUAL | MAN-MAP-08 |
