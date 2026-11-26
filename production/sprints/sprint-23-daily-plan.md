# Sprint 23 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-16 (Mon) -> 2026-11-20 (Fri) · playtest Sat 2026-11-21
> **Companion to**: `sprint-23.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-15 (kickoff, autonomous). Branch `sprint-23` from `sprint-22` tip (`37b56d76`).
> Sprint 22 closed SLIPPED (Must 0.05d / 1.05d, velocity 0.16, ≈2.5d off-plan output, playtest
> done: no). See `sprint-23.md` › Sprint 22 Closure.

---

## Status Verdict

**FINAL (weekly wrap-up, Sat 2026-11-21 22:00 slot): SLIPPED — evidence RUNTIME (automated QA
`20261121-2106`, Play Mode) + STATIC + LOG; no playtest sheet (cap CONCERNS does not lift it).**
Friday's verdict stands. Saturday's owner commits fixed BUG-129 (`be973605`, QA-PLY-05 PASS) but
`8005d27e` broke the damage chain for everyone (**BUG-135**, S1: hurtboxes no longer implement
`INegativeReceiver`, 15 QA FAILs), and the dash rework kept BUG-130 open. S23-05 and S23-01 still
untouched. Playtest done: **no** (27th carry). Handoff updated in "Week close".

*Friday wrap-up verdict (kept for history):* **SLIPPED — evidence STATIC + LOG (+ RUNTIME from the
Thu automated QA run `20261119-1638`; no playtest sheet, no Friday QA run yet).** Verified Must work:
S23-03 (BUG-125 FIXED, QA-CMB-01 PASS) and S23-04 (BUG-097 FIXED in code + data). S23-02 landed in code
but BUG-122 is PARTIAL (death state re-enters itself, Archer / Mage clips carry no event); S23-05 and
S23-01 untouched; Friday's `19540dff` added a new S1 (BUG-129, Champion confirm does nothing). Playtest
done: **no** (27th carry). Handoff: see "Week close" at the end.

*Day-5 standup verdict (kept for history):* **AT RISK on Must — day 5 (Fri 2026-11-20, standup 02:00).** Thursday landed **S23-02** in code
(`49c58e35` "fix death flow": `ON_PLAYER_DEATH` emitted from the death clip's action event, `OnDeath`
re-entry guard; `abdbb332` boss-exit → main menu, death flows documented) — but a static review found
a likely regression: `PlayerDeathState` now inherits the `IsDeath → ChangeState(DeathState)` check and
re-enters itself every frame (NOTE-20261120-1, S1 guess). Off-plan: enemy hit-investigate state and
pathfinding tile-centre fix (`c613e641`, `0e0dc874`). S23-05 (BUG-126, one line) and S23-01
(playtest, 26th carry) still untouched. 1 working day left / 0.4d Must before Sat (S23-05 0.05d,
S23-01 0.3d, NOTE-20261120-1 ≈0.05d) + ≈0.1d buffer. Evidence: COMPILED (`a34c8404`) + LOG + STATIC.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 0.95d | 0.5d (S23-03, S23-04 code, S23-02 code) | 0.45d (0.35d before Sat) + NOTE-20261120-1 |
| Should Have | 0.9d (0.7d after S23-06 cut) | 0.5d | 0.2d |
| Feature story S23-F1 (capped) | 1.0d | 1.0d (merged) | 0 |
| Off-plan (Tue: BUG-071 rework, QA automation fixes; Wed: map / spawn restructure beyond S23-04; Thu: enemy investigate state, pathfinding tile centres, QA runs) | — | ≈1.6d (est.) | — |
| Buffer | 1.0d | ≈0.9d consumed by off-plan | ≈0.1d |

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
| S23-04 BUG-096 Boss + BUG-097 | 0.15d | ✅ code + data Wed (`e2b7efee`, `a92a0c7b`, `3015e9cf`) — awaiting Play Mode | Run starts in boss room; 7th carry |
| S23-05 BUG-126 | 0.05d | ⬜ → Wed | One line; blocks any Player build (`TMP_TextInfoDebugTool.cs:4` still unguarded) |
| (off-plan) BUG-071 tick chain + vitals event | — | 🟡 code (`16f61da7`, `b5fb3581`, `0626dd59`, `58b3da35`) | Not in sprint plan; awaiting triage |
| S23-08 BUG-117 + 120 + 123 | 0.35d | ✅ code (Mon: `f528c087`, `25d869fb`, `8236f9d1`, `25d2d5e4`, `aa045f79`) | Awaiting runtime; see NOTE-20261117-1 |

### Wed 2026-11-18 — Owner feature branch
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-F1 Finish + merge `docs/gdd-pathfinding-ability` | ≤1.0d | ✅ merged Mon (`8459e384`) + dash state (`91c42d79`) | Wed freed for carry-over / buffer |
| Carry-over: S23-02 → S23-05 → S23-01 → S23-04 | 0.8d | 🟡 S23-04 done (code); S23-02 / 05 / 01 ⬜ → Thu | All Must |
| (off-plan) Map / spawn restructure: `EnemyManager` + `Grid` under `Room`, `LevelEditorTool` split, boss via `MapModel`, death clips no loop | — | ✅ code (`5e4ddb06`…`d0d814b6`) | Owner-approved session; NOTE-20261118-3, NOTE-20261119-1 |

### Thu 2026-11-19 — Weapon + data
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S23-06 BUG-106 player half + BUG-110 | 0.2d | ✂️ owner decision (NOTE-20261116-3) | — |
| S23-07 Editor / data block | 0.15d | ✅ Mon (Knight assets deleted `041bf198`, Arrow collider `25d869fb`; Lightning `layerMask` = 64) | — |
| Carry-over: S23-02 → S23-05 → S23-01 | 0.65d | 🟡 S23-02 ✅ code (`49c58e35`, `abdbb332`) — suspected regression NOTE-20261120-1; S23-05 / S23-01 ⬜ → Fri | Must; last full day before Friday buffer |
| (off-plan) Enemy hit → investigate state; pathfinding nodes on tile centres | — | ✅ code (`c613e641`, `0e0dc874`, `06adc2a0`) | Owner-approved session; not covered by Thu QA runs |
| S23-09 BUG-127 | 0.05d | ⬜ | Mage basic attack barely reaches (`speed: 1` still); Archer same data (NOTE-20261117-6) |

### Fri 2026-11-20 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| NOTE-20261120-1 death state re-enters itself (verify / one-line override) | ≈0.05d | ⬜ | S23-02 may not reach Game Over without it |
| S23-05 BUG-126 (carried) | 0.05d | ⬜ | One line; blocks any Player build |
| S23-01 Playtest / smoke incl. death → Game Over (carried) | 0.3d | ⬜ | 26th carry; Saturday sheet ready |
| S23-10 One-liners | 0.15d | 🟡 BUG-104 half (`Enter()` log removed in `3a2659ce`, `Exit()` log remains) | Carried 5–8 weeks; only if time is left |
| Buffer | 1d | ≈0.9d consumed | — |

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

### 2026-11-19 (Thu) — standup 02:00
- **Sync:** SKIPPED — owner branch `sprint-23` is already contained in `origin/sprint-23` (`d0d814b6`).
- **Yesterday (Wed):** owner-approved session restructured map / spawn (12 commits `89a22d25`…`d0d814b6`):
  `EnemyManager` and `Grid` under `Room`, direct `SpawnRoom()` / `SpawnBoss()` calls, `LevelEditorTool`
  split from `LevelManager`, start / end rooms by `roomType`, Champion + Bonfire moved to the start room,
  boss spawn marker, 88 death clips no longer loop, map docs refreshed. Closes **S23-04** in code + data.
  Burned: Must 0.15 / 0.8d planned; ≈0.7d off-plan. S23-01 / 02 / 05 not started (3rd day).
- **Runtime:** `compile-check.sh` COMPILED `d0d814b6` + 3 uncommitted paths (35 s). `Editor.log`
  (11-18 17:32, 0 Play Mode) 0 `error CS`, no `Assets/Script/` exception. `fix-survival.sh 7`: 3 rows PRESENT.
  Playtest: none recorded.
- **Review:** NOTE-20261119-1 — spawn early returns emit no door event (latent soft lock).
- **Owner live tree:** `LoadRandomMap.unity`, `Level Manager.prefab` modified; `AbilityInstance.cs` staged.
- **Today:** S23-02 death chain (0.3d) → S23-05 BUG-126 (0.05d) → S23-01 playtest incl. new room flow (0.3d).
- **Verdict:** yesterday SLIPPED on S1 Must (S23-04 done off-order), sprint AT RISK on Must (0.65d / 2d + 0.3d buffer).

### 2026-11-20 (Fri) — standup 02:00
- **Sync:** SKIPPED — owner branch `sprint-23` is already contained in `origin/sprint-23` (`a34c8404`).
- **Yesterday (Thu):** S23-02 landed in code — `49c58e35` "fix death flow" (`ON_PLAYER_DEATH` from
  `PlayerDeathState.AnimationOnAction()`, `OnDeath` re-entry guard, dash checked after abilities,
  attack `LogicUpdate` order) and `abdbb332` (`UIEvents.RequestReturnToMainMenu()` for the boss exit,
  ui-ux-flow 6.4 / 6.5). Off-plan: `c613e641` enemy hit → investigate state, `0e0dc874` pathfinding
  nodes on tile centres + corner rule, `06adc2a0` syntax / `PlayerManager` / dash tweaks. QA automation
  ran twice (PASS 162 / FAIL 3 / KNOWN 9; FAIL = dash soft lock NOTE-20261117-3), both before the
  death fix. Burned: Must 0.3 / 0.65d planned; ≈0.4d off-plan. S23-05 / S23-01 not started (4th day).
- **Runtime:** `compile-check.sh` COMPILED `a34c8404` (76 s, clean tree). `Editor.log` (11-19 16:15)
  and `Editor-prev.log` (11-18 17:32): 0 `error CS`, 0 Play Mode, no `Assets/Script/` exception.
  `fix-survival.sh 7`: no fix commits in window. Playtest: none recorded.
- **Review:** NOTE-20261120-1 (S1 guess) — `PlayerDeathState` lost its `LogicUpdate()` override and
  now re-enters itself every frame through the inherited `IsDeath` check.
- **Playtest sheet:** `production/qa/playtests/playtest-2026-11-21.md` generated (step A9 = S23-02 check).
- **Today:** NOTE-20261120-1 (≈0.05d) → S23-05 BUG-126 (0.05d) → S23-01 smoke incl. death → Game Over (0.3d).
- **Verdict:** yesterday ON-TRACK for S23-02 code (suspected regression), sprint AT RISK on Must (0.4d / 1d, buffer ≈0.1d).

### 2026-11-20 (Fri) — weekly wrap-up (scheduler fired Friday, one day before the Saturday slot)
- **Friday's commits:** `19540dff` "knockback, update UI confirm" (off-plan) — review found BUG-129 (S1,
  Champion confirm callback dropped) and BUG-133 (S4, knockback toward the attacker); `633b7647` QA
  coverage review (new EditMode / Play Mode cases, inbox NOTE-20261120-2..6). Editor.log 16:29: 6
  transient CS0117 `AccessAction` (NOTE-20261120-9; committed call sites are commented out).
  NOTE-20261120-1, S23-05, S23-01 not done.
- **Triage:** 31 notes → 6 confirmed (BUG-129…134), 11 resolved, 5 duplicate, 8 by design / hints,
  2 needs owner; 13 status changes (`production/qa/bug-triage-2026-11-20.md`). S1 count 5 → 6.
- **Fix survival (30 d):** 0 GONE, 0 NOT ANCESTOR, PARTIAL only on the multi-file `1370136c` (explained).
- **Verdict:** SLIPPED (final weekly verdict above).

### 2026-11-21 (Sat) — weekly wrap-up 22:00 (delta after Friday's early run)
- **Saturday's commits:** `be973605` "coding need check" (confirm callback in `ConfirnData`, first
  dash hurtbox toggle), `8005d27e` "update animation, asset" (`INegativeReceiver<TCore>`, dash before
  death in `PlayerBasicState`, ≈1.4k Erika animation assets), `9e5921c0`, `67c188b3` (`PlayerDashState.Exit()`
  order). All off-plan.
- **Automated QA:** runs `20261121-2050` and `-2106` at `bcedaebb`: PASS 194 / FAIL 15 / KNOWN 17 /
  SKIP 13 / OBS 1, new FAIL 2 (QA-VIT-15 real, QA-ENT-10 45 s timeout flaky suspect). 14 of 15 FAILs
  share one cause → BUG-135. QA-PLY-05 PASS x3 → BUG-129 FIXED.
- **Playtest:** sheet `playtest-2026-11-21.md` not filled → NOT RUN.
- **Triage:** 7 notes → BUG-135 (S1), 2 duplicates (BUG-130, BUG-135), 1 resolved, 1 by design,
  2 needs owner (`production/qa/bug-triage-2026-11-21.md`). S1 count stays 6.
- **Fix survival (30 d):** 1 GONE (BUG-120 `aa045f79`, intentional menu rename by the owner), no reopen.
- **Editor.log:** 0 errors (19:29); `Editor-prev.log` 9 transient CS errors of `8005d27e`.

---

## Week close (wrap-up 2026-11-20)

| ID | Est. | Final status |
|----|------|--------------|
| S23-01 Playtest / smoke | 0.3d | ⬜ not done — **27th carry** |
| S23-02 Death → Game Over | 0.3d | 🟡 code landed (`49c58e35`), BUG-122 PARTIAL — carry |
| S23-03 BUG-125 | 0.05d | ✅ FIXED, QA-CMB-01 PASS (automated) |
| S23-04 BUG-096 boss + BUG-097 | 0.15d | ✅ code + data (BUG-097 FIXED awaiting Play Mode; BUG-096 latent shape remains, Open) |
| S23-05 BUG-126 | 0.05d | ⬜ not done — carry |
| Saturday playtest | 0.1d | ⬜ not run at wrap-up time |
| S23-06 BUG-106 + BUG-110 | 0.2d | ✂️ owner decision (BUG-106 CLOSED, BUG-110 deferred) |
| S23-07 Editor / data block | 0.15d | ✅ |
| S23-08 BUG-117 + 120 + 123 | 0.35d | ✅ code (BUG-120 PARTIAL — pickup PASS, guard missing; 117 / 123 status not re-read) |
| S23-09 BUG-127 | 0.05d | ⬜ not done — carry (scope now Archer + Mage) |
| S23-10 One-liners | 0.15d | 🟡 BUG-104 half — carry the rest |
| S23-F1 Pathfinding branch | 1.0d | ✅ merged |

**Burn:** Must verified 0.2d / 0.95d; Should 0.5d; Feature 1.0d; off-plan ≈2.0d (BUG-071 rework, map /
spawn restructure, enemy investigate state, pathfinding tile centres, knockback / confirm UI, QA runs).

### Handoff for Sunday `/weekly-kickoff`

- **Velocity:** 1.7 estimate-days completed (S23-03 0.05 + S23-04 0.15 + S23-07 0.15 + S23-08 0.35 +
  S23-F1 1.0) / 4 = **0.43**. Must-only: 0.2 / 0.95.
- **Carry-over:** S23-01 playtest (27th) · S23-02 remainder = BUG-122 (empty `LogicUpdate()` override in
  `PlayerDeathState` + events on Archer / Mage death clips) with BUG-086 / BUG-065 · S23-05 BUG-126 ·
  S23-09 BUG-127 (Archer + Mage) · S23-10 remainder (BUG-104 / 103 / 102 / 068).
- **New blocker bugs (updated Sat 2026-11-21):** **BUG-135** (S1, one-line class header — first task
  Monday, rerun the automated suite the same day), **BUG-130** (S2, dash soft lock — now also a
  self re-entry in `PlayerBasicState.cs:31`). ~~BUG-129~~ FIXED Saturday (`be973605`).
- **Playtest done: no.** Saturday's sheet `production/qa/playtests/playtest-2026-11-21.md` is empty
  (checked at the 22:00 slot). Kickoff: put the playtest first on Monday, after BUG-135.
- **Velocity unchanged:** 0.43 (Saturday's work was unplanned bug work, not sprint tasks).
- **Needs owner:** NOTE-20261115-7 item 3 (stats key C), NOTE-20261117-4 (Move-state spawn intent).

### Retrospective

- **Went well:** BUG-125 fixed and confirmed by the automated run the same week; S23-04 closed in code +
  data; QA automation grew to 188 cases and found the dash soft lock (BUG-130) before any hand test.
- **Slipped:** the S1 Must items again came last — S23-02 landed Thursday with a regression, S23-05 (one
  line) untouched all week, the playtest carried a 27th time; Friday's unplanned UI change added a new S1.
- **Improve (one):** after any commit that touches the death flow or the Champion confirm path, run the
  automated QA suite (or the two smoke steps A3 / A9 by hand) the same day, before starting the next task.
- **Deferred for weeks:** S23-01 playtest (27 weeks), BUG-065 (15th carry), BUG-096 remainder (8th).
- **Saturday addendum:** went well — the automated run caught BUG-135 within two hours of the commit
  and confirmed the BUG-129 fix the same evening. Slipped — a large "update animation, asset" commit
  mixed ≈1.4k asset files with a damage-interface change and broke all combat. Improve — keep
  interface / damage-chain changes in their own commit, separate from asset imports, so a QA FAIL
  points at one small diff.
