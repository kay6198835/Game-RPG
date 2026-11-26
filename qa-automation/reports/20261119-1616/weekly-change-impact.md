# Weekly change impact

- Range: `dff8564816` (`dff8564816`) .. `origin/sprint-23` (`c613e6417c`)
- 21 commits (+3 merge commits), 157 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, ITEM, MAP, PATH, PLY, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `c613e641` | 2026-11-19 | Claude | feat(enemy): react to a hit from out of view - turn, look, investigate | 12 |
| `0e0dc874` | 2026-11-19 | Claude | fix(pathfinding): nodes on tile centres, no corner cutting, line walk check | 3 |
| `93371f6c` | 2026-11-19 | Kay | update clade setting | 19 |
| `40493b70` | 2026-11-19 | Kay | chore(standup): daily standup 2026-11-19 | 3 |
| `d0d814b6` | 2026-11-18 | Claude | docs: CLAUDE.md entry for the 2026-11-18 map / spawn restructure | 2 |
| `e7a76186` | 2026-11-18 | Claude | docs(map): refresh map README to the current code and room data | 3 |
| `7e6e2156` | 2026-11-18 | Claude | docs(map): refresh room-generation-architecture.md to the current code and room data | 3 |
| `3015e9cf` | 2026-11-18 | Claude | data(map): add the boss spawn marker to BossRoom_ThroneArena | 2 |
| `a92a0c7b` | 2026-11-18 | Claude | data(map): move Champion monuments and Bonfire from the boss room to the start room | 4 |
| `a48ed6c7` | 2026-11-18 | Claude | refactor(map): remove redundant fields and dead code in RoomGeneraterController | 7 |
| `e2b7efee` | 2026-11-18 | Claude | fix(map): pick start/end rooms by RoomType; guarantee required room groups | 7 |
| `e16111d3` | 2026-11-18 | Claude | refactor(map): per-run room list is a runtime List<RoomFile>; delete Maze_Load_Room | 9 |
| `0b959b97` | 2026-11-18 | Claude | refactor(level-edit): LevelEditorTool is room CRUD on LevelManager + own DungeonRoomSO | 7 |
| `d840f0d0` | 2026-11-18 | Claude | feat(map): Grid under Room, LevelManager by reference, LevelEditorTool split; death clips play once | 102 |
| `5e4ddb06` | 2026-11-18 | Claude | feat(enemy): EnemyManager under Room, direct spawn calls, MapModel boss | 11 |
| `89a22d25` | 2026-11-18 | Claude | docs(map): add room generation architecture doc | 2 |
| `4a2237e3` | 2026-11-18 | Kay | chore(standup): daily standup 2026-11-18 | 3 |
| `372a26ea` | 2026-11-17 | Kay | test(qa): inbox proposals - QA-ENT-15 evidence from run 20261117-1705 (written as NOTE-20261117-3..6, f3928194) | 1 |
| `f3928194` | 2026-11-17 | Kay | chore(qa-auto): weekly QA run 20261117-1620 - 4 inbox notes 2026-11-17 | 2 |
| `7dc2a4d5` | 2026-11-17 | Kay | test(qa): weekly automation run 20261117-1705 on origin/sprint-23 - FAIL | 12 |
| `67169b6c` | 2026-11-17 | Kay | test(qa): QA-ENT-09 waits for the hard-coded 10 s wander timeout; part C summary and inbox proposals for runs 20261117-1620/1646 | 4 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 1 | `Assets/Script/System/Abilities/Core/AbilityInstance.cs` |
| DOCS | 22 | `.claude/hooks/validate-commit.sh`<br>`.claude/hooks/validate-push.sh`<br>`.claude/scripts/bug-id.sh`<br>`.claude/settings.json`<br>... +18 |
| ENT | 9 | `Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityInput.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs`<br>`Assets/Script/Character/Entity/Entity.cs`<br>... +5 |
| MAP | 12 | `Assets/Data/Json/Room/BossRoom_ThroneArena.json`<br>`Assets/Data/Json/Room/StartRoom_Entrance.json`<br>`Assets/Script/Database-SO/Modal/MapModel.cs`<br>`Assets/Script/LevelEdit/LevelEditorTool.cs`<br>... +8 |
| OTHER | 1 | `.gitignore` |
| PATH | 2 | `Assets/Script/System/Pathfinding/Grid/GridBuilder.cs`<br>`Assets/Script/System/Pathfinding/Grid/PathfindingGrid.cs` |
| PLY | 1 | `Assets/Script/System/LifetimeScope/GameLifetimeScope.cs` |
| QA | 16 | `Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`qa-automation/cases/case-catalog.md`<br>`qa-automation/reports/20261117-1620/weekly-change-impact.md`<br>`qa-automation/reports/20261117-1646/inbox-proposals.md`<br>... +12 |
| STATIC | 5 | `Assets/Editor/LevelEditorToolEditor.cs`<br>`Assets/Editor/LevelEditorToolEditor.cs.meta`<br>`Assets/Scenes/Main/LoadRandomMap.unity`<br>`Assets/Scenes/Main/Test/Prefab/Level Manager.prefab`<br>... +1 |
| VISUAL | 88 | `Assets/Animation/Enemy/Role/Assasin/Death/Assasin_Die_dir 0.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Death/Assasin_Die_dir 1.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Death/Assasin_Die_dir 2.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Death/Assasin_Die_dir 3.anim`<br>... +84 |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityInput.cs`<br>`Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs`<br>... +6 | Item drop on enemy death and the player pickup (QA-ITEM-02..04) run in the ENEMY scene | ITEM |
| `Assets/Script/Map/Room/RoomGeneraterController.cs`<br>`Assets/Script/System/Enemy/EnemyManager.cs`<br>`Assets/Script/System/Enemy/EnemySpawner.cs` | Room clear depends on ON_ENEMY_DEATH from pooled enemies | ENT |
| `Assets/Script/System/Abilities/Core/AbilityInstance.cs` | Cast gating and cooldowns drive the player skill state, enemy abilities and the hotbar | PLY, ENT, UI |
| `Assets/Script/System/LifetimeScope/GameLifetimeScope.cs` | Injection of the whole player chain | VIT, CMB, ABL, ENT |
| `Assets/Script/System/Pathfinding/Grid/GridBuilder.cs`<br>`Assets/Script/System/Pathfinding/Grid/PathfindingGrid.cs` | Enemy wander and chase (QA-ENT-10 / QA-ENT-15) path on a PathfindingGrid | ENT |
| `Assets/SO/Database/Map/MapModel.asset`<br>`Assets/SO/Dungeon/Maze_Load_Room.asset` | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (2)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- `Assets/Script/Character/Entity/States/EntityInvestigateState.cs` - covered
- `Assets/Script/LevelEdit/LevelEditorTool.cs` - *no case yet*

**New assets (0)**:

- none

**Deleted (1)**:

- `Assets/SO/Dungeon/Maze_Load_Room.asset`

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).

Part C, 2026-11-19 (added after run `20261119-1616`):

| Behaviour (commit) | Kind | Case | Where |
|---|---|---|---|
| Path nodes on tile centres, floor mapping (`0e0dc874`) | AUTO | QA-PATH-04 (P1) | `QASystemsSuite` |
| Diagonal step needs both side cells walkable (`0e0dc874`) | AUTO | QA-PATH-05 (P1) | `QASystemsSuite` |
| `PathfindingGrid.IsLineWalkable` (`0e0dc874`) | AUTO | QA-PATH-06 (P2) | `QASystemsSuite` |
| Start / boss room by `RoomType`, required room groups (`e2b7efee`) - library data precondition | AUTO | QA-MAP-07 (P1) | `QASystemsSuite` |
| Hit from out of view: turn, look, investigate (`c613e641`) | AUTO (state, existing) + MANUAL | QA-ENT-11 (updated by the commit); MAN-ENT-11 | suite / manual Part 2 |
| Enemy paths on tile centres, no wall hugging (`0e0dc874`) | MANUAL | MAN-ENT-12 | manual Part 2 |
| Death clips play once (`d840f0d0`) | MANUAL | MAN-ENT-13 | manual Part 2 |
| Start room Champion / Bonfire, boss marker, required rooms in a run (`a92a0c7b`, `3015e9cf`, `e2b7efee`) | MANUAL | MAN-MAP-06 | manual Part 2 |
| `Assets/Script/LevelEdit/LevelEditorTool.cs` - room CRUD editor tool (`0b959b97`) | MANUAL | MAN-MAP-07 | manual Part 2 |

Not added: `BuildMazeRooms` itself is private on a scene MonoBehaviour that needs the tilemaps and `LevelManager`;
QA-MAP-07 checks the data it depends on and MAN-MAP-06 checks the result in play. Spawn early-return without
`ON_OPEN_DOOR` (`EnemySpawner`, NOTE-20261119-1) is latent with the committed data; no case until the owner
decides the fix direction.
