## QA Sign-Off Report: Sprint 20
**Date**: 2026-10-31
**QA level**: standard
**Branch / tree at sign-off**: `origin/feature/add-effect-component`, working tree dirty (14 modified `.cs`, uncommitted)
**Evidence sources**: `production/qa/qa-strategy-sprint-20-2026-10-31.md`, `production/qa/qa-plan-sprint-20-2026-10-31.md`, `production/qa/test-cases/s20-session1..5-*.md` (63 cases), tester answers in chat 2026-10-31

### Test Coverage Summary

| Story | Type | Auto Test | Manual QA | Result |
|---|---|---|---|---|
| S20-01 Playtest / Play Mode smoke | Integration | Not automatable | Walked (tester answer, 2026-10-31) | PASS — verbal only; `production/qa/playtests` sheet not filled, no retained evidence, build/commit not recorded. Weak evidence |
| S20-02 BUG-108 `[FormerlySerializedAs]` | Config/Data (S1 severity) | Not required (advisory) | Not executed (no tester answer) | NO EVIDENCE |
| S20-03 BUG-096 zero-spawn rooms | Logic | Required — impossible (BUG-084) | Not executed (no tester answer) | NO EVIDENCE; BLOCKING automated gap |
| S20-04 Weapon trio (BUG-106/107/110) | Integration (Logic inside) | Required for 107 null-guard — impossible (BUG-084) | Not executed (tester stopped session) | NO EVIDENCE |
| S20-05 BUG-097 room order | Logic (code route) or Config/Data (asset reorder) — owner unconfirmed | Required if code route — impossible (BUG-084) | Not executed (tester stopped session) | NO EVIDENCE |
| S20-06 Editor/data block (BUG-105, 095, 072) | Config/Data | Not required | Not executed (tester stopped session) | NO EVIDENCE; smoke UNKNOWN, advisory gate also unmet |
| S20-08 BUG-065 + 086 death / Respawn | Logic + Integration | Required — impossible (BUG-084) | Not executed (tester stopped session) | NO EVIDENCE |
| S20-09 BUG-092 + 066/070 verify | Logic (verification) | Required for DoT tick count — impossible (BUG-084) | Not executed (tester stopped session) | NO EVIDENCE; case S4-06 (BUG-092) BLOCKED — no asset uses the effect and the feature branch changes its semantics; owner decision needed |
| S20-10 BUG-115 runtime copy | Logic | Required — impossible (BUG-084) | Not executed (tester stopped session) | NO EVIDENCE |
| S20-11 One-liners (BUG-104, 103, 102, 068) | Mixed: 103/104/068 Logic, 102 UI | Required for 103/068 — impossible (BUG-084) | Not executed; BUG-102 screenshot absent from `production/qa/evidence/` | NO EVIDENCE; UI screenshot BLOCKING at standard |
| S20-12 Missing-script assets (BUG-073/090/112) | Config/Data | Not required | Not executed (tester stopped session) | NO EVIDENCE |
| S20-F1 Land `feature/add-effect-component` | Integration | Compile check script — not run | Not tested (tester: "not tested yet") | NO EVIDENCE; branch not landed, tree dirty |

S20-07 (process gate decision) is out of QA scope per the plan: a decision, not code.

**Smoke check**: UNKNOWN — no `production/qa/smoke-*.md` report exists for this sprint.

**Stories without executed evidence**: S20-02, S20-03, S20-04, S20-05, S20-06, S20-08, S20-09,
S20-10, S20-11, S20-12, S20-F1 — 11 of 12 in-scope stories. S20-01 is the only one walked, with its
evidence not retained.

**Evidence gap (BUG-084)**: no automated test exists or ran. There is no `.asmdef` under `Assets/`,
and `tests/EditMode` / `tests/PlayMode` sit outside `Assets/`, so Unity Test Runner cannot compile or
discover a test. At qa.level standard the Logic stories (S20-03, S20-05 code route, S20-08, S20-09,
S20-10, S20-11 parts 103/068) lack their BLOCKING automated evidence. A manual playtest substitutes
only with an owner-signed evidence-gap note; neither the substitute playtest nor the note happened
this sprint.

**Other concerns raised during test-case writing**
- 14 uncommitted `.cs` files overlap S20-04 (`AbilityHolderBase`) and S20-09/10 (`BaseStatsSO`, `StatHandlerBase`); no playtest on this tree can be credited to a committed fix.
- S20-F1 rewrites `VitalStatsBase`; S20-09/10 evidence taken before the merge must be repeated after it.
- Start / boss room open rule is undecided (BUG-096 vs BUG-097); needs a design decision before S20-03 and S20-05 can be judged.
- The champion class is now `PedestalChampionController`; `CLAUDE.md` and docs still name `ChampitionController` — doc drift for the next doc-truth pass.

### Bugs Found

No bugs were filed in Phase 5 (no case was executed to FAIL). Open S1/S2 bugs in scope already on file:

| ID | Story | Severity | Status |
|---|---|---|---|
| BUG-108 | S20-02 | S1 | Open |
| BUG-096 | S20-03 | S1 | Open |
| BUG-087 | S20-08 (Respawn path) | S1 | Partial |
| BUG-092 | S20-09 | S1 | Partial (S4-06 BLOCKED, owner decision) |
| BUG-065 | S20-08 | S2 | Open |
| BUG-086 | S20-08 | S2 | Open |
| BUG-066 | S20-09 | S2 | Open |
| BUG-070 | S20-09 | S2 | Open |
| BUG-071 | Carried (HoT/DoT; related to S20-09 / S20-F1) | S2 | Partial |
| BUG-072 | S20-06 | S2 | Open |
| BUG-095 | S20-06 | S2 | Partial |
| BUG-105 | S20-06 | S2 | Open |
| BUG-073 | S20-12 | S2 | Open |
| BUG-097 | S20-05 | S2 | Open |
| BUG-106 | S20-04 | S2 | Open |
| BUG-107 | S20-04 | S2 | Partial |
| BUG-110 | S20-04 | S2 | Open |
| BUG-115 | S20-10 | S2 | Open |
| BUG-083 | Carried (no sprint-20 story) | S2 | Open |
| BUG-084 | Carried (process; decision S20-D6) | S2 | Open |
| BUG-100 | Carried (no sprint-20 story) | S2 | Open |
| BUG-109 | Carried (`DamagesInManyPos.cs` modified in working tree) | S2 | Open |
| BUG-113 | Carried | S2 | Open |
| BUG-114 | Carried | S2 | Open |

Totals: 4 S1, 20 S2. S3/S4 bugs targeted by S20-11 / S20-12 (BUG-102, 103, 104, 068, 090, 112)
remain open and are not listed above.

### Verdict: NOT APPROVED

**Conditions**: none (conditions apply only to APPROVED WITH CONDITIONS).

Were every S1/S2 bug closed, the verdict would still be **NOT ASSESSED — NO EVIDENCE**: 11 of 12
stories produced no executed evidence, and the smoke check is UNKNOWN.

Deciding rule: the NOT APPROVED check runs first — open S1/S2 bugs in scope already on file make
the verdict NOT APPROVED regardless of unexecuted stories.

### Next Step

Resolve S1/S2 bugs and re-run `/team-qa` or targeted manual QA before advancing. S1 first: BUG-108,
BUG-096, BUG-087, BUG-092. The re-run must also produce:
- a `/smoke-check` report;
- the S20-01 playtest sheet with build/commit named;
- walked cases for S20-02…S20-12 and S20-F1 on a committed tree after S20-F1 lands;
- the BUG-102 screenshot in `production/qa/evidence/s20-11/`;
- owner-signed evidence-gap notes for each Logic story until BUG-084 is resolved.

Do not advance the build on this verdict.
