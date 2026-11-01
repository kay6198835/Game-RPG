# Sprint 21 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-02 (Mon) -> 2026-11-06 (Fri) · playtest Sat 2026-11-07
> **Companion to**: `sprint-21.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-01 (kickoff, autonomous). Branch `sprint-21` from `sprint-20` tip (`ed2f984b`).
> Sprint 20 closed SLIPPED (Must-Have 0 / 1.3d, planned velocity 0.13, ≈1.5d off-plan output,
> playtest done: no). See `sprint-21.md` › Sprint 20 Closure.

---

## Status Verdict

**ON-TRACK — day 1 (Mon 2026-11-02), nothing due yet** (evidence: COMPILED + LOG). 4.0 working
days left / 2.9d planned work remaining (Must 1.45d + Should 0.95d + F1 cap 0.5d).

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.45d | 0 | 1.45d |
| Should Have | 0.95d | 0 | 0.95d |
| Feature story S21-F1 (capped, gated by S21-01 + S21-03) | 0.5d | 0 | 0.5d |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S21-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | **23rd** |
| S21-02 | BUG-116 `UnityEditor` out of runtime `CharacterData` | 0.1d | Must | new (S1) |
| S21-03 | BUG-121 `[FormerlySerializedAs]` for `authoredModifiers` | 0.1d | Must | new (P1) |
| S21-04 | BUG-108 finish + Knight check | 0.1d | Must | 2nd |
| S21-05 | BUG-096 zero-spawn branch | 0.1d | Must | 5th |
| S21-06 | Weapon trio BUG-106 / 107 / 110 | 0.3d | Must | 2nd |
| S21-07 | BUG-097 start / boss order | 0.1d | Must | 5th |
| S21-08 | Editor / data block (BUG-105, BUG-095, BUG-072 check) | 0.15d | Must | 5th |
| S21-09 | Process gate decision | 0.1d | Must | **11th** |
| — | Saturday playtest | 0.1d | Must | — |
| S21-10 | BUG-117 + BUG-120 | 0.3d | Should | new |
| S21-11 | BUG-065 + BUG-086 | 0.2d | Should | 12th / 7th |
| S21-12 | BUG-115 runtime stat copy + asset revert | 0.3d | Should | 2nd |
| S21-13 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 3rd–6th |
| S21-F1 | Effect-source work (capped) | ≤0.5d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-02 — Play first, then the two one-line P1 fixes
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-01 Playtest / smoke through the menu | 0.3d | ⬜ | playtest done: no; 23rd carry; QA automation now covers part of the sheet |
| S21-02 BUG-116 | 0.1d | ⬜ | S1 — every Player build fails |
| S21-03 BUG-121 | 0.1d | ⬜ | Must land before anyone re-saves stat / weapon assets |

### Tue 2026-11-03 — Dungeon S1 blockers
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-04 BUG-108 finish + Knight check | 0.1d | ⬜ | S1 — same pattern as S21-03, do it while the context is warm |
| S21-05 BUG-096 | 0.1d | ⬜ | S1 — non-combat rooms stay sealed |
| S21-07 BUG-097 | 0.1d | ⬜ | Run starts in the boss room |

### Wed 2026-11-04 — Weapon session
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-06 BUG-106 / 107 / 110 | 0.3d | ⬜ | One file (`WeaponHolderBase`), three bugs; enemies cannot attack |
| S21-08 Editor / data block | 0.15d | ⬜ | BUG-105 drops sword stage damage to 0 |

### Thu 2026-11-05 — Process gate + effect bugs
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-09 Process gate decision | 0.1d | ⬜ | 11th carry; third serialized-rename loss this month |
| S21-10 BUG-117 + BUG-120 | 0.3d | ⬜ | Heal / damage and stat items are no-ops |
| S21-F1 Effect-source work (cap 0.5d) | ≤0.5d | ⬜ | Only after S21-01 + S21-03 |

### Fri 2026-11-06 — Death path + cleanup
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-11 BUG-065 + BUG-086 | 0.2d | ⬜ | Death path; read NOTE-20261101-1/2 first |
| S21-12 BUG-115 | 0.3d | ⬜ | Every Play Mode session can dirty committed stat assets |
| S21-13 One-liners | 0.15d | ⬜ | Small, carried 3–6 weeks |
| Buffer | 1d | — | — |

### Sat 2026-11-07 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri 23:00 QA run as input) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 23rd carry — a third skipped Saturday is an owner decision, not a silent carry.
- NOTE-20261101-1/2 (S1 guess, RUNTIME 4 of 4 automated runs): player death never emits
  `ON_PLAYER_DEATH`, enemy at HP 0 never dies. Not tasks until Saturday triage.
- BUG-121 must be fixed before any stat / weapon asset is re-saved.
- BUG-115: check `git status` for dirty `Assets/SO/Stat/*.asset` after Play Mode until S21-12 lands.
- No standup on record for Tue 2026-10-27 and Fri 2026-10-30 — check the routine fires on time.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_

### 2026-11-02 (Mon) — standup 02:12, evidence COMPILED + LOG
- **Sync**: SKIPPED — owner branch `origin/feature/add-effect-component` (`f37d6202`) is already
  an ancestor of `origin/sprint-21`.
- **Since Fri**: no owner game commits; weekend commits are routines only (QA automation run
  `20261101-0221`, wrap-up duplicate skipped, kickoff `f37d6202`).
- **Compile**: `compile-check.sh` COMPILED (31 s) on `f37d6202` + 15 uncommitted owner paths
  (`AbilityHolderBase`, `CharacterData`, `EffectReceiverBase`, `StatHandlerBase`, `RegenComponent`,
  `IEffectReceiver`, `BuffDebuffStatsForDuration`, `RecoveryReductionPerTimeForDuration`,
  `DamagesInManyPos`, `BaseStatsSO`, …) — looks like S21-F1 effect-source work, which is gated
  behind S21-01 + S21-03. Uncommitted work is not counted as progress.
- **Editor log**: 0 `error CS`; 14 Play Mode entries (Editor-prev.log, 2026-10-31); no
  `Assets/Script/` exception. Inbox: 7 open notes, 0 new.
- **Fix-survival**: not run — no game `.cs` committed since Fri.
- **Today**: S21-01 playtest (0.3d) → S21-02 BUG-116 (0.1d) → S21-03 BUG-121 (0.1d).
