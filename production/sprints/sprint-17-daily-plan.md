# Sprint 17 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-05 (Mon) -> 2026-10-09 (Fri)
> **Companion to**: `sprint-17.md` — day-by-day breakdown + live tracker
> **Daily routine: 10:00 → `/daily-standup`** · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-10-05 (kickoff, autonomous). Branch `sprint-17` from `sprint-16` tip (`96c65fa`).
> Sprint 16 closed PARTIAL (Must-Have 1/10, ≈0.01 planned velocity, ≈3.5d off-plan output, BUG-096 S1
> regression). See `sprint-16.md`.

---

## Status Verdict

**AT RISK — day 2 of 5 (2026-10-06, standup).** 0 / 8 Must-Have done, 0 blocked. HEAD `8da09b1`.
Working tree carries 4 uncommitted owner `.cs` edits (StatModifierGroup refactor — see log), not staged.
Mon block (S17-01..04) did not land; ≈1.6d of off-plan UIFlow work landed instead, **ahead of the S17-04
gate**. S17-14 (`/doc-sync`) closed off-schedule. Must-Have remaining 1.25d with 4 days left — still
fits, but only if today runs the Mon block before anything else.

*Previous verdict (2026-10-05):* OPEN — day 1 of 5, 0 / 8 Must-Have.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have | 1.25d | 0 | 1.25d |
| Should Have | 0.95d | 0.2d (S17-14) | 0.75d |
| Reserved (owner feature work) | ≈1.6d | ≈1.6d consumed — UIFlow real-game wiring (`cb0de496`, `8535b2fb`, `c511c538`) + Paladin assets (`f30b8343`), **landed before S17-04** | ≈0 |

---

## Day-by-Day Plan

### Mon 2026-10-05 — Unlock the loop, then play it
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S17-01 BUG-096 decision + restore no-spawn door branch | 0.05d | **CARRIED → Tue** (not started; `LoadRoom()` still has no `spawnPositions.Count == 0` branch) | S1 — first room of every run sealed |
| S17-02 BUG-097 start/boss order | 0.05–0.2d | **CARRIED → Tue** (`Maze_Storage.asset` untouched since `ac13ee4`) | Run currently starts in the Boss room |
| S17-03 Editor block: `Lightning.prefab` layerMask + `Arrow.prefab` collider | 0.1d | **CARRIED → Tue** (neither field serialized) | Smoke is meaningless without them |
| S17-04 Play Mode smoke (logged or signed) | 0.3d | **CARRIED → Tue** (no playtest log since 2026-06-12) | **12th carry. Gate was bypassed by UIFlow commits** |
| *(off-plan)* UIFlow: real game from menu + HUD bound to player; mock removal | — | DONE (`cb0de496`, `8535b2fb`, `c511c538`) | Reserved bucket, consumed early |
| *(off-plan)* Paladin asset + ability stats update | — | DONE (`f30b8343`) | Reserved bucket |

### Tue 2026-10-06 — Mon block (carried) first, then vitals + death path
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
| S17-14 `/doc-sync` | 0.2d | ✅ DONE Mon (`7d1b5c79` + CLAUDE.md header from the `cb0de496` merge) | CLAUDE.md stale on input, DI, projectile |
| Re-run smoke if Mon–Thu commits touched Map/Combat | 0.1d | NOT STARTED | Retro process rule |
| Buffer | 1d | — | |

---

## Risks (live)

- Smoke at 11th carry — signed accepted-risk note beats a silent carry.
- BUG-096 owner decision may change the fix (non-combat door rule vs. restore `40d2c79`).
- `.fbx` growth without LFS.
- `gh` unavailable — draft PR for `sprint-17` not opened.
- `production/sprint-status.yaml` stale (Sprint 14).
- **New 2026-10-06:** S17-04 gate bypassed — UIFlow now loads `LoadRandomMap` from `MainGamePlay`, so the
  smoke must be run **through the menu** (New Game → Loading → LoadRandomMap + GameplayUI), not only by
  opening `LoadRandomMap` directly. Scope +≈0.05d.
- **New 2026-10-06:** `RealPlayerDataProvider` is now the only `ON_PLAYER_DEATH` subscriber and relies on a
  `_deathReported` flag to survive BUG-086. S17-07 must keep that path working (verify Respawn after fix).
- **New 2026-10-06:** uncommitted owner refactor in `VitalStatsBase.cs` overlaps S17-06 / S17-09 (same
  file) — commit it before starting those, to avoid a self-conflict.

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

### 2026-10-06 (Tue) — standup, day 2 of 5 (autonomous run, 02:00 slot)

**Yesterday (Mon 2026-10-05) — heavy, almost entirely off-plan.** `sprint-17` advanced from `d3a57f0` to
`8da09b1` via merges of `origin/feature/ui-flow-maingameplay` and `origin/feature/fix-player-control`
(399 files, +61.8k / −18.6k, mostly UIFlow scenes/prefabs). Notable:
- `cb0de496` — UIFlow runs the real game: New Game / Continue → `LoadRandomMap` + `GameplayUI`;
  `RealPlayerDataProvider` binds HP/Mana, stats, 4-slot hotbar, cooldowns. Adds `ON_PLAYER_READY`
  (EventID → 24), `IVitalComponent.CurrentStatsChanged`, `AbilityHolderBase.AbilityCooldownStarted`.
  Deletes `StartScene`, `UISample`, `MainMenu.cs`, legacy `UIManager.cs` (TD-017 stub).
- `8535b2fb` / `c511c538` — UIFlow mock providers + mock scene removed; `WorldHealthBar` kept.
- `7d1b5c79` — doc sync to `93ba6d8e`, per-system docs. **Closes S17-14.**
- `f30b8343` — Paladin asset / ability stats update.
- `RoomGridController.cs` — minimap start cell now (Column, Row) — small map fix, not BUG-096.
- `255546a1` — monthly module quality audit (PM).

Non-UIFlow `.cs` delta is small (10 files, +24/−29) and touches none of S17-01/05/06/07.
Assessment: real product progress (menu → game → HUD loop now exists end to end), but **zero Must-Have
progress** and the "no feature commit before S17-04" gate was broken on day 1 — the exact retro pattern.

**Working tree (uncommitted, owner WIP — not staged by this run):** `VitalStatsBase.cs`, `Weapon.cs`,
`StatModifierGroup.cs` (−30 lines), `StatModifierTester.cs` — callers switch from
`StatModifierGroup.Apply/Remmove(...)` to direct `AddModifiersFromSource(this, group.Modifiers)` /
`RemoveModifiersFromSource(this)`. Not compiled or reviewed by this run.

**Re-check against source (read only, HEAD `8da09b1`):**
- BUG-096 — OPEN. `RoomGeneraterController.LoadRoom()` (`:134-144`) still emits `ON_GET_SPAWN_POSITIONS`
  for an uncleared room regardless of marker count; no `OpenDoors()` on the zero-spawn path.
- BUG-097 — `Maze_Storage.asset` last touched in `ac13ee4` (2026-10-04); not fixed.
- BUG-072 — `Lightning.prefab` serializes no `layerMask` → 0 (Nothing). OPEN.
- BUG-095 (Arrow) — `Arrow.prefab` has no `Collider2D`. OPEN.
- BUG-092 residual — `perTime` / `timeCount` still `private`, no `[SerializeField]`. OPEN.
- BUG-066+070 — `VitalStatsBase.cs` raw `currentStats[statType]` reads unchanged. OPEN.
- BUG-065 / BUG-086 — `PlayerDeathState` unchanged (Enter = base only; Emit every frame). OPEN.
- BUG-087 — PARTIAL (one subscriber now: `RealPlayerDataProvider`).

**Tracker:** S17-14 → DONE. S17-01..04 → CARRIED to Tue. Others NOT STARTED. Nothing blocked externally.

**Today (Tue) — Mon block first, then Tue block:**
| # | Task | Est. | Basis |
|---|------|------|-------|
| 0 | Commit (or stash) the StatModifierGroup WIP after one Editor compile | 0.05d | 4 files, mechanical; clears the way for S17-06 |
| 1 | S17-01 BUG-096 — decide + add zero-spawn branch (`BuildGrid` + `OpenDoors()`) | 0.05d | ~6 lines, one file, pattern existed in `40d2c79` |
| 2 | S17-02 BUG-097 — reorder `Maze_Storage.asset` (stop-gap) | 0.05d | Inspector drag; `RoomType` select = 0.2d, defer |
| 3 | S17-03 `Lightning.prefab` layerMask + `Arrow.prefab` Collider2D | 0.1d | Two Inspector edits; confirm in `git diff` |
| 4 | S17-04 Play Mode smoke **via MainGamePlay menu** → log in `production/qa/playtests/` | 0.35d | 9 items + menu→load→HUD path; 12th carry — log it or sign accepted-risk |
| 5 | S17-05 BUG-092 residual | 0.05d | Two attributes, one file |
| 6 | S17-06 BUG-066+070 guard | 0.15d | 7 reads in one file + `Reborn()` seeding audit; risk: missing HP key ≠ death |
| 7 | S17-07 BUG-065 + BUG-086 | 0.2d | One file; must not break `RealPlayerDataProvider` respawn |
| 8 | S17-08 process gate decision | 0.1d | Write-up only; yesterday is the concrete case |

Total ≈ 1.1d — more than one session. If time is short: do 0→4 (≈0.6d), push 5-8 to Wed (Wed has ≈0.2d planned).

**Blockers:** none external. Items 0-4 need the owner in the Unity Editor.

**Risks (emerging):**
- Smoke now **12th** carry, and the gate meant to protect it failed on day 1. If it slips again today,
  write the signed accepted-risk note today rather than Friday.
- Large merged scene/prefab churn (UIFlow scenes + `LoadRandomMap.unity`) with no recorded Play Mode run;
  merge-conflict risk on any parallel branch touching scenes.
- `MainGamePlay` is now build index 0 — a broken menu path hides every gameplay bug behind it.
- Uncommitted refactor in shared stat code (`VitalStatsBase`, `Weapon`) — compile-break risk (TD-048,
  three breaks in three weeks) if pushed without an Editor compile.
- Off-plan velocity ≫ planned velocity again (≈1.6d vs 0) — 5th sprint running.
