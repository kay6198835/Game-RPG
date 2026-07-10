# Sprint 5 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-13 (Mon) → 2026-07-17 (Fri)
> **Companion to**: `sprint-05.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes yesterday from git + this tracker, updates statuses, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the week's `.cs`, playtest log, bug-triage, light retro; finalizes verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates upcoming week's sprint plan.
> **Last updated**: 2026-07-13 (Sun) — automated weekly kickoff, sprint opened.

---

## Status Verdict: ⬜ NOT STARTED — Sprint just opened. Branch `sprint-05` created from `sprint-04` tip (`6aa3cc1`). One commit (`cb099ee`) and one stash entry (`wip-before-sprint-kickoff-2026-07-13`) from the prior branch were NOT auto-carried — tracked as S5-11.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | ~2.65 days (Must Have) + ~1.1 days (Should Have) + ~1.25 days (Nice to Have) |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 5 |
| Work committed/done | 0 |
| Off-plan this cycle | None yet — sprint just opened |
| Velocity | N/A — not started |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S5-01 | Fix BUG-06 — implement `NegativeReciver.TakeDamage()`, add `ON_PLAYER_DEATH` | 0.5 | Must | ⬜ Not started |
| S5-02 | Fix BUG-05 — `EntityMoveState` null-guard reorder | 0.25 | Must | ⬜ Not started |
| S5-03 | Fix BUG-07 — `EntityDeathState` extends `EntityState` | 0.5 | Must | ⬜ Not started |
| S5-04 | Fix BUG-08 — `EntityBasicState` death transition + `ON_ENEMY_DEATH` | 0.25 | Must | ⬜ Not started |
| S5-05 | Fix BUG-ES-1 — null-guard `RoomModel.GetSpawnSet()` | 0.25 | Must | ⬜ Not started |
| S5-06 | Fix BUG-ES-3 — add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` to `EventID` | 0.1 | Must | ⬜ Not started |
| S5-07 | Build `EnemyManager` lifecycle (BUG-EM-1) | 1.0 | Must | ⬜ Not started |
| S5-08 | Resolve BUG-ES-2 — single canonical spawn driver | 0.5 | Should | ⬜ Not started |
| S5-09 | S4-05 — `CancelInvoke` pairing in `PlayerInputHandle` | 0.25 | Should | ⬜ Not started |
| S5-10 | ADR-0002 `Proposed` → `Accepted` sign-off | 0.1 | Should | ⬜ Not started |
| S5-11 | Reapply carried WIP (`cb099ee` + stash) | 0.25 | Should | ⬜ Not started |
| S5-12 | Quick cleanup batch (BUG-AH-2, BUG-EM-2, BUG-WM-2, BUG-LM-1) | 0.25 | Nice | ⬜ Not started |
| S5-13 | S4-06 — `TalentManager` → SO-driven | 1.0 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/13 — PLAN
**Goal: Close the two S1 combat-loop bugs' easy half — player death (S5-01) and the enemy-death null-guard/base-class fixes (S5-02, S5-03) — plus the two trivial spawn fixes (S5-05, S5-06).**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-01** — `NegativeReciver.TakeDamage()`: decrement `PlayerData.currentHealth`, emit `ON_PLAYER_DEATH` at 0 HP; add enum value | 0.5d | 🔴 Must |
| 2 | **S5-02** — `EntityMoveState.LogicUpdate()`: move null-guard to top | 0.25d | 🔴 Must |
| 3 | **S5-03** — `EntityDeathState : EntityState` rewrite + wire into state machine | 0.5d | 🔴 Must |
| 4 | **S5-05** — Null-guard `RoomModel.GetSpawnSet()` / `EnemySpawner.GetRoomSpawnSet()` | 0.25d | 🔴 Must |
| 5 | **S5-06** — Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` to `EventID` (do first — S5-04/S5-07 depend on it) | 0.1d | 🔴 Must |

---

### Tue 07/14 — PLAN
**Goal: Close the death-chain loop (S5-04) and start `EnemyManager` lifecycle (S5-07) — the sprint's largest task.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-04** — `EntityBasicState` empty death block: transition to `EntityDeathState`, emit `ON_ENEMY_DEATH` | 0.25d | 🔴 Must |
| 2 | **S5-07 start** — `EnemyManager`: alive-count dictionary keyed per room, subscribe `ON_ENEMY_DEATH`, reset on `ON_LOAD_MAP` | 0.5d | 🔴 Must |
| 3 | **Smoke check** — Play Mode: melee an enemy to 0 HP → confirm `EntityDeathState` reached, no NullReferenceException | — | Advisory |

---

### Wed 07/15 — PLAN
**Goal: Finish `EnemyManager` lifecycle (S5-07), reapply carried WIP (S5-11).**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-07 finish** — emit `ON_CLEAR_ENEMY` when room alive-count hits 0; verify against ADR-0002 Migration Plan | 0.5d | 🔴 Must |
| 2 | **S5-11** — cherry-pick `cb099ee`, reapply stash `wip-before-sprint-kickoff-2026-07-13`, resolve against S5-05/S5-07 diffs | 0.25d | 🟡 Should |
| 3 | **Smoke check** — Play Mode: clear all enemies in a room → confirm `ON_CLEAR_ENEMY` fires exactly once | — | Advisory |

---

### Thu 07/16 — PLAN
**Goal: Should-Have cleanup — BUG-ES-2 resolution, S4-05, ADR-0002 sign-off.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-08** — Decide canonical spawn driver, delete the loser, route survivor through `EnemyManager` | 0.5d | 🟡 Should |
| 2 | **S5-09** — `PlayerInputHandle.cs` `CancelInvoke` pairing | 0.25d | 🟡 Should |
| 3 | **S5-10** — ADR-0002 owner sign-off, flip `Proposed` → `Accepted` | 0.1d | 🟡 Should |
| 4 | If time remains: **S5-12** quick cleanup batch | 0.25d | ⚪ Nice |

---

### Fri 07/17 — WRAPUP DAY
**Goal: Smoke-check full combat loop + run `/weekly-wrapup` → close sprint, code-review, retrospective, bug-triage.**

| # | Task | Est |
|---|------|-----|
| 1 | Full Play Mode pass: equip → attack → enemy dies → room clears → doors unlock; take damage → player dies → `ON_PLAYER_DEATH` fires | — |
| 2 | `/weekly-wrapup` — review the week's `.cs`, playtest log if any, bug triage, light retro | — |
| 3 | Record carry-over + velocity in `sprint-05.md` Sprint Close section | — |
| 4 | Flag `/qa-plan sprint` if still not run by this point — blocking for Sprint 6 gate | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Off-plan work pattern recurs | 🟡 WATCH — S4 broke the Must-Have streak but still had heavy off-plan volume | Hold new spawn-feature work until S5-01→S5-07 land |
| S5-07 is the sprint's largest single task with two dependents | 🟡 WATCH | Scheduled Tue-start, Wed-finish; don't let it slip to Thu/Fri |
| Carried WIP (S5-11) conflicts with S5-05 changes | 🟢 LOW — small 4-line diff, explicitly sequenced after S5-05 | Sequenced Wed, after S5-05 lands Mon |
| No QA plan for this sprint | 🔴 OPEN — flagged in `sprint-05.md`, no interactive session to run `/qa-plan sprint` during automated kickoff | Owner should run `/qa-plan sprint` before S5-07 implementation starts |
| No Unity CLI in this environment | 🟢 KNOWN CONSTRAINT (same as S4) | All Play Mode smoke checks require manual in-Editor confirmation, flagged per-task above |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-13 (Sun) — automated weekly kickoff**: Sprint 4 formally closed (Definition of Done finalized in `sprint-04.md`, retro/triage already on file from Saturday's wrap-up). Branch `sprint-05` created from `sprint-04` tip (`6aa3cc1`), per the kickoff rule of forking from the sprint branch itself rather than whatever branch was checked out. Found and stashed 1 uncommitted WIP diff (`EnemySpawner.cs` + `LoadRandomMap.unity`, 4 lines) as `wip-before-sprint-kickoff-2026-07-13` — not discarded, flagged as S5-11. Also found `cb099ee` "random padding position" on `origin/feature/spawn-enemy` (the branch checked out at kickoff time) that is NOT part of the `sprint-04` branch tip and so was not carried automatically — also flagged as S5-11. `gh` CLI unavailable in this environment — draft PR (`sprint-05` → `sprint-04`) not created; noted as a manual follow-up in `sprint-05.md`. Sprint plan authored via `/sprint-plan`: 7 Must-Have (combat-loop death chain + `EnemyManager` lifecycle + BUG-ES-1/ES-3), 4 Should-Have, 2 Nice-to-Have. No QA plan exists yet for Sprint 5 — flagged as a blocking follow-up, not silently skipped (review mode is `lean`, so the Producer feasibility gate was skipped per that mode's rule; QA plan gate still surfaced explicitly).
