# Sprint 14 — Daily Plan & Progress Tracker

> **Sprint**: 2026-09-14 (Mon) → 2026-09-18 (Fri)
> **Companion to**: `sprint-14.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-09-13 (Sunday 22:00 kickoff, on-slot) — autonomous scheduled run, no owner present.
> Branch `sprint-14` created from `sprint-13` tip (`c4be3c7`). Sprint 13 closed **FAIL** — 0/6 Must-Have
> met, 2/6 Should-Have landed (the two largest, not the smallest). 4 new S1/S2 bugs (BUG-067/068/069/070)
> shipped via unplanned off-plan work, caught only by the wrap-up's code review. See `sprint-13.md`
> closure block and `retro-sprint-13-2026-09-13.md` for full detail.

---

## Hard Gate (new this cycle — replaces the "schedule it first" note that failed 3 sprints running)

Per the Sprint 13 retro's explicit recommendation: this sprint is **not considered to have started
substantive work** until S14-01 through S14-06 exist as committed diffs. Every standup below checks
these six items, in this order, before reviewing anything else — including any unplanned work that
may have landed instead.

| # | Task | Est. |
|---|------|------|
| 1 | S14-01 (BUG-067 heal/damage swap) | 0.05d |
| 2 | S14-02 (BUG-068 unbound-slot NRE) | 0.05d |
| 3 | S14-03 (BUG-063 `[SerializeField]` removal) | 0.05d |
| 4 | S14-04 (BUG-064 item 7, `RangeWeapon.cs` DI wiring) | 0.1d |
| 5 | S14-05 (BUG-065, `PlayerDeathState` movement stop) | 0.1d |
| 6 | S14-06 (BUG-066 + BUG-070 dictionary guard) | 0.2d |

Combined: 0.55d — under one day's capacity.

---

## Day-by-Day Plan

### Mon 2026-09-14 — The six-item gate, first commit of the sprint, no exceptions

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S14-01 (BUG-067) | 0.05d | ⬜ NOT STARTED | First commit — player-facing regression, fix before anything else |
| S14-02 (BUG-068) | 0.05d | ⬜ NOT STARTED | Second commit |
| S14-03 (BUG-063) | 0.05d | ⬜ NOT STARTED | 26th+ carry |
| S14-04 (BUG-064 item 7) | 0.1d | ⬜ NOT STARTED | 3rd carry, pattern proven (`ItemSpawner.cs:8-10`) |
| S14-05 (BUG-065) | 0.1d | ⬜ NOT STARTED | 3rd carry |
| S14-06 (BUG-066 + BUG-070) | 0.2d | ⬜ NOT STARTED | 3rd carry on BUG-066, 1st on BUG-070 — fix together |

Goal: all six landed as committed diffs before Monday's session ends, regardless of what else is
in flight. This is the fourth consecutive sprint plan naming this exact failure mode — the gate
above exists because three prior "schedule it first" notes did not hold.

### Tue 2026-09-15 — Owner smoke session + process decisions

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| **S14-07 — Owner-in-Editor Play Mode smoke session** | 0.2d | ⬜ NOT STARTED | **Gate.** 6th consecutive sprint asking for this. Open `LoadRandomMap`, Console clean, kill one enemy, fire ranged weapon, press a skill |
| S14-08 (off-plan review gate decision) | 0.1d | ⬜ NOT STARTED | Producer decision — retro action item #4 |
| S14-09 (pre-push hook placeholder) | 0.15d | ⬜ NOT STARTED | 21st+ carry |

Goal: first-ever confirmed-running-build evidence this sprint, plus the process decision that's been
deferred twice.

### Wed 2026-09-16 — Owner sign-off batch

| Task | Est. | Notes |
|------|------|-------|
| S14-10 (S4-05/S4-06 forced decision) | 0.1d | 19th+ carry |
| S14-11 (ADR-0002 → Accepted) | 0.1d | 15th+ carry, trivial sign-off |

### Thu 2026-09-17 — Architecture decision + test + cooldown fix

| Task | Est. | Notes |
|------|------|-------|
| S14-12 (TD-040 ADR — Abilities v1/v2 convergence) | 0.3d | Widened scope of BUG-052 this cycle — a 4th sprint of v2 work must not ship with this still open |
| S14-13 (first EditMode test, vitals guard) | 0.3d | Pairs with S14-06 |
| S14-14 (BUG-069 cooldown wiring) | 0.15d | Every ability currently spammable |

### Fri 2026-09-18 — Stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S14-N2 (re-verify older bug set against the S14-07 smoke session) | 0.2d | Only if S14-07 landed Tuesday as planned |
| S14-N1 (first playtest) | — | Only if S14-07 confirmed stable — last log 2026-06-12 |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### Sun 2026-09-13 — Weekly Kickoff (autonomous, no owner present)

Sprint 13 closed FAIL (0/6 Must-Have met, 2/6 Should-Have landed) — already finalized in Saturday's
`pm-weekly-wrapup` run (2026-09-13), this kickoff only opened Sprint 14. Branch `sprint-14` created
cleanly from `sprint-13` tip (`c4be3c7`) — no stale local branch conflict this cycle (unlike Sprint 13's
kickoff). Re-verified the carried-forward state directly against `retro-sprint-13-2026-09-13.md` and
`production/qa/bug-triage-2026-09-13.md` (both written same-day by the wrap-up run) rather than
re-deriving from source:

- 🆕 **BUG-067** — `ResourceReceiver` heal/damage swap, live regression, found in wrap-up code review.
  Fix first. S14-01.
- 🆕 **BUG-068** — `AbilityHolder.GetAbility()` unguarded null deref, NRE on unbound ability slot, found
  in wrap-up code review. Fix second. S14-02.
- ❌ **BUG-063** — `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR` / `[SerializeField]`.
  26th+ consecutive carry. S14-03.
- ❌ **BUG-064 item 7** — `RangeWeapon.cs:7` still has no `[Inject]` wiring. 3rd carry. S14-04.
- ❌ **BUG-065** — `PlayerDeathState.Enter()` still only calls `base.Enter()`. 3rd carry. S14-05.
- ❌ **BUG-066** + 🆕 **BUG-070** — both `EntityVitalStats` and the new `VitalStatsComponent` twin index
  a dictionary with no guard. Bundle into one fix. S14-06.
- ✅ **S13-11 (batch bug-file generation) — resolved at this kickoff.** All 9 open bugs now have
  individual files (10 on disk incl. closed BUG-053). No longer carried.
- ❌ **S13-06 → S14-09** (pre-push hook) — still no `.git/hooks/pre-push`. 21st+ carry.
- ❌ **S13-08 → S14-10** (S4-05/S4-06 decision) — 18th+ carry, oldest unresolved item in the project.
- ❌ **S13-09 → S14-11** (ADR-0002) — still `Proposed`. 14th+ carry.
- 🆕 **S14-12** — TD-040 (Abilities v1/v2 convergence ADR) — `BUG-052`'s scope widened this cycle; needed
  before a 4th sprint of Abilities v2 work ships with the decision still open.
- 🆕 **S14-08** — producer decision on whether off-plan feature work needs a same-day review gate —
  retro action item #4, deferred from Sprint 13.

`gh` CLI still unavailable — draft PR not auto-created, manual command left in `sprint-14.md`. No QA
plan exists for the 26th+ consecutive cycle — flagged, deferred to owner per every prior cycle's
handling. No owner-in-Editor Play Mode session has occurred at any point across Sprint 8 through
Sprint 13 (6 sprints) — carried forward as S14-07, named explicitly. Per the Sprint 13 retro's explicit
recommendation, **a hard gate replaces the schedule-note mitigation this cycle** (see "Hard Gate"
section above) — three consecutive "schedule the cheap fix first" notes did not hold, so this kickoff
is trying a mechanical check instead of a fourth note.

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-067 / BUG-068 — both S1, both new, both live and player-reachable.** Fix before anything else
  — both threaten the Play Mode smoke session (S14-07) directly.
- **BUG-063 (`Stat.cs` `[SerializeField]` regression)** — 26th+ consecutive carry on a one-line fix with
  an explanatory comment already in the file. No technical blocker has ever existed for this item.
- **BUG-064 item 7 — `RangeWeapon.cs` DI wiring.** 3rd carry. Fix pattern already proven same-repo
  (`ItemSpawner.cs:8-10`).
- **BUG-065 / BUG-066 / BUG-070** — small isolated fixes with no dependencies, unchanged/new since
  introduced.
- **Owner-in-Editor Play Mode session (S14-07)** — has not happened once across Sprint 8 through
  Sprint 13 (6 sprints). Per the retro: if it does not happen this cycle either, Sprint 15 should stop
  scheduling it as a task and instead record it as an explicit accepted-risk decision.
- **S14-08 (off-plan review gate decision)** — retro action item #4, new this cycle.
- **S14-10 (S4-05/S4-06)** — 18th+ carry, zero movement any cycle. Decision-avoidance, not an
  estimation problem.
- **S14-11 (ADR-0002 Accept)** — 14th+ carry, trivial sign-off-only change.
- **S14-12 (TD-040 — Abilities v1/v2 convergence)** — new this cycle, widened from BUG-052.
- **S14-09 process gate** — now 21st carry, same underlying pattern since Sprint 6/9.
- **BUG-069 (ability cooldown never enforced)** — new this cycle, every ability currently spammable.
- QA plan — 26th+ consecutive cycle with none. Flagged in `sprint-14.md`, deferred to owner.
