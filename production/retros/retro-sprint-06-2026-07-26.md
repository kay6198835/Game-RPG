# Retrospective: Sprint 6 Close-Out

Period: 2026-07-21 -- 2026-07-25 (planned 5-day window, branch `sprint-06`)
Generated: 2026-07-26 (automated `/weekly-sprint wrapup --auto`, scheduled Saturday 22:00 slot)

---

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| `.cs` files changed (`sprint-06`, since last wrap-up `226f645`) | — | 75 files | — |
| Commits since kickoff (`4085cc7`..HEAD) | — | 39 (incl. 5 standup chores) | — |
| Must-Have tasks (S6-01..S6-09) | 9 (≈2.20d) | ~5 landed (S6-01→S6-05 chain, S6-09 decision claimed) / 4 not landed (S6-06/07 spawn guards, S6-08 write-through) | ~55% by task count |
| P1 bugs closed this sprint | — | 3 real wins (Bug #5, #7, #8 — death chain, confirmed by direct code read) | — |
| P1 bugs still open | 9 carried/reopened (this cycle's triage) | 9 | 0 net (offset by 6 *new* compile-blocking bugs) |
| New bugs found (this cycle's review + triage) | — | 13 | — |
| Compile-blocking errors found (6 parallel specialist reviews) | 0 expected | 6 independent (CS0592, CS0103, CS0029, CS0102, 2x invalid operator) | +6 |
| Playtest sessions | 1 planned (S6-N1) | 0 | -1 (7th consecutive cycle) |
| QA plan | Flagged missing, deferred to owner | Still does not exist | 0 (5th consecutive cycle) |

### Velocity Trend

| Sprint | Planned Must-Have | Landed | Rate |
|--------|---------|-----------|------|
| Sprint 4 | — | — | — |
| Sprint 5 | ~3.95d | 1.35d | 34% |
| Sprint 6 (current) | 2.20d (9 tasks) | ~5/9 tasks, but net code health **regressed** (build now likely broken) | ~55% by count, effectively negative by outcome |

**Trend**: Unstable, not simply "increasing/decreasing." Task-count completion improved over Sprint 5,
but this is the **third consecutive sprint** where unplanned architecture work (this time: a new
Pathfinding module + a Base/CoreBase hub refactor) consumed the back half of the week and, unlike
Sprint 5's clean-but-unplanned framework work, this time shipped with 6 independent compile-blocking
errors. Landing real fixes and breaking the build in the same week is a net-negative outcome even
though the task-completion percentage looks better on paper.

---

### What Went Well

- **The core death-chain finally landed cleanly, ending a 7-cycle carry.** Bug #5 (`EntityMoveState`
  null-deref), Bug #7 (`EntityDeathState` wrong base class), and Bug #8 (empty `Health<=0` block) are
  all confirmed **FIXED** by direct code read this cycle — `EntityDeathState : EntityState`, wired
  into `Entity.cs`, and `EntityBasicState` correctly transitions on death. This is the single most
  important structural win in several sprint cycles and should be called out explicitly, not buried
  under this week's regressions.
- Commit `d3b29d9` ("close S6-01/02/03/04 bugs, resolve S6-09 data-model decision") shows the sprint
  *did* start on-plan — the first 1-2 days landed the intended Must-Have scope before drifting.
- The new Pathfinding module (A*, priority queue, grid, request manager) is architecturally sound in
  intent — correctly avoids a new singleton (routes through the already-ratified `EnemyManager`), and
  keeps all Physics2D/Unity API calls on the main thread. The design is fixable, not fundamentally
  wrong.

---

### What Went Poorly

- **Off-plan architecture work recurred a 3rd consecutive sprint, and this time it broke the build.**
  Sprint 5's retro already flagged "4 consecutive days of off-plan work" as a confirmed pattern, not a
  one-off, and named the exact mitigation ("investigate why before re-carrying a 3rd time"). That
  investigation did not visibly happen — the same shape repeated: S6-01→S6-05 landed early in the
  week, then commits shift to "start refactor enemy bot" → "big update core and corecomponet, add
  interface" → a full A* pathfinding implementation → "need fix class bassEntity" (the commit message
  itself admits the refactor is unfinished).
- **6 independent compile-blocking errors** were found by 6 separate specialist code-review agents
  each working a disjoint file set (CS0592 on `CoreComponentBase.cs:5`, CS0103 in
  `PlayerDisadvantageState.cs:20`, CS0029 in `PlayerDeathState.cs:17,21`, CS0102 duplicate enum in
  `EventManager.cs:42`, plus two invalid-operator compile errors in `EntityInput.cs`/`EntityMovement.cs`).
  Unlike Sprint 5's single *suspected* CS0592 (never confirmed), this cycle's volume and the fact every
  reviewer independently hit one is a strong signal the branch was never opened in the Unity Editor
  after these changes landed.
- **The Base/CoreBase hub refactor is functionally broken even past the syntax errors.**
  `Core.cs`/`EntityCore.cs`'s `Awake()` hides (doesn't override) `CoreBase.Awake()`, so `Setup()` —
  the component-registration scan — never runs; `CoreComponentBase.Setup()`'s own override is
  commented out, so the `Core` back-reference is always null. Together these mean
  `Core.GetCoreComponent<T>()`, which CLAUDE.md calls the *only* sibling-discovery mechanism, is
  currently dead for both Player and Entity — independent of whether the CS0592 is fixed.
- **BUG-ES-1 (spawn null-guard) is still not fixed, and now looks more dangerous.**
  `EnemySpawner.cs:62`'s `if (set.Count == 0 || set == null)` dereferences `.Count` before the null
  check — an active regression risk that reads as "guarded" at a glance but isn't. This was S6-06,
  explicitly scheduled Must-Have work this sprint, and it did not land.
- **BUG-06 (player damage/death chain) failed a second time, in a different way.** Last cycle it threw
  `NotImplementedException`; this cycle the exception is gone, but `NegativeReciver.currentHealth` is
  a separate `public int` field that defaults to 0 and is never connected to `PlayerData.currentHealth`
  — every hit is a silent no-op from frame one, and no script anywhere subscribes to `ON_PLAYER_DEATH`.
  S6-08 (the exact task to fix this write-through) was Must-Have and did not land.
- **Enemy skill system regressed**: `EntityWeaponMelee.SetAbility()`'s `input` field assignment is
  commented out, turning a previously-working code path into a guaranteed NullReferenceException —
  this looks like a mid-refactor edit that was committed before finishing.
- **Playtest gap is now 7 consecutive cycles** with zero sessions (since 2026-06-12), and **QA plan is
  now a 5th consecutive cycle** with no file created. Both were flagged as blocking or high-priority in
  the last 2 retros and remain untouched.

---

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No Unity CLI in this environment — compile errors can only be found by static code read, not an actual build | Ongoing, all sprints | Specialist agents read code directly this cycle and found 6 suspected errors; none independently confirmed by an actual Editor compile | Owner should open the project in Unity Editor at least once per sprint before the wrap-up review, ideally mid-week |
| Scope drift into new architecture (Pathfinding + Base/CoreBase) without a corresponding plan update | Days 2-5 (est.) | None — ran to the end of the sprint window | Same recommendation Sprint 5's retro made and that went unaddressed: an explicit scope-discipline check-in mid-sprint, not just at wrap-up |

---

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S6-03/04/05 (death chain fixes) | 1.0d combined | Landed within first ~1-2 days per commit `d3b29d9` | ~on target | Well-scoped, small, testable units — the model that works |
| S6-06/07/08 (spawn guards + BUG-06 write-through) | 0.6d combined | Not landed at all | +100% (infinite, never started) | Displaced by unplanned Pathfinding/Base refactor work that consumed the rest of the week |
| "New Pathfinding module" (unplanned) | 0d (not in plan) | Consumed ~3 days (per commit density `6468d7e`→`9c75d3d`) | N/A — unplanned | Same root cause as Sprint 5: substantial architecture work started without being added to the sprint file first |

**Overall estimation accuracy**: The tasks that were estimated and started were mostly accurate
(death chain ~on target); the sprint's real problem is not bad estimates but unplanned scope
displacing scoped work, for the third sprint running.

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-ES-1/ES-4 (spawn null/index guards) | Sprint 4 | 4 (S4→S5→S5x2→S6) | Small, unblocked, still not picked up despite being Must-Have again this sprint | Sprint 7 — literally the first task, before any other work starts |
| BUG-06 write-through (`NegativeReciver`→`PlayerData`) | Sprint 1 (root), reopened Sprint 5 | 2 (as "partial fix" reopen) | Displaced by off-plan work again | Sprint 7 — pair with an EditMode test per test-standards.md so "Done" can't be marked prematurely again |
| ADR-0002/ADR-0003 status flips | Sprint 4 | 3 | Simple 0.1d task, deprioritized every cycle | Sprint 7 — no excuse to carry a 4th time |
| S4-05/S4-06 keep-or-cut decision | Sprint 2/3 | 5 | Never resolved | Needs a forced decision at Sprint 7 kickoff, not another silent carry |
| First playtest | — | 7 cycles | Tied to death-chain landing, which *did* land this cycle — but build is now broken, so playtest still can't run | Sprint 7 — now genuinely unblocked once the 6 compile errors are fixed; should be the first Nice-to-Have attempted |
| QA plan | Sprint 3 | 5 | Repeatedly deferred | Needs an explicit owner commitment, not another flag-and-skip |
| Off-plan architecture work displacing scoped tasks | Sprint 4 | 3 (S4→S5→S6) | No process change made after Sprint 5's retro named this exact pattern | Sprint 7 — this needs to be a real kickoff conversation, not a bullet point that gets carried again |

---

### Previous Action Items Follow-Up (from `retro-sprint-05-2026-07-20.md`)

| # | Action Item | Status | Notes |
|---|--------------|--------|-------|
| 1 | Verify `PoolMember.cs` CS0592 in Unity Editor Console | Done (superseded) | S6-01 addressed the specific line; unrelated new CS0592 appeared this cycle in `CoreComponentBase.cs:5` — same defect class recurring in new code |
| 2 | Merge `origin/feature/spawn-enemy` into `sprint-05` | Done | Confirmed merged — `18f1978`/`588a709`/`ff7ab50`/`9bb216d` merge commits present in `sprint-06` history |
| 3 | Make BUG-05/07/08 the first Sprint 6 task, block new architecture until landed | Partially Done | Fixed correctly early in the week — but "block new architecture" was not honored; Pathfinding + Base refactor followed anyway |
| 4 | Fix `RoomModel.GetSpawnSet()` weight==0 hang | Presumed Done (S6-02) | Not independently re-verified this cycle's reviews; recommend explicit re-check next cycle |
| 5 | Route `NegativeReciver` through `PlayerData.currentHealth` | Not Done | Still a separate uninitialized field — same gap, third cycle now |
| 6 | `EnemyModal` SO-vs-plain-class decision | Presumed Done (S6-09 commit message claims it) | Not independently re-verified against `RoomModel.cs`'s actual current shape this cycle |
| 7 | Flip ADR-0002 to Accepted | Unknown | Not re-checked this cycle — carry the verification forward |
| 8 | Run `/qa-plan sprint` before further Track B/C work | Not Done | 5th consecutive cycle skipped |
| 9 | Investigate root cause of off-plan pattern at Sprint 6 kickoff | Not Done (or not visible in commit history) | The pattern repeated exactly a 3rd time; this is now the single most important unaddressed item across 2 retros |

**2 of 9 fully done, 2 partially, 1 presumed-but-unverified, 4 not done.** The one action item this
retro considers most load-bearing — investigating *why* off-plan work keeps displacing scoped work —
appears not to have happened again, and the consequence escalated this cycle from "wasted time" to
"broken build."

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | Open the project in Unity Editor and confirm/deny all 6 suspected compile errors (BUG-024 through BUG-029 in `bug-triage-2026-07-26.md`) — this blocks everything else | Owner (Kay) | Critical | Before Sprint 7 kickoff |
| 2 | Do not start any new architecture work until the Base/CoreBase hub refactor compiles AND `Core.GetCoreComponent<T>()` is verified working in Play Mode for both Player and Entity | ai-programmer / lead-programmer | Critical | Sprint 7, Day 1 |
| 3 | Fix BUG-ES-1 (spawn null-guard order) and BUG-06 write-through — both are small, both have now failed to land twice in a row despite being scheduled | gameplay-programmer | High | Sprint 7, before any spawn/damage testing |
| 4 | Hold the actual "why does off-plan work keep recurring" conversation at Sprint 7 kickoff that Sprint 5's retro asked for and didn't happen — this is the 3rd cycle naming the same root cause | Owner (Kay) / Producer | Critical | Sprint 7 kickoff conversation |
| 5 | Run `/qa-plan sprint` before Sprint 7's first story starts — 5th consecutive cycle skipped | qa-lead | High | Sprint 7, Day 1 |

---

### Process Improvements

- **A mid-sprint scope check-in** (e.g. Wednesday standup explicitly asks "has anything unplanned
  been started this week?") would catch this 3-cycle-running pattern before Friday instead of after.
- **New architecture work should require a one-line sprint-file amendment before starting**, even
  mid-week — not to block it (the Pathfinding module and Base/CoreBase unification are both
  reasonable ideas), but so scope tracking reflects reality and the next wrap-up isn't surprised.
- **A "verify it actually compiles" step before marking any refactor task Done** — this cycle's 6
  independent compile errors were all avoidable with a single Editor reload; recommend the owner do
  one mid-week Editor check as a habit, not just at wrap-up time.

---

### Summary

Sprint 6 is a mixed result that is hard to summarize with a single number: the sprint's actual
headline goal — closing the 7-cycle-carried enemy death chain (Bug #5/7/8) — **landed cleanly and is
confirmed by direct code read**, which is real, durable progress. But the same off-plan-architecture
pattern flagged as a confirmed, named risk in the last two retros recurred a third time, and this time
it did not just displace scoped work (Sprint 5's outcome) — it shipped 6 independent compile-blocking
errors plus a non-functional component-hub refactor into the branch. Two of the sprint's own scheduled
Must-Have bug fixes (BUG-ES-1 spawn guard, BUG-06 write-through) did not land for the second sprint
running. Verdict: **CONCERNS** — the death-chain win is genuine and should be recognized, but the
sprint cannot be called healthy while the branch likely does not compile. Sprint 7 should not start
new work until the Editor-confirmed compile status is known.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-07-26.md`
- Sprint plan: `production/sprints/sprint-06.md`
- Daily plan tracker: `production/sprints/sprint-06-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-05-2026-07-20.md`
- ADR-0002: `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
- ADR-0003: `docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md`
