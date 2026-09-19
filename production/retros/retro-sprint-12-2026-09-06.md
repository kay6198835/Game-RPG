## Retrospective: Sprint 12
Period: 2026-08-31 -- 2026-09-04 (scheduled), wrap-up run 2026-09-06
Generated: 2026-09-06 (Saturday weekly wrap-up, `pm-weekly-wrapup` scheduled task, autonomous)

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have tasks | 5 (S12-01..S12-05) | 0 fully met, 2 partial (S12-01, S12-03), 1 not met (S12-02), 2 not met/no evidence (S12-04, S12-05) | -3 |
| Should-Have tasks | 6 (S12-06..S12-11) | 0 confirmed done | -6 |
| Nice-to-Have tasks | 3 (S12-N1..N3) | 0 confirmed | -3 |
| Completion Rate (Must-Have, strict acceptance criteria) | -- | 0% clean / ~40-50% functional (source-level) | -- |
| Story Points / Effort Days (Must-Have, ~1.0d planned) | 1.0d | ~0.5d landed in source, 0d verified in Play Mode | -0.5d |
| Bugs fixed this cycle (source-verified) | -- | BUG-053 (fully), BUG-064 (6 of 7 sub-items) | +2 (partial) |
| Bugs still open, verified against current source | -- | BUG-063 (24+ cycles), BUG-064 item 7, BUG-052 (grown in scope) | -- |
| New bugs found this cycle | -- | BUG-065 (`PlayerDeathState` movement regression), BUG-066 (`EntityVitalStats` dictionary risk) | +2 |
| Commits (`6348dc6..f3f5f08`) | -- | 24 | -- |
| `.cs` files touched | -- | 87 (largest single-week diff on record) | -- |

### Velocity Trend

| Sprint | Must-Have Planned | Must-Have Completed | Rate |
|--------|---------|-----------|------|
| Sprint 10 | 1.0d | 0d | 0% |
| Sprint 11 | 0.75d | ~2/5 clean, 1 partial | ~40-50% |
| Sprint 12 (current) | 1.0d | 0/5 clean, 2 partial (source-level only) | ~40-50% functional, 0% verified |

**Trend**: Flat-to-stable in raw functional terms (a third consecutive sprint landing roughly
half its Must-Have scope), but the *quality* of "done" is degrading: Sprint 11 at least closed two
items completely clean (BUG-033, BUG-044). Sprint 12 closes zero items completely clean — every
"fixed" this cycle is fixed-in-source-only, unconfirmed by any Play Mode session. The **verified**
completion rate, as opposed to the functional one, is 0% for the second sprint running.

### What Went Well
- **BUG-064 (the sprint's sole stated goal) went from "does not compile" to "compiles, one DI wire
  short of a full smoke pass."** 6 of 7 sub-items verified fixed directly against source: the
  `EntityStatsSO`/`Entity.Data`/`EntityFindTarget` dangling-reference chain is gone, `EntityFindTarget`
  was properly reimplemented (not just deleted, closing part of NEW-1's unfinished half), the
  `ON_ENEMY_DEATH` payload mismatch is resolved, and the `Resgister`/`UnResgister` typo is fixed. This
  is real, substantial movement on the project's single hardest blocker.
- **BUG-053 (enemy health routing) is now genuinely, cleanly fixed** — `EntityNegativeReciver.cs` was
  fully rewritten to route through `EntityVitalStats`/`EntityStatsHandler`/`EntityUIController`, with
  no trace of the old player-only logic. This closes a bug that had carried 6+ cycles.
- **Thursday's scope drift was caught and reversed the same day.** The unplanned boss-framework commit
  (`ffe1976`) was reverted (`4421fdc`, `ff67f4d`) rather than left to compound — a real instance of the
  Sprint 11 retro's "was this drift visible in real time" question getting answered "yes" this time.
- **The daily-plan tracker's text-verification discipline held up under an 87-file diff.** Friday's
  standup entry correctly isolated the single remaining blocker (`RangeWeapon.cs` DI) out of a huge
  changeset, and this wrap-up's independent re-verification confirms every claim in that entry was
  accurate — the tracking process itself is working, even though the underlying item didn't land.

### What Went Poorly
- **BUG-063 — the cheapest item in the entire project backlog — is now on its 24th+ consecutive carry.**
  One line, a comment in the file explaining exactly what to do, explicitly flagged "land it first" in
  three consecutive sprint plans running. It is not a sizing problem; it is a prioritization pattern
  that has now failed to self-correct across an entire sprint's worth of explicit flagging.
- **BUG-064 item 7 (`RangeWeapon` DI) — the single smallest piece of the sprint's stated goal — did not
  land**, despite the fix pattern existing verbatim elsewhere in the same week's diff
  (`ItemSpawner.cs`). The daily plan named this exact gap and this exact fix at both Thursday and
  Friday's standups; it remained open through the sprint's last scheduled day.
- **No owner-in-Editor Play Mode session occurred at any point in Sprint 12** — the fourth+ consecutive
  sprint with this exact gap (tracked as S11-07 and equivalents since Sprint 8). Every fix claimed
  "done" this cycle is unverifiable against a running build. This is the same structural risk Sprint
  11's retro named as the reason its own "fixed" claims turned out to hide a compile break.
- **Zero of the six Should-Have process/debt items landed**: pre-push hook (S12-05, now 19+ carry),
  ADR-0002 Accept (S12-07, 13+ carry), S4-05/S4-06 decision (S12-06, 17+ carry), DI/VContainer ADR
  (S12-08, 2nd carry but now covering a much larger undocumented surface), batch bug-file generation
  (S12-09, still 4 of 9+ items filed before this cycle), doc-sync (S12-10). None require deep technical
  work — all are owner-judgment or single-session mechanical tasks, and all lost every session to the
  BUG-064 fix.
- **CLAUDE.md's documentation drift has grown, not shrunk.** This week's 298-file folder restructure
  (`Assets/Script/*` → `Assets/Script/System/*`) plus the new DI layer and new Item system are entirely
  absent from CLAUDE.md's Repository Layout — a larger version of the exact gap BUG-052 already flagged
  a cycle ago. `/doc-sync` (S12-10) losing its session again means the gap compounded instead of
  closing.
- **This wrap-up's own review surfaced two bugs that were not caught by any prior session**:
  `PlayerDeathState` silently lost its movement-stop behaviour (BUG-065) — CLAUDE.md still claims this
  is fixed — and `EntityVitalStats` has an unguarded dictionary indexer on the live damage chain
  (BUG-066). Neither would have been caught without a fresh source read against CLAUDE.md's specific
  claims; this is the same failure mode BUG-052/BUG-053 already demonstrated (docs asserting a state
  that current code does not match).

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No owner-in-Editor session, entire sprint | All 5 sprint days | None — every fix this cycle is source-verified only | Same recurring gap across 4+ sprints; no automated fix possible, needs an explicit owner-time commitment |
| No Unity CLI in this environment | Ongoing | Play Mode smoke gates (S12-01, S12-03) unreached again | Structural — cannot be resolved autonomously |
| No pre-push gate | All of Sprint 12 | S12-05 not landed, 19th+ carry | Land even a placeholder hook — now recommended 4 sprints running |

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S12-01 (BUG-064) | 0.4d | ~0.35d landed (6/7 sub-items), item 7 (~0.05d equivalent) not done | Close, missed the smallest remaining slice | Large refactor absorbed the bulk of the item; the final DI-wiring step is genuinely tiny but kept losing to other work in the diff |
| S12-02 (BUG-063) | 0.05d | 0d landed | -0.05d (100% short) | Same pattern as Sprint 11 — deprioritized every session despite being flagged first, 5th consecutive sprint this has happened |
| S12-03 (BUG-053/054) | 0.2d | ~0.2d landed in source, 0d verified | On estimate for code, missing verification entirely | Verification requires the same missing Editor session as S12-01 |
| S12-05 (pre-push hook) | 0.15d | 0d landed | -0.15d (100% short) | Owner/producer-only task, no autonomous path — unchanged reason from Sprint 11 |

**Overall estimation accuracy**: the code-writing estimates (S12-01, S12-03) were roughly accurate;
the two purely mechanical, zero-blocker items (S12-02, S12-05) are the ones that stalled completely,
identically to Sprint 11 — confirming this is a prioritization pattern, not an estimation one.

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-063 (`Stat.cs` regression) | Sprint 10 (NEW-4 regression) | 24th+ cycle | Consistently deprioritized behind larger work despite being the cheapest backlog item | Land as a fully isolated, single-purpose commit in sprint-13 — do not bundle with anything else this time |
| BUG-064 item 7 (`RangeWeapon` DI) | Sprint 12 (this cycle) | 1st carry | Smallest remaining slice of a large refactor, lost to the size of the rest of the diff | First commit of sprint-13, pattern already proven in `ItemSpawner.cs` |
| Pre-push hook (S12-05) | Sprint 6 | 19th+ carry | Requires owner/producer action, not agent time | Land a minimal placeholder — recommended every cycle for 4+ sprints running |
| S4-05/S4-06 decision (S12-06) | Sprint 4 | 17th+ carry | Decision-avoidance, no technical blocker | Force a written decision regardless of owner availability |
| ADR-0002 → Accepted (S12-07) | Sprint 9 | 13th+ carry | Low urgency, no consequence for staying Proposed | Batch with S4-05/S4-06 as one owner sign-off pass |
| DI/VContainer ADR (S12-08) | Sprint 10 | 3rd carry | New architecture (now including a full Item system) keeps landing faster than it's documented | Escalate to blocking for sprint-13 — the undocumented surface has roughly tripled this cycle |
| `/doc-sync` (S12-10) | Sprint 11 | 2nd carry | Consistently deprioritized behind code work | Escalate to blocking given the scale of drift after this week's 298-file restructure |
| First playtest | Long-standing | Unreached, last log 2026-06-12 | Blocked on a confirmed Play Mode session that has not occurred in 4+ sprints | Sequence directly after item 7 lands and one Console-clean check happens |

### Technical Debt Status
- TODO/FIXME/HACK inline comments: not tracked by convention in this codebase (per Sprint 10 retro) —
  `production/qa/bugs/` + bug-triage reports remain the real debt ledger.
- Bug files on disk: 6 (`BUG-052/053/063/064/065/066.md`) — up from 4 last cycle (2 new this wrap-up).
  S12-09 (batch-generate remaining files) still did not land as its own task.
- **Documentation drift is now the largest debt item in the project**: CLAUDE.md's Repository Layout
  is stale against roughly half the codebase following this week's restructure. See BUG-052 update in
  `production/qa/bug-triage-2026-09-06.md`.

### Previous Action Items Follow-Up (from Sprint 11 retro)

| Action Item (from Sprint 11 retro) | Status | Notes |
|---|---|---|
| 0. Fix the compile break before anything else | **Mostly Done** | 6 of 7 sub-items fixed; item 7 (`RangeWeapon` DI) still open |
| 1. Land BUG-063 as the literal first commit of Sprint 12 | **Not Started** | 24th+ carry, still untouched |
| 2. Land the pre-push hook as a bare placeholder | **Not Started** | 19th+ carry |
| 3. Force S4-05/S4-06 AND flip ADR-0002 in one owner pass | **Not Started** | Both still open, 17th+/13th+ carries |
| 4. File the VContainer/DI ADR before more `LifetimeScope/` code lands | **Not Started, situation worsened** | A full new Item system and folder restructure landed on top of the DI layer with still no ADR |
| 5. Run `/doc-sync` for BUG-053's CLAUDE.md disagreement | **Not Started** | BUG-053 is now actually fixed in source, but CLAUDE.md was never reconciled — and the drift has grown far beyond just this one entry |
| 6. Batch-generate remaining `BUG-NNN.md` files | **Not Started as a task, but 2 filed as a byproduct of this review** (BUG-065, BUG-066) | Still short of "all P1 items have files" |

**0 of 6 action items were completed as their own task this cycle** (item 0 landed mostly, but as a
byproduct of the sprint's main line of work, not as a discrete action item) — a regression from
Sprint 11's "2 of 5 completed." The five purely mechanical/owner-judgment items are exactly the ones
still at zero.

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | **Finish BUG-064 item 7** — add `[Inject] Construct(IObjecPoolService)` to `RangeWeapon.cs`, mirroring `ItemSpawner.cs` (2-3 lines, pattern proven same-week same-repo) | gameplay-programmer | **Blocking** | Sprint 13, first commit |
| 2 | **Get one owner-in-Editor session**: confirm Console-clean, kill one enemy, fire the ranged weapon once. This single session would verify or invalidate every "fixed" claim in this retro and the last two triage reports | Owner (Kay) | **Blocking** | Sprint 13, as early as possible |
| 3 | **Land BUG-063 in total isolation** — a single-purpose commit, not bundled with any other task, given the demonstrated 24-cycle pattern of it losing priority when bundled | gameplay-programmer | Critical | Sprint 13, Day 1, before opening any other file |
| 4 | **Land the pre-push hook as a bare placeholder**, even `exit 0` + TODO — stop the 19-cycle silent carry | Owner (Kay) / producer | Critical | Sprint 13 kickoff |
| 5 | **Escalate `/doc-sync` to a blocking sprint-13 task**, not Should-Have — the drift is now large enough to actively mislead any session that trusts CLAUDE.md's Repository Layout or Known Bugs table without re-verifying | lead-programmer | High | Sprint 13, Day 1-2 |
| 6 | **File the VContainer/DI + Item-system ADR** before any further `System/` code lands — the undocumented surface has roughly tripled this cycle | technical-director | High | Sprint 13, Day 1-2 |

### Process Improvements
- **Bundling the cheapest backlog item with the sprint's hardest task keeps failing.** Three sprints
  running (10, 11, 12) have explicitly planned to "land BUG-063 first" alongside a larger blocker, and
  three sprints running it has lost. Recommend sprint-13 schedule BUG-063 (and the pre-push hook) as
  the **only** items available in the first work session, with the larger BUG-064 item 7 fix
  deliberately withheld from that same session — remove the competition entirely rather than relying
  on sequencing within one session.
- **A source-level "fixed" claim without Play Mode verification is now the default state of this
  project's bug backlog**, not an exception. Recommend the next retro track "verified" vs.
  "source-fixed" as two separate columns going forward, rather than one collapsed status — the gap
  between them is now the most consequential unresolved risk in the project, larger than any single
  bug.

### Summary
Sprint 12 delivered real, substantial progress on its one stated goal — the compile break is 6/7 of
the way fixed and the long-stuck enemy-health-routing bug (BUG-053) is genuinely closed — but it
closes having landed zero Must-Have items completely clean and zero Should-Have items at all, with
the sprint's single cheapest backlog item (BUG-063) now on its 24th consecutive carry and still no
Play Mode verification anywhere in the project's last four sprints. The pattern is now clear and
repeating rather than incidental: large, urgent technical work reliably gets done; small,
zero-blocker mechanical items and owner-judgment decisions reliably do not, regardless of how many
times they're flagged "land first." Sprint 13 should treat that pattern itself as the thing to fix —
not just the individual carried items — by removing trivial items from competition with large ones
entirely, and by finally securing one owner-in-Editor session to convert this sprint's source-verified
fixes into confirmed ones.
