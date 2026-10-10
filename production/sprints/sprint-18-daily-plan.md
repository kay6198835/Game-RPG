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

**Weekly wrap-up 2026-10-10 (Sat) — verdict for the week ending 2026-10-10: SLIPPED** (evidence
`LOG` + `STATIC`; capped at CONCERNS at best — playtest sheet empty). No `.cs` commit since the
2026-10-09 wrap-up; Sprint 17 already closed SLIPPED (0/8 Must, velocity 0.05). Sprint 18 work has not
started (begins Mon 2026-10-12).

**Handoff to Sunday `/weekly-kickoff`:**
- **playtest done: no** (15th consecutive week without runtime evidence) → S18-01 stays first on Monday
- **velocity: 0.05** (unchanged; no estimate-days completed since the 2026-10-09 close)
- **carry-over:** the whole Sprint 18 plan as written, plus newly confirmed **BUG-105** (S2, P1 —
  `[FormerlySerializedAs("attackDamege")]` on `AttackSO.attackDamage` + re-save `SnS_State1-3.asset`;
  ≈0.05d, recommend Must-Have next to S18-04) and **BUG-103** (S3, ranged `RecoveryTime` dead; bundle
  with S18-04) and **BUG-104** (S4, bundle with any `PlayerState` edit)
- **status changes to know:** BUG-064 now FIXED in code (awaiting playtest A6); BUG-087 now PARTIAL —
  S18-14's design note starts from `RealPlayerDataProvider.Respawn()` (scene reload), not from zero
- `221d54be` (BUG-096 start-room half) is already on `sprint-18` via `c6cbc57a` — S18-02 shrinks to the
  zero-spawn branch in `LoadRoom()`

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

### 2026-10-10 (Sat) — `/weekly-wrapup --auto`

- **Code review:** 0 `.cs` files changed since the 2026-10-09 wrap-up (all branches). Nothing to review.
- **Playtest:** `playtest-2026-10-10.md` present but empty → `NOT RUN`. Editor was opened twice today
  (Play Mode once per session) — `editor-log.sh` clean: 0 compile errors, no project exception.
- **Fix survival (30 d):** 16 PRESENT / 4 PARTIAL / 1 GONE — all already triaged 2026-10-09.
- **Inbox triage:** 4 notes → 3 confirmed (**BUG-103** S3, **BUG-104** S4, **BUG-105** S2), 1 status
  update (BUG-064 → fixed in code, BUG-087 → partial); NOTE-2 closed (not reproduced); NOTE-6 carried
  (needs owner). Report: `production/qa/bug-triage-2026-10-10.md`.
- **Doc drift:** 92 STALE claims of 587; 0 bug-ID status mismatches → `/doc-sync` 23:00.
- **Retro:** went well — inbox flow worked end to end (notes written Fri, IDs allocated once, Sat);
  slipped — playtest (15th week), all Sprint 17 Must-Haves; improve — the routines fired early on
  Friday 2026-10-09 (wrap-up, doc-sync, kickoff all ran Fri), so Saturday's run had no new week to
  close: check the schedule times of `pm-weekly-wrapup` / `pm-weekly-kickoff`.
- **Verdict:** SLIPPED (`LOG`). Handoff above under Status Verdict.
