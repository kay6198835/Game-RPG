# Weekly change impact

- Range: `d1288be071` (`d1288be071`) .. `origin/sprint-23` (`dff8564816`)
- 2 commits (+2 merge commits), 19 files changed
- Systems that get a deep (P2) run this week: **none**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `ec9caa44` | 2026-11-17 | Kay | test(qa): weekly automation run 20261117-1646 on origin/sprint-23 - FAIL | 12 |
| `5fd1c81c` | 2026-11-17 | Kay | test(qa): adapt ENT-08/09/12/13 to sprint-23 dev changes, add QA-PLY-06 dash and QA-ENT-16 source independence, map BUG-126/127/128, baseline role EntityData orphan keys | 7 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| QA | 19 | `Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`Assets/QA/Runtime/Suites/QAPlayerFlowSuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>... +15 |

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
