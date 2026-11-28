# Weekly change impact

- Range: `be3c7054a5` (`be3c7054a5`) .. `origin/sprint-24` (`f41f451b6c`)
- 2 commits (+2 merge commits), 21 files changed
- Systems that get a deep (P2) run this week: **none**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `50789d11` | 2026-11-27 | Kay | test(qa): weekly automation run 20261127-0218 on origin/sprint-24 - FAIL | 12 |
| `aeb4dbfc` | 2026-11-27 | Kay | test(qa): part C for run 20261127-0141 - probe implements IEffectReceiver, QA-ENT-17 DamageEffect, ITEM-10 known BUG-120, STATIC-12 baseline, 8 manual rows | 9 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| QA | 21 | `Assets/QA/Runtime/QAProbeTarget.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>... +17 |

## 3. Interaction risks (re-checked by the deep run)

None matched.

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
