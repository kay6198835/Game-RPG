# Weekly change impact

- Range: `075711d9ad` (`075711d9ad`) .. `origin/sprint-24` (`be3c7054a5`)
- 3 commits (+2 merge commits), 13 files changed
- Systems that get a deep (P2) run this week: **none**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `be3c7054` | 2026-11-27 | Kay | chore(standup): duplicate standup firing 2026-11-27 - SKIPPED | 1 |
| `56fc14c6` | 2026-11-27 | Kay | test(qa): weekly automation run 20261127-0141 on origin/sprint-24 - FAIL | 12 |
| `f576f1e6` | 2026-11-27 | Kay | test(qa): weekly automation run 20261127-0134 - RUNNER_ERROR | 1 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| DOCS | 1 | `production/session-state/routine-log.md` |
| QA | 12 | `qa-automation/reports/20261127-0141/change-impact.json`<br>`qa-automation/reports/20261127-0141/report.md`<br>`qa-automation/reports/20261127-0141/results.json`<br>`qa-automation/reports/20261127-0141/run.log`<br>... +8 |

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
