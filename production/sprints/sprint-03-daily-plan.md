# Sprint 3 — Daily Plan & Progress Tracker

> **Sprint**: 2026-06-23 (Mon) → 2026-06-27 (Fri)
> **Companion to**: `sprint-03.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes yesterday from git + this tracker, updates statuses, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the week's `.cs`, playtest log, bug-triage, light retro; finalizes verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates upcoming week's sprint plan.
> **Last updated**: 2026-06-22 (Sun) — kickoff

---

## Status Verdict: ⬜ NOT STARTED — Sprint begins Mon 2026-06-23

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 2.5 days (Must + Should) |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 5 |
| Work committed/done | 0 |
| Work remaining | 2.5 days |
| Slack | +1.5 days |

---

## Task Estimates (from sprint-03.md)

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S3-01 | Fix BUG-AH-1 — remove `UnityEditor` imports from `AbilityHolder.cs` | 0.25 | Must | ⬜ Not started |
| S3-02 | Fix Bug #9 — `AnimationPlayerController` double-registration | 0.25 | Must | ⬜ Not started |
| S3-03 | Complete S2-03 — `Core.GetCoreComponent<T>()` LINQ → foreach | 0.25 | Must | ⬜ Not started |
| S3-04 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach (melee damage) | 0.25 | Must | ⬜ Not started |
| S3-05 | Fix BUG-PIH-1 — `CancelInvoke` pairing in `PlayerInputHandle` | 0.25 | Should | ⬜ Not started |
| S3-06 | Stats system promotion — `TalentManager` → SO-driven | 1.0 | Should | ⬜ Not started |
| S3-07 | S2-02 carry — decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | 1.0 | Nice | ⬜ Not started |
| S3-08 | EditMode test for `Core.GetCoreComponent<T>()` | 0.5 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 23/06 — PLAN
**Goal: Commit dirty working tree + land all 4 P1 Must-Have fixes (S3-01 → S3-04)**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 0 | **Commit dirty working tree** (PlayerInputHandle, PlayerState, PlayerAttackState, WeaponMelee, animation clips, SetLevel.unity) | 0.25d | Prerequisite |
| 1 | **S3-01** — Fix BUG-AH-1: `AbilityHolder.cs` remove `using UnityEditor.*`; wrap editor-only code in `#if UNITY_EDITOR` | 0.25d | 🔴 Must |
| 2 | **S3-02** — Fix Bug #9: `AnimationPlayerController.cs` line 21 `StartAnimation` → `EndAnimation`; line 29 OnDisable mirror | 0.25d | 🔴 Must |
| 3 | **S3-03** — `Core.GetCoreComponent<T>()`: replace LINQ with `foreach`; verify no `"<T> not found"` warnings | 0.25d | 🔴 Must |
| 4 | **S3-04** — `WeaponMelee.Attack()`: add `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)` | 0.25d | 🔴 Must |
| 5 | Smoke check: equip → attack → enemy takes damage | — | Advisory |

*If all 4 Must-Haves land Mon: combat is testable for the first time in 6 weeks.*

---

### Tue 24/06 — PLAN
**Goal: S3-05 (BUG-PIH-1) + begin S3-06 (TalentManager promotion)**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S3-05** — `PlayerInputHandle`: audit all `Invoke`/`InvokeRepeating` calls; add `CancelInvoke` in `OnDisable` | 0.25d | Should |
| 2 | **S3-06 start** — Read `TalentManager.cs` prototype; create `PlayerStatsSO` (or extend `StatsCharacter`) with the 5 fields | 0.5d | Should |

---

### Wed 25/06 — PLAN
**Goal: Complete S3-06 + begin S3-07 (Weapon decoupling) if capacity allows**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S3-06 finish** — Wire `TalentManager` to SO; remove hardcoded `Awake` values; Inspector-assign SO instance | 0.5d | Should |
| 2 | **S3-07 start** — Decouple `Weapon.Interact()` / `WeaponHolder.Equip()` (P2 — time-box to 0.5d; carry if over) | 0.5d | Nice |

---

### Thu 26/06 — PLAN
**Goal: S3-07 finish OR S3-08 EditMode test + buffer / first playtest session**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S3-07 finish** OR **S3-08** EditMode test for `Core.GetCoreComponent<T>()` | 0.5–1.0d | Nice |
| 2 | **First playtest session** — if S3-04 landed Mon: run combat loop, log findings under `production/qa/playtests/` | — | Advisory |

---

### Fri 27/06 — WRAPUP DAY
**Goal: Smoke-check + run `/weekly-wrapup` → close sprint, code-review, retrospective**

| # | Task | Est |
|---|------|-----|
| 1 | Final smoke-check: full demo loop (equip → melee hit → ability run+exit → player takes damage) | — |
| 2 | `/weekly-wrapup` — code review `.cs` changes, playtest log if session happened, bug triage, retro | — |
| 3 | Record carry-over + velocity in `sprint-03.md` Sprint Close section | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Dirty working tree (4 `.cs` + 4 assets) blocks Sprint 3 start | 🔴 Active | Commit first thing Mon 23/06 before any sprint task |
| Recurring developer absence (zero-commit days) pattern | 🔴 Watch | S3-01→S3-04 total ~1d; even 2 productive days covers all Must-Haves |
| BUG-AH-1 unknown scope (how many files have `UnityEditor` import?) | 🟡 Unknown | Grep `using UnityEditor` in `Assets/Script/` Mon; fix all at once |
| Stats system (S3-06) larger than 1d estimate | 🟡 Watch | Should Have — cut to Sprint 4 if Must-Haves + S3-05 absorb Mon–Tue |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-06-22 (Sun)**: Sprint-03 kickoff — branch `sprint-03` created from `origin/fix-player-control`; `sprint-03.md` + this tracker written. Carry-over: 4 P1 items + 1 P2 bug + S2-02 task.
