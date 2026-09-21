# Sprint 15 — Daily Plan & Progress Tracker

> **Sprint**: 2026-09-21 (Mon) -> 2026-09-25 (Fri)
> **Companion to**: `sprint-15.md` — day-by-day breakdown + live tracker
> **Routines**: Mon-Fri 10:00 `/daily-standup` · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-09-20 (Sunday 22:00 kickoff, autonomous). Branch `sprint-15` from `sprint-14` tip (`15242e6`).
> Sprint 14 closed FAIL (0/9 Must-Have). See `sprint-14.md`, `retro-sprint-14-2026-09-19.md`.

---

## Opening Block (checked first at every standup, before any other diff)

| # | Task | Est. | Status |
|---|------|------|--------|
| 1 | S15-01 BUG-063 | 0.05d | NOT STARTED |
| 2 | S15-02 BUG-064 item 7 | 0.1d | NOT STARTED |
| 3 | S15-03 BUG-065 | 0.1d | NOT STARTED |
| 4 | S15-04 BUG-066 + BUG-070 | 0.2d | NOT STARTED |

Combined 0.45d. Rule: no feature merge into `sprint-15` until all four are committed.

## Day-by-Day Plan

### Mon 2026-09-21 — Opening block + decisions
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-01, S15-02, S15-03 | 0.25d | NOT STARTED | Three isolated commits |
| S15-04 | 0.2d | NOT STARTED | Guard both vitals components |
| S15-06 / S15-07 decisions | 0.2d | NOT STARTED | Owner + producer, written into `sprint-15.md` |

### Tue 2026-09-22 — Abilities v2 S2 bugs
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-05 BUG-073 | 0.15d | NOT STARTED | Owner-assisted in Editor if needed |
| S15-08 BUG-071 | 0.15d | NOT STARTED | Same file as S15-04 |
| S15-09 hook placeholder | 0.1d | NOT STARTED | |

### Wed 2026-09-23 — Summon damage + first test
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-10 BUG-072 | 0.2d | NOT STARTED | |
| S15-12 first EditMode test | 0.3d | NOT STARTED | Needs S15-04 |

### Thu 2026-09-24 — Decisions and docs
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-11 TD-040 ADR | 0.3d | NOT STARTED | |
| S15-13 sign-off pass | 0.15d | NOT STARTED | |

### Fri 2026-09-25 — Doc sync + buffer
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-14 `/doc-sync` | 0.2d | NOT STARTED | |
| Nice-to-Have S15-N1..N3 | 0.3d | NOT STARTED | Buffer day |

---

## Standup Log

### Mon 2026-09-21 - Daily Standup (autonomous, no owner present)

Branch `sprint-15` (HEAD `d08a7d1`). Zero commits since the Sunday kickoff; working tree clean. The
weekend window (Sat 18:44 -> Mon) had no code activity.

**Opening Block re-verified against source: 0/4.**
- S15-01 BUG-063 - `Stat.cs:63-65` still `#if UNITY_EDITOR [SerializeField]` - NOT STARTED
- S15-02 BUG-064 item 7 - `RangeWeapon.cs` still no `[Inject]` - NOT STARTED
- S15-03 BUG-065 - `PlayerDeathState.Enter()` still only `base.Enter()` - NOT STARTED
- S15-04 BUG-066+070 - no `TryGetValue` in `EntityVitalStats.cs` or `VitalComponent.cs` - NOT STARTED

**Today (Mon), estimates:**

| Task | Est. | Complexity / Risk |
|------|------|-------------------|
| S15-01, S15-02, S15-03 (3 isolated commits) | 0.25d | Low; pattern exists (`ItemSpawner.cs:8-10`) |
| S15-04 guard both vitals components | 0.2d | Low-Med; 2 files, ~7 sites |
| S15-06 / S15-07 written decisions | 0.2d | Owner-dependent |

Blockers: none technical; S15-06/07 need owner sign-off. Risks: opening block loses to feature work a 5th
time; `feature/fix-player-control` remains the source of off-plan merges; ~230 binary assets merged last
week (size/LFS decision pending, S15-N3); `gh` unavailable so the sprint-15 draft PR is still not created.

---

_(appended by `/daily-standup`)_

## Carry-over Watchlist

- BUG-063 29th+ carry; BUG-064-7 / 065 / 066+070 5th carry
- S14-07 Play Mode: 8th sprint — decision required (S15-06)
- QA plan 32nd+ cycle — owner decision
