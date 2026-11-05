# QA case catalog

The single list of every QA case. IDs are stable: a case keeps its ID forever, even when its code moves.
A retired case keeps its row with status **Retired**; its ID is never reused.

- **ID**: `QA-<SYS>-NN`. A case run once per class or per ability slot adds a suffix:
  `QA-PLY-05.Paladin`, `QA-ABL-01.Archer.Secondary`.
- **Type**: AUTO = runs in `run-weekly.ps1`, no human. MANUAL = needs eyes or feel; it lives in
  `qa-automation/manual/manual-weekly-checklist.md`. HYBRID = AUTO checks the numbers, MANUAL checks how it
  looks.
- **Priority**: P0 smoke (always runs) · P1 regression (always runs) · P2 deep (runs only in a week when the
  change scan touches that system, or with `-Deep ALL`).
- **Group**: the QA scene that runs it (STATIC runs in Edit Mode with no scene).
- **Source**: where the case code lives. Each suite file lists its cases in `Run()` in this order.

## Systems

| SYS | System | Group / scene | Suite | Code under test |
|---|---|---|---|---|
| PLY | Player spawn, class pick (Champion monument), states | PLAYER / `QA_PlayerFlow` | `QAPlayerFlowSuite` | `Character/Player/`, `Character/Base/`, `System/PlayerSystem/`, `System/StartGameSystem/` |
| CMB | Basic attack: melee damage + combo, ranged projectiles | PLAYER / `QA_PlayerFlow` | `QAPlayerFlowSuite` | `Weapons/`, `WeaponHolderBase`, `ProjectileBody` |
| ABL | Abilities v2: cast, cost, cooldown, effects | PLAYER / `QA_PlayerFlow` | `QAPlayerFlowSuite` | `System/Abilities/`, `AbilityHolderBase` |
| UI | In-game HUD bound to the player | PLAYER / `QA_PlayerFlow` | `QAPlayerFlowSuite` | `UIFlow/` (`RealPlayerDataProvider`, hotbar) |
| VIT | Current HP / Mana, damage into the player, death | VITALS / `QA_Vitals` | `QAVitalsSuite` | `VitalStatsBase`, `NegativeReceiverBase`, `PlayerDeathState` |
| ENT | Enemy: pooled spawn, weapon, damage, death, respawn | ENEMY / `QA_Combat` | `QAEnemySuite` | `Character/Entity/` |
| EVT | Event bus | SYSTEMS / `QA_Systems` | `QASystemsSuite` | `Manager/EventManager.cs` |
| STAT | Stat modifiers, asset hygiene | SYSTEMS / `QA_Systems` (+ STAT-10 in PLAYER) | `QASystemsSuite` | `System/StatSystem/` |
| POOL | Object pool | SYSTEMS / `QA_Systems` | `QASystemsSuite` | `System/PoolableService/` |
| MAP | Room clear counter, map / room models, room JSON | SYSTEMS / `QA_Systems` | `QASystemsSuite` | `Map/`, `Database-SO/Modal/`, `Assets/Data/Json/Room/` |
| PATH | A* pathfinding | SYSTEMS / `QA_Systems` | `QASystemsSuite` | `System/Pathfinding/` |
| ITEM | Item drop table | SYSTEMS / `QA_Systems` | `QASystemsSuite` | `System/Item/` |
| STATIC | Asset and source hygiene | STATIC (Edit Mode) | `QAStaticChecks` | `Assets/**` |

The path → system map used by the weekly change scan is `qa-automation/config/impact-map.json`.

## AUTO cases

| ID | Pri | Case | Expected | Known bug |
|---|---|---|---|---|
| QA-PLY-01 | P0 | Player spawns once at the spawn point | 1 Player, at the spawn point, camera parented, input/weapon/ability/stat/vital components found | |
| QA-PLY-02 | P2 | Before a class is picked: attack and ability input | OBSERVED — design open: is an unpicked player meant to be unarmed? | |
| QA-PLY-03.<Class> | P0 | G at the monument opens the confirm panel | `UIEvents.OpenConfirnPanel` raised exactly once | |
| QA-PLY-04.<Class> | P1 | Nothing changes before Confirm | `Player.Data` unchanged, no `ON_OPEN_DOOR` | |
| QA-PLY-05.<Class> | P0 | Confirm applies the class | Data, name, animator controller, weapon (exactly one), HP/Mana full, `ON_OPEN_DOOR` once, unpaused, panel hidden | |
| QA-UI-01.<Class> | P1 | Hotbar shows every bound ability | named hotbar slots = bound abilities | |
| QA-CMB-01.<Class> | P0 | Melee hit damage = PhysicalDamage + stage damage (+ CritDamage on crit) | exact value; stage damage > 0 | BUG-105 |
| QA-CMB-02.<Class> | P1 | Melee combo reaches every stage | highest stage = StageCount | |
| QA-CMB-03.<Class> | P1 | Melee out of range deals no damage | 0 hits at range + 2.5 | |
| QA-CMB-04.<Class> | P0 | Arrow hits and despawns | hit within 2.5 s; no live `ProjectileBody` 2 s later | BUG-095 |
| QA-CMB-05.<Class> | P1 | Shots are at least RecoveryTime apart | min interval ≥ `RangeAttackSO.RecoveryTime` | BUG-103 |
| QA-CMB-06.<Class> | P1 | Attack with no weapon | no attack state, no damage, no exception | |
| QA-ABL-01.<Class>.<Slot> | P0 Primary / P1 others | Cast pays cost, starts cooldown, applies effect | cooldown event = definition; cost paid in the activation frame; spawn effect hits / stat effect visible | BUG-114 (Paladin Utility) |
| QA-ABL-02.<Class>.<Slot> | P1 | No recast while on cooldown | second press: no cast, no cost | |
| QA-ABL-03.<Class>.<Slot> | P2 | Refused when the cost cannot be paid | no cast, no cost, player alive; HP costs OBSERVED | |
| QA-STAT-10 | P0 | Class stat assets unchanged after a full player run | in-memory `FullStatsValue()` of every class stat asset equal before/after | BUG-115 |
| QA-VIT-01 | P0 | `ReloadComponent` fills HP and Mana to max | current = `StatHandler.GetStatValue` max | |
| QA-VIT-02 | P0 | Reduction lowers by exactly the amount, raises `CurrentStatsChanged` once | exact | |
| QA-VIT-03 | P1 | Reduction larger than current clamps at 0 | 0 | |
| QA-VIT-04 | P1 | Recovery below max restores exactly the amount | exact | |
| QA-VIT-05 | P1 | Recovery never pushes above max | ≤ max | BUG-119 |
| QA-VIT-06 | P0 | Damage through the player hurtbox lowers HP, player survives | 0 < lost ≤ sent | |
| QA-VIT-07 | P0 | Lethal damage → `PlayerDeathState` + `ON_PLAYER_DEATH` | HP 0, state entered, event emitted | |
| QA-VIT-08 | P1 | `ON_PLAYER_DEATH` emitted once per death | exactly 1 | BUG-086 |
| QA-ENT-01 | P0 | Pool spawns the enemy prefab with core components and full HP | active, Entity + vital + stat + hurtbox, HP = max | |
| QA-ENT-02 | P0 | Spawned enemy has its default weapon (spawn applies `EntityData` like `EnemySpawner.ApplyData()` since 2026-11-05) | `EntityWeaponHolder.Weapon != null` within 1 s | BUG-106 |
| QA-ENT-03 | P0 | Damage lowers HP by amount − Defense; below Defense = 0 | exact | |
| QA-ENT-04 | P0 | Lethal damage emits `ON_ENEMY_DEATH` once, payload = enemy; the suite releases the enemy on that emit like `EnemySpawner.ReleaseEnemy()` (spawn-system contract, owner decision 2026-11-05) | once within 5 s; enemy inactive after the emit; still once 1 s later | |
| QA-ENT-05 | P1 | Released then re-spawned enemy: same instance, full HP | reused, HP = max | |
| QA-ENT-06 | P2 | Enemy stat asset unchanged after spawn / damage / respawn | equal | |
| QA-ENT-07 | P1 | Spawned enemy reads move speed and attack range from its stats (added 2026-11-05, `6094aa10`) | `MoveSpeed` = stat > 0; `MaxRange` = `MaxRangeAttack` > 0; `MinRange` < `MaxRange`; detection range ≥ `MaxRange` | |
| QA-EVT-01 | P0 | Emit delivers the payload to a registered listener | 1 call, same payload | |
| QA-EVT-02 | P0 | `UnResgister` stops delivery | 0 calls | |
| QA-EVT-03 | P1 | Two listeners both receive; Emit with no listener is safe | 1 + 1, no throw | |
| QA-STAT-01 | P0 | Flat modifier adds its value | `(base + level + flat) + equipmentByPrimary` | |
| QA-STAT-02 | P1 | PercentAdd modifiers sum before multiplying | ×(1 + Σ) once | |
| QA-STAT-03 | P1 | PercentMult multiplies | ×(1 + v) after flat | |
| QA-STAT-04 | P0 | `RemoveModifiersFromSource` restores the value | back to before, 0 modifiers | |
| QA-STAT-05 | P1 | Removing one source keeps another source's modifier | other source kept (BUG-100 pattern) | |
| QA-STAT-06 | P0 | Working on an Instantiate copy leaves the stat asset unchanged | asset values equal | |
| QA-POOL-01 | P0 | Spawn returns an active instance at the position | active, at position, not the template | |
| QA-POOL-02 | P0 | Release deactivates; next Spawn reuses the same instance | reused | |
| QA-POOL-03 | P1 | Two spawns without release are distinct | distinct | |
| QA-MAP-01 | P0 | Room clear: `ON_OPEN_DOOR` once, at the last death | 0 after 2/3 deaths, 1 after 3/3 | |
| QA-MAP-02 | P1 | Room clear counts an extra spawned enemy | door shut until the extra one dies | |
| QA-MAP-03 | P1 | `MapModel.GetRandomRoom` deals every room once per cycle | N draws = N distinct rooms | |
| QA-MAP-04 | P1 | `RoomModel.GetSpawnSet` within weight budget + overflow (20 seeds × room) | never empty, total weight ≤ budget × (1 + overflow) (ADR-0003) | |
| QA-MAP-05 | P1 | Every room JSON parses with matching tiles / poses / layers counts | equal and > 0 | |
| QA-MAP-06 | P2 | Combat and boss rooms carry a spawn marker | ≥ 1 `Tile_Spawn_Enemy`; others reported (BUG-096 context) | BUG-096 (Boss room) |
| QA-PATH-01 | P0 | A* finds a path to the farthest reachable tile | success, waypoints walkable and adjacent, ends at target | |
| QA-PATH-02 | P1 | A* to a wall tile fails cleanly | `Success == false`, no exception | |
| QA-PATH-03 | P1 | Same query twice gives the same path | identical waypoints | |
| QA-ITEM-01 | P1 | DepotItem rolls only table items; same seed same item | 0 foreign items; deterministic | |
| QA-STATIC-01 | P0 | No asset / prefab / scene references a missing script | every `m_Script` guid resolves | BUG-073 (ShootSpirit) |
| QA-STATIC-02 | P1 | No SO under `Assets/SO` references a deleted asset | every guid resolves | BUG-090 (ShootSpirit pair) |
| QA-STATIC-03 | P1 | No serialized key left by a field rename | every YAML key is a serialized property of the type | BUG-105, BUG-108, BUG-121 |
| QA-STATIC-04 | P0 | No runtime stat modifier saved into a stat asset | no non-empty `modifiers:` list | BUG-115 (BUG-063 deferred) |
| QA-STATIC-05 | P0 | Runtime scripts use `UnityEditor` only inside `#if UNITY_EDITOR` | none | BUG-116 |
| QA-STATIC-06 | P0 | Every enabled build scene exists | all files present | |
| QA-STATIC-07 | P1 | Every enemy role `EntityData` (`Assets/SO/Enemy/Role`) has a weapon, a move speed and a consistent attack range (added 2026-11-05, `6094aa10`) | `Weapon` set; `MoveSpeed` > 0; 0 ≤ `MinRangeAttack` < `MaxRangeAttack` ≤ `rangeCheckFieldOfView` | |
| QA-LOG.<group> | P0 | Errors logged outside a case | none (created only when it happens) | |
| QA-RUNNER.<group> | P0 | Group finished Play Mode and wrote its results | created only when a group crashed | |

The **Known bug** column mirrors `qa-automation/config/known-issues.json`, which is what the report uses.

## MANUAL cases

Listed with steps in `qa-automation/manual/manual-weekly-checklist.md`: MAN-PLY-01…05, MAN-CMB-01…04,
MAN-ABL-01…04, MAN-ENT-01…03, MAN-UI-01…04, MAN-MAP-01…03, MAN-AUD-01.

## ID map from the sprint-20 player-flow probe

`Assets/Script/FastTest/QAProbe/QAPlayerFlowProbe.cs` (branch `qa/player-flow-automation`) used local IDs. They map
one to one:

| Old | New | Old | New |
|---|---|---|---|
| A-01 | QA-PLY-01 | C-03 | QA-CMB-04 |
| A-02 | QA-PLY-02 | C-04 | QA-CMB-05 |
| B-01 | QA-PLY-03 | C-05 | QA-CMB-06 |
| B-03 | QA-PLY-04 | D-01 | QA-ABL-01 |
| B-02 | QA-PLY-05 (hotbar part split out as QA-UI-01) | D-02 | QA-ABL-02 |
| C-01 | QA-CMB-01 | D-03 | QA-ABL-03 |
| C-02 | QA-CMB-02 | E-01 | QA-STAT-10 |
| C-02b | QA-CMB-03 | | |

## Adding a case

1. Pick the next free number for the system in the table above and add a row (ID, priority, expected).
2. Add one `yield return Case("QA-SYS-NN", "SYS", QAPriority.Px, "title", Body());` line to the suite's `Run()`
   and write `Body()`: assert with `Check(condition, passText, failText)`; never `Debug.Log` a verdict.
3. Rules: work on `Object.Instantiate` copies of ScriptableObjects; destroy what you create (`Cleanup()`); wait
   with `Wait` / `WaitUntil` (game time is fixed at 60 fps); never compare against `Time.time` directly.
4. If the case fails because of an open bug file, add an entry to `config/known-issues.json`.
5. Run `Tools > QA > Automation > Run Everything` once in the Editor before committing.
