# Sprint 2 — Daily Plan & Progress Tracker

> **Sprint**: 2026-06-15 (Mon) → 2026-06-19 (Fri)
> **Companion to**: `sprint-02.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Daily routine**: 10:00 every day → run `/daily-standup` — summarizes/analyzes
>   yesterday from git + this tracker, updates statuses below, and lists today's
>   tasks (with estimates) from the day-by-day breakdown. Wire it as a 10:00
>   daily routine in the Claude Code web app.
> **Last updated**: 2026-06-17 (Wed)

---

## Status Verdict: 🔴 AT-RISK

**Why**: 2 of 4 capacity days elapsed (Mon/Tue), **no committed progress** on S2-01→S2-06 (only `start sprint 2` on 15/06). 3.5 days of work remain against 3 working days left (Wed–Fri). This is the 5th consecutive week the combat-loop work has slipped — flagged in `sprint-02.md` risks.

**Recommended action**: protect the *playtestable* outcome. Land S2-05 (melee damage) early so combat is testable even if the refactor slips. Time-box S2-03 (OCP refactor) — cut it first if Thu slips, per the sprint's own mitigation.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 3.5 days |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 2 (Mon, Tue) |
| Days remaining | 3 (Wed, Thu, Fri) |
| Work committed/done | 0.0 days |
| Work remaining | 3.5 days |
| Slack | −0.5 day (tight) |

---

## Task Estimates (from sprint-02.md)

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S2-01 | Stabilize + commit the 28-file working tree (clean base) | 0.5 | Must (blocker) | ⬜ Not started |
| S2-02 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` (push-on-equip) | 1.0 | Must | ⬜ Not started |
| S2-03 | `Core.GetCoreComponent<T>()` + self-register + lazy-cache (OCP) | 1.0 | Must (cut-first if slipping) | ⬜ Not started |
| S2-04 | Fix Bug #9 — AnimationPlayerController double-registration | 0.25 | Must | ⬜ Not started |
| S2-05 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach | 0.25 | Must | ⬜ Not started |
| S2-06 | One EditMode test for equip→ability path | 0.5 | Should | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 15/06 — elapsed
- Planned: S2-01 (0.5d) + start S2-02
- Actual: sprint set up (`start sprint 2`); no task progress committed
- Note: working tree reported clean — S2-01 (commit clean base) appears NOT done or done elsewhere. **Confirm with owner.**

### Tue 16/06 — elapsed
- Planned: S2-02 (decouple Weapon)
- Actual: no commit found
- Note: carried over.

### Wed 17/06 — TODAY 🎯
**Goal: get a clean base + land the damage fix so combat is testable.**
| Order | Task | Est | Why now |
|-------|------|-----|---------|
| 1 | S2-01 commit clean base | 0.5 | Hard blocker for all refactor edits |
| 2 | S2-05 melee damage (Bug #4) | 0.25 | De-risks the whole sprint — combat testable regardless of refactor |
| 3 | Start S2-02 decouple Weapon | (0.25 of 1.0) | Begin the core refactor |
- Day load: ~1.0d planned. Tight but the highest-value ordering.

### Thu 18/06
| Order | Task | Est | Notes |
|-------|------|-----|-------|
| 1 | Finish S2-02 decouple Weapon | 0.75 | Pairs with S2-04 to verify Enter→Exit |
| 2 | S2-04 Bug #9 anim double-register | 0.25 | Must verify with S2-02 (ability Exit) |
| 3 | S2-03 OCP refactor — START, time-boxed | (begin) | **Decision gate EOD: if S2-02/04 not done, CUT S2-03** |
- Day load: ~1.0d + S2-03 start.

### Fri 19/06
| Order | Task | Est | Notes |
|-------|------|-----|-------|
| 1 | Finish/cut S2-03 | ≤1.0 | Cut if Thu slipped |
| 2 | S2-06 EditMode test | 0.5 | BLOCKING gate for the Logic story |
| 3 | Wrap-up: `/smoke-check`, `/playtest-report`, `/bug-triage`, `/retrospective` | — | Friday ritual |
- Day load: review + test.

---

## Risks (live)
| Risk | Status | Mitigation |
|------|--------|------------|
| Refactor absorbs sprint, combat stays unplayable (5th wk) | 🔴 Active | S2-05 pulled to Wed; combat testable even if refactor slips |
| S2-01 status unclear (clean tree vs 28 uncommitted files) | 🟡 Needs confirm | Ask owner Wed standup |
| S2-03 (OCP) too big for remaining time | 🟡 Watch | Cut-first; decision gate Thu EOD |
| Zero automated tests (TD-014) | 🟡 Open | S2-06 adds the first — do not skip |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each session.

- **2026-06-17 (Wed)**: Tracker created. Sprint flagged AT-RISK (0 committed progress, 2 days elapsed). Awaiting owner confirmation on S2-01 actual state.
