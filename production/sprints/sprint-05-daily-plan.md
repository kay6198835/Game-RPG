# Sprint 5 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-14 (Mon) → 2026-07-18 (Fri)
> **Companion to**: `sprint-05.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Last updated**: 2026-07-11 (Fri) — sprint plan renewed after the enemy-spawn doc-sync; replaces
> the original kickoff draft. Adopts Option C (Room Budget + Candidate Pool + Spawn Chance) as the
> locked spawn direction; re-scopes Sprint 5 around design-lock + combat death loop, defers the
> algorithm rewrite + `EnemyManager` runtime to Sprint 6.

---

## Status Verdict: ⬜ NOT STARTED — renewed plan, sprint window 07/14→07/18.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 3.85d (Track A ~1.75 + B ~1.6 + C ~1.0, overlapping) + Should ~0.85d + Nice ~1.75d |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 5 |
| Work committed/done | 0 |
| Velocity | N/A — not started |

---

## Task Estimates

| ID | Task | Est (d) | Track/Pri | Status |
|----|------|---------|-----------|--------|
| S5-A1 | Option C full spec into GDD + resolve 5 open-Qs | 0.5 | A / Must | ⬜ Not started |
| S5-A2 | ADR-0003 ratify Option C | 0.25 | A / Must | ⬜ Not started |
| S5-A3 | `EnemyModal` refactor (`weight`→`cost` clamp, +`spawnChance`/`tier`) + migrate 6 assets | 0.5 | A / Must | ⬜ Not started |
| S5-A4 | `RoomModel` refactor (+`roomType`/`budgetTolerance`, −dead fields) + migrate assets | 0.5 | A / Must | ⬜ Not started |
| S5-B1 | Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` to `EventID` | 0.1 | B / Must | ⬜ Not started |
| S5-B2 | `NegativeReciver.TakeDamage()` + `ON_PLAYER_DEATH` (BUG-06) | 0.5 | B / Must | ⬜ Not started |
| S5-B3 | `EntityMoveState` null-guard (BUG-05) | 0.25 | B / Must | ⬜ Not started |
| S5-B4 | `EntityDeathState : EntityState` (BUG-07) | 0.5 | B / Must | ⬜ Not started |
| S5-B5 | `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25 | B / Must | ⬜ Not started |
| S5-C1 | `GetSpawnSet()` empty-list not null (BUG-ES-1) | 0.25 | C / Must | ⬜ Not started |
| S5-C2 | Markerless-room fallback | 0.25 | C / Must | ⬜ Not started |
| S5-C3 | Dedupe spawn driver (BUG-ES-2) | 0.5 | C / Must | ⬜ Not started |
| S5-D1 | Reapply carried WIP (`cb099ee` + stash) | 0.25 | Should | ⬜ Not started |
| S5-D2 | ADR-0002 Proposed→Accepted | 0.1 | Should | ⬜ Not started |
| S5-D3 | S4-05 `CancelInvoke` pairing | 0.25 | Should | ⬜ Not started |
| S5-D4 | Quick cleanup batch (AH-2/EM-2/WM-2/LM-1) | 0.25 | Should | ⬜ Not started |
| S5-N1 | Start Candidate-Pool `GetSpawnSet()` rewrite (stretch) | 0.75 | Nice | ⬜ Not started |
| S5-N2 | S4-06 `TalentManager` → SO | 1.0 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/14 — PLAN
**Goal: Land the events enum, the Option C design-lock (spec + ADR), and player death.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-B1** — add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` to `EventID` (do first — unblocks B2/B5) | 0.1d | 🔴 Must |
| 2 | **S5-A1** — write full Option C spec into GDD; resolve the 5 open-Qs (band under-spend, room→preset, pick algorithm, `roomType` role, RNG) | 0.5d | 🔴 Must |
| 3 | **S5-A2** — `/architecture-decision` → ADR-0003 ratifying Option C | 0.25d | 🔴 Must |
| 4 | **S5-B2** — `NegativeReciver.TakeDamage()`: decrement HP, emit `ON_PLAYER_DEATH` at 0 (BUG-06) | 0.5d | 🔴 Must |

### Tue 07/15 — PLAN
**Goal: Data-model refactor for Option C + start the enemy death chain.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-A3** — `EnemyModal`: `weight`→`cost` (`[Range(1,99)]`+clamp, `[FormerlySerializedAs("weight")]`), add `spawnChance`/`tier`; migrate 6 assets | 0.5d | 🔴 Must |
| 2 | **S5-A4** — `RoomModel`: add `roomType`/`budgetTolerance`, remove dead fields; migrate assets | 0.5d | 🔴 Must |
| 3 | **S5-B3** — `EntityMoveState` null-guard to top (BUG-05) | 0.25d | 🔴 Must |

### Wed 07/16 — PLAN
**Goal: Finish the enemy death chain + spawn safety guards.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-B4** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must |
| 2 | **S5-B5** — `EntityBasicState` death transition + emit `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must |
| 3 | **S5-C1** — `GetSpawnSet()` empty-list not null (BUG-ES-1) | 0.25d | 🔴 Must |
| 4 | **S5-C2** — markerless-room fallback (room-centre + warning) | 0.25d | 🔴 Must |
| 5 | **Smoke** — Play Mode: melee enemy to 0 HP → `EntityDeathState` reached, `ON_ENEMY_DEATH` once | — | Advisory |

### Thu 07/17 — PLAN
**Goal: Dedupe spawn driver + carry-over reconciliation + ADR-0002 accept.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-C3** — pick canonical spawn driver, delete the loser (BUG-ES-2) | 0.5d | 🔴 Must |
| 2 | **S5-D1** — reapply `cb099ee` + stash onto `sprint-05`, reconcile vs C1/C3 | 0.25d | 🟡 Should |
| 3 | **S5-D2** — ADR-0002 Proposed→Accepted | 0.1d | 🟡 Should |
| 4 | If time: **S5-D3** (`CancelInvoke`) / **S5-D4** (cleanup batch) | 0.5d | 🟡 Should |

### Fri 07/18 — WRAPUP DAY
**Goal: Full-loop smoke-check + `/weekly-wrapup`.**

| # | Task | Est |
|---|------|-----|
| 1 | Full Play Mode pass: take damage → player dies → `ON_PLAYER_DEATH`; kill enemy → `ON_ENEMY_DEATH` → `EntityDeathState`; empty-pool + markerless rooms load without throw | — |
| 2 | `/weekly-wrapup` — review week's `.cs`, playtest log if any, bug triage, light retro | — |
| 3 | Record carry-over + velocity in `sprint-05.md` Sprint Close | — |
| 4 | Confirm `/qa-plan sprint` was run (blocking for S6 gate); flag if not | — |
| 5 | If Must-Haves done early: **S5-N1** (start Candidate-Pool rewrite) | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| `weight`→`cost` rename drops serialized values on 6 assets | 🔴 WATCH | Use `[FormerlySerializedAs("weight")]` on `cost`; verify each asset opens with value intact before moving on |
| Scope heavy (design + refactor + death chain in 4d) | 🟡 WATCH | Algorithm rewrite + `EnemyManager` deferred to S6; N1/N2 stretch only |
| Off-plan work recurs | 🟡 WATCH | Track A + B are critical path; hold new spawn-feature work until they land |
| No QA plan (lean) | 🔴 OPEN | Run `/qa-plan sprint` before Track B |
| No Unity CLI | 🟢 KNOWN | Play Mode smoke = manual in-Editor |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-11 (Fri) — plan renewed**: Original kickoff sprint-05 draft (combat-loop-only, EnemyManager-body-as-Must-Have) replaced after this week's enemy-spawn doc-sync. Doc-sync established: data+algorithm layer already built, `Tile_Spawn_Enemy` parser exists, `EnemySpawner` wired, and owner committed to Option C (Room Budget + Candidate Pool + Spawn Chance). Renewed plan approved via plan mode. Re-scoped into Track A (adopt Option C — spec + ADR-0003 + data refactor), Track B (combat death loop), Track C (spawn stabilization); deferred the `GetSpawnSet()` rewrite + `EnemyManager` runtime to Sprint 6. No QA plan yet — flagged. `gh` unavailable — draft PR not created.
