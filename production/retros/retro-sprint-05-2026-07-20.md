# Retrospective: Sprint 5 Close-Out

Period: 2026-07-13 -- 2026-07-20 (extended window; formal Fri 07/17 wrap-up did not run on time —
this is the delayed Saturday-slot catch-up, executed 2026-07-20)
Generated: 2026-07-20 (automated `/weekly-sprint wrapup --auto`, scheduled Saturday 22:00 slot)

> **Scope note**: Sprint 5's own daily tracker never reached a Friday close entry — the last log line
> is Thu 07/16's day-4 standup. This retro treats Thu 07/16 as the last verified checkpoint and adds
> what changed between then and now (2 commits on `origin/feature/spawn-enemy`, not yet merged into
> `sprint-05`).

---

### Metrics

| Metric | Value |
|--------|-------|
| Sprint window | 07/13 → 07/17 (extended from original 07/14→07/18, see sprint-05.md reopen note) |
| Days elapsed (execution) | 4 of 4 planned + 3 unplanned extra days of drift before this wrap-up ran |
| `.cs` files changed (`sprint-05` branch, since last wrap-up `7ca465f`) | 39 files, 347 insertions / 333 deletions across 11 commits |
| Must-Have work committed | 1.35d of ~3.95d planned (34%) |
| P1 bugs closed this sprint | 1 of 6 (BUG-06, partially — see below) |
| P1 bugs still open | 6 (3 carried 7 cycles: BUG-05/07/08; 2 carried 2-3 cycles: BUG-ES-1/ES-4; 1 new: possible build break in `PoolMember.cs`) |
| New bugs found (code review, this triage) | 9 |
| Playtest sessions | 0 (still — 6th consecutive retro with zero movement, last session 2026-06-12) |
| Unmerged commits outside `sprint-05` | 2 (`dce9be1`, `d653654` on `origin/feature/spawn-enemy`, both 07-16) |

---

### Previous Action Items Follow-Up (from `retro-interim-2026-07-11.md`)

| # | Action Item | Status | Notes |
|---|--------------|--------|-------|
| 1 | Commit/stash carried WIP before Sprint 5 Day 1 | Done | `cb099ee` + stash landed as S5-D1 ✅ before Mon work began |
| 2 | Land BUG-ES-1 and BUG-ES-4 together in S5-C1 | Not Done | Both still open, confirmed by direct code read 2026-07-20 — S5-C1/C4 never started |
| 3 | Decide on BUG-ES-6 (padding bounds clamp) | Unknown | Not re-verified this cycle — carry the check forward |
| 4 | Schedule the playtest session as a fixed slot | Not Done | Still 0 sessions since 06-12; this is now the 6th retro in a row flagging it |
| 5 | Assign an owner to the Skill Enhance vs `ActivateSkill` ADR | Not Done | No commit/doc found this window either |

**1 of 5 done.** The one win (WIP committed cleanly) held; everything else repeated its exact status
from the last retro, unchanged.

---

### What Went Well

- **BUG-06 (player death chain) finally landed** after carrying since Sprint 1 (6 prior cycles) —
  `NegativeReciver.TakeDamage()` no longer throws, decrements health, and emits `ON_PLAYER_DEATH`.
  Confirmed by direct code read, not log inference. (Caveat below — it's not fully correct.)
- **The `Base/` framework unification (`BaseEntity`, `CoreBase`, `CoreComponentBase`, `IState`,
  `StateMachine<T>`, `DirectionResolver`) is a genuine, clean deduplication win**, independently
  confirmed by 2 of 3 review agents: it removed real duplicated state-machine and 8-direction-angle
  code between `Player` and `Entity`, and correctly preserved the `EntityState` contract with no
  states accidentally regressing to `MonoBehaviour`.
- **`EventID` extension done correctly** — `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` added
  as pure enum values, matching `manager-event-code.md`'s rule (extend the enum, not new static
  `Action` fields).
- **`ObjectPoolManager` avoided a new singleton** — wired via `RequireComponent` + `GetComponent`,
  correctly not adding a 3rd exception beyond `MazeController`/`EnemyManager` (ADR-0002).

---

### What Went Poorly

- **4 consecutive days of off-plan work, a pattern now confirmed to recur, not a one-off.** Every
  single day this sprint (Mon architecture pivot, Tue/Wed framework unification, Thu pooling system)
  produced substantial unplanned code instead of the affirmed day's plan. Net result: only 1.35d of
  3.95d committed Must-Have work landed. The daily tracker itself calls this out explicitly at every
  checkpoint — this retro isn't discovering it, just confirming it held through to the end.
- **The trio of oldest S1 bugs (BUG-05/07/08 — the actual "combat death loop" the sprint was framed
  around) carried a 7th cycle with zero code movement**, despite this sprint's off-plan work
  repeatedly touching the exact files that own them (`EntityMoveState.cs`, `EntityBasicState.cs`,
  `EntityDeathState.cs` were all in-scope for the `Base/` refactor's Entity-side changes, yet none of
  the 3 bugs were fixed in passing).
- **BUG-06's fix is incomplete in a way that wasn't caught until this wrap-up's review**:
  `NegativeReciver` keeps its own `currentHealth` field instead of writing through to
  `PlayerData.currentHealth` — the canonical SO field per `scriptableobject-data.md`'s "Single Source
  of Truth" rule. `PlayerData.Reborn()` can't reset this, so a run-restart likely leaves stale HP.
  Marking BUG-06 fully "Done" in the tracker was premature.
- **A new S1-class bug was introduced this week and shipped unreviewed**: `RoomModel.GetSpawnSet()`'s
  Phase-1 loop can hang indefinitely if any enemy's `weight == 0`, because the `[Range]` clamp that
  used to guard this was dropped when `EnemyModal` was rewritten from an SO asset into an inline
  plain class. Two independent review agents found this from a cold read of the diff.
- **A possible build-break was found**: `PoolMember.cs:9` applies `[SerializeField]` directly to an
  auto-implemented property, which is CS0592 under standard C# rules — flagged by the unity-specialist
  review agent and confirmed by direct read of the file. Not yet verified against an actual Unity
  compile (no Unity CLI available in this environment, same constraint noted all sprint). This needs
  first-priority verification in the Editor before Sprint 6 planning proceeds.
- **`EnemyModal` regressed from a reusable SO asset to a plain per-room class**, undoing part of the
  data model ADR-0003 just ratified for it — and ADR-0003's own file still reads `Status: Proposed`
  even though the tracker marks its ratification task (S5-A2) done, a doc/tracker mismatch.
- **Branch scatter returned**: `origin/feature/spawn-enemy` sits 2 commits ahead of `sprint-05` again
  (`dce9be1`, `d653654`, both 07-16) — the same risk the Wed 07/15 standup already flagged and
  believed resolved by Thu.
- **Playtest gap is now 6 consecutive retros with zero movement** (since 2026-06-12). The combat
  death loop that would make a playtest meaningful is also the thing that hasn't landed, so this is
  coupled to the BUG-05/07/08 carry, not an independent scheduling failure alone.
- **QA plan still doesn't exist** — 4th consecutive cycle this gate is noted and skipped.

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-05/07/08 (enemy death chain) | Sprint 1 | 7 (S1→S2→S3→S4→S5, 3 cycles within S5) | Sprint goal's own "Track B" repeatedly deprioritized in favor of off-plan work | Sprint 6 — make this the literal first task, no parallel scope allowed |
| BUG-ES-1/ES-4 (spawn null/index guards) | Sprint 4 | 3 (2nd/3rd cycle within S5) | Small, unblocked, still not picked up | Sprint 6 — pair with BUG-ES-1 fix as originally planned |
| NEW-1 (`GetSpawnSet` weight==0 hang) | Sprint 5 (new) | 0 | Introduced this sprint by the `EnemyModal` rewrite, found only by this wrap-up's review | Sprint 6, high priority — the class of bug that already existed (see `weight` clamp history) reappeared |
| Possible `PoolMember.cs` build break | Sprint 5 (new) | 0 | Introduced this sprint, found only by this wrap-up's review | Verify in Editor immediately, before any Sprint 6 planning |
| ADR-0002 Proposed→Accepted | Sprint 4 | 2 | Simple status flip, deprioritized every cycle | Sprint 6, S5-D2-equivalent — 0.1d, no excuse to carry a 3rd time |
| S4-05 (`CancelInvoke` pairing), S4-06 (`TalentManager`→SO) | Sprint 2/3 | 4 | Deliberately pended repeatedly | Sprint 6 — 4th carry, needs an explicit keep-or-cut decision, not another silent carry |
| Skill Enhance vs `ActivateSkill` ADR | Sprint 3 | 3 | Still no commit/doc found | Needs a named owner before it carries a 4th time |
| First playtest | — | 6 retros running | Combat death loop (the thing worth playtesting) hasn't landed | Tie explicitly to BUG-05/07/08 landing in Sprint 6 — don't schedule independently again |
| `EnemyModal` SO-vs-plain-class decision | Sprint 5 (new) | 0 | Regressed silently this sprint, unresolved | Needs an explicit owner decision before more rooms are authored around either shape |
| `origin/feature/spawn-enemy` ahead of `sprint-05` | Sprint 5 (recurring) | 2nd occurrence this sprint alone | No branch-sync step in the daily routine | Merge before Sunday kickoff; consider adding a branch-sync check to `/daily-standup` |

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | Verify the `PoolMember.cs` CS0592 finding in the Unity Editor Console — confirm build is/isn't broken | Owner (Kay) | Critical | Before Sprint 6 kickoff |
| 2 | Merge `origin/feature/spawn-enemy` (`dce9be1`/`d653654`) into `sprint-05` | Owner (Kay) | High | Before Sunday kickoff |
| 3 | Sprint 6: make BUG-05/07/08 (entity death chain) the first task, explicitly block new architecture/framework work until it lands | Producer / ai-programmer | Critical | Sprint 6, Day 1 |
| 4 | Fix `RoomModel.GetSpawnSet()`'s `weight == 0` hang risk (restore `[Range(1,N)]` clamp or add iteration cap) | gameplay-programmer | Critical | Sprint 6, before any spawn testing |
| 5 | Re-open BUG-06 as "partial" — route `NegativeReciver` through `PlayerData.currentHealth` instead of a local field | gameplay-programmer | High | Sprint 6 |
| 6 | Get an explicit owner decision on `EnemyModal`: SO asset (reusable) vs plain class (per-room) — ADR-0003 addendum if needed | Owner (Kay) / systems-designer | High | Sprint 6, before Track A resumes |
| 7 | Flip ADR-0002 to Accepted (or explicitly reject) — 0.1d task carried 2 cycles now | Producer | Medium | Sprint 6 |
| 8 | Run `/qa-plan sprint` before any further Track B/C work — 4th cycle skipped | qa-lead | High | Sprint 6, before Track B resumes |
| 9 | Investigate root cause of 4-straight-day off-plan pattern directly, per Sprint 5's own risk table ("if this window also closes near 0%, the next kickoff should investigate why before re-carrying a 3rd time") | Owner (Kay) / Producer | High | Sprint 6 kickoff conversation |

---

### Process Improvements

- **The sprint's own risk table predicted this outcome and named the exact mitigation** ("investigate
  why before re-carrying a 3rd time") — that investigation should happen as a real conversation at
  Sunday kickoff, not another silent re-carry. Four straight days of high-quality-but-unplanned
  architecture work is not a discipline failure in the code (all 3 review agents rated the new
  framework code well); it's a planning/prioritization failure that no amount of code review catches.
- **Wire a lightweight branch-sync check into `/daily-standup`** — this is the second time this sprint
  alone that `sprint-05` and `origin/feature/spawn-enemy` diverged without anyone flagging it until a
  wrap-up or standup did a manual `git log` comparison.
- **Bugs marked "Done" in the tracker should get a one-line verification note pointing at the exact
  contract they were supposed to satisfy** (e.g. BUG-06's fix should have been checked against "does
  it write through `PlayerData`," not just "does it stop throwing") — this would have caught the
  partial-fix gap before it was marked complete.

---

### Summary

Sprint 5 closes at 34% of its committed Must-Have work after a 7-day-extended window, for the same
root cause named in every checkpoint since Monday: high-quality but unplanned architecture and
tooling work (framework unification, pooling system) repeatedly displaced the tracked death-loop and
spawn-stabilization tasks. One real win landed (BUG-06, though incompletely), but the sprint's actual
goal — a stable enemy death chain enabling the first playtest — carries a 7th cycle untouched, and
this week's off-plan work introduced two new high-severity risks (a possible build break, a new
infinite-loop hazard) on top of it. Verdict: **CONCERNS**, bordering on **FAIL** against the sprint's
own stated goal — recommend Sprint 6 open with an explicit scope-discipline conversation before any
task assignment, not just a re-carry of the same list.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-07-20.md`
- Sprint plan: `production/sprints/sprint-05.md`
- Daily plan tracker: `production/sprints/sprint-05-daily-plan.md`
- Prior retro: `production/retros/retro-interim-2026-07-11.md`
- GDD: `design/gdd/enemy-spawn-system.md`
- ADR-0002: `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
- ADR-0003: `docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md`
