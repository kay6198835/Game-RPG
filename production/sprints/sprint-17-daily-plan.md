# Sprint 17 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-05 (Mon) -> 2026-10-09 (Fri)
> **Companion to**: `sprint-17.md` — day-by-day breakdown + live tracker
> **Daily routine: 10:00 → `/daily-standup`** · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-10-05 (kickoff, autonomous). Branch `sprint-17` from `sprint-16` tip (`96c65fa`).
> Sprint 16 closed PARTIAL (Must-Have 1/10, ≈0.01 planned velocity, ≈3.5d off-plan output, BUG-096 S1
> regression). See `sprint-16.md`.

---

## Status Verdict

**OPEN — day 1 of 5 (2026-10-05).** 0 / 8 Must-Have done. HEAD `96c65fa`, tree clean.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have | 1.25d | 0 | 1.25d |
| Should Have | 0.95d | 0 | 0.95d |
| Reserved (owner feature work) | ≈1.6d | 0 — **gated behind S17-04** | 1.6d |

---

## Day-by-Day Plan

### Mon 2026-10-05 — Unlock the loop, then play it
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-01 BUG-096 decision + restore no-spawn door branch | 0.05d | NOT STARTED | S1 — first room of every run sealed |
| S17-02 BUG-097 start/boss order | 0.05–0.2d | NOT STARTED | Run currently starts in the Boss room |
| S17-03 Editor block: `Lightning.prefab` layerMask + `Arrow.prefab` collider | 0.1d | NOT STARTED | Smoke is meaningless without them |
| S17-04 Play Mode smoke (logged or signed) | 0.3d | NOT STARTED | **11th carry. No feature commit before this** |

### Tue 2026-10-06 — Vitals + death path
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-05 BUG-092 residual | 0.05d | NOT STARTED | Every HoT/DoT silently one tick |
| S17-06 BUG-066+070 guard | 0.15d | NOT STARTED | Live death path |
| S17-07 BUG-065 + BUG-086 | 0.2d | NOT STARTED | One file; precondition for BUG-087 |
| S17-08 Process gate decision | 0.1d | NOT STARTED | BUG-096 showed the cost |

### Wed 2026-10-07 — Should-Have code
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-09 BUG-071 residual | 0.15d | NOT STARTED | Same file as S17-06 |
| S17-10 BUG-068 `:28` | 0.05d | NOT STARTED | Enemies run it on pooled objects |
| Reserved feature work | — | — | Opens once S17-04 is done |

### Thu 2026-10-08 — Editor cleanup + decisions
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-11 BUG-073+090 asset cleanup | 0.15d | NOT STARTED | Missing-script warnings |
| S17-13 Git LFS decision | 0.1d | NOT STARTED | Before more `.fbx` lands |
| S17-12 BUG-087 design note | 0.3d | NOT STARTED | Unblocked by S17-07 |

### Fri 2026-10-09 — Docs + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-14 `/doc-sync` | 0.2d | NOT STARTED | CLAUDE.md stale on input, DI, projectile |
| Re-run smoke if Mon–Thu commits touched Map/Combat | 0.1d | NOT STARTED | Retro process rule |
| Buffer | 1d | — | |

---

## Risks (live)

- Smoke at 11th carry — signed accepted-risk note beats a silent carry.
- BUG-096 owner decision may change the fix (non-combat door rule vs. restore `40d2c79`).
- `.fbx` growth without LFS.
- `gh` unavailable — draft PR for `sprint-17` not opened.
- `production/sprint-status.yaml` stale (Sprint 14).

---

## Daily Log

_(appended by `/daily-standup`)_
