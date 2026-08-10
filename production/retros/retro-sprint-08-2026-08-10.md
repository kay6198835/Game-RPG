# Retrospective: Sprint 8 Close-Out

Period: 2026-08-03 -- 2026-08-07 (planned 5-day window, branch `sprint-08`)
Generated: 2026-08-10 (automated `pm-weekly-wrapup` scheduled task, run late relative to the
Saturday 22:00 slot — see note in `bug-triage-2026-08-10.md`)

> **Sequencing / verdict note**: by the time this run executed, a Monday overrun standup and kickoff had
> already closed Sprint 8 directly in `sprint-08.md` with verdict **CONCERNS** and opened `sprint-09`
> (commits `74db8f1`, `eb65772`). This retro was produced independently (deeper code review, full
> retro template) and lands on **FAIL** for the same underlying facts — 0/8 Must-Have items landed, the
> first time this project has recorded a fully-zero sprint. The two verdicts agree on every fact below;
> they disagree only on severity threshold. **CONCERNS is the closure of record** (already committed to
> `sprint-08.md`); treat this retro's FAIL as a supplementary, stricter reading for the trend data, not
> a correction.

---

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| `.cs` files changed (`sprint-08`, since last wrap-up `4898179`) | — | 26 existing + 2 deleted (28 touched) | — |
| Commits since kickoff | — | 5 on `sprint-08` proper (kickoff, Mon standup, `b74a14f`, `c5f26b1` merge, Thu/Fri standups) + ~8 commits / ~1300 lines on `origin/feature/enemy-control` merged in Thu night | — |
| Must-Have tasks (S8-00..S8-12) | 8 (≈1.75d) | **0 confirmed landed** | 0% |
| P1 bugs closed this sprint | — | 0 | 0 |
| P1 bugs still open | 11 carried (last cycle's triage) | 11 (unchanged set; BUG-042/053 shown to be a deeper 2-defect chain, not resolved) | 0 net, but underlying defect count for BUG-042/053 grew (BUG-054 discovered) |
| New bugs found (this cycle's 3 parallel reviews) | — | 7 (BUG-054 through BUG-061, minus BUG-062 unused) | — |
| Combat functional in either direction | Goal: yes, verified in Play Mode | No — confirmed broken both directions by 3 independent code-review agents | Sprint's own stated goal not met, 2nd sprint running |
| S8-00 root-cause conversation | Scheduled Monday, first task | Not held — confirmed 0 times across the sprint's 5 scheduled days | Escalation condition (per Sprint 8's own risk table) met 2026-08-06, still unaddressed |
| Playtest sessions | S8-N1 stretch, gated on combat working | 0 | 9th consecutive cycle with none |
| QA plan | Flagged missing, deferred to owner | Still does not exist | 7th consecutive cycle |

### Velocity Trend

| Sprint | Planned Must-Have | Landed | Rate |
|--------|---------|-----------|------|
| Sprint 6 | 2.20d (9 tasks) | ~5/9 tasks, build likely broken | ~55% by count, negative by outcome |
| Sprint 7 | 2.65d (14 tasks) | 6/14 tasks, 2 new combat-breaking regressions | ~43% by count, negative by outcome |
| Sprint 8 (current) | 1.75d (8 tasks) — deliberately narrowed as a recovery sprint | **0/8 tasks** | **0%** |

**Trend**: Sprint 8 was scoped narrower than Sprint 7 specifically to make landing achievable — 8 tasks,
1.75d total, explicitly billed as "recovery, not new scope." It is the first sprint in this project's
visible history to land **zero** of its own Must-Have items. The velocity line is not just flat, it
inverted: narrowing scope did not translate into completion this cycle.

---

### What Went Well

- **The divergent `origin/feature/enemy-control` branch was finally merged** (`c5f26b1`, Thu
  2026-08-06 22:08) — a risk flagged as early as Tuesday was resolved before sprint close, bringing in
  real enemy-AI/pathfinding progress (A* chase system, attack-speed GDD, idle↔move↔attack flow) rather
  than leaving it to diverge further.
- **This cycle's 3 parallel code-review agents produced sharper diagnosis than the prior cycle's
  triage had.** BUG-042/053 were previously logged as "the exception is gone, roughly progressed" —
  this cycle's review found the replacement code is *also* non-functional for an independent reason
  (BUG-054: uninitialized health field makes `TakeDamage()` a silent no-op). That is a more accurate
  picture of remaining work than what stood in the tracker a week ago.
- **`EntityMovement.cs` correctly uses `GameConstants.SettingStats.PADDING_NODE_VALUE`** instead of a
  magic number, and all four reviewed `Entity*State` classes correctly extend `EntityState`, not
  `MonoBehaviour` — the state-machine discipline established in earlier sprints held under this week's
  changes, no regression there.
- **`PlayerInputHandler`'s `+=`/`-=` subscriptions remain correctly paired** in `OnEnable`/`OnDisable`
  across this week's changes — no new leak introduced despite heavy churn in the Player files.
- **The two files deleted this week (`Shooting.cs`, `WeaponAttack.cs`) left no dangling references** —
  confirmed by repo-wide grep during review, a clean deletion.

---

### What Went Poorly

- **Zero commits landed on `sprint-08` after Thursday night's merge.** The sprint's last scheduled day
  (Friday) opened with 0/8 Must-Have done and closed the same way — no owner-authored coding session
  happened on the branch that day at all, per the Friday standup's own "0d burned since last night"
  assessment, confirmed again by this run finding an unchanged HEAD.
- **S8-00, the off-plan-work root-cause conversation, was scheduled as literally the first task of the
  sprint and was still never held.** This is now the 3rd time within Sprint 8 alone it was
  re-scheduled and skipped, on top of 2 prior sprints (6 and 7) — a conversation now attempted and
  skipped 5 times running. The sprint's own risk table set an explicit trigger for escalating past
  "schedule it again" once this recurred, and that trigger fired on 2026-08-06 with no follow-through.
- **The exact off-plan-work pattern the sprint was designed to stop recurred inside the sprint's own
  window, before the merge resolved it.** Tuesday and Wednesday saw ~1300 lines of real work
  (pathfinding, attack-speed GDD, AI state-flow polish) land on `origin/feature/enemy-control` instead
  of `sprint-08`, exactly the pattern S8-00 exists to address — it recurred inside the very sprint
  scoped to stop it, before that branch was even merged back.
- **The Thursday-night merge, while resolving branch divergence, introduced a new S1 (BUG-053) rather
  than fixing anything.** `EntityNegativeReciver.cs` was rewritten during the merge with player-copied
  logic (wrong component hub, wrong death event) — net effect on the sprint's #1 blocker was negative,
  not neutral.
- **This cycle's review found BUG-042/053 is a 3-defect chain, not the 1-2 defects previously
  tracked**: `NotImplementedException` removed (real, but insufficient) → wrong-hub crash (BUG-053,
  known) → uninitialized-health no-op masking the crash entirely (BUG-054, newly found). Each layer
  removed makes the next one visible; the actual damage path is further from working than "exception
  gone" suggested.
- **S8-05 (Bug #6, 9th carry) was never started at all** — scheduled for Wednesday, no standup was
  recorded that day (a tracker continuity gap the Thursday standup itself flagged and could not
  explain), and Thursday/Friday were consumed entirely by BUG-041/042/053 recovery attempts that also
  did not land.
- **Tue/Wed standup gap**: no `chore(standup)` commit exists for either day on `sprint-08` — the
  Thursday standup had to reconstruct activity from `git log --all` against the divergent branch
  instead of session continuity. This is a process gap independent of the code bugs.

---

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No owner-authored coding session on `sprint-08` after the Thursday night merge | Fri (last scheduled day) | None — sprint closed with the same HEAD | Sprint 9 should not assume autonomous standups can substitute for an actual session landing code; recommend the first Sprint 9 standup confirm a session is scheduled, not just planned |
| S8-00 requires the owner in the room — no code path substitutes | All 5 scheduled days | Not resolved | Per the sprint's own risk table, escalate to a hard process gate (branch protection / pre-push compile+smoke check) instead of a 6th conversation attempt |
| No Unity CLI in this environment | Ongoing, all sprints | None — all Play Mode verification (S8-12) stayed manual and never happened this cycle since no code landed to verify | Unchanged constraint — moot this cycle since nothing reached the point of needing Play Mode confirmation |

---

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S8-01 (BUG-041) | 0.2d | Not started | +100% | No coding session landed on the branch after Monday |
| S8-02 (BUG-042) | 0.2d (revised to 0.3d Thu night after BUG-053 folded in) | Not started | +100% | Same |
| S8-06/S8-07 (one-line fixes) | 0.1d each | Not started | +100% | Cheapest possible credit in the sprint, still didn't land — confirms the blocker was session time, not task difficulty |
| S8-00 (root-cause conversation) | 0.1d | Not held | +100% | Requires owner presence; no autonomous run can substitute, stated plainly in the sprint's own daily-plan every day this week |
| "Enemy-control merge reconciliation" (unplanned) | 0d (not in original plan) | 0.5d+ actually consumed Thursday evening | N/A | Correctly identified as a real, necessary task mid-week; consumed the session time that might otherwise have gone to S8-01/02 |

**Overall estimation accuracy**: the individual task estimates were not wrong — S8-06/S8-07 really are
one-line fixes. The sprint's actual constraint was session availability, not sizing. A recovery sprint
scoped at 1.75d of Must-Have work still requires someone to sit down and do 1.75d of work; narrowing
scope alone does not create that time.

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-041 (player attack unwired) | Sprint 7 (new there) | 1 | No coding session landed this sprint | Sprint 9 — unchanged fix, still small and well-scoped |
| BUG-042/BUG-053/BUG-054 (enemy TakeDamage chain) | Sprint 7 (042/053), this cycle (054, new) | 1 (as a bundle) | Same — no session; also now understood to be 3 defects, not 1-2 | Sprint 9 — bundle as one story: delete `EntityNegativeReciver.cs`, implement `EntityCore.TakeDamage()` against `EntityStatsSO.Health` for real, confirm in Play Mode before closing |
| BUG-043 (divergent enemy attack paths) | Sprint 7 | 2 | Depends on BUG-042 landing first, never reached | Sprint 9 |
| BUG-044 (PlayerDeathState orphaned) | Sprint 7 | 2 | No session | Sprint 9 |
| Bug #6 / S7-11 (player HP write-through) | Sprint 1 (root) | 4 (as distinct-failure-mode reopens) | Never started this sprint — S8-05 not begun | Sprint 9 — same dedicated-spike recommendation as last retro, now overdue |
| BUG-032 (enemy skill NullRef) | Sprint 6 | 3 | Trivial fix, still displaced by lack of session time | Sprint 9 — literally uncomment one line, do it first, in any session that lands |
| BUG-033/BUG-ES-1 (spawn null-guard order) | Sprint 4 | 6 | Same pattern, 6th cycle | Sprint 9 |
| ADR-0002 status flip | Sprint 4 | 5 | Simple 0.1d task, still not touched | Sprint 9 — no excuse remains; recommend closing this literally before any other task |
| S4-05/S4-06 keep-or-cut decision | Sprint 2/3 | 9 | Never resolved | Owner must decide directly — this is the oldest unresolved item in the entire tracker |
| Off-plan-work root-cause conversation (S8-00) | Sprint 4 (as a pattern) | 5 attempts, 0 held | Requires owner presence; escalation condition from the sprint's own risk table met and unaddressed | Sprint 9 — implement the hard process gate instead of scheduling a 6th conversation |
| First playtest | — | 9 cycles | Gated on BUG-041/042 landing | Sprint 9, still gated |
| QA plan | Sprint 3 | 7 | Repeatedly deferred | Needs explicit owner commitment |

---

### Previous Action Items Follow-Up (from `retro-sprint-07-2026-08-02.md`)

| # | Action Item | Status | Notes |
|---|--------------|--------|-------|
| 1 | Fix BUG-041 and BUG-042 — both block the sprint goal outright | Not Done | Neither landed; BUG-042 turned out to need a 3rd fix (BUG-054) once the first layer was examined |
| 2 | Hold the off-plan-work root-cause conversation as literally the first act of Sprint 8 | Not Done | Scheduled Monday per the sprint's own plan, never held — 3 attempts within this sprint's own window, 0 held |
| 3 | Force the S4-05/S4-06 decision directly | Not Done | 9th carry now |
| 4 | Re-scope Bug #6 + PlayerDeathState wiring as one dedicated story with mandatory EditMode test | Not Done | S8-05 (the task created to do this) was never started |
| 5 | File individual `production/qa/bugs/BUG-NNN.md` reports | Not Done | Still only 2 files (`BUG-052.md`, `BUG-053.md`) exist; this cycle added 7 more IDs to the triage doc without individual files |
| 6 | Run `/qa-plan sprint` before Sprint 8's first story | Not Done | 7th consecutive cycle skipped |

**0 of 6 done.** This is now the 2nd consecutive retro where every carried action item from the prior
retro remains undone. The action items themselves are not the problem — they are correctly scoped and
small (a one-line uncomment, a status flip, a 5-minute decision) — the problem is that no session
landed to execute any of them.

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | **Land an actual coding session on Sprint 9's branch before Wednesday.** Two consecutive sprints have now closed with most or all Must-Have work undone not because the fixes are hard, but because session time didn't materialize. This is the single highest-leverage action available. | Owner (Kay) | Critical | Sprint 9, Day 1-2 |
| 2 | **Stop scheduling the root-cause conversation a 6th time — implement the hard process gate instead** (branch protection on `sprint-NN`, or a required pre-push compile+smoke check that blocks commits touching `Pathfinding/`/`Character/Base/`/combat files without a matching sprint task ID). The sprint's own risk table called for exactly this escalation on 2026-08-06; it has not happened. | Owner (Kay) / Producer | Critical | Sprint 9 kickoff |
| 3 | Bundle BUG-041 + BUG-042/053/054 as Sprint 9's literal first story — small in code size (three files), now precisely diagnosed by this cycle's review, gate on Play Mode confirmation not "no longer throws" | gameplay-programmer / ai-programmer | Critical | Sprint 9, Day 1 |
| 4 | Force the S4-05/S4-06 decision — 9 cycles of carry, a 5-minute call | Owner (Kay) | High | Sprint 9 kickoff |
| 5 | File individual `BUG-NNN.md` reports for at least the 11-item P1 table in `bug-triage-2026-08-10.md` | qa-lead | Medium | Sprint 9, Day 1 |
| 6 | Run `/qa-plan sprint` before Sprint 9's first story — 7th consecutive cycle skipped | qa-lead | High | Sprint 9, Day 1 |

---

### Process Improvements

- **Recovery sprints need a session guarantee, not just a narrower scope.** Sprint 8 correctly
  diagnosed that Sprint 7 was too broad and cut Must-Have to 1.75d — but scope size was never actually
  the constraint; session availability was. Recommend Sprint 9's kickoff include an explicit
  commitment of when the owner will sit down and code, not just what gets coded.
- **"No longer throws" is not the same as "fixed" — this cycle's review demonstrated it a second
  time on the same bug (BUG-042/053/054).** Recommend triage/retro cycles require either a Play Mode
  confirmation or an explicit "static-only, unverified" caveat before downgrading a bug's status, to
  avoid the tracker looking more progressed than the game actually is.
- **Standup continuity gaps (Tue/Wed this sprint) reduce the quality of later reconstruction.** When an
  autonomous standup can't run, even a one-line "skipped, no session" marker would preserve continuity
  better than a silent gap the next standup has to explain after the fact.

---

### Summary

Sprint 8 was explicitly scoped as a narrow recovery sprint — 8 tasks, 1.75d, deliberately light — and
closed having landed **zero** of them, the first sprint in this project's visible history to do so.
The individual task estimates were not wrong: BUG-032/BUG-033 really are one-line fixes, and they still
didn't land, which is the clearest evidence available that the constraint this sprint was session time,
not task difficulty or scope size. The one piece of real progress — merging the divergent
`origin/feature/enemy-control` branch — resolved a real risk but introduced a new S1 (BUG-053) in the
process, and this cycle's code review found the underlying damage-chain bug is a 3-defect problem
(BUG-042/053/054), not the 1-2 defects the tracker previously showed. S8-00, the root-cause conversation
meant to stop off-plan work from displacing scoped work, was scheduled and skipped a 5th time, even
though the sprint's own risk table set an explicit trigger to escalate past "schedule it again" once it
recurred — that trigger fired mid-sprint and nothing followed. Verdict: **FAIL** — not because the code
regressed further this cycle (it is roughly stable, modulo BUG-054's new detail), but because the
sprint's own Definition of Done was not approached at all, and the process pattern named as the top
risk in three consecutive retros (Sprint 6, 7, 8) remains completely unaddressed after five scheduled
attempts to discuss it.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-08-10.md`
- Sprint plan: `production/sprints/sprint-08.md`
- Daily plan tracker: `production/sprints/sprint-08-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-07-2026-08-02.md`
- BUG-053 individual file: `production/qa/bugs/BUG-053.md`
