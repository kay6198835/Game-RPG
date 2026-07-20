# Sprint 6 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-21 (Mon) → 2026-07-25 (Fri)
> **Companion to**: `sprint-06.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-07-20 (Sun 22:00 kickoff) — autonomous scheduled run, no user present. Branch
> `sprint-06` created from `sprint-05` tip (`cc543ba`).

---

## Status Verdict: 🟢 NOT STARTED — sprint opens 2026-07-21

Sprint 5 closed CONCERNS/bordering-FAIL at 34% Must-Have (1.35d/3.95d) after 4 consecutive days of
off-plan work. Sprint 6 is deliberately scoped narrower (2.20d Must-Have vs 4d capacity) to make
landing the carried death-chain and spawn-stabilization bugs achievable even with some slippage.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 2.20d + Should ≈ 1.20d + Nice ≈ stretch |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 4 |
| Work committed/done | 0d |
| Velocity | N/A — sprint has not started |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S6-00 | Verify branch parity (`origin/feature/spawn-enemy` commits present) | 0.1 | Must | ⬜ Not started |
| S6-01 | Verify `PoolMember.cs` compiles (CS0592 risk) | 0.1 | Must | ⬜ Not started |
| S6-02 | Fix NEW-1 — `GetSpawnSet()` weight==0 hang risk | 0.25 | Must | ⬜ Not started |
| S6-03 | Fix BUG-05 — `EntityMoveState` null-guard to top | 0.25 | Must | ⬜ Not started |
| S6-04 | Fix BUG-07 — `EntityDeathState : EntityState` rewrite + wire | 0.5 | Must | ⬜ Not started |
| S6-05 | Fix BUG-08 — `EntityBasicState` death transition + `ON_ENEMY_DEATH` | 0.25 | Must | ⬜ Not started |
| S6-06 | Fix BUG-ES-1 — `GetSpawnSet()` empty list not null | 0.25 | Must | ⬜ Not started |
| S6-07 | Fix BUG-ES-4 — guard `EnemySpawner.cs` empty `spawnPosition` read | 0.1 | Must | ⬜ Not started |
| S6-08 | Fix BUG-06 partial — write-through to `PlayerData.currentHealth` | 0.25 | Must | ⬜ Not started |
| S6-09 | Decision — `EnemyModal` SO-vs-plain-class | 0.15 | Must | ⬜ Not started |
| S6-D1 | ADR-0002 Proposed→Accepted | 0.1 | Should | ⬜ Not started |
| S6-D2 | ADR-0003 Proposed→Accepted (post S6-09) | 0.1 | Should | ⬜ Not started |
| S6-D3 | Dedupe spawn driver (BUG-ES-2) | 0.5 | Should | ⬜ Not started |
| S6-D4 | `CancelInvoke` pairing | 0.25 | Should | ⬜ Not started |
| S6-D5 | Cleanup batch (AH-2/EM-2/WM-2/LM-1) | 0.25 | Should | ⬜ Not started |
| S6-D6 | Decision — S4-05/S4-06 keep-or-cut | 0.1 | Should | ⬜ Not started |
| S6-N1 | First playtest session | — | Nice | ⬜ Not started |
| S6-N2 | Start `EnemyManager` lifecycle body (stretch) | 1.0 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/21 — PLAN
**Goal: Verify no build break, close the highest-severity carried bugs first (hang risk, entity death chain start).**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-00** — verify branch parity | 0.1d | 🔴 Must |
| 2 | **S6-01** — verify `PoolMember.cs` compiles, fix if broken | 0.1d | 🔴 Must |
| 3 | **S6-02** — fix NEW-1 hang risk (`weight == 0`) | 0.25d | 🔴 Must |
| 4 | **S6-03** — `EntityMoveState` null-guard (BUG-05) | 0.25d | 🔴 Must |

### Tue 07/22 — PLAN
**Goal: Finish the enemy death chain.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-04** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must |
| 2 | **S6-05** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must |
| 3 | **Smoke** — Play Mode: melee enemy to 0 HP → `EntityDeathState` reached, event fires once | — | Advisory |

### Wed 07/23 — PLAN
**Goal: Spawn stabilization + BUG-06 write-through fix.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-06** — `GetSpawnSet()` empty list not null (BUG-ES-1) | 0.25d | 🔴 Must |
| 2 | **S6-07** — guard `EnemySpawner.cs` empty `spawnPosition` read (BUG-ES-4) | 0.1d | 🔴 Must |
| 3 | **S6-08** — `NegativeReciver` write-through to `PlayerData.currentHealth` (BUG-06 partial fix) | 0.25d | 🔴 Must |
| 4 | **S6-09** — `EnemyModal` SO-vs-plain-class decision | 0.15d | 🔴 Must |

### Thu 07/24 — PLAN
**Goal: Should-Have carry cleanup — ADR flips, driver dedupe, small fixes.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-D1** — ADR-0002 Proposed→Accepted | 0.1d | 🟡 Should |
| 2 | **S6-D2** — ADR-0003 Proposed→Accepted (post S6-09) | 0.1d | 🟡 Should |
| 3 | **S6-D3** — dedupe spawn driver (BUG-ES-2) | 0.5d | 🟡 Should |
| 4 | **S6-D4** — `CancelInvoke` pairing | 0.25d | 🟡 Should |
| 5 | **S6-D5** — cleanup batch | 0.25d | 🟡 Should |
| 6 | **S6-D6** — S4-05/S4-06 keep-or-cut decision | 0.1d | 🟡 Should |

### Fri 07/25 — WRAPUP DAY
**Goal: Full-loop smoke-check + `/weekly-wrapup`.**

| # | Task | Est |
|---|------|-----|
| 1 | Full Play Mode pass: enemy death chain end-to-end; empty-pool + empty-spawn-position rooms load without throw; player damage → `Reborn()` restores the same HP field | — |
| 2 | `/weekly-wrapup` — review week's `.cs`, playtest log if S6-N1 ran, bug triage, light retro | — |
| 3 | Record carry-over + velocity in `sprint-06.md` if anything slips | — |
| 4 | Confirm `/qa-plan sprint` was run this cycle (blocking for next gate); flag again if not — 4th consecutive miss would be worth investigating as a process issue | — |
| 5 | If Must-Haves done early: **S6-N1** (playtest) or **S6-N2** (`EnemyManager` lifecycle start) | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Off-plan work recurs a 5th time | 🔴 WATCH | Scope deliberately narrow this sprint (2.2d vs 4d capacity) — flag immediately if unplanned architecture work appears in `git log` at any standup |
| `PoolMember.cs` build break unverified | 🔴 WATCH | First task Monday, verify before anything else |
| `EnemyModal` decision blocks ADR-0003 flip and further spawn-system work | 🟡 WATCH | S6-09 scheduled Wed, before Thu's ADR cleanup |
| No QA plan — 3rd consecutive cycle | 🔴 OPEN | Flagged in `sprint-06.md`; recommend running `/qa-plan sprint` before S6-03 starts |
| No Unity CLI | 🟢 KNOWN | Play Mode smoke = manual in-Editor |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-20 (Sun 22:00) — kickoff, autonomous scheduled run**: Sprint 5 closed (`sprint-05.md`
  marked Status: COMPLETE). Branch `sprint-06` created from `sprint-05` tip (`cc543ba`) —
  `git checkout sprint-05 && git checkout -b sprint-06`. `gh` CLI unavailable in this environment;
  draft PR not created — run manually: `gh pr create --draft --base sprint-05 --head sprint-06
  --title "Sprint 6"`. Sprint plan (`sprint-06.md`) and this tracker written via `/sprint-plan`,
  scoped narrower than Sprint 5 (2.20d Must-Have vs 4d capacity) specifically because Sprint 5 closed
  at only 34% Must-Have across 4 days of off-plan work — the goal this cycle is closing the carry,
  not adding scope. QA plan gate still open (3rd consecutive cycle) — flagged, not silently dropped.
  No user was present for this run; scoping calls (task breakdown, estimates, day sequencing) made
  autonomously per the kickoff routine's standing authorization — review at Monday's standup.
