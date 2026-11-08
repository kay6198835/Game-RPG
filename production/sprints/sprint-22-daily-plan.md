# Sprint 22 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-09 (Mon) -> 2026-11-13 (Fri) · playtest Sat 2026-11-14
> **Companion to**: `sprint-22.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-08 (kickoff, autonomous). Branch `sprint-22` from `sprint-21` tip (`d2d2b22e`).
> Sprint 21 closed SLIPPED (Must 0.3d / 1.45d, velocity 0.08, ≈1.7d off-plan output, playtest
> done: no). See `sprint-22.md` › Sprint 21 Closure.

---

## Status Verdict

**ON-TRACK — day 0** (sprint opens Monday; nothing due yet). Evidence: STATIC.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.05d | 0 | 1.05d |
| Should Have | 0.8d | 0 | 0.8d |
| Feature story S22-F1 (capped, gated by S22-01) | 0.5d | 0 | 0.5d |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S22-01 | Playtest / Play Mode smoke via menu (+ awaiting-runtime bug rows) | 0.3d | Must | **25th** |
| S22-02 | BUG-122 player death → Game Over | 0.1d | Must | new (S1, P1) |
| S22-03 | BUG-096 Boss room remainder | 0.05d | Must | 6th |
| S22-04 | BUG-097 start / boss order | 0.1d | Must | 6th |
| S22-05 | BUG-124 pooled re-spawn anchor | 0.05d | Must | new (P2) |
| S22-06 | BUG-106 player half + BUG-110 | 0.2d | Must | 3rd |
| S22-07 | Editor / data block (BUG-095, BUG-072, re-save, BUG-108 key) | 0.15d | Must | 6th |
| — | Saturday playtest | 0.1d | Must | — |
| S22-09 | BUG-065 + BUG-086 (with S22-02) | 0.2d | Should | **13th** / 8th |
| S22-10 | BUG-117 + BUG-120 + BUG-123 | 0.35d | Should | 2nd |
| S22-11 | BUG-115 asset cleanup | 0.1d | Should | 3rd |
| S22-12 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 4th–7th |
| S22-F1 | Merge `feature/polish-game` (capped) | ≤0.5d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-09 — Play first, then the S1 death fix
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-01 Playtest / smoke through the menu | 0.3d | ⬜ | playtest done: no; 25th carry; seven bugs wait on runtime confirmation |
| S22-02 BUG-122 | 0.1d | ⬜ | S1 P1; QA-VIT-07 failed 10 runs in a row |
| S22-09 BUG-065 + BUG-086 | 0.2d | ⬜ | Same file as S22-02; fixing BUG-122 makes BUG-086 fire every frame |

### Tue 2026-11-10 — Room path
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-04 BUG-097 | 0.1d | ⬜ | Run starts in the boss room; 6th carry |
| S22-03 BUG-096 Boss room | 0.05d | ⬜ | Last sealed room type |
| S22-05 BUG-124 | 0.05d | ⬜ | One-line ordering fix; every pooled re-spawn |
| S22-07 Editor / data block | 0.15d | ⬜ | Inspector-only work, 6th carry |

### Wed 2026-11-11 — Weapon + owner branch
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-F1 Merge `feature/polish-game` (cap 0.5d) | ≤0.5d | ⬜ | Only after S22-01; touches `WeaponHolderBase` — merge before S22-06 |
| S22-06 BUG-106 player half + BUG-110 | 0.2d | ⬜ | Same file as the polish branch |

### Thu 2026-11-12 — Effects and items
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-10 BUG-117 + BUG-120 + BUG-123 | 0.35d | ⬜ | Heal / damage and stat / coin items are no-ops |
| S22-11 BUG-115 asset cleanup | 0.1d | ⬜ | QA-STATIC-04 still FAILs |

### Fri 2026-11-13 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-12 One-liners | 0.15d | ⬜ | Carried 4–7 weeks |
| Buffer | 1d | — | — |

### Sat 2026-11-14 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri 23:00 QA run as input) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 25th carry — owner decision: record one sheet or sign accepted risk.
- S22-02 without S22-09 → `ON_PLAYER_DEATH` every frame.
- `feature/polish-game` (`38d40925`) changes `WeaponHolderBase` / `Weapon` — conflicts with S22-06 if
  done in parallel.
- Needs-owner notes NOTE-20261107-2/-3 (NREs at `PlayerState.cs:42`, `BaseStatsSO.cs:243`) — not tasks.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_
