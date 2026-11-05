# Weekly change impact

- Range: `e22cd6ab59` (`e22cd6ab59`) .. `origin/sprint-21` (`4b2133fb2d`)
- 2 commits (+2 merge commits), 18 files changed
- Systems that get a deep (P2) run this week: **none**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `7addb4bc` | 2026-11-05 | Kay | test(qa): weekly automation run 20261105-1053 on origin/sprint-21 - FAIL | 11 |
| `5252a2b7` | 2026-11-05 | Kay | test(qa): run 20261105-1002 part C - QA-ENT-07, QA-STATIC-07, manual Part 2, baseline and known issues | 7 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| QA | 18 | `Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>`qa-automation/config/known-issues.json`<br>... +14 |

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
