# Weekly change impact

- Range: `09ca5d1905` (`09ca5d1905`) .. `origin/sprint-20` (`0a8f24c052`)
- 13 commits (+2 merge commits), 82 files changed
- Systems that get a deep (P2) run this week: **STATIC**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `7cc77969` | 2026-11-01 | Kay | docs(qa): README - sprint merge step | 1 |
| `0fae2699` | 2026-11-01 | Kay | test(qa): merge qa/weekly-automation into the tested sprint branch after a run whose tests ran | 1 |
| `f1685862` | 2026-11-01 | Kay | test(qa): push qa/weekly-automation to origin after each run as a backup | 2 |
| `a9c09609` | 2026-11-01 | Kay | chore(qa-automation): weekly QA run feeds the inbox and the Saturday wrap-up 2026-11-01 | 5 |
| `088a2568` | 2026-11-01 | Kay | test(qa): static baseline completed, inbox proposals for run 20261101-0150 | 2 |
| `b207967d` | 2026-11-01 | Kay | test(qa): weekly automation run 20261101-0150 on origin/sprint-20 - FAIL | 11 |
| `65cc5f6c` | 2026-11-01 | Kay | test(qa): cap QA Play Mode at 60 fps, retry once when the Editor itself crashes | 2 |
| `44dc8701` | 2026-11-01 | Kay | test(qa): weekly automation run 20261101-0126 on origin/sprint-20 - RUNNER_ERROR | 8 |
| `7734ca07` | 2026-11-01 | Kay | test(qa): static baseline for legacy asset debt, FormerlySerializedAs-aware orphan check, auto-commit excludes QA output | 3 |
| `79f08c54` | 2026-11-01 | Kay | test(qa): weekly automation run 20261101-0117 on origin/sprint-20 - FAIL | 11 |
| `64e0fd51` | 2026-11-01 | Kay | test(qa): unimplemented abilities SKIP, static baseline, death-event diagnostics, scene cleanup | 7 |
| `61941a66` | 2026-11-01 | Kay | test(qa): weekly automation run 20261101-0104 on origin/sprint-20 - FAIL | 11 |
| `a92804ca` | 2026-11-01 | Kay | test(qa): weekly QA automation - harness, suites, static checks, runner scripts | 41 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| DOCS | 5 | `.claude/docs/pm-routine-protocol.md`<br>`.claude/rules/bug-inbox.md`<br>`.claude/skills/weekly-wrapup/SKILL.md`<br>`production/qa/bug-inbox.md`<br>... +1 |
| QA | 76 | `Assets/QA/.gitignore`<br>`Assets/QA/Editor.meta`<br>`Assets/QA/Editor/QARunner.cs`<br>`Assets/QA/Editor/QARunner.cs.meta`<br>... +72 |
| STATIC | 1 | `Assets/QA.meta` |

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
