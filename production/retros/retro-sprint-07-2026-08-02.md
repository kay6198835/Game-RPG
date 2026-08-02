# Retrospective: Sprint 7 Close-Out

Period: 2026-07-27 -- 2026-07-31 (planned 5-day window, branch `sprint-07`)
Generated: 2026-08-02 (automated `/weekly-sprint wrapup --auto`, scheduled Saturday 22:00 slot)

---

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| `.cs` files changed (`sprint-07`, since last wrap-up `a27cb34`) | — | 41 files | — |
| Commits since kickoff (`79f5057`..HEAD) | — | 55 (incl. 5 standup chores, 6 merges of `origin/feature/enemy-control`) | — |
| Must-Have tasks (S7-00..S7-13) | 14 (≈2.65d) | 6 confirmed done (S7-00/01/03/05/06/07); 8 not done (S7-02 dead-code-only, S7-04 one-line-from-done, S7-08/09/10/11/12/13) | ~43% by task count |
| P1 bugs closed this sprint | — | 6 real wins (BUG-024/025/027/029/030/031 — compile errors, confirmed by direct code read) | — |
| P1 bugs still open | 9 carried (this cycle's triage) | 12 (8 carried + 2 net-new combat-breaking + off-plan-work + ADR-0002) | +3 |
| New bugs found (this cycle's review + verification) | — | 11 (BUG-041 through BUG-051) | — |
| Combat functional in either direction | Goal: yes, verified in Play Mode | No — confirmed broken both directions by direct code read | Sprint's own stated goal not met |
| Playtest sessions | 1 stretch goal (S7-N1), gated on combat working | 0 | -1 (8th consecutive cycle) |
| QA plan | Flagged missing, deferred to owner | Still does not exist | 0 (6th consecutive cycle) |

### Velocity Trend

| Sprint | Planned Must-Have | Landed | Rate |
|--------|---------|-----------|------|
| Sprint 5 | ~3.95d | 1.35d | 34% |
| Sprint 6 | 2.20d (9 tasks) | ~5/9 tasks, build likely broken | ~55% by count, negative by outcome |
| Sprint 7 (current) | 2.65d (14 tasks) | 6/14 tasks, 2 new combat-breaking regressions | ~43% by count, still negative by outcome |

**Trend**: Third straight cycle where task-count velocity and actual product health point in opposite
directions. Sprint 7 landed real, verifiable fixes (6 compile errors, confirmed by direct code read
this cycle, not just a commit message claim) — but the sprint's own headline goal ("verify the enemy
death chain end-to-end in Play Mode") ends the week further away than it started, because combat now
fails in *both* directions instead of one.

---

### What Went Well

- **6 of last cycle's 9 compile-blocking S1 bugs are genuinely fixed**, confirmed today by direct
  `grep`/`Read` of current file contents, not by trusting a commit message: BUG-024
  (`[SerializeField]` on auto-property), BUG-025 (`EndRangeTrigger` scope), BUG-027 (Transform-as-bool),
  BUG-029 (duplicate `ON_PLAYER_DEATH`), BUG-030 (`Awake()` now correctly overrides `CoreBase.Awake()`),
  BUG-031 (`CoreComponentBase.Setup()` restored, `Core` back-reference populated). This is real,
  durable progress on the hub-refactor foundation that Sprint 6 left broken.
- **`Core.GetCoreComponent<T>()` is structurally sound now** — both `Core.cs` and `EntityCore.cs`
  correctly call `base.Awake()`, and `Setup()` is no longer commented out. The one thing S7-08 was
  meant to verify (Play Mode confirmation) is the only piece still missing, not the code itself.
- **Known Bugs #5, #7, #8 (entity death chain) remain fixed and stable** — re-verified this cycle,
  no regression. `EntityDeathState` still correctly extends `EntityState`/`EntityBasicState`, and
  `EntityBasicState` still transitions on `Health <= 0`.
- **Bug #9 (AnimationPlayerController double-registration) is fixed**, confirmed this cycle —
  `StartAnimation`/`EndAnimation` are now registered as distinct callbacks in both `OnEnable`/`OnDisable`.
  This was not a scheduled task this sprint; a genuine incidental win.
- **Bug #13 (start-room teleport) is mostly resolved** — 2 of 3 call sites are now active; only one
  commented line remains in a currently-unreachable method.
- Two independent lead-programmer review agents, working on disjoint file sets, each surfaced
  demo-blocking findings this cycle that a Friday human-authored review (`docs/reviews/melee-combat-review.md`)
  had already flagged 3 days earlier — the combat-breaking regression was not a surprise, it was visible
  and named before this wrap-up ran.

---

### What Went Poorly

- **Off-plan work recurred for a 7th consecutive cycle, in direct violation of this sprint's own
  written scope.** `sprint-07.md` states explicitly: "Explicitly deferred, do not start this sprint:
  BUG-035/036/037 (Pathfinding correctness/perf)... no further work on Pathfinding or Base/CoreBase
  until the hub refactor is confirmed compiling and working (S7-08 gate)." The daily-plan standups
  (Wed 07-29, Thu 07-30, Fri 07-31) each independently confirm Pathfinding/Entity-chase work continued
  anyway — 3 separate merges of `origin/feature/enemy-control` landed this sprint, and S7-08 (the gate
  that was supposed to block this) was never confirmed. This is not a new finding — it is the same
  named pattern from Sprint 5's and Sprint 6's retros, now one cycle older.
- **S7-D4 — the root-cause conversation specifically scheduled to break this pattern — was never
  held**, for the 2nd sprint running it was explicitly scheduled and skipped. This is now the single
  most consequential unaddressed action item across 3 retros in a row (Sprint 5, Sprint 6, Sprint 7).
- **Two new combat-breaking S1 regressions appeared this cycle**: `WeaponMelee.Attack()` went from
  "unwired but content-correct" (per the 07-30 independent review) to **completely empty** — someone
  removed the working `OverlapCircle`/`TakeDamage` body rather than just leaving it unwired. Separately,
  a new file `EntityNegativeReciver.cs` was added that duplicates the exact `NotImplementedException`
  pattern `EntityCore.TakeDamage()` already had, on the same enemy hierarchy — two broken damage
  receivers where there should be one working one.
- **Bug #6 (player damage/death) failed a 3rd distinct way across 3 cycles**: `NotImplementedException`
  (Sprint 5) → disconnected health field (Sprint 6) → still disconnected, and this cycle
  `PlayerDeathState` itself is confirmed never instantiated or transitioned-to at all, so even a working
  health decrement would have nowhere to route. S7-11 (the story scoped specifically with a mandatory
  EditMode test to stop this pattern) did not start.
- **4 of the 6 Must-Have items that were "one line from done" at Wednesday's standup (S7-04, S7-09,
  S7-10) still did not land by Saturday** — the daily-plan's own Wed/Thu/Fri notes repeatedly observe
  these are trivial fixes not landing because attention went to the unplanned Pathfinding work instead.
  This is the clearest evidence in 3 cycles that off-plan work is actively displacing scoped work, not
  just running in parallel with it.
- **ADR-0002 remained `Status: Proposed` the entire sprint** despite being flagged as a "trivial, 3rd
  day untouched" task in Wednesday's own standup — not a complexity problem, a prioritization one.
- **S4-05/S4-06 is now an 8th consecutive carry with zero movement.** This was explicitly named
  "no further silent re-carry" in this sprint's own plan and was carried silently anyway.
- **Playtest gap is now 8 consecutive cycles** (since 2026-06-12) and **QA plan is now a 6th
  consecutive cycle** with none filed. Both remain gated on combat working, which regressed this cycle
  rather than progressing toward unblocking them.

---

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No Unity CLI in this environment — S7-08 (Play Mode gate) could never be confirmed, so downstream tasks (S7-09, S7-11) stayed technically ungated all week | Ongoing, all sprints | None — gate never cleared, work proceeded anyway without it | Owner should run the S7-08 Play Mode check as literally the first action of a session, before any further code changes land on top |
| Off-plan Pathfinding/Base work displacing scoped Must-Have items, 7th consecutive cycle | Days 1-5 (per daily-plan standups) | None — ran to the end of the sprint window again | Same recommendation named in Sprint 5 and Sprint 6 retros, still not acted on — see Action Items below |

---

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S7-00/01/03/05/06/07 (compile-error fixes) | ~1.15d combined | Landed and confirmed by direct code read | ~on target | Same pattern as Sprint 6's death-chain fixes — small, well-scoped, testable units land reliably |
| S7-04/09/10 ("one-line" fixes per Wed standup) | ~0.35d combined | Still not landed by Saturday | +100% (never finished despite being trivial) | Not a difficulty problem — the daily-plan's own notes attribute this to attention going to unplanned Pathfinding work instead |
| S7-11 (Bug #6 re-scope + EditMode test) | 0.4d | Not started | +100% | Gated behind S7-08/09/10 in sequencing, none of which cleared |
| "Pathfinding/Entity-chase continuation" (unplanned, explicitly deferred by the sprint plan) | 0d (explicitly deferred) | Consumed most of Tue-Fri per commit density, incl. 3 branch merges | N/A — explicitly out of scope | 7th consecutive cycle of the same root cause named in Sprint 5's retro |

**Overall estimation accuracy**: identical pattern to the last 2 cycles — estimates for scoped,
small tasks are accurate when actually worked; the sprint's real problem is not sizing but that
explicitly-deferred work keeps landing anyway and displacing the scoped list.

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-032 (enemy skill NullRef) | Sprint 6 | 2 | "Trivial" per Wed standup, displaced by off-plan work | Sprint 8 — literally uncomment one line, do it first |
| BUG-033/BUG-ES-1 (spawn null-guard order) | Sprint 4 | 5 (S4→S5→S5x2→S6→S7) | Same — trivial, repeatedly deprioritized | Sprint 8 — first task, before anything else, per Sprint 6's retro which already said this |
| Bug #6 write-through (`NegativeReciver`→`PlayerData`) + PlayerDeathState wiring | Sprint 1 (root) | 3 (as distinct-failure-mode reopens) | Displaced by off-plan work for the 3rd time running | Sprint 8 — dedicated spike, not another opportunistic fix; EditMode test mandatory before marking Done |
| ADR-0002 status flip | Sprint 4 | 4 | Simple 0.1d task, deprioritized every cycle including this one where it was called "trivial, 3rd day untouched" | Sprint 8 — no excuse to carry a 5th time |
| S4-05/S4-06 keep-or-cut decision | Sprint 2/3 | 8 | Never resolved despite "no further silent re-carry" written into this sprint's own plan | Owner must make this call directly at Sprint 8 kickoff — recommend timeboxing to 5 minutes |
| First playtest | — | 8 cycles | Tied to combat working, which regressed this cycle instead of clearing | Sprint 8 — now blocked on BUG-041/042 landing first |
| QA plan | Sprint 3 | 6 | Repeatedly deferred | Needs explicit owner commitment |
| Off-plan architecture work displacing scoped tasks | Sprint 4 | 4 (S4→S5→S6→S7) | Root-cause conversation scheduled twice (S6, S7-D4), held zero times | Sprint 8 — this must be the first act of the sprint, not a scheduled-and-skipped line item again |
| NEW: BUG-041 (player attack empty+unwired) | Sprint 7 | 0 (new, but same underlying symptom class as old Bug #4) | Regression — body was previously content-correct, now empty | Sprint 8 — small, well-scoped, should land in <0.5d given the fix is already documented in `melee-combat-review.md` |
| NEW: BUG-042 (enemy TakeDamage duplicate-NotImplemented) | Sprint 7 | 0 (new) | New file introduced the same broken pattern a second time on the same hierarchy | Sprint 8 — pick one canonical receiver, delete the other |

---

### Previous Action Items Follow-Up (from `retro-sprint-06-2026-07-26.md`)

| # | Action Item | Status | Notes |
|---|--------------|--------|-------|
| 1 | Open the project in Unity Editor and confirm/deny all 6 suspected compile errors before Sprint 7 kickoff | Presumed Done | The 6 named compile errors (BUG-024-031) are confirmed fixed by direct code read this cycle — consistent with the Editor having been opened and errors addressed, though no explicit Console-clean confirmation note was found |
| 2 | Do not start any new architecture work until Base/CoreBase compiles AND `Core.GetCoreComponent<T>()` verified in Play Mode | Not Done | S7-08 (the Play Mode verification) was never confirmed, and Pathfinding/Entity work continued anyway all week — this is the exact violation the action item was written to prevent |
| 3 | Fix BUG-ES-1 (spawn null-guard) and BUG-06 write-through | Not Done | Both still open — BUG-033/Bug #6 confirmed unchanged by direct code read today |
| 4 | Hold the "why does off-plan work keep recurring" conversation at Sprint 7 kickoff | Not Done | No evidence of this conversation in `sprint-07.md`, `sprint-07-daily-plan.md`, or commit history — S7-D4 (its scheduled slot) itself was never held |
| 5 | Run `/qa-plan sprint` before Sprint 7's first story | Not Done | 6th consecutive cycle skipped |

**0 of 5 fully done, 1 presumed-done-but-unverified, 4 not done.** Every action item this project's
retros have called "Critical" for 2 cycles running was not completed this cycle either. This is now
a pattern about the action items themselves, not just the underlying bugs.

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | Fix BUG-041 (wire + re-implement `WeaponMelee.Attack()`) and BUG-042 (pick one canonical `INegativeReceiver` on Entity, delete the duplicate) — both block the sprint goal outright and are small | gameplay-programmer / ai-programmer | Critical | Sprint 8, Day 1, before anything else |
| 2 | **Hold the off-plan-work root-cause conversation as literally the first act of Sprint 8** — this is the 3rd consecutive retro naming this as Critical and the 2nd consecutive sprint where it was scheduled and skipped. If the standard scheduling approach keeps failing, consider a structural change instead (e.g., a hard scope-lock that blocks commits touching `Pathfinding/` or `Character/Base/` without a sprint-file amendment first) | Owner (Kay) / Producer | Critical | Sprint 8 kickoff, before any code work |
| 3 | Force the S4-05/S4-06 decision directly — 8 cycles of carry is not an estimation gap, it is a decision that needs 5 minutes of owner attention | Owner (Kay) | High | Sprint 8 kickoff |
| 4 | Re-scope Bug #6 + PlayerDeathState wiring as one dedicated story mirroring the now-proven `EntityBasicState → EntityDeathState` pattern, EditMode test mandatory before Done | gameplay-programmer | High | Sprint 8, after Action 1 |
| 5 | File individual `production/qa/bugs/BUG-NNN.md` reports (S7-D3) — recommend literally the first 30 minutes of Sprint 8 | qa-lead | Medium | Sprint 8, Day 1 |
| 6 | Run `/qa-plan sprint` before Sprint 8's first story — 6th consecutive cycle skipped | qa-lead | High | Sprint 8, Day 1 |

---

### Process Improvements

- **The scheduling of "hold a conversation" as a sprint task has now failed twice in a row (S6-D4-equivalent,
  S7-D4).** Scheduling it a 3rd time with the same mechanism is unlikely to produce a different result.
  Recommend a structural alternative: a pre-push or pre-commit check that flags commits touching
  `Assets/Script/Pathfinding/` or `Assets/Script/Character/Base/` when those paths are not named in the
  current sprint's Must-Have/Should-Have task list, surfaced at the next standup automatically rather
  than relying on a scheduled conversation that keeps not happening.
- **"One line from done" items (S7-04/09/10) sitting open for 3+ days is a signal to watch for going
  forward** — when a daily standup explicitly says a fix is trivial and it still doesn't land by the
  next standup, that is itself evidence attention went elsewhere, and is worth flagging in the standup
  output the same day rather than only at Saturday wrap-up.
- **Independent human review (`melee-combat-review.md`, 07-30) caught the WeaponMelee regression 3 days
  before this automated wrap-up did.** This is a good signal that the review cadence is working — but
  the finding sitting unaddressed for 3 days before the code got worse (empty body vs. unwired-but-correct)
  suggests review findings need a faster feedback loop than "wait for Saturday," e.g. a same-day
  acknowledgment even if the fix itself waits.

---

### Summary

Sprint 7 is a genuine split result. The narrow, well-scoped compile-error fixes (BUG-024 through
BUG-031, S7-00/01/03/05/06/07) landed cleanly and are confirmed by direct code read — 6 of 9 of
Sprint 6's most urgent findings are real, durable wins, and the component-hub refactor is now
structurally sound where it was previously non-functional. But the sprint's actual stated goal —
verify the enemy death chain end-to-end in Play Mode — is further from reachable at the end of the
week than at the start, because combat now fails in *both* directions (`WeaponMelee.Attack()` regressed
to empty; `EntityCore`/`EntityNegativeReciver.TakeDamage()` both throw). The root cause is the same
one named in the last two retros and not yet acted on: off-plan Pathfinding/Base-refactor work
recurred for a 7th consecutive cycle, this time in direct violation of the sprint's own written
"do not start" instruction, and the conversation scheduled specifically to address this (S7-D4) was
not held for the 2nd sprint running. Verdict: **CONCERNS** — real structural progress happened, but
the sprint cannot be called healthy while the game's core combat loop is confirmed non-functional in
both directions and the process pattern causing repeated scope displacement remains unaddressed after
three consecutive retros naming it.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-08-02.md`
- Sprint plan: `production/sprints/sprint-07.md`
- Daily plan tracker: `production/sprints/sprint-07-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-06-2026-07-26.md`
- Independent human review: `docs/reviews/melee-combat-review.md`
- ADR-0002: `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
