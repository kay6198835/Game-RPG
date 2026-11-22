# Sprint 24 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-23 (Mon) -> 2026-11-27 (Fri) · playtest Sat 2026-11-28
> **Companion to**: `sprint-24.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-22 (kickoff, autonomous). Branch `sprint-24` from `sprint-23` tip (`69f59a3b`).
> Sprint 23 closed SLIPPED (Must 0.2d / 0.95d, velocity 0.43, ≈2.0d off-plan output, playtest
> done: no). See `sprint-24.md` › Sprint 23 Closure.

---

## Status Verdict

**ON-TRACK (day 0, kickoff).** Nothing started. First task Monday: S24-01 playtest, then S24-02
BUG-135 with a same-day automated QA rerun.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 0.95d | 0 | 0.95d |
| Should Have | 0.65d | 0 | 0.65d |
| Feature story S24-F1 (capped) | 0.75d | 0 | 0.75d |
| Off-plan | — | 0 | — |
| Buffer | 1.0d | 0 | 1.0d |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S24-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | **28th** |
| S24-02 | BUG-135 receiver interface + same-day QA rerun | 0.1d | Must | new (S1) |
| S24-03 | BUG-122 remainder + BUG-086 + BUG-065 (death → Game Over) | 0.2d | Must | 3rd / 10th / **16th** |
| S24-04 | BUG-130 dash soft lock | 0.2d | Must | new (S2) |
| S24-05 | BUG-126 `using UnityEditor` guard | 0.05d | Must | 2nd |
| — | Saturday playtest | 0.1d | Must | — |
| S24-06 | BUG-127 Archer + Mage bolt data | 0.05d | Should | 2nd |
| S24-07 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 6th–9th |
| S24-08 | BUG-087 death / restart design note | 0.3d | Should | **9th** |
| S24-09 | BUG-096 remainder (doors without spawn marker) | 0.15d | Should | **9th** |
| S24-F1 | Erika animation / dash VFX assets, own commits (capped) | ≤0.75d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-23 — Play first, then restore damage
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-01 Playtest / smoke through the menu (or sign accepted risk) | 0.3d | ⬜ | playtest done: no; 28th carry; BUG-135 FAILs are valid evidence |
| S24-02 BUG-135 + automated QA rerun | 0.1d | ⬜ | S1: no hit damages anyone since `8005d27e`; blocks every combat check |
| S24-05 BUG-126 | 0.05d | ⬜ | One line; blocks any Player build; carried once |

### Tue 2026-11-24 — Death → Game Over
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-03 BUG-122 remainder + BUG-086 + BUG-065 | 0.2d | ⬜ | S1; needs damage back (S24-02); core of the demo loop |
| S24-04 BUG-130 dash soft lock (check `1e5afb3c` first) | 0.2d | ⬜ | S2; Space locks the player in every class |

### Wed 2026-11-25 — Restart design + rooms
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-08 BUG-087 design note | 0.3d | ⬜ | Death reaches Game Over after S24-03; restart path undefined for 9 sprints |
| S24-09 BUG-096 remainder | 0.15d | ⬜ | S1 soft lock in marker-less rooms |

### Thu 2026-11-26 — Owner feature
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-F1 Erika / dash VFX assets, own commits | ≤0.75d | ⬜ | Declared feature; uncommitted Erika work exists; keep separate from code |

### Fri 2026-11-27 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-06 BUG-127 | 0.05d | ⬜ | Ranged basic attack barely reaches |
| S24-07 One-liners | 0.15d | ⬜ | Carried 6–9 weeks |
| Buffer | 1d | — | Off-plan work |

### Sat 2026-11-28 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri QA run as input) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 28th carry — owner decision: record one sheet or sign accepted risk.
- BUG-135 blocks all combat checks until S24-02 lands; rerun the automated suite the same day.
- `1e5afb3c` (Sun, after wrap-up) changed dash / spawn code untriaged — the Monday QA rerun covers it.
- Large mixed asset + code commits (`8005d27e` pattern) — keep S24-F1 asset-only.
- PM worktree `D:/Fork/Game-RPG-pm` dirty with `Assets/Sprite/Erika/**` — owner should commit or clean.
- Needs-owner notes (NOTE-20261115-7 item 3, NOTE-20261117-4, NOTE-20261122-1) — not tasks.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_
