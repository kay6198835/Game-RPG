# Sprint 4 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-07 (Mon) → 2026-07-11 (Fri)
> **Companion to**: `sprint-04.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes yesterday from git + this tracker, updates statuses, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the week's `.cs`, playtest log, bug-triage, light retro; finalizes verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates upcoming week's sprint plan.
> **Last updated**: 2026-07-07 (Mon) — Day 1 standup

---

## Status Verdict: 🔴 AT RISK — Day 1 opened with zero Must-Have progress; off-plan work (StatSystem) continued overnight

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 2.5 days (Must + Should) |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 1 (Day 1 morning) |
| Days remaining | 5 |
| Work committed/done | 0 (S4-01→S4-04 all still Not Started) |
| Work remaining | 2.5 days |
| Velocity | 0% so far — off-plan commits only (StatSystem/Abilities) |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S4-01 | Fix BUG-AH-1 — remove `UnityEditor` imports from `AbilityHolder.cs` + all runtime scripts | 0.25 | Must | ⬜ Not started |
| S4-02 | Fix Bug #9 — `AnimationPlayerController` double-registration | 0.25 | Must | ⬜ Not started |
| S4-03 | `Core.GetCoreComponent<T>()` LINQ → foreach + lazy cache | 0.25 | Must | ⬜ Not started |
| S4-04 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach (add TakeDamage) | 0.25 | Must | ⬜ Not started |
| S4-05 | Fix BUG-PIH-1 — `CancelInvoke` pairing in `PlayerInputHandle` | 0.25 | Should | ⬜ Not started |
| S4-06 | Stats system — `TalentManager` prototype → SO-driven | 1.0 | Should | ⬜ Not started |
| S4-07 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | 1.0 | Nice | ⬜ Not started |
| S4-08 | EditMode test for `Core.GetCoreComponent<T>()` | 0.5 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/07 — PLAN
**Goal: Land all 4 P1 Must-Have fixes (S4-01 → S4-04). These are the ONLY tasks until done.**

> These four items have been carried 8 sprints. Each is ≤0.25d. A single focused session (2–3h) closes all of them.

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-01** — `AbilityHolder.cs` line 4: remove `using UnityEditor.Experimental.GraphView;`. Then grep full `Assets/Script/` for `using UnityEditor` and fix all runtime hits. | 0.25d | 🔴 Must |
| 2 | **S4-02** — `AnimationPlayerController.cs` line 21: change second `StartAnimation` → `EndAnimation`; line 29 OnDisable mirror. Verify in Play Mode weapon state exits. | 0.25d | 🔴 Must |
| 3 | **S4-03** — `Core.cs` `GetCoreComponent<T>()`: replace LINQ `OfType<T>().FirstOrDefault()` with `foreach`. Add `??=` lazy-cache so loop runs once per type. | 0.25d | 🔴 Must |
| 4 | **S4-04** — `WeaponMelee.cs` Attack() foreach body: add `INegativeReceiver dmg = enemy.GetComponentInChildren<INegativeReceiver>(); if (dmg != null) dmg.TakeDamage(currrentSA.attackDamege, transform.position);` | 0.25d | 🔴 Must |
| 5 | **Smoke check** — Enter Play Mode: equip → LMB attack → confirm enemy Health decreases in Inspector | — | Advisory |

*If all 4 land Mon: combat is testable for the first time in 8 weeks.*

---

### Tue 08/07 — PLAN
**Goal: S4-05 (BUG-PIH-1) + start S4-06 (TalentManager SO)**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-05** — `PlayerInputHandle.cs`: audit all `Invoke`/`InvokeRepeating` calls; pair each with `CancelInvoke` in `OnDisable` | 0.25d | Should |
| 2 | **S4-06 start** — Read `TalentManager.cs`; create or extend `StatsCharacter` SO with the 5 fields (`strength`, `dex`, `int`, `cha`, `skillPoint`); Inspector-assignable | 0.5d | Should |

---

### Wed 09/07 — PLAN
**Goal: Complete S4-06 + begin S4-07 if capacity allows**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-06 finish** — Wire `TalentManager` to SO; remove hardcoded `Awake` assignments; add `[Range]` validators on SO fields | 0.5d | Should |
| 2 | **S4-07 start** — Decouple `Weapon.Interact()` / `WeaponHolder.Equip()` (time-box 0.5d; carry if over) | 0.5d | Nice |

---

### Thu 10/07 — PLAN
**Goal: S4-07 finish OR S4-08 EditMode test + first playtest session**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-07 finish** OR **S4-08** EditMode test for `Core.GetCoreComponent<T>()` | 0.5–1.0d | Nice |
| 2 | **First playtest session** — if S4-04 landed Mon: run combat loop, log findings under `production/qa/playtests/` | — | Advisory |

---

### Fri 11/07 — WRAPUP DAY
**Goal: Smoke-check + run `/weekly-wrapup` → close sprint, code-review, retrospective**

| # | Task | Est |
|---|------|-----|
| 1 | Final smoke-check: full demo loop (equip → melee hit deals damage → ability run + exit → player takes damage) | — |
| 2 | `/weekly-wrapup` — code review `.cs` changes, playtest log if session happened, bug triage, retro | — |
| 3 | Record carry-over + velocity in `sprint-04.md` Sprint Close section | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Off-plan work displacing Must-Haves (pattern from 8 prior sprints) | 🔴 CONFIRMED — commits `6cd30f1`, `c181bd2`, `c16ee77`, `dac227d` (2026-07-06 evening, all StatSystem/Abilities) landed with zero S4-01→S4-04 progress; would be the 9th consecutive sprint with this pattern if Day 1 doesn't correct course | S4-01→S4-04 are hardcoded first; no other work may start until all 4 done |
| BUG-AH-1 scope wider than `AbilityHolder.cs` alone | 🟡 Confirmed | Grep found 7+ files; S4-01 acceptance criteria requires 0 runtime hits — fix all on first pass |
| Developer absence / zero-commit days | 🔴 Watch | Must-Have block ≈ 1d; even 3h of focused work Mon closes it |
| S4-03 lazy-cache breaks call sites | Low | Grep all `GetCoreComponent<T>()` call sites before and after; verify no Console warnings |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-05 (Sun)**: Sprint-04 opened. Branch `sprint-04` created from `sprint-03`. All items carry from Sprint 3 (0% velocity, 8th consecutive sprint with same P1 backlog). Sprint-03 formally closed. 22:00 scheduled wrapup confirmed: bug-triage-2026-07-05.md, retro-sprint-03-2026-07-05.md, sprint-03.md close — all committed @ 2fb9211. No new CS changes since 20:08 close. Sprint-04 opens Mon 2026-07-07.
- **2026-07-06 (Sun)**: Pre-sprint day. **Zero sprint task progress** — all S4-01→S4-08 remain Not Started. ⚠️ RISK DETECTED: uncommitted changes found in `Assets/Skill Enhance/` (off-plan folder): `IAbilityOwner.cs` (3 interface members commented out), `SpiritDoTBehaviour.cs` (entire DoT implementation commented out). Also untracked `StatsSO.cs.meta`. None of these belong to S4 Must-Have tasks. Pattern warning: off-plan work occurring before sprint Day 1 — exact same pattern as 8 prior sprints. Sprint Day 1 (Mon 07/07) must focus exclusively on S4-01→S4-04.
- **2026-07-07 (Mon, Day 1) — 02:00 standup**: The pre-sprint uncommitted work from 07-06 was committed anyway, off-plan: `c181bd2` (StatType/Stat serialization), `6cd30f1` "coding" (StatsSO + new `Assets/.../Abilities/` files: `Conditions/`, `Core/AbilityInstance.cs`, `Effects/`, `Runtime/SpiritDoTBehaviour.cs`, `SpiritOrbProjectile.cs` — a system not referenced anywhere in `CLAUDE.md`), `c16ee77` (ADR-0001 for the StatSystem dual data structure), `dac227d` "fix base calculate stats" (StatsSO/Stat/DerivedStatFormula). **Zero commits touch S4-01→S4-04.** Verified directly against acceptance criteria: `grep -r "using UnityEditor" Assets/Script/` still returns 7 runtime hits (S4-01 open); `AnimationPlayerController.cs` line 21 still registers `StartAnimation` twice instead of `EndAnimation` (S4-02 open); `Core.cs` line 24 still uses `coreComponents.OfType<T>().FirstOrDefault()` (S4-03 open); `WeaponMelee.cs` `Attack()` foreach body (line 30-32) is still empty (S4-04 open). This is now day 1 of the 9th sprint carrying the exact same pattern flagged in the sprint's own risk register. Separately, a WIP uncommitted change to `Assets/Script/Utility/GameConstants.cs` exists on branch `feature/stats-system` (stashed during this standup, not evaluated) — also off S4-plan.
