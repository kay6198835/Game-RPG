# EditMode tests: local verification checklist

Written 2026-11-12 for branch `test/editmode-logic-cases` (base: `chore/asmdef-test-setup`).

**None of the tests below has been compiled or run.** They were written by reading the code, in an environment
with no Unity Editor. This file lists what each test assumes, so a failure on a real machine can be read
quickly: a compile error or a wrong assumption in the test is far more likely than a game defect on the first
run. The 4 tests that existed before (`StatSheetTests`, first four methods) already passed on Unity and are not
listed.

## How to run

Close the Unity Editor first (batch mode cannot open a project the Editor holds).

```
git fetch origin
git checkout test/editmode-logic-cases
"<Unity 2022.3.62f3>\Editor\Unity.exe" -batchmode -nographics -projectPath D:\Fork\Game-RPG -runTests -testPlatform EditMode -testResults D:\Fork\editmode-results.xml -logFile D:\Fork\editmode.log
```

Do not add `-quit`: with `-runTests` Unity exits by itself when the run ends.

| Outcome | Where to look |
|---|---|
| Exit code 0, `editmode-results.xml` has `result="Passed"` on the root `test-run` | Green. 64 tests expected (4 old + 60 new) |
| No XML file written | Compile error. Search `editmode.log` for `error CS` |
| Exit code 2, XML written | Some tests failed. Search the XML for `result="Failed"`; the `<message>` under it says why |

Alternative with the Editor open: `Window > General > Test Runner > EditMode > Run All`.

Report the result back (the `error CS` lines, or the failed test names with their messages) and the tests are
fixed on this branch. Do not edit game code to make a test pass before the failure has been read: see
"If a test fails" at the end.

## Risks that apply to every file

| # | Risk | Symptom | Likely fix |
|---|---|---|---|
| G1 | One compile error anywhere fails the whole test assembly, including the 4 tests that passed before | No XML, `error CS…` in the log pointing at `Assets/Tests/EditMode/` | Fix that line; nothing else is affected |
| G2 | `GameRPG.EditModeTests.asmdef` now lists `VContainer` in `references` (added because `ObjectPoolManager` has a public `Construct(IObjectResolver)`). Assumes the VContainer assembly is named exactly `VContainer`, as `GameRPG.asmdef` already does | Warning about an unresolved assembly reference, or `error CS0012` naming `VContainer` | Check the name in `GameRPG.asmdef`; if the reference turns out to be unnecessary it can be removed again |
| G3 | The eight new `.meta` files were written by hand (random GUIDs, copied from `StatSheetTests.cs.meta`) | Unity rewrites a `.meta` on import (shows as a modified file in git) | Commit Unity's version |
| G4 | Private fields and private setters are reached by reflection, by **name** (`TestSupport.SetField` / `SetProperty`). A rename in game code breaks the test, not the game | Test fails with "has no field '…' (test assumption out of date)" | Update the name in the test |
| G5 | Tests whose name starts with `Committed…` read real assets by fixed path. A moved or renamed asset fails the test | "asset not found: …" | Update the path constant at the top of the test file |
| G6 | Synthetic ScriptableObjects are made with `ScriptableObject.CreateInstance` (never saved), committed assets are used through `Object.Instantiate` or read-only; all are destroyed in `[TearDown]` with `DestroyImmediate`. Assumes `OnValidate` does not run for these objects | An unexpected "dirty asset" in `git status` after a run | Report it; that would be a test defect |
| G7 | `UnityEngine.Random` state is saved in `[SetUp]` and restored in `[TearDown]`; each test seeds it with a fixed value | None expected | — |

## Tests, assumptions and specific risks

Case IDs in the weekly report are `QA-UNIT.<Class>.<Test>`.

### `EventManagerTests` (7 tests) — mirrors QA-EVT-01…03

Assumes: `EventManager.Resgister / UnResgister / Emit(EventID, object = null)` are public static and take an
`Action<object>`; `EventID.ON_TEST` and `EventID.ON_ROOM_CLEAR` exist. Every test uses `ON_TEST` and removes
its listeners in `[TearDown]`.

| Test | Mirrors | Specific risk |
|---|---|---|
| `Emit_DeliversPayloadOnceToRegisteredListener` | EVT-01 | Low |
| `Emit_WithoutPayload_DeliversNull` | — | Low |
| `UnResgister_StopsDelivery` | EVT-02 | Low |
| `Emit_CallsEveryListenerOfTheEventOnce` | EVT-03 | Low |
| `Emit_AfterEveryListenerRemoved_DoesNotThrow` | EVT-03 | Low |
| `UnResgister_OneListener_KeepsTheOther` | — | Low |
| `Emit_DoesNotReachListenersOfAnotherEvent` | — | Emits `ON_ROOM_CLEAR`. If run inside an Editor session with domain reload disabled after Play Mode, a left-over game listener of that event would also be called |

### `StatSheetTests` (11 new tests) — mirrors QA-STAT-01…06

Assumes: `BaseStatsSO` is a concrete class with private fields `level` (int), `primaryStats`
(`List<PrimaryStatEntry>`) and `derivedStats` (`DerivedStatFormula[]`); `PrimaryStatEntry(StatType, float)` is
public; `DerivedStatFormula` and its `StatContribution` have public fields; `Stat.Modifiers` is public.
Synthetic profile: level 3, STR 10, VIT 8, `Defense = 5 + level × 2 + VIT × 0.5` → 15.

| Test | Mirrors | Expected value | Specific risk |
|---|---|---|---|
| `NewSheet_FromProfile_ComputesPrimaryBaseAndDerivedFormula` | — | STR 10, VIT 8, Defense 15, HP 0 | G4 |
| `FlatModifier_AddsItsValueToFinalValue` | STAT-01 | 20 | G4 |
| `PercentAddModifiers_SumBeforeMultiplyingOnce` | STAT-02 | 19.5 | G4 |
| `PercentMultModifier_MultipliesAfterFlat` | STAT-03 | 28.5 | G4 |
| `RemoveModifiersFromSource_RestoresValueAndLeavesNoModifier` | STAT-04 | 15, 0 modifiers | G4 |
| `RemoveModifiersFromSource_OnProfileSheet_KeepsOtherSourceModifier` | STAT-05 | 18, 1 modifier | G4 |
| `PrimaryPoint_RaisesDerivedStatThroughItsCoefficient` | — | VIT 10, Defense 16, `Changed` raised for exactly VIT and Defense | G4 |
| `ModifierOnPrimaryStat_ReachesDerivedStatAsPrimaryBonus` | — | VIT 12, Defense 17 | G4 |
| `AddPrimaryPoint_OnDerivedStat_IsRefused` | — | value unchanged | Uses `LogAssert.Expect` for the warning text "not a primary stat"; fails if the message is reworded |
| `TwoSheetsFromOneProfile_DoNotShareModifiers` | — | second sheet unchanged | G4 |
| `CommittedStatAssets_AreUnchangedAfterASheetIsBuffed` | STAT-06 | every asset equal before / after | Uses `AssetDatabase.FindAssets("t:BaseStatsSO", Assets/SO)`; assumes the filter also returns subclasses (`EnemyStatSO`). Checks every stat asset, where the Play Mode case checks `PaladinStats` only |

### `ObjectPoolManagerTests` (5 tests) — mirrors QA-POOL-01…03

Assumes: `ObjectPoolManager` works when created with `new GameObject` + `AddComponent` and **no** VContainer
resolver (`Pool.Spawn` calls `resolver?.InjectGameObject`); `Object.Instantiate` and `SetActive` behave in
Edit Mode as in Play Mode; `Pool` and `PoolMember` have no `Awake` / `Start` logic.

| Test | Mirrors | Specific risk |
|---|---|---|
| `Spawn_ReturnsActiveInstanceAtRequestedPosition` | POOL-01 | G2. Objects are created in whatever scene is open; they are destroyed in `[TearDown]` |
| `Release_DeactivatesAndNextSpawnReusesTheSameInstance` | POOL-02 | G2 |
| `Spawn_TwiceWithoutRelease_GivesTwoDistinctInstances` | POOL-03 | G2 |
| `Release_Twice_DoesNotQueueTheInstanceTwice` | — | G2 |
| `Release_ObjectThatNeverCameFromThePool_IsIgnored` | — | G2 |

### `MapModelTests` (6 tests) — mirrors QA-MAP-03

Assumes: `MapModel.fullRoomList` is a public list; `MapModel` and `RoomModel` can be made with `CreateInstance`.

| Test | Mirrors | Specific risk |
|---|---|---|
| `GetRandomRoom_DealsEveryRoomOncePerCycle` | MAP-03 | Low |
| `GetRandomRoom_StartsANewCycleWhenTheBagIsEmpty` | MAP-03 | Low |
| `GetRandomRoom_SkipsNullEntries` | — | Low |
| `GetRandomRoom_WithNoRoom_ReturnsNull` | — | Low |
| `GetRandomRoom_WithOnlyNullEntries_ReturnsNull` | — | Low |
| `CommittedMapModel_DealsEveryRoomOncePerCycle` | MAP-03 | G5 (`Assets/SO/Database/Map/MapModel.asset`). Fails if the asset lists the same room twice — the Play Mode case has the same rule |

### `RoomModelTests` (6 tests) — mirrors QA-MAP-04

Assumes: private fields `enemiesOfRoom` (`List<EnemyModal>`), `weightBudget` (int), `overflowPercent` (float);
`EnemyModal` has public `weight` and `rarityTier`; `GetSpawnSet()` keeps picking while the remaining budget is
above 10 % of the budget and returns `null` for a room with no enemy type.

| Test | Mirrors | Specific risk |
|---|---|---|
| `GetSpawnSet_WithNoEnemyType_ReturnsNull` | — | G4 |
| `GetSpawnSet_SingleEnemyType_SpendsBudgetDownToTenPercent` | — | Expects exactly 9 picks (budget 100, weight 10). Pins the current 10 % stop rule: a deliberate tuning change needs this number updated |
| `GetSpawnSet_GroupsRepeatedPicksIntoOneEntryPerEnemyType` | — | 50 seeds. G4 |
| `GetSpawnSet_WithSmallWeights_SpendsBetweenNinetyAndHundredPercentOfBudget` | MAP-04 | 50 seeds; holds for any seed because every weight ≤ 10 % of the budget |
| `GetSpawnSet_SameSeed_GivesTheSameSet` | — | Low |
| `CommittedRooms_SpawnSetStaysWithinBudgetPlusOverflow` | MAP-04 | G5. Same seeds (1000–1019) and same limit as the Play Mode case, so it should agree with QA-MAP-04. Can fail on real data when the `retry > 4` fallback picks an enemy heavier than the remaining budget (known open item, Demo checklist 15 in `CLAUDE.md`). A room whose serialized `candidateEnemies` list is not empty could loop forever; all nine committed rooms store `[]` today |

### `RoomJsonTests` (3 tests) — mirrors QA-MAP-05

Assumes: rooms live in `Assets/Data/Json/Room/*.json` (read through `Application.dataPath`); `LevelData` has
public `tiles`, `poses`, `layerIndices`. The data was checked outside Unity with a script: all 20 files have
equal, non-zero counts and a `Tile_Floor` tile, so a failure here points at `JsonUtility` parsing, not the data.

| Test | Mirrors | Specific risk |
|---|---|---|
| `RoomFolder_ContainsRoomFiles` | MAP-05 | Low |
| `EveryRoomJson_ParsesWithMatchingTilesPosesAndLayers` | MAP-05 | Low |
| `EveryRoomJson_HasAtLeastOneFloorTile` | — | Low |

### `DepotItemTests` (8 tests) — mirrors QA-ITEM-01

Assumes: `DepotItem.listItems` is public, `listTierValidItems` is a private field; `DepotItemSlot.DepotItem` /
`.RarityTier` and `DropRateItemSlot.TierRate` are auto-properties with private setters (set by reflection);
`DepotItem.InvalidateCache()` rebuilds the tier cache on the next roll.

| Test | Mirrors | Specific risk |
|---|---|---|
| `TryRollItem_EmptyDepot_NeverDrops` | — | G4 |
| `TryRollItem_TierAtHundredPercent_AlwaysDropsAnItemOfTheTable` | ITEM-01 | 200 rolls. `Random.Range(0f, 100f)` can return exactly 100, which drops nothing; about a 1 in 40 000 chance for the whole test, and fixed by the seed either way |
| `TryRollItem_TierWithZeroRate_NeverDrops` | — | G4 |
| `TierWithoutAnyItem_IsExcludedFromTheDropRate` | — | G4 |
| `TryRollItem_ReportsTheTierOfTheItemItReturns` | — | G4 |
| `TryRollItem_ThirtyPercentTier_DropsAboutThirtyPercentOfRolls` | — | Statistical: 2000 rolls, passes between 400 and 800 drops (expected 600, about ±10 standard deviations) |
| `TryRollItem_SameSeed_GivesTheSameItem` | ITEM-01 | Low |
| `CommittedDepot_RollsOnlyItemsOfItsTableAndIsDeterministicPerSeed` | ITEM-01 | G5 (`Assets/SO/Item/Depot Item 1.asset`) |

### `AStarTests` (14 tests) — mirrors QA-PATH-01…03

Assumes: `PathfindingGrid.BuildGrid(LevelData, Vector2)`, `GetNode(Vector2)`; `AStar.FindPath(grid, start,
target)` returns a `NavPath` (`Success`, `Waypoints`); a tile is walkable only when it is `Tile_Floor` or
`Tile_Spawn_Enemy`; a diagonal step is refused only when **both** side tiles are walls; world position = tile
coordinate when the grid origin is zero. Small grids are drawn in the test as strings (`.` floor, `#` wall).

| Test | Mirrors | Specific risk |
|---|---|---|
| `FindPath_StraightCorridor_ReturnsEveryTileInOrder` | PATH-01 | Exact waypoint list |
| `FindPath_OpenDiagonal_StepsDiagonally` | PATH-01 | Exact waypoint list (unique shortest path) |
| `FindPath_AroundAWall_TakesTheOnlyShortestDetour` | PATH-01 | Exact list of 5 waypoints. Worked out by hand from the neighbour rule; the most likely test in this file to be wrong if that reading is off |
| `FindPath_StartEqualsTarget_ReturnsASingleWaypoint` | — | Pins current behaviour (success, 1 waypoint) |
| `FindPath_ToAWallTile_FailsCleanly` | PATH-02 | Low |
| `FindPath_FromAWallTile_Fails` | — | Low |
| `FindPath_TargetOutsideTheGrid_Fails` | — | Low |
| `FindPath_TargetWalledOff_Fails` | PATH-02 | Low |
| `FindPath_DiagonalBetweenTwoWalls_IsNotAllowed` | — | Pins the corner rule |
| `FindPath_SameQueryAfterAnotherSearch_GivesTheSamePath` | PATH-03 | Low |
| `FindPath_OnALargerGrid_WaypointsAreWalkableAndAdjacent` | PATH-01 | Low |
| `CommittedRoom_PathToFarthestReachableTile_IsWalkableAdjacentAndEndsAtTarget` | PATH-01 | Reads `CombatRoom_GrandArena.json`. **Stronger than the Play Mode case**, see the note below, so it can fail where QA-PATH-01 passes |
| `CommittedRoom_PathToAWallTile_FailsCleanly` | PATH-02 | Low |
| `CommittedRoom_SameQueryTwice_GivesTheSamePath` | PATH-03 | Low |

**Note on QA-PATH-01 (QA case weakness, not a game defect).** The Play Mode case starts from the first
walkable tile found by its scan. In `CombatRoom_GrandArena.json` that tile is (-15, 2), part of a 2-tile floor
pocket beside a door, cut off from the room by the door tiles. Its "farthest reachable tile" is therefore one
step away (the 2026-11-07 report shows "path found, 2 waypoints"), so QA-PATH-01 has only ever tested a
one-step path. The EditMode test starts from the largest connected floor region instead, which gives a path
across the room. Suggested follow-up, on a machine with Unity: make the same change in
`QASystemsSuite.CasePathFound()`.

## Not converted

| Case | Reason |
|---|---|
| QA-VIT-01…05 | `VitalStatsBase<TCore>` is an abstract generic `MonoBehaviour`. It needs a `CoreBase` (the character Core hub) that provides an `IStatService<TCore>`, which in turn needs `CharacterData`; and its setup runs from `Awake()`, which Unity does not call in Edit Mode. A stand-in hub would have to be a `MonoBehaviour` inside the Editor-only test assembly, which `AddComponent` does not support reliably. These cases need the Play Mode suite, or a PlayMode test assembly in a later round |
| QA-MAP-01, MAP-02 | Not in this round's scope (`RoomCell` is a `MonoBehaviour`). Candidate for the next round: the Play Mode case only calls three public methods on a bare `AddComponent<RoomCell>()` |
| QA-MAP-06 | Not in scope (P2, known FAIL under BUG-096) |
| QA-VIT-06…08 | Need the player, hurtbox and state machine |

## If a test fails

1. **Compile error** → a wrong guess about a name or an access level in the test. Fix the test.
2. **A synthetic-data test fails** (no `Committed` in the name) → compare with the Play Mode case of the same
   ID in the latest weekly report. Play Mode PASS + EditMode FAIL means the test's expectation is wrong. Both
   FAIL means a suspected game defect: record a note in `production/qa/bug-inbox.md` (never a bug ID).
3. **A `Committed…` test fails** → first check that the asset still exists at the path, then treat as in 2.
4. Once the run is green on a real machine, tell QA: the Play Mode twins (QA-EVT, QA-STAT-01…06, QA-POOL,
   QA-MAP-03…05, QA-PATH, QA-ITEM-01) can then be retired in a later round. Their IDs stay in the catalog.

## Reaching the Friday run

`run-weekly.ps1` tests the newest `origin/sprint-NN`. These tests run on Friday only after both
`chore/asmdef-test-setup` and this branch are merged into that sprint branch. The merge is the owner's decision.
