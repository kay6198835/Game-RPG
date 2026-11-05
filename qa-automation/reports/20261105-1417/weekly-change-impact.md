# Weekly change impact

- Range: `92f53a6ecb` (`92f53a6ecb`) .. `origin/sprint-21` (`8140a56de7`)
- 2 commits (+1 merge commits), 15 files changed
- Systems that get a deep (P2) run this week: **none**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `8140a56d` | 2026-11-05 | Claude | test(qa): QA-ENT-04 releases the dead enemy like EnemySpawner (by design) | 4 |
| `27c33665` | 2026-11-05 | Kay | test(qa): weekly automation run 20261105-1327 on origin/sprint-21 - FAIL | 11 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| DOCS | 2 | `.claude/rules/ai-code.md`<br>`production/qa/bug-inbox.md` |
| QA | 13 | `Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>`qa-automation/reports/20261105-1327/change-impact.json`<br>`qa-automation/reports/20261105-1327/report.md`<br>... +9 |

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

Filled in by the Friday Claude task (part C), 2026-11-05.

No new runtime script, asset or gameplay change in this range: the two commits touch only QA code, QA
reports and docs. No AUTO case and no MANUAL row added. Part 2 of `manual/manual-weekly-checklist.md`
keeps the rows written for this week by run `20261105-1002` (MAN-ENT-04/05/06).
