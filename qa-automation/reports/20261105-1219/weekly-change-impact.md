# Weekly change impact

- Range: `4b2133fb2d` (`4b2133fb2d`) .. `origin/sprint-21` (`4097be4cce`)
- 5 commits (+3 merge commits), 30 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ENT, PLY, STAT, STATIC, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `4097be4c` | 2026-11-05 | Claude | Read death from the input flag instead of switching state in the receiver | 14 |
| `7cee3bf6` | 2026-11-05 | Kay | chore(qa-auto): weekly QA run 20261105-1002 - 1 inbox notes 2026-11-05 | 2 |
| `850ce7bb` | 2026-11-05 | Kay | test(qa): inbox proposals for run 20261105-1002 | 1 |
| `f05e2980` | 2026-11-05 | Kay | test(qa): weekly automation run 20261105-1102 on origin/sprint-21 - FAIL | 11 |
| `8b6f6721` | 2026-11-05 | Kay | test(qa): enemy suite spawns like EnemySpawner.ApplyData (QA-ENT-02 test defect) | 2 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| DOCS | 2 | `production/qa/bug-inbox.md`<br>`production/session-state/routine-log.md` |
| ENT | 6 | `Assets/Script/Character/Entity/CoreComponent/EntityNegativeReciver.cs`<br>`Assets/Script/Character/Entity/States/EntityAbilityState.cs`<br>`Assets/Script/Character/Entity/States/EntityAttackState.cs`<br>`Assets/Script/Character/Entity/States/EntityBasicState.cs`<br>... +2 |
| PLY | 6 | `Assets/Script/Character/Base/CharacterBase.cs`<br>`Assets/Script/Character/Base/CharacterInputBase.cs`<br>`Assets/Script/Character/Player/States/PlayerBasicState.cs`<br>`Assets/Script/Character/Player/States/PlayerDisadvantageState.cs`<br>... +2 |
| QA | 14 | `Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>`qa-automation/reports/20261105-1002/inbox-proposals.md`<br>`qa-automation/reports/20261105-1102/change-impact.json`<br>... +10 |
| VIT | 2 | `Assets/Script/Character/Base/NegativeReceiverBase.cs`<br>`Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/Script/Character/Base/CharacterBase.cs` | Serialized by every PlayerData / EntityData asset — a rename without [FormerlySerializedAs] empties fields (BUG-105/108/121) | ENT, CMB, ABL, STATIC |
| `Assets/Script/Character/Base/NegativeReceiverBase.cs` | One base for player and enemy vitals; abilities pay costs and apply effects through it; the HUD reads CurrentStatsChanged | ENT, ABL, UI, STAT |

## 4. New and deleted

**New runtime scripts (0)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- none

**New assets (0)**:

- none

**Deleted (0)**:

- none

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
