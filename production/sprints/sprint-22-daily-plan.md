# Sprint 22 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-09 (Mon) -> 2026-11-13 (Fri) · playtest Sat 2026-11-14
> **Companion to**: `sprint-22.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-08 (kickoff, autonomous). Branch `sprint-22` from `sprint-21` tip (`d2d2b22e`).
> Sprint 21 closed SLIPPED (Must 0.3d / 1.45d, velocity 0.08, ≈1.7d off-plan output, playtest
> done: no). See `sprint-22.md` › Sprint 21 Closure.

---

## Status Verdict

**SLIPPED — day 3 (Wed 2026-11-11, 02:00)** — no commit on any branch on Tuesday (only the standup
commit `5e30cf72`), so no sprint task moved: S22-01 playtest (25th carry), S22-02 BUG-122 (S1),
S22-09 and Tuesday's room tasks S22-04 / S22-03 / S22-07 are all still ⬜. Must burned 0.05d / 1.05d
with 3 days left; 1.0d Must + 0.8d Should remain against 3 days + 1d buffer — still feasible, but
the buffer is now the plan. The owner's folder holds uncommitted room-interaction work
(`InteractiveObjectsRoom.cs` new, `PedestalChampionController`, `UIEvents`, `GameplayUIController`,
`Entity.cs`, `LoadRandomMap.unity`) — off-plan (weekly to-do, room unification), not counted.
Compile COMPILED at `58601f57` + 9 uncommitted owner paths (34 s); Editor log 1 distinct transient
`error CS0534` (NOTE-20261111-1), 0 exceptions. Evidence: COMPILED + LOG + STATIC.

*Was:* SLIPPED — day 2 (Tue 2026-11-10, 02:00) — none of Monday's three tasks (S22-01 playtest,
S22-02 BUG-122, S22-09) moved; Monday went to off-plan work (stat split `a0e250fb`, item key
migration `094ec9df`) plus S22-05 done early (`b33dedf9`). Must burned 0.05d / 1.05d with 4 days
left; the work still fits the 1d buffer, but the S1 death fix and the 25th-carry playtest have not
started. Compile COMPILED at `58601f57`; Editor log 0 errors. Evidence: COMPILED + LOG + STATIC.

*Was:* ON-TRACK — day 1 (Mon 2026-11-09). *Before that:* ON-TRACK — day 0. Evidence: STATIC.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.05d | 0.05d (S22-05) | 1.0d |
| Should Have | 0.8d | 0 (S22-11 looks done by `a0e250fb`, unverified) | 0.8d |
| Off-plan (2026-11-09) | — | ≈1d (stat split, item key migration, anim-viewer tools) | — |
| Feature story S22-F1 (capped, gated by S22-01) | 0.5d | 0.5d (merged by sync) | 0 (compile / runtime check rides on S22-01) |
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
| S22-01 Playtest / smoke through the menu | 0.3d | ⬜ → carried to Tue | playtest done: no; 25th carry; seven bugs wait on runtime confirmation |
| S22-02 BUG-122 | 0.1d | ⬜ → carried to Tue | S1 P1; QA-VIT-07 failed 10 runs in a row |
| S22-09 BUG-065 + BUG-086 | 0.2d | ⬜ → carried to Tue | Same file as S22-02; fixing BUG-122 makes BUG-086 fire every frame |

### Tue 2026-11-10 — Room path
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-04 BUG-097 | 0.1d | ⬜ → carried to Wed | Run starts in the boss room; 6th carry |
| S22-03 BUG-096 Boss room | 0.05d | ⬜ → carried to Wed | Last sealed room type |
| S22-05 BUG-124 | 0.05d | ✅ 2026-11-09 `b33dedf9` (`Pool.Reload()` places before `SetActive(true)`; awaiting runtime) | One-line ordering fix; every pooled re-spawn |
| S22-07 Editor / data block | 0.15d | ⬜ → carried to Wed | Inspector-only work, 6th carry |

### Wed 2026-11-11 — Weapon + owner branch
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-F1 Merge `feature/polish-game` (cap 0.5d) | ≤0.5d | ✅ merged 2026-11-09 by the standup sync (`a5d671c2`: `38d40925`, `0f3644bd`, `bd75d17c`) — not compile-checked; verify in S22-01 | Touches `WeaponHolderBase` — now merged before S22-06 as planned |
| S22-06 BUG-106 player half + BUG-110 | 0.2d | ⬜ | Same file as the polish branch |

### Thu 2026-11-12 — Effects and items
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S22-10 BUG-117 + BUG-120 + BUG-123 | 0.35d | ⬜ | Heal / damage and stat / coin items are no-ops |
| S22-11 BUG-115 asset cleanup | 0.1d | 🟡 `a0e250fb` rewrote the player stat assets as data only (modifier lists gone); confirm with QA-STATIC-04 | QA-STATIC-04 still FAILs |

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
- (2026-11-10) Owner's `production/weekly-todo-2026-11-09.md` (`4e14862b`) adds feature work not in
  this sprint (room unification + level expansion, boss, bonfire, dash, animation review). Item 1
  (death → replay) matches S22-02 / S22-09; the rest is scope beyond the plan and the 1d buffer.

---

## Daily Log

_(appended by `/daily-standup`)_

### 2026-11-09 (Mon) — standup 02:00, evidence LOG + STATIC
- **Sync**: merged owner branch `origin/feature/polish-game` (local ref
  `refs/heads/origin/feature/polish-game`, 3 commits: `38d40925` Paladin combo attack,
  `0f3644bd` Mage animation + weapon attack, `bd75d17c` save asset) into `sprint-22` → `a5d671c2`.
  This is S22-F1; it landed before S22-01 (the plan's gate) because the standup sync is owner-mandated.
- **Weekend**: the three commits above (off-plan for sprint 21, planned as S22-F1). Gameplay changes:
  `WeaponHolderBase` lazy `Input` / `StatService` (null-safe damage), `Weapon.aim` read through the
  holder + `chainWindowGrace` 0.15 s, `PlayerAttackState` chain moved from `AnimationFinishTrigger` to
  `AnimationEnd`, enemy idle/move "comfort range" hold, `EntityInput.ReloadComponent()` clears the
  lock. New Mage weapon data (`MageWeapon`, 3 attack stages, `MagicBolt.prefab`).
- Owner also has a large uncommitted stat-split change set (stash `5ba9091f` "pre-stat-split",
  2026-11-09 01:08) — invisible to this run; not a sprint task.
- **Runtime**: compile SKIPPED (Editor open); Editor.log 0 `error CS`, 0 exceptions with an
  `Assets/Script/` frame (11 Play Mode entries). Fix-survival 7 d: BUG-106 / BUG-107 PRESENT.
- **Today**: S22-01 playtest (25th carry) → S22-02 BUG-122 → S22-09 BUG-065 + BUG-086. No new inbox notes.

### 2026-11-10 (Tue) — standup 02:00, evidence COMPILED + LOG + STATIC
- **Sync**: SKIPPED — owner branch `sprint-22` already contained in `origin/sprint-22` (`58601f57`).
- **Yesterday** (after the 02:16 standup, all by the owner, merged via `58601f57`):
  `a0e250fb` stat split (`BaseStatsSO` data only + `StatSheet` calculator; 15 stat assets migrated;
  Paladin INT 100→10, Boss HP 4450→1080) — off-plan, closes the runtime path of BUG-063 / BUG-115;
  `b33dedf9` BUG-124 pool ordering + pooled-state reset on `ReloadComponent()` + enemy Defense as
  `damage * 100 / (100 + Defense)` (= S22-05); `094ec9df` ItemSO key migration + audit script
  (off-plan, 4th serialized-rename loss caught statically); `b47f7a62` anim-viewer tools;
  `4e14862b` owner weekly to-do.
- **Analysis**: Monday planned 0.6d (S22-01 / 02 / 09), burned 0 of it; 0.05d Must done early (S22-05);
  ≈1d off-plan. Verdict SLIPPED — the S1 death fix and the playtest did not start.
- **Runtime**: compile COMPILED at `58601f57` (+4 uncommitted owner paths, 42 s); Editor.log /
  Editor-prev.log 0 `error CS`, 0 exceptions with an `Assets/Script/` frame (12 Play Mode entries).
  Fix-survival 7 d: BUG-106 PRESENT; BUG-107 / 115 / 116 / 121 PARTIAL (76 %) — all four cite the
  multi-file commit `1370136c`, re-touched by `a0e250fb` / `b33dedf9`. Spot-checked: BUG-116
  (`RuntimeAnimatorController`, `CharacterData.cs:23`) and BUG-121 (`[FormerlySerializedAs("authoredModifiers")]`,
  `StatModifierGroup.cs:24`) intact; BUG-115's runtime-clone fix is superseded by the `StatSheet`
  design, not reverted. No note.
- **Inbox**: 0 new notes; NOTE-20261107-3 got evidence (site deleted by `a0e250fb`, candidate resolved).
- **Today**: carry S22-01 → S22-02 → S22-09 first, then Tue's S22-04 BUG-097 / S22-03 / S22-07.

### 2026-11-11 (Wed) — standup 02:00, evidence COMPILED + LOG + STATIC
- **Sync**: SKIPPED — owner branch `sprint-22` (`58601f57`) already contained in `origin/sprint-22` (`5e30cf72`).
- **Yesterday**: no commit on any branch except the standup commit `5e30cf72`. The owner's folder has
  9 uncommitted paths: new `System/RoomGameSystem/InteractiveObjectsRoom.cs` (+ `.meta`), modified
  `PedestalChampionController.cs`, `UIEvents.cs`, `GameplayUIController.cs`, `Entity.cs`,
  `LoadRandomMap.unity`, two generated `tools/anim-viewer/*.html`. Reads as room-interaction work
  from the owner's weekly to-do (off-plan); uncommitted work is not counted as progress.
- **Analysis**: Tuesday planned 0.3d (S22-04 / 03 / 07) plus 0.6d carried from Monday; burned 0.
  Must 0.05d / 1.05d after 2 of 5 days. Verdict SLIPPED — nothing planned has started.
- **Runtime**: compile COMPILED at `58601f57` + 9 uncommitted owner paths (34 s). `Editor.log`
  (2026-11-10 17:22, 2 Play Mode entries): 3 `error CS` lines, 1 distinct —
  `InteractiveObjectsRoom.cs(3,14)` CS0534 (`InteractiveObjects.Interact(Interact)` not
  implemented); the later batchmode compile of the same tree passed, so it was transient
  (NOTE-20261111-1). 0 exceptions with an `Assets/Script/` frame. `Editor-prev.log` clean.
  Fix-survival: NOT RUN — no `.cs` commit yesterday.
- **Inbox**: 1 new note (NOTE-20261111-1, S4, candidate "Resolved in week").
- **Today**: S22-01 playtest → S22-02 BUG-122 + S22-09 → S22-04 / S22-03 → S22-06 (Wed's own task)
  if time remains; S22-07 slides to Thu.
