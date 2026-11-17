# Sprint 23 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-16 (Mon) -> 2026-11-20 (Fri) · playtest Sat 2026-11-21
> **Companion to**: `sprint-23.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-15 (kickoff, autonomous). Branch `sprint-23` from `sprint-22` tip (`37b56d76`).
> Sprint 22 closed SLIPPED (Must 0.05d / 1.05d, velocity 0.16, ≈2.5d off-plan output, playtest
> done: no). See `sprint-23.md` › Sprint 22 Closure.

---

## Status Verdict

**AT RISK on Must — day 3 (Wed 2026-11-18, standup 02:00).** Tuesday's owner work was off-plan:
BUG-071 tick chain / vitals event rework (`16f61da7`, `b5fb3581`, `0626dd59`, `58b3da35`) plus the
weekly QA automation run (FAIL 4 after test fixes; dash soft lock NOTE-20261117-3, S2). No planned
Must item moved: S23-01 (playtest, 26th carry), S23-02 (death → Game Over), S23-04, S23-05 still ⬜.
3 working days left (Wed–Fri) / 1.1d planned work remaining + 1d buffer — still fits, but only if
Must starts today. Sync SKIPPED (owner `sprint-23` = `origin/sprint-23`). Evidence: LOG + STATIC
(compile-check SKIPPED — Editor open); QA run 2026-11-17 is RUNTIME for the tests it covers.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 0.95d | 0.05d | 0.9d |
| Should Have | 0.9d (0.7d after S23-06 cut) | 0.5d | 0.2d |
| Feature story S23-F1 (capped) | 1.0d | 1.0d (merged) | 0 |
| Off-plan (Tue: BUG-071 rework, QA automation fixes) | — | ≈0.5d (est.) | — |
| Buffer | 1.0d | ≈0.5d consumed by off-plan | ≈0.5d |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S23-01 | Playtest / Play Mode smoke via menu (+ awaiting-runtime rows) | 0.3d | Must | **26th** |
| S23-02 | BUG-122 + BUG-065 + BUG-086 (death → Game Over) | 0.3d | Must | 2nd / **14th** / 9th |
| S23-03 | BUG-125 melee stage damage twice | 0.05d | Must | new (S2, P1) |
| S23-04 | BUG-096 Boss remainder + BUG-097 | 0.15d | Must | **7th** |
| S23-05 | BUG-126 `using UnityEditor` guard | 0.05d | Must | new (S2) |
| — | Saturday playtest | 0.1d | Must | — |
| S23-06 | BUG-106 player half + BUG-110 | 0.2d | Should | 4th |
| S23-07 | Editor / data block (BUG-095, BUG-072, BUG-108 key) | 0.15d | Should | **7th** |
| S23-08 | BUG-117 + BUG-120 + BUG-123 | 0.35d | Should | 3rd |
| S23-09 | BUG-127 Mage bolt data | 0.05d | Should | new (S3) |
| S23-10 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 5th–8th |
| S23-F1 | Merge `docs/gdd-pathfinding-ability` (capped) | ≤1.0d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-16 — Play first, then death + damage
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-01 Playtest / smoke through the menu (or sign accepted risk) | 0.3d | ⬜ → Tue | playtest done: no; 26th carry; eight bugs wait on runtime |
| S23-02 BUG-122 + BUG-065 + BUG-086 | 0.3d | ⬜ → Tue | S1; same file; untouched two sprints |
| S23-03 BUG-125 | 0.05d | ✅ code (`b92b6f2a`) | One line; every melee number wrong until fixed |

### Tue 2026-11-17 — Room path + build guard
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-01 + S23-02 (carried from Mon) | 0.6d | ⬜ → Wed | Must, S1; carried |
| S23-04 BUG-096 Boss + BUG-097 | 0.15d | ⬜ → Wed | Run starts in boss room; 7th carry |
| S23-05 BUG-126 | 0.05d | ⬜ → Wed | One line; blocks any Player build (`TMP_TextInfoDebugTool.cs:4` still unguarded) |
| (off-plan) BUG-071 tick chain + vitals event | — | 🟡 code (`16f61da7`, `b5fb3581`, `0626dd59`, `58b3da35`) | Not in sprint plan; awaiting triage |
| S23-08 BUG-117 + 120 + 123 | 0.35d | ✅ code (Mon: `f528c087`, `25d869fb`, `8236f9d1`, `25d2d5e4`, `aa045f79`) | Awaiting runtime; see NOTE-20261117-1 |

### Wed 2026-11-18 — Owner feature branch
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-F1 Finish + merge `docs/gdd-pathfinding-ability` | ≤1.0d | ✅ merged Mon (`8459e384`) + dash state (`91c42d79`) | Wed freed for carry-over / buffer |
| Carry-over: S23-02 → S23-05 → S23-01 → S23-04 | 0.8d | ⬜ | All Must; Wed is the last day with room for all of them |

### Thu 2026-11-19 — Weapon + data
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-06 BUG-106 player half + BUG-110 | 0.2d | ✂️ owner decision (NOTE-20261116-3) | — |
| S23-07 Editor / data block | 0.15d | ✅ Mon (Knight assets deleted `041bf198`, Arrow collider `25d869fb`; Lightning `layerMask` = 64) | — |
| S23-09 BUG-127 | 0.05d | ⬜ | Mage basic attack barely reaches (`speed: 1` still) |

### Fri 2026-11-20 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-10 One-liners | 0.15d | 🟡 BUG-104 half (`Enter()` log removed in `3a2659ce`, `Exit()` log remains) | Carried 5–8 weeks |
| Buffer | 1d | — | — |

### Sat 2026-11-21 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri 23:00 QA run as input) | 0.1d | ⬜ | Reserved every Saturday; feeds wrap-up |

---

## Risks (live)

- Playtest at 26th carry — owner decision: record one sheet or sign accepted risk.
- S23-02 must include BUG-086, otherwise `ON_PLAYER_DEATH` fires every frame once BUG-122 is fixed.
- Owner branch serializes `VitalModifier` without the BUG-117 sign fix — do S23-08 first.
- Needs-owner notes (NOTE-20261115-5/-6/-7 and older) — not tasks.
- `gh` unavailable — draft PR not opened.
- ~~Owner branch `docs/gdd-pathfinding-ability` conflicts with `sprint-23`~~ — resolved Mon:
  merged into `sprint-23` (`8459e384`); NOTE-20261116-1 candidate "Resolved in week".
- Off-plan dash state work is now committed (`91c42d79`) — no longer a risk to the plan.
- NEW 2026-11-17: `StatModifier.Value` now returns `Math.Abs(value)` (`f528c087`), but `StatSheet`
  adds `Value` without reading `ImpactType` — a negative max-stat modifier (debuff) would apply as a
  buff. Latent: no committed asset has a negative value (NOTE-20261117-1).
- Pattern: Should / Feature work done first while the two S1 Must items wait — 2nd sprint in a row.
  Tue 2026-11-17 repeated it (BUG-071 rework, off-plan).
- NEW 2026-11-18: dash soft lock from the QA run (NOTE-20261117-3, S2 guess) — player controllers have
  no Dash state / `IsDash` parameter; pressing Space may lock the player. Affects S23-01 playtest.

---

## Daily Log

_(appended by `/daily-standup`)_

### 2026-11-16 (Mon) — standup 02:00
- **Sync:** FAILED — merge of `docs/gdd-pathfinding-ability` (`f6921dc9`, `3b02df7d`, 2 commits not
  on `sprint-23`) conflicted in 4 `.md` files; aborted, nothing pushed (NOTE-20261116-1).
- **Yesterday (Sun):** only PM routine commits (wrap-up, doc-sync, kickoff). No owner commit since
  `3b02df7d` (Sat 2026-11-14 23:29, already reviewed by the wrap-up). Burned 0 / 0d planned.
- **Runtime:** `compile-check.sh` COMPILED owner HEAD `3b02df7d` + 10 uncommitted paths (36 s).
  `Editor.log` (00:03, 8 Play Mode entries): 9 `error CS0117` `IS_DASH` in `PlayerDashState.cs`,
  transient — the uncommitted `GameConstants.cs` compiles (NOTE-20261116-2). No `Assets/Script/`
  exception.
- **Owner live tree:** uncommitted dash state wiring (off-plan).
- **Today:** S23-01 playtest (0.3d) → S23-02 death chain (0.3d) → S23-03 BUG-125 (0.05d).
- **Verdict:** ON-TRACK (day 1).

### 2026-11-17 (Tue) — standup 02:00
- **Sync:** merged `origin/feature/fix-bug-sprint-23` (16 owner/session commits) into `sprint-23`
  as `9a66ca0c`; the owner then merged `sprint-23` back (`cf50be42`, now the `sprint-23` tip).
- **Yesterday (Mon):** BUG-125 fixed (`b92b6f2a`, stage damage no longer re-added; `SnS_State1-3`
  retuned 8 / 10 / 15); BUG-117 / 120 / 123 fixed in code + data; BUG-095 Arrow collider; Knight
  enemy assets deleted (BUG-108); ShootSpirit / SpiritBomd removed (BUG-073 / 090); `SourceEffect`
  despawn; ranged basic attack uses `finalDamage` (NOTE-20261104-4 owner decision); BUG-104 half;
  pathfinding branch merged (S23-F1) and dash state committed. BUG-106 player half by design,
  BUG-110 deferred → S23-06 cut. Burned: Must 0.05 / 0.65d planned; Should 0.5d + Feature 1.0d
  unplanned for Monday. S23-01 / S23-02 not started.
- **Runtime:** `compile-check.sh` COMPILED `1d05475a` (59 s) and the merged tip `cf50be42` (22 s).
  `Editor.log` (2026-11-16 17:26, 0 Play Mode): 48 `error CS` (SourceEffect / MeleeWeapon /
  RangeWeapon / QAEnemySuite) — mid-edit, the committed tip compiles (NOTE-20261117-2). No
  `Assets/Script/` exception. Playtest: none recorded.
- **Review:** NOTE-20261117-1 (`StatModifier.Value` absolute value vs `StatSheet`).
- **Today:** S23-01 playtest (0.3d) → S23-02 death chain (0.3d) → S23-05 BUG-126 (0.05d) →
  S23-04 (0.15d).
- **Verdict:** yesterday SLIPPED on Must (S1 items untouched), sprint ON-TRACK (1.1d / 4d).

### 2026-11-18 (Wed) — standup 02:00
- **Sync:** SKIPPED — owner branch `sprint-23` is already contained in `origin/sprint-23` (`125a4a41`).
- **Yesterday (Tue):** owner off-plan BUG-071 work (`16f61da7` "fix bug 071", `b5fb3581`, `0626dd59`,
  `58b3da35`): `IVitalComponent.CurrentStatsChanged` now `Action<VitalStatType>`, death check driven
  from `VitalStatsBase`, `EffectReceiverBase` tick chain changes. Weekly QA automation ran 3 times
  (final: PASS 151 / FAIL 4 / KNOWN 9 / SKIP 13 / OBS 1; 64 EditMode PASS), inbox NOTE-20261117-3..6.
  Burned: Must 0 / 0.75d planned; ≈0.5d off-plan. S23-01 / 02 / 04 / 05 not started.
- **Runtime:** `compile-check.sh` SKIPPED (Editor open). `Editor.log` (02:37) 0 `error CS`;
  `Editor-prev.log` (11-17 16:19) 18 CS0123 delegate mismatches — transient mid-edit (NOTE-20261118-1).
  No `Assets/Script/` exception. `fix-survival.sh 7`: 3 rows PRESENT. Playtest: none recorded.
- **Inbox:** NOTE-20261118-2 — `NOTE-20261117-1` / `-2` IDs used twice.
- **Owner live tree:** `AbilityInstance.cs` staged, uncommitted (invisible to the routine).
- **Today:** S23-02 death chain (0.3d) → S23-05 BUG-126 (0.05d) → S23-01 playtest (0.3d) → S23-04 (0.15d).
- **Verdict:** yesterday SLIPPED (Must untouched 2nd day), sprint AT RISK on Must (1.1d / 3d + 0.5d buffer).
