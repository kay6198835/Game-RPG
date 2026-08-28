## Retrospective: Sprint 10
Period: 2026-08-17 -- 2026-08-21 (scheduled), work continued through 2026-08-22
Generated: 2026-08-22 (Saturday weekly wrap-up, `pm-weekly-wrapup` scheduled task, autonomous)

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have tasks | 6 (S10-01..S10-06) | 0 completed | -6 |
| Should-Have tasks | 6 (S10-07..S10-12) | 1 completed (S10-11, WIP reconciliation) | -5 |
| Nice-to-Have tasks | 4 (S10-N1..N4) | 0 completed | -4 |
| Completion Rate (Must-Have) | -- | 0% | -- |
| Story Points / Effort Days (Must-Have) | 1.0d | 0d landed | -1.0d |
| Bugs Found | -- | 1 new (BUG-062) | -- |
| Bugs Fixed | -- | 0 of the 9 tracked P1 items | -- |
| Unplanned Tasks Added | -- | ~90 commits of StatSystem/UI/DI work not on the sprint-10 task list | large |
| Commits (`7eb3378..HEAD`) | -- | ~90 | -- |

### Velocity Trend

| Sprint | Must-Have Planned | Must-Have Completed | Rate |
|--------|---------|-----------|------|
| Sprint 8 | ~1.2d | partial (per retro-08) | mixed |
| Sprint 9 | 1.05d | BUG-041 + BUG-059 closed (weapon refactor) | first real close in 3 cycles |
| Sprint 10 (current) | 1.0d | 0d | 0% |

**Trend**: Decreasing — reverses Sprint 9's first-ever real close.
Sprint 9 broke a two-sprint zero-Must-Have streak. Sprint 10 opened with the exact same item
(BUG-042/053/054) as its literal first task and closed the week having spent its commit volume entirely
outside the declared Must-Have scope.

### What Went Well
- Sprint 9's real work held: `MeleeWeapon.OnActivate()` (BUG-041) and `RangeWeapon.OnActivate()` (BUG-059)
  are unchanged and still correct this cycle — no regression in the weapon-architecture refactor.
- A genuine architectural improvement landed: a VContainer-based DI layer
  (`Assets/Script/LifetimeScope/`, `IPlayerStatService`/`PlayerStatService`) now sits between
  `StatsUIController` and `StatsSO`, moving most stat-mutation calls behind an interface instead of a
  direct SO reference — real progress toward `ui-code.md`'s "UI must not own game state" rule, even
  though the migration is incomplete (see BUG-062).
- `NEW-3` (`StatsSO.RecalculateDerived()` skip-guard using `||` instead of `&&`) and `NEW-4`
  (`Stat.modifiers` leaking runtime buffs into `.asset` files via unwanted serialization) were both fixed
  as a side effect of this week's StatSystem work — neither was a tracked backlog item, but both are real
  correctness fixes.
- Documentation stayed current: `CLAUDE.md`, three ADRs, five GDDs, and the tech-debt register were all
  synced against source this week (`docs(...)` commits throughout the log), preventing the kind of
  multi-week doc drift seen in earlier sprints.

### What Went Poorly
- **The sprint's sole declared Must-Have blocker (S10-01, bundling BUG-042/053/054) received zero code
  movement — the 5th consecutive triage cycle at zero for this exact item.** `EntityCore.cs` and
  `EntityNegativeReciver.cs` are both byte-identical to two weeks ago. This was scheduled as "the literal
  first task" of Sprint 10 per Sprint 9's own retro action item #1, and it was not started.
- **S10-02 (process gate) did not land**, 6th carry — no `.git/hooks/pre-push` or equivalent exists. This
  directly enabled the next problem:
- **S10-11 (WIP reconciliation) landed without its own stated gate.** `sprint-10.md` explicitly scoped
  S10-11 to depend on S10-02 landing first, specifically to avoid repeating the pattern of ungated
  off-branch work reaching the sprint branch. The `origin/feature/fix-player-control` merge (`518ab63`)
  happened anyway, before S10-02 existed. The merged work looks sound on inspection, but the sprint's own
  safety mechanism was bypassed to land it.
- **Effort was almost entirely absorbed by unplanned work.** ~90 commits touched the StatSystem/UI/DI
  area — none of which was S10-01 through S10-06. This is not "scope creep" in the traditional
  small-addition sense; it is a near-total substitution of the week's actual plan with different,
  real, but unscheduled work.
- **All 6 of Sprint 9's retro action items carried into Sprint 10 with zero completion**: S10-01 (item 1)
  not started, S10-03 Play Mode session (item 2) not run, S10-02 process gate (item 3) not landed,
  S4-05/S4-06 decision (item 4) still not made, individual `BUG-NNN.md` files (item 5) still at 2 of 9,
  `/qa-plan sprint` (item 6) still not run — 10th consecutive cycle without a QA plan.
- **New regression risk introduced**: `StatsUIController.cs` now mixes the old direct-`StatsSO` pattern
  with the new DI service mid-file (filed as BUG-062) — an incomplete refactor is riskier than either the
  old pattern or a finished new one, since two paths to the same state can now disagree.

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No Unity CLI in this automated environment | All of Sprint 8, 9, 10 (3 sprints) | None — S10-03 Play Mode verification requires an owner-in-Editor session that has not happened | Explicitly schedule an owner session, or accept this gate cannot close autonomously and stop re-carrying it silently |
| `gh` CLI unavailable | Sprint 10 kickoff | Draft PR command documented for manual run, not executed | Same as prior sprints — accepted low-impact constraint |
| No process gate (S10-02) to distinguish safe vs. risky off-branch merges | All of Sprint 10 | None — S10-11 merged anyway without the gate | Land even a minimal pre-push compile check before the next off-branch reconciliation |

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S10-01 (BUG-042/053/054 bundle) | 0.3d | 0d landed | -0.3d (100% short) | Not started — effort went to unplanned StatSystem/DI work instead |
| S10-11 (WIP reconciliation) | 0.3d, gated on S10-02 | Landed (merge `518ab63`) without waiting for its gate | Delivered, but the gate condition itself was skipped, not satisfied |
| Unplanned StatSystem/DI work | not estimated | ~90 commits across ~5 days | N/A | Real work was in flight on `origin/feature/fix-player-control` before Sprint 10 even opened (per the kickoff finding) and continued to absorb the week instead of being explicitly re-scoped into the plan |

**Overall estimation accuracy**: 0% of Must-Have tasks landed within their estimate — none were started.
The sprint's actual bottleneck is not estimation accuracy but plan adherence: the declared plan and the
actual week's work were almost entirely disjoint sets.

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-042/053/054 (S10-01) | Sprint 7 | 6th carry | No owner/agent time allocated to it despite being scheduled first | Must open Sprint 11 with zero other Must-Have items competing for the first commit |
| S10-02 process gate | Sprint 6 (S9-00) | 6th carry | Written-policy version keeps being deferred in favor of a "real" enforced version that never lands | Land the smallest possible version (even a no-op placeholder hook that just runs `dotnet build` or equivalent) rather than continuing to wait for a complete design |
| S4-05/S4-06 decision | ~Sprint 4 | 12th carry | Decision-avoidance, not estimation — no blocker exists other than making the call | Force a written decision in the next sprint file, even a one-line "cut" or "keep, revisit in Sprint N" |
| QA plan | Sprint 1 (effectively) | 10th consecutive cycle skipped | No owner session to make the judgment call on full-plan-now vs. defer | Same deferral pattern as always — needs an explicit owner decision, not another autonomous carry |
| Individual `BUG-NNN.md` files | Sprint 7 (S7-D3) | 5th+ cycle recommended | Low priority relative to code work every cycle | Still only 2 of 9 P1 items have files — recommend qa-lead batch-generate these as a single low-effort session rather than one-off |

### Technical Debt Status
- Current TODO count: 0 (previous cycles: not tracked in this format before — establishing baseline)
- Current FIXME count: 0
- Current HACK count: 0
- Trend: Flat at zero — this project tracks technical debt via `production/qa/bugs/` and
  `.claude/rules` "Known Bugs" tables rather than inline TODO/FIXME/HACK comments, consistent with the
  project's "no comments unless the WHY is non-obvious" convention. The zero count is a style artifact,
  not a signal of low debt — the 30-item open bug backlog (`bug-triage-2026-08-22.md`) is the real debt
  ledger.
- Concern: `Assets/Script/LifetimeScope/` (new this sprint, VContainer DI layer) has no ADR and is not
  yet in `CLAUDE.md`'s Repository Layout — same undocumented-subsystem pattern already tracked by
  BUG-052 for `Character/Base/`, `Pathfinding/`, and `Poolable/`.

### Previous Action Items Follow-Up

| Action Item (from Sprint 9 retro) | Status | Notes |
|-------------------------------|--------|-------|
| 1. Open Sprint 10 with BUG-042/BUG-053/BUG-054 as the literal first task | **Not Started** | `sprint-10.md` did schedule it first (S10-01), but zero code was written against it all week |
| 2. Get an owner-in-Editor Play Mode session for S9-12/S10-03 | **Not Started** | No Unity CLI in this environment; requires the human owner, still hasn't happened across 4 sprints |
| 3. Land S9-00/S10-02 as a minimal pre-push compile check | **Not Started** | No hook exists in `.git/hooks/`; still written-policy-only |
| 4. Force the S4-05/S4-06 decision | **Not Started** | 12th carry now, up from 10th at last retro |
| 5. File individual `BUG-NNN.md` reports for the 9-item P1 table | **Not Started** | Still 2 of 9 |
| 6. Run `/qa-plan sprint` before Sprint 10's first story | **Not Started** | 10th consecutive cycle without a QA plan |

**0 of 6 action items from the previous retrospective were completed.** This is the first cycle in this
retrospective series with a 0/6 follow-through rate — worth naming plainly rather than folding into the
general narrative.

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | **Open Sprint 11 with BUG-042/BUG-053/BUG-054 as the ONLY Must-Have item until it lands** — 6th carry, do not schedule other Must-Have work alongside it again; if unplanned work displaces the plan a second time, that is itself the signal to address (see #4) | ai-programmer | Critical | Sprint 11, Day 1 |
| 2 | **Land S10-02/S9-00 as a minimal pre-push hook this cycle, no further deferral** — even a bare compile-check placeholder closes the gap that let S10-11 bypass its own gate this week | Owner (Kay) / producer | Critical | Sprint 11 kickoff |
| 3 | **Finish the `StatsUIController.cs` DI migration (BUG-062)** — remove the leftover direct `StatsSO` field now that `IPlayerStatService` exists, closing the dual-path regression risk while the context is still fresh | gameplay-programmer | Medium | Sprint 11, Day 1-2 |
| 4 | **Explicitly decide whether the sprint-planning process itself needs to change** — two consecutive sprints have now had a written Must-Have plan largely superseded by different, unplanned (but real) work landing via feature branches. Either the planning process should account for in-flight branch work before committing to a plan, or in-flight branches need a check-in gate before a new sprint opens | producer | High | Sprint 11 kickoff |
| 5 | Force the S4-05/S4-06 decision — 12 cycles of carry, oldest open item in the project | Owner (Kay) | High | Sprint 11 kickoff |

### Process Improvements
- **A sprint plan built without checking for in-flight, uncommitted branch work will keep losing to that
  work.** Sprint 10 itself opened with a documented finding of 13+ uncommitted files on
  `origin/feature/fix-player-control` — the plan noted this but still scheduled S10-01 as the top
  priority, and the in-flight branch work won the week anyway. Recommend the kickoff ritual treat a
  large in-flight branch as a scope input, not just a stash-and-note item.
- **"Landed but ungated" needs to be distinguishable from both "landed correctly" and "not landed."**
  S10-11 shipped without its own dependency (S10-02) being satisfied. A future retro/triage format should
  flag this case explicitly rather than crediit it as a clean completion.
- **Zero TODO/FIXME/HACK comments is not evidence of low debt in this codebase** — future retros should
  read the open bug count in `production/qa/bugs/` + the latest bug-triage report as the debt signal
  instead of grepping for inline markers that this project's conventions actively discourage.

### Summary
Sprint 10 was a **plan-adherence failure, not an effort failure**: real, working code landed all week
(a DI layer for the stat system, two correctness fixes, doc sync), but almost none of it was the Must-Have
work the sprint was built around. The sprint's single declared blocker — the enemy damage chain
(BUG-042/053/054) — is now at 5 consecutive cycles of zero movement despite being named "the literal first
task" in two straight sprint plans. The most important thing to change going forward is not more
estimation or more scope-cutting, but closing the gap between what gets planned and what an in-flight
branch is already doing — Sprint 11 should not repeat the pattern of scheduling the same top-priority item
a third time without first checking whether unplanned work is again positioned to displace it.
