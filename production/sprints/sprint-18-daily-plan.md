# Sprint 18 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-12 (Mon) -> 2026-10-16 (Fri) · playtest Sat 2026-10-17
> **Companion to**: `sprint-18.md` — day-by-day breakdown + live tracker
> **Daily routine: 10:00 → `/daily-standup`** · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-10-09 (kickoff, autonomous). Branch `sprint-18` from `sprint-17` tip (`d8962b8d`).
> Sprint 17 closed SLIPPED (Must-Have 0/8, planned velocity 0.05, ≈2.6–3.1d off-plan output,
> playtest done: no). See `sprint-17.md`.

---

## Status Verdict

**OPEN — day 0 of 5 (2026-10-09, kickoff).** 0 / 8 Must-Have. Evidence `STATIC`.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.3d | 0 | 1.3d |
| Should Have | 1.0d | 0 | 1.0d |
| Feature story S18-F1 (capped, gated by S18-01) | 0.5d | 0 | 0.5d |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S18-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | 15th |
| S18-02 | BUG-096 merge `221d54be` + zero-spawn branch | 0.1d | Must | 2nd |
| S18-03 | BUG-097 start/boss order | 0.05–0.2d | Must | 2nd |
| S18-04 | Lightning layerMask + Arrow collider | 0.1d | Must | 2nd |
| S18-05 | BUG-092 residual | 0.05d | Must | 4th |
| S18-06 | BUG-066 + BUG-070 | 0.15d | Must | 9th |
| S18-07 | BUG-065 + BUG-086 | 0.2d | Must | 9th |
| S18-08 | Process gate decision | 0.1d | Must | 7th |
| — | Saturday playtest | 0.1d | Must | — |
| S18-09 | BUG-100 buff source | 0.2d | Should | new |
| S18-10 | BUG-071 residual | 0.15d | Should | 5th |
| S18-11 | BUG-068 `:28` + BUG-102 | 0.1d | Should | 3rd / new |
| S18-12 | BUG-073 + BUG-090 | 0.15d | Should | 5th |
| S18-13 | Git LFS decision | 0.1d | Should | 2nd |
| S18-14 | BUG-087 design note | 0.3d | Should | 3rd |
| S18-F1 | Start room / Champion / shop feature | ≤0.5d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-10-12 — Play first, then unlock the loop
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S18-01 Playtest / smoke through the menu | 0.3d | ⬜ | playtest done: no; 15th carry; gates all feature work |
| S18-02 BUG-096 on `sprint-18` | 0.1d | ⬜ | S1 — non-combat rooms sealed |
| S18-03 BUG-097 | 0.05–0.2d | ⬜ | Run starts in the wrong room |
| S18-04 Editor block | 0.1d | ⬜ | Abilities / ranged weapon untestable without it |

### Tue 2026-10-13 — Vitals + death path (one file session)
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S18-05 BUG-092 residual | 0.05d | ⬜ | Every HoT/DoT one tick |
| S18-06 BUG-066 + BUG-070 | 0.15d | ⬜ | Live death path; precondition for S18-09/10 |
| S18-07 BUG-065 + BUG-086 | 0.2d | ⬜ | Precondition for BUG-087 |
| S18-08 Process gate decision | 0.1d | ⬜ | 7th carry — decide or drop |

### Wed 2026-10-14 — Should-Have code
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S18-09 BUG-100 | 0.2d | ⬜ | S2; same file as S18-06 |
| S18-10 BUG-071 residual | 0.15d | ⬜ | Same file |
| S18-11 BUG-068 + BUG-102 | 0.1d | ⬜ | Two one-liners |

### Thu 2026-10-15 — Feature story + Editor cleanup
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S18-F1 Start room / Champion / shop (cap 0.5d) | ≤0.5d | ⬜ | Only after S18-01 is filled |
| S18-12 BUG-073 + BUG-090 | 0.15d | ⬜ | Missing-script warnings |
| S18-13 Git LFS decision | 0.1d | ⬜ | Before more `.fbx` |

### Fri 2026-10-16 — Design + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S18-14 BUG-087 design note | 0.3d | ⬜ | Unblocked by S18-07 |
| Buffer | 1d | — | |

### Sat 2026-10-17 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` — not present on `sprint-18`; use the `playtest-sheet.sh` sheet) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Smoke at 15th carry — Monday first, signed accepted-risk beats a silent carry.
- BUG-096 partial fix lives on `feature/update-bug-document` only.
- `VitalStatsBase.cs` is the target of four tasks — do them in one session, in order.
- `pm-routine-protocol.md` not on the sprint branch.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_
