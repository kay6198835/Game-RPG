# Retrospective: Sprint 4

Period: 2026-07-07 -- 2026-07-11 (this retro generated 2026-07-10, sprint Day 4 of 5 — the scheduled Saturday 22:00 wrap-up ran one day ahead of the sprint's own designated Friday wrap day; Friday 07-11's work is NOT yet reflected here)
Generated: 2026-07-10 (automated `/weekly-wrapup`)

---

### Metrics

| Metric | Planned | Actual (through 07-10) | Delta |
|--------|---------|-------------------------|-------|
| Must-Have tasks (S4-01→S4-04) | 4 | 4 done | 0 — **first 100% Must-Have completion in 9 sprints** |
| Should-Have tasks (S4-05, S4-06) | 2 | 0 done (both PENDED → Sprint 5, Day 2 owner decision) | -2 (deliberate descope, not slippage) |
| Design tasks (S4-D1→S4-D4) | 4 | 4 done | 0 — GDD approved, ADR-0001 + ADR-0002 written, epic created |
| Off-plan `.cs` files changed | 0 (Day-2 pivot said "no code") | 27 files touched this week, ~9 of them enemy-spawn prototype code landed after the pivot | Significant — see "What Went Poorly" |
| Bugs found (this triage) | -- | 5 new (1 P1, 4 P2) | -- |
| Bugs fixed | -- | 4 P1 closed (BUG-04, BUG-09, BUG-AH-1, BUG-CORE-1) | -- |
| Playtest sessions | 1 (target: Day 1-2 once P1s land) | 0 | -1 — blocker removed 07-07, still not run |
| Commits in window | -- | 31 | -- |

---

### Velocity Trend

| Sprint | Must-Have Planned | Must-Have Completed | Rate |
|--------|--------------------|-----------------------|------|
| Sprint 2 | 7 | 1 pending merge | ~14% |
| Sprint 3 | 4 (carried) | 0 | 0% |
| Sprint 4 (through 07-10) | 4 | 4 | **100%** |

**Trend**: Sharp reversal — improving. First sprint since tracking began where the carried P1 block actually closed. The mechanism that broke the streak was explicit, not accidental: Day-2 standup found the fixes already existed on an unmerged parallel branch (`origin/feature/enhance-stats-system`) and the day's whole task was merging them in, not writing new code. Worth naming this as the actual lesson — see Action Items.

---

### What Went Well

- **9-sprint P1 backlog fully closed.** BUG-04 (melee damage), BUG-09 (animation end-event), BUG-AH-1 (build-breaking import), BUG-CORE-1 (LINQ allocation) — all four verified fixed by direct code read, not just commit messages. Combat has a working damage chain for the first time this project has been tracked.
- **Design track finished clean and on schedule.** GDD (`enemy-spawn-system.md`), both ADRs (0001 StatSystem, 0002 EnemyManager singleton), and the epic file all landed within the 2.25-day estimate, all 6 open design questions resolved.
- **Standup discipline caught real problems same-day.** The Day-2 standup found the P1 fixes stranded on the wrong branch (`git merge-tree` confirmed a clean fast-forward) and got them merged same day. The Day-4 standup independently flagged a plausible `.ConvertTo<T>()` compile-error risk and a `[SerializeField]`-on-auto-property pattern — both later corroborated by this wrap-up's code review.
- **Enemy Spawn System now has an executable path**, however imperfect: `RoomGeneraterController` → `ON_GET_SPAWN_POSITIONS` → `EnemySpawner` → `RoomModel.GetSpawnSet()` → `Instantiate`. Wasn't planned for this sprint, but it's real, wired, working data flow.

---

### What Went Poorly

- **The "design only" pivot was broken the same night it was made.** Day-2 standup (07-08) explicitly decided "no code this sprint" for enemy-spawn. Commits `a420d5e`/`9f1d96b` — new SO classes, prefabs, `LevelManager.cs` edits — landed ~4 hours later, 00:20-00:23 on 07-09. The pattern then continued three more days (`dde19e9`, `f398e9a` on 07-09; more edits 07-10) — this is the 9th consecutive sprint where off-plan work is flagged as a pattern, but this iteration is sharper: the decision was explicit and violated within hours, not a vague drift.
- **Off-plan enemy-spawn code now diverges from its own GDD.** `RoomModel.GetHybridEnemySet()`'s Phase-2 pick uses `Random.Range` instead of the GDD-locked `argmin(|weight-remaining|)` tie-break, and takes no seed parameter despite the GDD's Open Question #5 asking for one. Writing the design carefully and then not following it while coding it anyway gets the worst of both: the design effort doesn't constrain the implementation it was meant to.
- **A live NullReferenceException risk shipped into the new spawn code**: `RoomModel.GetSpawnSet()` returns `null` on an empty enemy list; neither caller (`EnemySpawner` nor the duplicate `LevelManager.SpawnRoomEnemies()`) guards against it. Two independent reviews (this wrap-up's manual check and the background code-review agent) found it separately — it's real, not a false positive.
- **Playtest still didn't happen.** Last retro named the exact blocker (BUG-04/BUG-09) and predicted a playtest "Day 1-2 once P1s land." P1s landed Day 1. No playtest has run in over a month (`playtest-2026-06-12-weekly-wrapup.md` is still the newest file). The blocker excuse is gone; the session simply hasn't happened.
- **A second, parallel spawn driver appeared** (`LevelManager.SpawnRoomEnemies()` duplicating `EnemySpawner.SpawnRoomEnemies()` almost verbatim) — neither routes through `EnemyManager`, which is the entire point of ADR-0002. The architecture the design track spent 2+ days locking down isn't being followed by the code landing in parallel with it.

---

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| P1 fixes existed but were stranded on an unmerged branch | ~1 day (discovered Day 2, merged same day `204be85`) | Direct `git merge-tree` check confirmed clean fast-forward; merged immediately | Standup should check `git branch -a` / unmerged remotes routinely, not just the working branch — this cost a full day of apparent "zero progress" that was actually already done elsewhere |
| Enemy-death chain (Bugs #5/#7/#8) still blocks room-clear | Ongoing, deferred since Sprint 1 | Not resolved — explicitly deferred to Sprint 5/6 per design-track scoping | Tracked in epic; no new mitigation this sprint |

---

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S4-01→S4-04 (Must-Have block) | 1.0d combined | ~0d net new work (already done on parallel branch) + 0.25d merge/reverify | Under by ~0.75d | Work had already happened off-branch; estimate was accurate for the coding itself, just not visible until the merge |
| S4-D1→S4-D4 (design track) | 2.25d | ~2.25d (Day 2-3, some retroactively confirmed already done) | On target | Well-scoped from the start |
| Off-plan enemy-spawn prototype | 0d (not planned) | ~3-4d equivalent across 4 commits | N/A — unplanned | Recurring pattern; see Action Items |

**Overall estimation accuracy**: Planned items landed close to estimate. The gap this sprint isn't estimation — it's scope discipline: unplanned work is consistently larger than planned work.

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| S4-05 — Fix BUG-PIH-1 (`CancelInvoke`) | Sprint 2 | 3 (S2→S3→S4→S5) | Deliberately pended, not blocked — Day-2 pivot chose design track over this | Carry to Sprint 5, small (0.25d), should be first item picked up |
| S4-06 — `TalentManager` → SO-driven | Sprint 3 | 2 (S3→S4→S5) | Same pivot decision | Carry to Sprint 5 |
| ADR: Skill Enhance vs `ActivateSkill` pipeline | Sprint 3 | 2 (S3→S4→S5) | Never started — no commit or doc found addressing this decision this sprint | Carry to Sprint 5; flag as aging since `Skill Enhance` code has been sitting uncommitted/off-plan since before Sprint 3 |
| Bug #6 (player death), Bugs #5/#7/#8 (enemy death chain) | Sprint 1 | 4 (S1→S2→S3→S4→S5) | Explicitly deferred pending design-track completion, now unblocked design-wise | Highest-priority carry for Sprint 5 — both `EnemyManager` lifecycle and player death chain depend on it |
| BUG-ES-1, BUG-ES-2, BUG-ES-3 (new, enemy-spawn architecture drift) | Sprint 4 (new) | 0 | Found this triage | Should not carry past Sprint 5 — small fixes, high value (unblocks Sprint 6) |

---

### Technical Debt Status

- Current TODO/FIXME/HACK count in `Assets/Script/`: 0 exact-keyword hits (repo doesn't use those markers; debt is tracked via CLAUDE.md's "Known Bugs" table and bug-triage reports instead)
- Trend: **stable in marker terms, growing in tracked-bug terms** — 18 → 20 open bugs this triage (net +2), driven entirely by the new Enemy Spawn subsystem (5 of 20 bugs), which is expected for week-one code on a new system, not a quality regression on existing code
- Area of concern: `StatsSO`'s mutable dictionary (BUG-SS-2, prior triage) is **partially mitigated, not resolved** — `OnEnable() => initialized = false` now rebuilds the lookup index on SO load, but the underlying serialized `Stat`/modifier data on the shared SO asset is still not instanced per character. Worth a follow-up check before StatSystem sees more runtime use.

---

### Previous Action Items Follow-Up

| Action Item (from Sprint 3 retro) | Status | Notes |
|-------------------------------|--------|-------|
| S4-01 Fix BUG-AH-1 | Done | Verified 07-07, re-verified post-merge 07-08 |
| S4-02 Fix BUG-09 | Done | Verified 07-07, re-verified post-merge 07-08 |
| S4-03 Fix BUG-CORE-1 | Done | Verified 07-07, re-verified post-merge 07-08 |
| S4-04 Fix BUG-04 | Done | Verified 07-07, re-verified post-merge 07-08 |
| S4-05 Fix BUG-PIH-1 | Not Started | Pended by Day-2 pivot decision — carries to Sprint 5 |
| S4-06 Fix BUG-SS-2 | Partially Done | `OnEnable` reset added, but shared-SO mutable-state root cause not fully addressed |
| ADR: Skill Enhance vs ActivateSkill | Not Started | No commit or doc found this sprint; still open |
| Remove/wire `TalentManager` | Not Started | Still hardcodes stats in `Awake()`, confirmed by 07-08 standup |
| First playtest | Not Started | Blocker (BUG-04/BUG-09) resolved 07-07; no session logged since |

**4 of 9 action items done, 1 partial, 4 not started** — the 4 done are exactly the 4 Must-Have P1s; everything else (playtest, ADR, TalentManager, S4-05) lost out to the same-week pivot to design work, then to off-plan enemy-spawn coding.

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | Fix BUG-ES-1 (`RoomModel.GetSpawnSet()` null return + unguarded caller) before any further enemy-spawn integration work | gameplay-programmer | High | Sprint 5, Day 1 |
| 2 | Explicit owner decision: fold the existing enemy-spawn prototype code into formal Sprint 5 scope (and align `GetHybridEnemySet()` to the GDD's locked `argmin` + seed spec), or relocate it under `prototypes/` per `prototype-code.md` — stop letting it sit in an undecided state | Owner (Kay) | High | Sprint 5 kickoff |
| 3 | Run the first playtest session since 2026-06-12 — the blocker that justified skipping it is gone | Owner (Kay) | Medium | Sprint 5, before Day 3 |
| 4 | Write the Skill Enhance vs `ActivateSkill` ADR — it has carried 2 sprints unstarted and blocks a decision on a chunk of already-written code | lead-programmer | Medium | Sprint 5 |
| 5 | Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` to `EventID` and reconcile the two parallel spawn drivers (`EnemySpawner` vs `LevelManager.SpawnRoomEnemies()`) against ADR-0002 | ai-programmer / lead-programmer | High | Sprint 5-6 boundary |

---

### Process Improvements

- **When a pivot decision is made ("design only", "no code this sprint"), say so in the commit or PR description of any code that follows, or don't write it that session.** The Day-2 pivot was clear in the sprint doc but invisible at the point someone sat down to code 4 hours later — a lightweight guard (a comment at the top of the daily standup, or just re-reading the day's own plan before opening the editor) would have caught it same-night instead of 3 days later.
- **Standup should check unmerged remote branches for already-completed work before re-estimating "not started" tasks.** This sprint lost a full day's clarity because P1 fixes existed on `origin/feature/enhance-stats-system` and nobody looked there until Day 2's standup happened to check `git merge-tree`.
- **Playtest needs its own trigger independent of "blockers cleared."** Two sprints running the blocker has been resolved and the playtest still didn't happen — treat it as a scheduled task, not a "whenever it's convenient" follow-up.

---

### Summary

Sprint 4 is the best sprint on record by its primary metric — the 9-sprint-old P1 backlog is fully closed and verified, and the design track for Enemy Spawn finished clean and on time. But the same pattern that has shown up in every retro so far (off-plan work outrunning planned work) didn't go away when the Must-Haves got done — it just moved to a new target, and this time it broke an explicit same-day decision rather than a vague prioritization drift. The single most important change for Sprint 5 is turning the enemy-spawn prototype from an ambiguous, undecided pile of code into either committed scope (aligned to its own GDD) or an isolated prototype — right now it is neither, and it is quietly drifting from the architecture (ADR-0002) the team just spent two days agreeing on.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-07-10.md`
- Daily plan tracker: `production/sprints/sprint-04-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-03-2026-07-05.md`
- GDD: `design/gdd/enemy-spawn-system.md`
- ADRs: `docs/architecture/adr-0001-statsystem-dual-data-structure.md`, `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
- Epic: `production/epics/enemy-spawn/EPIC.md`
