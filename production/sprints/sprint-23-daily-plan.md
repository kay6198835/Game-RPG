# Sprint 23 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-16 (Mon) -> 2026-11-20 (Fri) · playtest Sat 2026-11-21
> **Companion to**: `sprint-23.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-15 (kickoff, autonomous). Branch `sprint-23` from `sprint-22` tip (`37b56d76`).
> Sprint 22 closed SLIPPED (Must 0.05d / 1.05d, velocity 0.16, ≈2.5d off-plan output, playtest
> done: no). See `sprint-23.md` › Sprint 22 Closure.

---

## Status Verdict

**ON-TRACK — day 0 (Sun 2026-11-15, kickoff).** Nothing started. Evidence: STATIC.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 0.95d | 0 | 0.95d |
| Should Have | 0.9d | 0 | 0.9d |
| Feature story S23-F1 (capped) | 1.0d | 0 | 1.0d |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S23-01 | Playtest / Play Mode smoke via menu (+ awaiting-runtime rows) | 0.3d | Must | **26th** |
| S23-02 | BUG-122 + BUG-065 + BUG-086 (death → Game Over) | 0.3d | Must | 2nd / **14th** / 9th |
| S23-03 | BUG-125 melee stage damage twice | 0.05d | Must | new (S2, P1) |
| S23-04 | BUG-096 Boss remainder + BUG-097 | 0.15d | Must | **7th** |
| S23-05 | BUG-126 `using UnityEditor` guard | 0.05d | Must | new (S2) |
| — | Saturday playtest | 0.1d | Must | — |
| S23-06 | BUG-106 player half + BUG-110 | 0.2d | Should | 4th |
| S23-07 | Editor / data block (BUG-095, BUG-072, BUG-108 key) | 0.15d | Should | **7th** |
| S23-08 | BUG-117 + BUG-120 + BUG-123 | 0.35d | Should | 3rd |
| S23-09 | BUG-127 Mage bolt data | 0.05d | Should | new (S3) |
| S23-10 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 5th–8th |
| S23-F1 | Merge `docs/gdd-pathfinding-ability` (capped) | ≤1.0d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-16 — Play first, then death + damage
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-01 Playtest / smoke through the menu (or sign accepted risk) | 0.3d | ⬜ | playtest done: no; 26th carry; eight bugs wait on runtime |
| S23-02 BUG-122 + BUG-065 + BUG-086 | 0.3d | ⬜ | S1; same file; untouched two sprints |
| S23-03 BUG-125 | 0.05d | ⬜ | One line; every melee number wrong until fixed |

### Tue 2026-11-17 — Room path + build guard
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-04 BUG-096 Boss + BUG-097 | 0.15d | ⬜ | Run starts in boss room; 7th carry |
| S23-05 BUG-126 | 0.05d | ⬜ | One line; blocks any Player build |
| S23-08 BUG-117 + 120 + 123 (start) | 0.35d | ⬜ | Must land before the owner branch merges `VitalModifier` |

### Wed 2026-11-18 — Owner feature branch
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-F1 Finish + merge `docs/gdd-pathfinding-ability` | ≤1.0d | ⬜ | Declared main work (sprint-22 retro); after Mon/Tue fixes |

### Thu 2026-11-19 — Weapon + data
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-06 BUG-106 player half + BUG-110 | 0.2d | ⬜ | Weapon bookkeeping; 4th carry |
| S23-07 Editor / data block | 0.15d | ⬜ | Inspector-only, 7th carry |
| S23-09 BUG-127 | 0.05d | ⬜ | Mage basic attack barely reaches |

### Fri 2026-11-20 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-10 One-liners | 0.15d | ⬜ | Carried 5–8 weeks |
| Buffer | 1d | — | — |

### Sat 2026-11-21 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri 23:00 QA run as input) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 26th carry — owner decision: record one sheet or sign accepted risk.
- S23-02 must include BUG-086, otherwise `ON_PLAYER_DEATH` fires every frame once BUG-122 is fixed.
- Owner branch serializes `VitalModifier` without the BUG-117 sign fix — do S23-08 first.
- Needs-owner notes (NOTE-20261115-5/-6/-7 and older) — not tasks.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_
