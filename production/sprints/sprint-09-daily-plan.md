# Sprint 9 — Daily Plan & Progress Tracker

> **Sprint**: 2026-08-10 (Mon) → 2026-08-14 (Fri)
> **Companion to**: `sprint-09.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-08-10 (overrun Sunday 22:00 kickoff, ran Monday) — autonomous scheduled run, no user
> present. Branch `sprint-09` created from `sprint-08` tip (`a29895b`). Sprint 8 closed CONCERNS with
> 0/8 Must-Have items landed — see `sprint-08.md` closure section for full detail.

---

## Status Verdict: 🟡 DAY 1 / OPENING (2026-08-10) — fresh branch, no work started yet. Scope deliberately cut to 1.0d Must-Have (vs. Sprint 8's 1.75d, of which 0d landed) to test whether a narrower cut succeeds where three consecutive lighter-scoped recovery attempts (Sprints 7, 8, and implicitly 6) did not.

---

## Day-by-Day Plan

### Mon 2026-08-10 — Process gate + combat fixes batch 1

| Task | Est. | Notes |
|------|------|-------|
| S9-00 (process gate artifact) | 0.1d | Draft only — hook config or written rule; owner sign-off still needed to formally adopt, but drafting doesn't require synchronous presence, unlike the retired S8-00 conversation |
| S9-01 (BUG-041, player attack unwired) | 0.2d | P0 — 2nd carry, unchanged from Sprint 8's own Day 1 plan since zero progress was made |
| S9-02 (BUG-042 + BUG-053, enemy TakeDamage + duplicate receiver) | 0.3d | P0 — now explicitly scoped as one reconciliation (delete `EntityNegativeReciver.cs`), not two separate fixes |
| S9-06 (BUG-032, one-line fix) | 0.1d | Trivial, 3rd carry |
| S9-07 (BUG-033, one-line fix) | 0.1d | Trivial, 6th carry |

Goal: land the two P0 bugs plus both trivial one-liners on Day 1 — Sprint 8 never got any of these
past "planned," so this is the sprint's real first test.

### Tue 2026-08-11 — Verification + Should-Have start

| Task | Est. | Notes |
|------|------|-------|
| S9-12 (Play Mode verify, both attack directions) | 0.2d | **Gate** — owner confirms in-Editor; do not treat Mon's items as done without this |
| S9-04 (BUG-044, PlayerDeathState orphaned) | 0.15d | Should-Have, independent |
| S9-05 (Bug #6, 9th carry) start | partial of 0.4d | Should-Have this cycle, not Must-Have — start if Mon's Must-Have landed clean |

Goal: S9-12 actually confirmed (unlike S7-08/S8-12, never confirmed in either prior sprint).

### Wed 2026-08-12 — Bug #6 completion + BUG-043

| Task | Est. | Notes |
|------|------|-------|
| S9-05 (Bug #6, 9th carry) finish | remainder of 0.4d | Do not close without the EditMode test |
| S9-03 (BUG-043 consolidation) | 0.3d | Depends on S9-02 landing first |

Goal: single HP source of truth confirmed via passing EditMode test; enemy attack path consolidated.

### Thu 2026-08-13 — Decisions + remaining Should-Have

| Task | Est. | Notes |
|------|------|-------|
| S9-10 (ADR-0002 Accepted) | 0.1d | 5th carry, trivial |
| S9-11 (S4-05/S4-06 forced decision) | 0.1d | 9th carry — make the call |
| S9-08 (Bug #14, missing `return`) | 0.1d | Should-Have, quick |
| S9-D1 (individual BUG-NNN.md files) | 0.2d | Should-Have |
| Buffer / catch-up | — | Reserved for Must-Have slippage |

### Fri 2026-08-14 — Nice-to-Have stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S9-09 (Bug #15, build-safe JSON load) | 0.5d | If Must-Have closed clean |
| S9-N1 (first playtest) | — | Only if S9-01/02/12 all confirmed stable — 10th cycle attempt |
| S9-N2 (`/doc-sync`, stale filenames in CLAUDE.md) | 0.3d | If time remains |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

*(No standups logged yet — sprint opened 2026-08-10, same day as this kickoff.)*

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-041/BUG-042/BUG-053 — P0/S1, 2nd carry now, combat non-functional in both directions.** Zero
  progress across all of Sprint 8's 5 scheduled days despite being the sprint's explicitly stated
  single goal. If Day 1 of Sprint 9 also shows zero movement, escalate: the "recovery sprint" framing
  itself may need to change (e.g., a single dedicated pairing session rather than distributed daily
  autonomous check-ins).
- **S9-00 process gate** — replaces the S8-00 conversation (0/6 held across 3 sprints). Verify it
  actually gets adopted (hook committed or rule written into a project doc), not just proposed again.
- Bug #6 — 9th carry cycle, regressed twice historically, deliberately Should-Have (not Must-Have)
  this cycle to avoid Sprint 8's overcommitment pattern.
- S9-11 (S4-05/S4-06) — 9th carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem — recommend the owner just make the call.
- QA plan — 8 consecutive cycles with none. Flagged in `sprint-09.md`, deferred to owner.
