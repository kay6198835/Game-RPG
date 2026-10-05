# Sprint 17 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-05 (Mon) -> 2026-10-09 (Fri)
> **Companion to**: `sprint-17.md` — day-by-day breakdown + live tracker
> **Daily routine: 10:00 → `/daily-standup`** · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-10-05 (kickoff, autonomous). Branch `sprint-17` from `sprint-16` tip (`96c65fa`).
> Sprint 16 closed PARTIAL (Must-Have 1/10, ≈0.01 planned velocity, ≈3.5d off-plan output, BUG-096 S1
> regression). See `sprint-16.md`.

---

## Status Verdict

**OPEN — day 1 of 5 (2026-10-05, standup).** 0 / 8 Must-Have done, 0 blocked. HEAD `d3a57f0`, tree clean.
On track only if the Mon block (S17-01..04, ≈0.5d) lands today.

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

### 2026-10-05 (Mon) — standup, day 1 of 5 (autonomous run, 02:00 slot)

**Yesterday (Sun 2026-10-04) — off-plan, Sprint 16 scope, already scored by the wrap-up.**
Six commits (`8b778fd`…`ac13ee4`): room set rebuilt as 20 English-named rooms (start → boss),
`NormalRoom_13-27` registered in `Maze_Storage.asset`, old room data deleted, and `ac13ee4`
"done load map." (8 files, `LoadRandomMap.unity` +18 986 lines churn, `RoomGeneraterController.cs`
±9). `ac13ee4` is the commit that removed the no-spawn door branch added in `40d2c79` — that is
**BUG-096**. No `.cs` change after it. Assessment: real progress on map content, but it landed the
S1 regression the sprint opens on, and no Play Mode load is recorded for it.

**Since kickoff (`d3a57f0`):** no commits. Tree clean, `sprint-17` in sync with `origin/sprint-17`.

**Re-check against source (read only):**
- BUG-096 — still OPEN. `RoomGeneraterController.LoadRoom()` has no `spawnPositions.Count == 0`
  branch; an uncleared room with zero `SPAWN` markers emits `ON_GET_SPAWN_POSITIONS` with an empty
  list and never calls `OpenDoors()`.
- BUG-097 — not re-verifiable from text alone (order of `Maze_Storage.asset` vs `room[0]/room[last]`);
  carried as recorded by the wrap-up.
- No playtest log since 2026-06-12 in `production/qa/playtests/`.

**Tracker:** all 14 tasks NOT STARTED (expected — sprint opened today). No item moved to blocked.

**Today (Mon) — order matters:**
| # | Task | Est. | Basis |
|---|------|------|-------|
| 1 | S17-01 BUG-096 — decide (restore `40d2c79` branch vs. non-combat door rule), then code | 0.05d (≈20 min) | ≈6 lines, one file, pattern already existed in `40d2c79`; risk = decision, not code |
| 2 | S17-02 BUG-097 — reorder `Maze_Storage.asset` (stop-gap) | 0.05d | Inspector drag; `RoomType` selection = 0.2d, defer unless Bug #16 is wanted now |
| 3 | S17-03 `Lightning.prefab` layerMask + `Arrow.prefab` Collider2D | 0.1d | Two Inspector fields; confirm in `git diff` |
| 4 | S17-04 Play Mode smoke, logged in `production/qa/playtests/` | 0.3d | 9 checklist items; one run + notes. **Gate for all feature work** |
Total ≈ 0.5d — fits one session with margin.

**Blockers:** none external. Everything today needs the owner in the Unity Editor (no agent can
open Play Mode). S17-04 depends on S17-01..03.

**Risks (emerging):**
- Smoke now at **11th** carry; if Monday ends without it, write the signed accepted-risk note
  rather than carry silently.
- `ac13ee4` pattern (a "done" commit that reverts a fix, no Play Mode load) — S17-08 rule should
  land this week, not Thursday.
- Yesterday's `LoadRandomMap.unity` churn (≈19k lines) raises scene merge-conflict risk if any
  other branch touches the scene.
