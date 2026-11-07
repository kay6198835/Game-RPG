# Weekly change impact

- Range: `4097be4cce` (`4097be4cce`) .. `origin/sprint-21` (`92f53a6ecb`)
- 2 commits (+2 merge commits), 14 files changed
- Systems that get a deep (P2) run this week: **ABL, ENT, PLY, STAT, UI, VIT**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `1c3a34fc` | 2026-11-05 | Kay | fix flow | 3 |
| `74f45752` | 2026-11-05 | Kay | test(qa): weekly automation run 20261105-1219 on origin/sprint-21 - FAIL | 11 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| PLY | 2 | `Assets/Script/Character/Base/CharacterInputBase.cs`<br>`Assets/Script/Interface/ICharacterInput.cs` |
| QA | 11 | `qa-automation/reports/20261105-1219/change-impact.json`<br>`qa-automation/reports/20261105-1219/report.md`<br>`qa-automation/reports/20261105-1219/results.json`<br>`qa-automation/reports/20261105-1219/run.log`<br>... +7 |
| VIT | 1 | `Assets/Script/Character/Base/NegativeReceiverBase.cs` |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
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
