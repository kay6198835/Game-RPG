# Sprint 19 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-19 (Mon) -> 2026-10-23 (Fri) · playtest Sat 2026-10-24
> **Companion to**: `sprint-19.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-10-19 (kickoff, autonomous; fired Monday, not Sunday). Branch `sprint-19` from
> `sprint-18` tip (`ea11e017`). Sprint 18 closed SLIPPED (Must-Have 0/8, planned velocity 0.0,
> ≈2.5–3d off-plan output, playtest done: no). See `sprint-18.md` › Closure.

---

## Status Verdict

**OPEN — day 1 of 5 (2026-10-19, Mon). BLOCKED at open — `sprint-19` inherits the non-compiling
`sprint-18` tip** (merge `f552f701`, NOTE-20261013-1). The owner's working tree compiles; S19-02
(commit the fix) unblocks everything. Evidence `STATIC` (+ last standup `LOG`).

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.4d | 0 | 1.4d |
| Should Have | 1.0d | 0 | 1.0d |
| Feature story S19-F1 (capped, gated by S19-01 + S19-02) | 0.5d | 0 | 0.5d |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S19-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | **16th** |
| S19-02 | Commit + push build fix | 0.05d | Must | 2nd |
| S19-03 | BUG-096 zero-spawn branch | 0.1d | Must | 3rd |
| S19-04 | BUG-097 start/boss order | 0.05–0.2d | Must | 3rd |
| S19-05 | Editor / data block (BUG-072 push, BUG-095, BUG-105) | 0.15d | Must | 3rd / new |
| S19-06 | BUG-092 + BUG-066/070 verify + seeding audit | 0.15d | Must | 5th / 10th |
| S19-07 | BUG-065 + BUG-086 | 0.2d | Must | 10th / 5th |
| S19-08 | Process gate decision | 0.1d | Must | **8th** |
| — | Saturday playtest | 0.1d | Must | — |
| S19-09 | BUG-100 buff source | 0.2d | Should | 2nd |
| S19-10 | BUG-071 residual | 0.15d | Should | 6th |
| S19-11 | BUG-068 `:28` + BUG-102 | 0.1d | Should | 4th / 2nd |
| S19-12 | BUG-103 + BUG-104 | 0.1d | Should | new |
| S19-13 | BUG-073 + BUG-090 | 0.15d | Should | 6th |
| S19-14 | BUG-087 design note | 0.3d | Should | 4th |
| S19-F1 | Class abilities / Champion equip onto `sprint-19` | ≤0.5d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-10-19 — Play first, then make the branch compile
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S19-01 Playtest / smoke through the menu | 0.3d | ⬜ | playtest done: no; 16th carry; working tree compiles, so it can run today |
| S19-02 Commit + push build fix | 0.05d | ⬜ | `sprint-19` tip does not compile; gates every code task |
| S19-03 BUG-096 zero-spawn branch | 0.1d | ⬜ | S1 — non-combat rooms sealed |
| S19-04 BUG-097 | 0.05–0.2d | ⬜ | Run starts in the wrong room |

### Tue 2026-10-20 — Data block + vitals verification
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S19-05 Editor / data block | 0.15d | ⬜ | BUG-105 halves sword damage; BUG-072 fix is sitting unpushed |
| S19-06 BUG-092 + BUG-066/070 verify | 0.15d | ⬜ | Code already on branch — needs one Play Mode check |
| S19-08 Process gate decision | 0.1d | ⬜ | 8th carry; 4th build break last week |

### Wed 2026-10-21 — Death path + vitals Should-Have
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S19-07 BUG-065 + BUG-086 | 0.2d | ⬜ | Last Must; precondition for BUG-087 |
| S19-09 BUG-100 | 0.2d | ⬜ | Same file as S19-06 — one session |
| S19-10 BUG-071 residual | 0.15d | ⬜ | Same file |

### Thu 2026-10-22 — Feature story + one-liners
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S19-F1 Class abilities / Champion equip (cap 0.5d) | ≤0.5d | ⏸️ gated on S19-01 + S19-02 | Brings `5c8245c9` / `cae75317` onto the sprint branch |
| S19-11 BUG-068 + BUG-102 | 0.1d | ⬜ | Two one-liners |
| S19-12 BUG-103 + BUG-104 | 0.1d | ⬜ | Two one-liners |

### Fri 2026-10-23 — Editor cleanup + design
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S19-13 BUG-073 + BUG-090 | 0.15d | ⬜ | Missing-script warnings |
| S19-14 BUG-087 design note | 0.3d | ⬜ | After S19-07 |
| Buffer | 1d | — | — |

### Sat 2026-10-24 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 16th carry — Monday first; a signed accepted-risk note beats a silent carry.
- `sprint-19` tip does not compile until S19-02 lands.
- Owner work spread over an unpushed local commit (`cae75317`), a remote feature branch (`5c8245c9`,
  `b364c303`) and the working tree — merge onto `sprint-19` in the order S19-02 → S19-05 → S19-F1.
- Saturday 2026-10-17 wrap-up and doc-sync did not run — 4 inbox notes untriaged; check the schedules.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_
