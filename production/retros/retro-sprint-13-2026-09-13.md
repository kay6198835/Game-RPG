## Retrospective: Sprint 13
Period: 2026-09-08 -- 2026-09-12 (scheduled), wrap-up run 2026-09-13
Generated: 2026-09-13 (Saturday weekly wrap-up, `pm-weekly-wrapup` scheduled task, autonomous)

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have tasks | 6 (S13-01..S13-06) | 0 landed — all 6 verified byte-for-byte unchanged from sprint open | -6 |
| Should-Have tasks | 6 (S13-07..S13-12) | 2 done (S13-07 doc-sync, S13-10 VContainer ADR), 4 not done | -4 |
| Nice-to-Have tasks | 2 (S13-N1, S13-N2) | 0 done (both blocked on S13-02, which never ran) | -2 |
| Completion Rate (Must-Have, verified against source) | -- | 0% | -- |
| Story Points / Effort Days (Must-Have, 0.75d planned) | 0.75d | 0d | -0.75d |
| Bugs fixed this cycle (source-verified) | -- | 0 | -- |
| Bugs still open, verified against current source | -- | BUG-052, BUG-063, BUG-064 (item 7), BUG-065, BUG-066 — all unchanged | -- |
| New bugs found this cycle | -- | BUG-067, BUG-068, BUG-069, BUG-070 (all in unplanned Abilities v2 / Vitals work) | +4 |
| Commits (`ab39d45..0622b07`) | -- | 16 (incl. 3 merges, 3 standups) | -- |
| `.cs` files touched | -- | 43 | -- |

### Velocity Trend

| Sprint | Must-Have Planned | Must-Have Completed | Rate |
|--------|---------|-----------|------|
| Sprint 10 | 1.0d | 0d | 0% |
| Sprint 11 | 0.75d | ~2/5 clean, 1 partial | ~40-50% |
| Sprint 12 | 1.0d | 0/5 clean, 2 partial (source-level only) | ~40-50% functional, 0% verified |
| Sprint 13 (current) | 0.75d | 0/6 clean, 0 partial | 0% |

**Trend**: Declining, not flat. Sprint 13 is the first of the last four sprints to land **zero**
functional progress on its own Must-Have list — not even the partial credit Sprint 11/12 got for
large-refactor progress. All six Must-Have items were individually the *cheapest, most isolated*
fixes in the entire backlog (0.05d-0.2d each, fully diagnosed, fix pattern already proven in-repo)
specifically because the last two retros recommended removing them from competition with larger
work. That mitigation did not hold: none were touched. Meanwhile a large unplanned feature
(Abilities v2 completion) shipped instead and introduced 4 new bugs of its own.

### What Went Well
- **`/doc-sync` (S13-07) finally landed** (`bbfb302`) after losing its session in Sprint 11 and
  Sprint 12 — CLAUDE.md's Repository Layout, Known Bugs, and Event System sections are now
  reconciled against the 298-file restructure and current source, ending a 3-cycle drift.
- **The VContainer/DI ADR (S13-10) landed** as ADR-0004, status Accepted — the DI layer that shipped
  undocumented in Sprint 10 (`aa4e620`) finally has a governing decision record, closing the
  specific gap BUG-052 and the Sprint 12 retro both named as the largest undocumented surface.
- **The Abilities v2 promotion is now functionally complete in source** — all 17 files from
  `prototypes/skill-enhance-abilities/` are fully integrated into `Assets/Script/System/Abilities/`,
  with `AbilityHolder` driving the full `SkillState` lifecycle. Whatever its bugs (see below), this
  closes out a piece of long-running architectural debt (the framework had been "promoted but not
  finished" for a full sprint).
- **This wrap-up's code-review pass caught 4 live bugs in the new work before it reached a Play Mode
  session** — including one (BUG-067, heal/damage swap) that a player would have hit on the very
  first item pickup. Catching this in review rather than in a smoke test is the process working as
  intended, even though the underlying discipline that let it ship un-reviewed did not.

### What Went Poorly
- **All six Must-Have bug-fix tasks (S13-01, S13-03, S13-04, S13-05, plus the S13-06 process gate)
  failed to land, for the third sprint running on the exact same items.** BUG-063, BUG-064 item 7,
  BUG-065, and BUG-066 are byte-for-byte identical to their state at sprint-13 open. This is not a
  new finding — it is the Sprint 12 retro's Action Item #1 and #3, verbatim, unactioned a second
  time, despite that retro explicitly naming "remove trivial items from competition with large ones
  entirely" as the fix.
- **Unplanned work displaced the entire sprint plan a second consecutive cycle**, and this time it
  shipped bugs of its own. The week's real commits (`9b8d40f`, `5c7afba`, `4e4eff5`, `9f1258c`,
  `e1c9606`, `a4d1793`, `0622b07`) trace to zero sprint-13 task IDs. Unlike Sprint 12 (where the
  off-plan-shaped work was at least the previous sprint's stated goal, BUG-064), this cycle's
  off-plan work — finishing Abilities v2 — was never on any sprint plan at all, and introduced
  4 new S1/S2 bugs (BUG-067 heal/damage swap, BUG-068 unguarded NRE on ability activation, BUG-069
  cooldown never enforced, BUG-070 unguarded dictionary indexer mirroring the still-open BUG-066)
  with no story, no QA plan, and no test coverage gating it.
- **The Owner-in-Editor Play Mode session (S13-02) did not occur for the fifth consecutive sprint**
  (tracked since Sprint 8/S11-07). Every "fixed" or "done" claim on the Abilities v2 work — and every
  still-open bug — remains unverified against a running build. BUG-068 in particular means the very
  first skill press in that session would likely have thrown an NRE.
- **BUG-063 is now on its 25th+ consecutive carry** with an unchanged one-line fix, spanning five
  sprints (10 through 13) inclusive.
- **Zero of the pure owner-judgment/mechanical items moved**: pre-push hook (S13-06, 20th+ carry),
  S4-05/S4-06 decision (S13-08, 18th+ carry), ADR-0002 Accept (S13-09, 14th+ carry), batch bug-file
  generation (S13-11 — the 4 new files this cycle came from the code-review pass, not from the
  scoped qa-lead task).
- **QA plan gap is now the 26th+ consecutive cycle** without one — sprint-13 opened and closed with
  no `production/qa/qa-plan-sprint-13.md`.

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No owner-in-Editor session, entire sprint | All 5 sprint days | None — nothing verified against a running build | 5th consecutive sprint with this exact gap; needs an explicit owner-time commitment, not another automated pass |
| No Unity CLI / no `gh` CLI in this environment | Ongoing | Play Mode and PR gates unreached again | Structural — cannot be resolved autonomously |
| Unplanned work absorbed all available session time | All 5 sprint days | None — the scoped Must-Have list was never opened | Sprint-14 should consider a hard commit-order gate (e.g., the cheap fixes must be the first commit of the sprint, checked at kickoff) rather than a schedule note, since the schedule-note mitigation has now failed 3 times running |

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S13-01 (BUG-064 item 7) | 0.1d | 0d | -0.1d (100% short) | Lost to unplanned Abilities v2 work, identical failure mode to Sprint 12 |
| S13-03 (BUG-063) | 0.05d | 0d | -0.05d (100% short) | Same pattern, now 5 sprints running |
| S13-04 (BUG-065) | 0.1d | 0d | -0.1d (100% short) | Same pattern |
| S13-05 (BUG-066) | 0.15d | 0d | -0.15d (100% short) | Same pattern |
| S13-07 (doc-sync) | 0.4d | ~0.4d landed | On estimate | The one large Should-Have item that *did* get session time |
| S13-10 (VContainer ADR) | 0.3d | ~0.3d landed (via same doc-sync session) | On estimate | Bundled with S13-07's session |

**Overall estimation accuracy**: identical conclusion to Sprint 11 and Sprint 12 — every
zero-blocker mechanical item estimated correctly but received exactly 0% of its planned session
time. The two items that *did* land were the largest-effort Should-Have items, not the smallest
Must-Have ones — an inversion of the sprint's own stated priority order.

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-063 (`Stat.cs` regression) | Sprint 10 (NEW-4 regression) | 25th+ cycle | Deprioritized behind unplanned work a 5th sprint running | Recommend treating as accepted risk requiring explicit owner review, not a 6th automatic carry |
| BUG-064 item 7 (`RangeWeapon` DI) | Sprint 12 | 2nd carry | Same as above | First commit of sprint-14, no exceptions |
| BUG-065 (`PlayerDeathState` movement) | Sprint 12 (found in wrap-up) | 1st carry | Lost to unplanned work | Bundle with BUG-063/064 item 7 as the sprint-14 opening block |
| BUG-066 (`EntityVitalStats` indexer) | Sprint 12 (found in wrap-up) | 1st carry | Lost to unplanned work; twin bug (BUG-070) now exists on the player side too | Fix BUG-066 and BUG-070 together |
| Pre-push hook (S13-06) | Sprint 6 | 20th+ carry | Owner/producer action, not agent time | Land a minimal placeholder — recommended 5+ sprints running |
| S4-05/S4-06 decision (S13-08) | Sprint 4 | 18th+ carry | Decision-avoidance, no technical blocker | Force a written decision regardless of owner availability |
| ADR-0002 → Accepted (S13-09) | Sprint 9 | 14th+ carry | Low urgency, no consequence for staying Proposed | Batch with S4-05/S4-06 as one owner sign-off pass |
| First EditMode test (S13-12/TD-014) | Long-standing | Unreached | `tests/` still `.gitkeep`-only for the entire project history | BUG-066/BUG-070's identical shape is a natural shared first assertion |
| First playtest | Long-standing | Unreached, last log 2026-06-12 | Blocked on a Play Mode session that has not occurred in 5+ sprints | Sequence directly after one Console-clean check happens |
| **BUG-067/068/069/070 (new, unplanned Abilities v2 work)** | Sprint 13 | 0 (new) | Shipped without review gate; caught by this wrap-up's code review | Fix BUG-067 and BUG-068 first — both threaten the still-unrun Play Mode smoke session directly |

### Technical Debt Status
- TODO/FIXME/HACK inline comments: 0 found via grep across `Assets/Script/**/*.cs` — not tracked by
  convention in this codebase (consistent with prior cycles); `production/qa/bugs/` remains the real
  debt ledger.
- Bug files on disk: 10 (`BUG-052/053/063-070.md`) — up from 6 last cycle. 4 new files this cycle came
  from the autonomous code-review pass, not from the still-uncompleted S13-11 batch-generation task.
- Documentation drift **improved** this cycle (S13-07 landed) — the first cycle in three where this
  metric moved in the right direction instead of compounding.

### Previous Action Items Follow-Up (from Sprint 12 retro)

| Action Item (from Sprint 12 retro) | Status | Notes |
|---|---|---|
| 1. Finish BUG-064 item 7 | **Not Started** | Verified byte-for-byte unchanged at HEAD |
| 2. Get one owner-in-Editor session | **Not Started** | 5th consecutive sprint with this gap |
| 3. Land BUG-063 in total isolation | **Not Started** | 25th+ carry |
| 4. Land the pre-push hook as a bare placeholder | **Not Started** | 20th+ carry |
| 5. Escalate `/doc-sync` to a blocking sprint-13 task | **Done** | `bbfb302`, verified current against source |
| 6. File the VContainer/DI + Item-system ADR | **Done** | ADR-0004, Status: Accepted |

**2 of 6 action items completed this cycle** — an improvement over Sprint 12's "0 of 6," but the two
completed items are exactly the two largest-effort ones, while the four *blocking* items (all small,
all previously flagged "land first") remain at zero for a second consecutive retro. The pattern is
now sharp enough to name directly: **this project reliably executes large-effort work items and
reliably fails to execute small, unblocked ones**, regardless of stated priority.

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | **Fix BUG-067 (heal/damage swap) and BUG-068 (unguarded NRE in `AbilityHolder.GetAbility()`) before anything else** — both are live, reachable, and BUG-068 will likely crash the very Play Mode session sprint-14 needs | gameplay-programmer | **Blocking** | Sprint 14, first commit |
| 2 | **Land BUG-063, BUG-064 item 7, BUG-065, BUG-066 as a dedicated pre-session block with no other file open** — the "schedule it first" mitigation has now failed 3 sprints running; consider gating sprint-14's kickoff on these 4 commits existing before any other work starts | gameplay-programmer / ai-programmer | **Blocking** | Sprint 14, before any other task |
| 3 | **Get one owner-in-Editor session** — this is the 5th sprint asking for the same thing; if it cannot happen, the sprint plan should stop scheduling it as a task and instead record it as an explicit accepted-risk decision | Owner (Kay) | **Blocking** | Sprint 14, as early as possible |
| 4 | **Decide whether unplanned feature work (this cycle: Abilities v2) should route through even a lightweight QA gate before merging to a shared branch** — 4 new S1/S2 bugs shipped this cycle with zero review until the wrap-up caught them 5 days later | producer | High | Sprint 14 kickoff |
| 5 | **Fix BUG-066 and BUG-070 together** (identical unguarded-dictionary shape, enemy and player sides) and land the first EditMode test against it — the natural TD-014 starting point named for 2 cycles running | ai-programmer | High | Sprint 14 |

### Process Improvements
- **"Schedule the cheap fix first" has now failed as a written instruction for 3 consecutive sprints.**
  Recommend replacing the schedule-note approach with a hard mechanical gate for sprint-14: the
  sprint cannot be marked as having started substantive work until BUG-063/064-item-7/065/066 exist
  as committed diffs. A note in a plan file is not enough when the same note has been ignored 3
  times running.
- **Off-plan work needs the same review gate as scoped work, not less.** This cycle's unplanned
  Abilities v2 completion shipped 4 new bugs with zero review until 5 days later. Recommend: any
  multi-file change touching a new or actively-being-promoted system gets a same-day `/code-review`
  pass, regardless of whether it was on the sprint plan.

### Summary
Sprint 13 is the first of the last four sprints to land zero functional progress on its own
Must-Have list, repeating — verbatim — two of the previous retro's top action items for a second
unactioned cycle. The sprint's real output was unplanned: finishing the Abilities v2 promotion,
which is genuine architectural progress but shipped with 4 new S1/S2 bugs (including one, BUG-067,
that a player would hit on the very first healing-item pickup) because it went through no review
gate until this autonomous wrap-up caught it. The two items that did land (doc-sync, the VContainer
ADR) were the largest-effort Should-Have tasks, not the smallest Must-Have ones — confirming the
pattern named in the last two retros: this project executes large work reliably and small, unblocked
work almost never, regardless of how many times the small items are flagged "land first." Sprint 14
should treat that meta-pattern, not the individual bugs, as the thing that must change — via a hard
gate rather than another schedule note — while separately deciding whether off-plan feature work
needs a same-day review requirement going forward.
