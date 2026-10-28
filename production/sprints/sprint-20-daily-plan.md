# Sprint 20 — Daily Plan & Progress Tracker

> **Sprint**: 2026-10-26 (Mon) -> 2026-10-30 (Fri) · playtest Sat 2026-10-31
> **Companion to**: `sprint-20.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-10-25 (kickoff, autonomous). Branch `sprint-20` from `sprint-19` tip (`566a684d`).
> Sprint 19 closed SLIPPED (Must-Have 1/9, planned velocity 0.01, ≈3d off-plan output, playtest
> done: no). See `sprint-20.md` › Sprint 19 Closure.

---

## Status Verdict

**SLIPPED (day 4, 2026-10-29 standup)** — Must burned 0 / 1.3d with 2 days left. Wed output was
a CombatTest harness (`87e053b4`, on `sprint-20`; fix `6f9d0971` on `combat-test-scene` only) —
it serves S20-01 but is not the smoke itself. ⚠️ `sprint-20` tip expected not to compile in the
Editor assembly (NOTE-20261029-1, STATIC). Owner's folder COMPILED (`6f9d0971` + 48 uncommitted);
Editor.log shows 15 Play Mode entries. Evidence `COMPILED` + `LOG` + `STATIC`.
*Was (2026-10-28):* **SLIPPED (day 3)** — Mon + Tue plan (S20-01…S20-05, 0.9d) not started; the
only `.cs` commit is `6e484f0c` "update logic effect" (S20-F1 follow-up, off-plan). Must burned
0 / 1.3d with 3 days left. `compile-check.sh` **COMPILED** `beca1bcc` (+1 uncommitted line).
Evidence `COMPILED` + `LOG`.
*Was (2026-10-26):* **ON-TRACK (day 1)** — no planned task due before today. Sunday evening the
owner landed S20-F1 (`feature/add-effect-component`, merge `9348d151` + `e156bdf4`) ahead of its
S20-01 gate, plus `041292ce` (champion pick flow). `compile-check.sh` **COMPILED** `e156bdf4`.
Risk: `041292ce` puts `UnityEditor.Animations` in runtime `CharacterData` → player builds fail
(NOTE-20261026-1, S1 guess). Evidence `COMPILED` + `LOG`.
*Was (2026-10-25, kickoff):* OPEN — not started.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.3d | 0 (day 4; 1.15d was due by Wed) | 1.3d |
| Should Have | 1.0d | 0 | 1.0d |
| Feature story S20-F1 (capped, gated by S20-01) | 0.5d | 0.5d (landed early, unverified) | 0 |
| Buffer | 1.0d | — | — |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S20-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | **21st** |
| S20-02 | BUG-108 `[FormerlySerializedAs]` on `CharacterData` | 0.1d | Must | new (S1) |
| S20-03 | BUG-096 zero-spawn branch | 0.1d | Must | 4th |
| S20-04 | Weapon trio BUG-106 / 107 / 110 | 0.3d | Must | new |
| S20-05 | BUG-097 start / boss order | 0.1d | Must | 4th |
| S20-06 | Editor / data block (BUG-105, BUG-095, BUG-072 check) | 0.15d | Must | 4th |
| S20-07 | Process gate decision | 0.1d | Must | **10th** |
| — | Saturday playtest | 0.1d | Must | — |
| S20-08 | BUG-065 + BUG-086 | 0.2d | Should | 11th / 6th |
| S20-09 | BUG-092 + BUG-066/070 verify | 0.15d | Should | 6th / 11th |
| S20-10 | BUG-115 runtime stat copy + asset revert | 0.3d | Should | new |
| S20-11 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 2nd–5th |
| S20-12 | BUG-073 / 090 / 112 missing scripts | 0.2d | Should | 7th / new |
| S20-F1 | Land `feature/add-effect-component` | ≤0.5d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-10-26 — Play first, then the S1 blockers
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S20-01 Playtest / smoke through the menu | 0.3d | 🟡 harness only (CombatTest scene `87e053b4`; 15 Play Mode entries in Editor.log 10-28/29, no sheet) | playtest done: no; 21st carry; no `RUNTIME` evidence for 17 weeks |
| S20-02 BUG-108 | 0.1d | ⬜ | S1 — Knight enemies throw on first stat read; combat rooms cannot clear |
| S20-03 BUG-096 | 0.1d | ⬜ | S1 — non-combat rooms stay sealed |

### Tue 2026-10-27 — Weapon session
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S20-04 BUG-106 / 107 / 110 | 0.3d | ⬜ | One file (`WeaponHolderBase`), three bugs; enemies cannot attack |
| S20-05 BUG-097 | 0.1d | ⬜ | Run starts in the boss room |

### Wed 2026-10-28 — Data block + process gate
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S20-06 Editor / data block | 0.15d | ⬜ | BUG-105 drops sword stage damage to 0 |
| S20-07 Process gate decision | 0.1d | ⬜ | 10th carry; 5 build breaks in 3 weeks |
| S20-08 BUG-065 + BUG-086 | 0.2d | ⬜ | Death path; precondition for BUG-087 |

### Thu 2026-10-29 — Stat asset safety + feature
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S20-10 BUG-115 | 0.3d | ⬜ | Every Play Mode session can dirty committed stat assets |
| S20-F1 Land `feature/add-effect-component` (cap 0.5d) | ≤0.5d | ✅ merged 2026-10-25 (`9348d151`, `e156bdf4`; COMPILED, not played) | Unblocks the deferred stat / vitals bugs |

### Fri 2026-10-30 — Verify + cleanup
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S20-09 BUG-092 + BUG-066/070 verify | 0.15d | ⬜ | After S20-F1 so it checks the new `VitalStatsBase` |
| S20-11 One-liners | 0.15d | ⬜ | BUG-104 regressed |
| S20-12 Missing-script assets | 0.2d | ⬜ | Missing Script warnings in every clone |
| Buffer | 1d | — | — |

### Sat 2026-10-31 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 21st carry — a second skipped Saturday is an owner decision, not a silent carry.
- `feature/add-effect-component` (`786c031b`) rewrites `VitalStatsBase.cs`; stat / vitals bugs wait for it.
- BUG-115: check `git status` for dirty `Assets/SO/Stat/*.asset` after Play Mode until S20-10 lands.
- `05dcf630` changed `.cs` after the last COMPILED tip — Monday standup compile-check matters.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_

### 2026-10-26 (Mon, day 1) — ON-TRACK · evidence COMPILED + LOG
- **Since kickoff (Sun evening):** `786c031b` effect component (EffectReceiverBase, IEffectReceiver,
  Player/Entity effect receivers, `VitalStatsBase` −70 lines, item stat-modifier effects) merged
  as `9348d151` + conflict fix `e156bdf4`; `041292ce` champion pick flow (`Player.SetCharacterData()`
  re-seeds vitals via `ReloadComponent()`, interact fires on `AnimationEnd`, new
  `FastTest/AttackFlowDebugger.cs`). S20-F1 done ahead of its S20-01 gate — off-order, not off-plan.
- **Runtime:** `compile-check.sh` COMPILED `e156bdf4` (78 s). Editor.log 2026-10-25: 342 `error CS`
  mid-merge (transient, NOTE-20261026-2), 12 Play Mode entries, 1× NRE @ `AttackFlowDebugger.cs:162`
  (NOTE-20261026-3). `fix-survival.sh 7`: BUG-107 PRESENT.
- **New notes:** NOTE-20261026-1 (S1 guess — `using UnityEditor.Animations` + `AnimatorController`
  field in runtime `CharacterData`, player builds fail; `BaseData.asset` override controller drops
  to null), -2, -3.
- **Today:** S20-01 playtest first (22nd carry if skipped), then S20-02 BUG-108, S20-03 BUG-096.
  S20-09 (vitals verify) is now unblocked by S20-F1.

### 2026-10-28 (Wed, day 3) — SLIPPED · evidence COMPILED + LOG
- **No standup row for Tue 2026-10-27** in `routine-log.md` — that run did not happen; this entry
  covers Mon 26 + Tue 27.
- **Since 2026-10-26:** owner `6e484f0c` "update logic effect" (15 `.cs`: `EffectReceiverBase` −100
  lines, `IEffectReceiver`, `IVitalComponent`, `AbilityContext.Recipient`, stat effects,
  `StatModifier` / `StatModifierGroup`) merged into `sprint-20` as `beca1bcc`; interactive docs
  `26c5b4e1` (core component access plan), `ef0d9c6f` / `3e90f323` (effect source plan). No commit
  for S20-01…S20-05.
- **Runtime:** `compile-check.sh` COMPILED `beca1bcc` + 1 uncommitted line in `EffectReceiverBase.cs`
  (38 s). `Editor-prev.log` (2026-10-27 07:58): 6 `error CS` (`AbilityContext.Recipient` missing),
  transient — `AbilityContext.cs:20` declares it at HEAD; `Editor.log` (20:35) 0 errors, 0 Play Mode.
  `fix-survival.sh 7`: no fix commits in window. Owner working tree shows no dirty
  `Assets/SO/Stat/*.asset` (BUG-115 watch). NOTE-20261026-1 still present (`CharacterData.cs:2,20`).
- **New notes:** none.
- **Today:** S20-01 playtest first (0.3d) — the effect / vitals refactor of the last three days has
  never run in Play Mode; then S20-02 BUG-108 + S20-03 BUG-096 (S1, 0.2d). Wed items S20-06 / S20-07
  move to Thu; S20-08 (Should) at risk.

### 2026-10-29 (Thu, day 4) — SLIPPED · evidence COMPILED + LOG + STATIC
- **Since 2026-10-28 standup:** `87e053b4` CombatTest scene builder (class pick via the three
  Champion monuments, F1–F3 hotkeys, two `TrainingDummy` targets, lean `CombatTestLifetimeScope`)
  pushed to `sprint-20`; `6f9d0971` (qualify `System.IO.Path`) on `combat-test-scene` only. Owner
  stash `82e76292` on `combat-test-scene` shows uncommitted `CombatTest.unity`, `VitalModifier.cs`,
  `SourceEffect.cs`, `tools/anim-viewer/`. No commit for S20-02…S20-07.
- **Runtime:** `compile-check.sh` COMPILED `combat-test-scene` `6f9d0971` + 48 uncommitted paths
  (115 s). `Editor.log` (02:12): 15 Play Mode entries, 1 distinct transient `error CS0311`
  (`PlayerMoveState.cs:14`, uncommitted), 6× NRE @ `PlayerState.cs:42` (NOTE-20261029-2).
  `Editor-prev.log` (10-28 22:06): clean. `fix-survival.sh` not run — yesterday's `.cs` commits only
  add new files. Owner tree has modified `Assets/SO/Stat/Player/{Archer,Paladin}Stats.asset`
  (BUG-115 watch — check before committing).
- **New notes:** NOTE-20261029-1 (S1 guess — `sprint-20` tip does not compile: `Path` shadowing in
  `CombatTestSceneBuilder.cs:86`; fix exists on `combat-test-scene`), NOTE-20261029-2.
- **Today:** land `6f9d0971` on `sprint-20` (≈0d), then S20-01 — fill the smoke sheet using the
  CombatTest scene plus one menu run (0.3d); then S20-02 BUG-108 + S20-03 BUG-096 (0.2d).
  S20-10 BUG-115 (Should) moves to Fri; S20-06 / S20-07 still open.
