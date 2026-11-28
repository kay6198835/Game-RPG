# Sprint 24 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-23 (Mon) -> 2026-11-27 (Fri) · playtest Sat 2026-11-28
> **Companion to**: `sprint-24.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-22 (kickoff, autonomous). Branch `sprint-24` from `sprint-23` tip (`69f59a3b`).
> Sprint 23 closed SLIPPED (Must 0.2d / 0.95d, velocity 0.43, ≈2.0d off-plan output, playtest
> done: no). See `sprint-24.md` › Sprint 23 Closure.

---

## Status Verdict

**Final weekly verdict (wrap-up 2026-11-28): SLIPPED.** Evidence: RUNTIME (automated QA run
`20261127-0228`) + LOG + STATIC; **no playtest** (sheet `playtest-2026-11-28.md` empty, 0 Play Mode
entries in today's Editor logs), so no verdict above CONCERNS was possible in any case. Must verified
0.15d / 0.95d: S24-05 ✅, S24-02 ✅ (BUG-135 FIXED, confirmed by the automated run); S24-04 still FAILs
at runtime (QA-PLY-06 KNOWN, 3 classes); S24-03 and S24-01 not started. The week's output was ≈3.5d of
off-plan ability / status / tooling work. See Week close below.

_Day 5 verdict (standup 2026-11-27):_ **SLIPPED.** Thursday's output was again off-plan ability work (≈1.0d):
`85899a52` Delivery / Payload split + `SelfApplyEffect`, `83800891` `StatModifier` magnitude +
`impactType` serialized (A1, closes NOTE-20261123-1 code half), owner `8377ebaf` / `c973a8fb` /
`a9398fc5` / `f75573c8` trap prefab, summon / rain-arrows / damage-area fixes, and four anim fixes
(`e52431d6`, `43bb14be`, `6dd66791`, `fa206f82`). No Must task moved for the third day: S24-02 QA
rerun, S24-03 death → Game Over and S24-01 playtest still open. Must ≈0.65d remains for Friday plus
Saturday's playtest — the sprint cannot close its Must set unless Friday is Must-only. Evidence:
COMPILED `fa206f82` (+1 uncommitted scene) + LOG.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 0.95d | 0.15d verified (S24-05 ✅, S24-02 ✅); S24-04 code only, FAIL at runtime | 0.8d |
| Should Have | 0.65d | 0 | 0.65d |
| Feature story S24-F1 (capped) | 0.75d | 0 | 0.75d |
| Off-plan | — | ≈0.5d (`df9e98fe` status effects) + ≈0.3d (`7a41d7c4` room TextAsset, `070bc0a5` build cleanup) + ≈0.6d (`b001d78e` DamageEffect, `f2a580b2` trap) + ≈1.0d (Thu: `85899a52` Delivery / Payload, `83800891` A1, trap prefab + spawn / anim fixes) | — |
| Buffer | 1.0d | 0 | 1.0d |

---

## Task Estimates

| ID | Task | Est. | Priority | Carries |
|----|------|------|----------|---------|
| S24-01 | Playtest / Play Mode smoke via menu | 0.3d | Must | **28th** |
| S24-02 | BUG-135 receiver interface + same-day QA rerun | 0.1d | Must | new (S1) |
| S24-03 | BUG-122 remainder + BUG-086 + BUG-065 (death → Game Over) | 0.2d | Must | 3rd / 10th / **16th** |
| S24-04 | BUG-130 dash soft lock | 0.2d | Must | new (S2) |
| S24-05 | BUG-126 `using UnityEditor` guard | 0.05d | Must | 2nd |
| — | Saturday playtest | 0.1d | Must | — |
| S24-06 | BUG-127 Archer + Mage bolt data | 0.05d | Should | 2nd |
| S24-07 | BUG-104 / 103 / 102 / 068 one-liners | 0.15d | Should | 6th–9th |
| S24-08 | BUG-087 death / restart design note | 0.3d | Should | **9th** |
| S24-09 | BUG-096 remainder (doors without spawn marker) | 0.15d | Should | **9th** |
| S24-F1 | Erika animation / dash VFX assets, own commits (capped) | ≤0.75d | Feature | — |

---

## Day-by-Day Plan

### Mon 2026-11-23 — Play first, then restore damage
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-01 Playtest / smoke through the menu (or sign accepted risk) | 0.3d | ⬜ | playtest done: no; 28th carry; BUG-135 FAILs are valid evidence |
| S24-02 BUG-135 + automated QA rerun | 0.1d | ⬜ | S1: no hit damages anyone since `8005d27e`; blocks every combat check |
| S24-05 BUG-126 | 0.05d | ⬜ | One line; blocks any Player build; carried once |

### Tue 2026-11-24 — Death → Game Over
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-02 BUG-135 + automated QA rerun (carried from Mon) | 0.1d | 🟡 | Code fixed `e47f5c38`; QA rerun not run yet |
| S24-05 BUG-126 (carried from Mon) | 0.05d | ✅ | `070bc0a5` deleted `TMP_TextInfoDebugTool.cs` with TMP Examples |
| S24-01 Playtest / smoke (carried from Mon) | 0.3d | ⬜ | 29th carry; run after the S24-02 QA rerun |
| S24-03 BUG-122 remainder + BUG-086 + BUG-065 | 0.2d | ⬜ | S1; damage chain is back in code, now unblocked |
| S24-04 BUG-130 dash soft lock (check `1e5afb3c` first) | 0.2d | 🟡 | Dash state + `IsDash` added to all 3 controllers `b0cfe539`; runtime unverified |

### Wed 2026-11-25 — Restart design + rooms
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-02 QA rerun (carried from Tue) | 0.05d | ⬜ | Confirms BUG-135 + BUG-130 at runtime in one run |
| S24-03 BUG-122 remainder + BUG-086 + BUG-065 (carried from Tue) | 0.2d | ⬜ | S1; now unblocked |
| S24-01 Playtest / smoke (carried) | 0.3d | ⬜ | 29th carry |
| S24-08 BUG-087 design note | 0.3d | ⬜ | Death reaches Game Over after S24-03; restart path undefined for 9 sprints |
| S24-09 BUG-096 remainder | 0.15d | ⬜ | S1 soft lock in marker-less rooms |

### Thu 2026-11-26 — Owner feature
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-02 QA rerun (carried from Wed) | 0.05d | ⬜ | Damage path changed again (`b001d78e` DamageEffect); one run confirms BUG-135 + BUG-130 + new ability damage |
| S24-03 BUG-122 remainder + BUG-086 + BUG-065 (carried from Wed) | 0.2d | ⬜ | S1; 17th carry for BUG-065 |
| S24-01 Playtest / smoke (carried) | 0.3d | ⬜ | 30th carry |
| S24-F1 Erika / dash VFX assets, own commits | ≤0.75d | ⬜ | Declared feature; only after the Must items above |
| S24-08 / S24-09 (carried from Wed) | 0.45d | ⬜ | Should — move to Fri buffer if Must not done |

### Fri 2026-11-27 — Cleanup + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S24-02 QA rerun (carried from Thu) | 0.05d | ✅ | Runs `20261127-0141` / `-0228`: BUG-135 FIXED; BUG-130 still FAIL. Damage path changed twice more (`85899a52`, `8377ebaf`); one run covers BUG-135 + BUG-130 + ability damage |
| S24-03 BUG-122 remainder + BUG-086 + BUG-065 (carried from Thu) | 0.2d | ⬜ | S1; 18th carry for BUG-065 |
| S24-01 Playtest / smoke (carried) | 0.3d | ⬜ | 31st carry; sheet `playtest-2026-11-28.md` ready |
| S24-06 BUG-127 | 0.05d | ⬜ | Ranged basic attack barely reaches |
| S24-07 One-liners | 0.15d | ⬜ | Carried 6–9 weeks |
| Buffer | 1d | — | Off-plan work |

### Sat 2026-11-28 — Weekly playtest
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| Weekly playtest (`tests/smoke/critical-paths.md` + `playtest-sheet.sh` sheet; Fri QA run as input) | 0.1d | ⬜ | Not run — sheet empty at the 22:00 wrap-up |

---

## Risks (live)

- Playtest at 28th carry — owner decision: record one sheet or sign accepted risk.
- BUG-135 fixed in code (`e47f5c38`, 2026-11-24) — rerun the automated suite before trusting any combat result.
- `1e5afb3c` (Sun, after wrap-up) changed dash / spawn code untriaged — the Monday QA rerun covers it.
- Large mixed asset + code commits (`8005d27e` pattern) — keep S24-F1 asset-only.
- ~~PM worktree `D:/Fork/Game-RPG-pm` dirty with `Assets/Sprite/Erika/**`~~ — clean on 2026-11-23 (standup ran in it).
- `df9e98fe` `StatModifier.impactType` not serialized (NOTE-20261123-1) — debuff statuses may load as buffs.
- Needs-owner notes (NOTE-20261115-7 item 3, NOTE-20261117-4, NOTE-20261122-1) — not tasks.
- `gh` unavailable — draft PR not opened.

---

## Daily Log

_(appended by `/daily-standup`)_

### 2026-11-23 (Mon) — standup
- Sync: SKIPPED — owner branch `origin/feature/fix-bug-sprint-24` (local) already an ancestor of `origin/sprint-24` (`df9e98fe`).
- Commits since kickoff: `df9e98fe` "add status effect" (status → stat config `StatusStatsEffect`,
  `StatModifier` impact type, `StatEffectBase` auto-sync, QA / EditMode tests adapted,
  `design/balance/status-debuff-stat-combos.md`) — off-plan (closest: S24-D1 effect work).
- Planned Monday tasks: S24-01 ⬜, S24-02 ⬜ (BUG-135 still present), S24-05 ⬜ (`using UnityEditor;` still unguarded at `:4`).
- Runtime: compile COMPILED (`df9e98fe` + 4 uncommitted SO paths, 28 s); Editor.log 66 `error CS` = mid-edit
  state (NOTE-20261123-2); fix-survival 7d: 4 PARTIAL rows on BUG-066 / 071 / 119 from `df9e98fe`
  touching the same files (expected refactor, not noted).
- Inbox: +2 (NOTE-20261123-1 S2 `impactType` not serialized; NOTE-20261123-2 S4 transient).
- Verdict: SLIPPED — day 1 Must burn 0 / 0.4d planned.

### 2026-11-24 (Tue) — standup
- Sync: SKIPPED — owner branch `origin/feature/fix-bug-sprint-24` (local) already an ancestor of `origin/sprint-24` (`39280c8d`).
- Commits since the 2026-11-23 standup: none (only the standup commit `39280c8d`). Owner has 4 uncommitted
  Paladin effect SO edits (`Avatar of Light`, `Blessing`) in the owner folder — invisible to the routine.
- Planned tasks: S24-01 / 02 / 05 still ⬜ (carried to Tue); S24-03 / 04 ⬜.
- Runtime: compile SKIPPED (Unity Editor open); Editor.log 0 `error CS`, 1 Play Mode entry, no `Assets/Script/`
  exception; Editor-prev.log = the 66 errors already in NOTE-20261123-2. Fix-survival not run (no new `.cs` commit).
- Inbox: 5 open, +0 today.
- Verdict: SLIPPED — Must burn 0 / 0.4d (Mon); 0.95d Must left over 3.5 days.

### 2026-11-25 (Wed) — standup
- Sync: merged owner branch `origin/feature/fix-bug-sprint-24` (1 commit, `e47f5c38` "fix bug") into `sprint-24` → `5127ff40`.
- Commits since the 2026-11-24 standup: `b0cfe539` dash state in Paladin / Mage / Archer controllers (S24-04),
  `7a41d7c4` room JSON via `TextAsset` (Bug #15 / TD-022, off-plan), `070bc0a5` removed TMP Examples + test
  leftovers, QA scenes moved to `Assets/QA/Scenes/Test` (closes S24-05 / BUG-126 in code), `e47f5c38`
  BUG-135 fix + Paladin effect SO data + controller edits.
- Planned tasks: S24-05 ✅; S24-02 🟡 (code, no QA rerun); S24-04 🟡 (assets, no runtime); S24-01 / 03 ⬜.
- Runtime: compile COMPILED `e47f5c38` (156 s, no uncommitted changes); Editor.log 0 `error CS`, 8 Play Mode
  entries, no `Assets/Script/` exception; Editor-prev.log = the 66 errors of NOTE-20261123-2. Fix-survival 7d:
  BUG-097 / 122 / 129 / 135 rows all PRESENT.
- Inbox: 5 open, +0 new; NOTE-20261123-1 updated (new Paladin effect assets in `e47f5c38` still carry no `impactType`).
- Verdict: SLIPPED, recovering — Must ≈0.65d left over 2.5 days; all three fixes await runtime evidence.

### 2026-11-26 (Thu) — standup
- Sync: SKIPPED — owner branch `sprint-24` (local, `381171a9`) already an ancestor of `origin/sprint-24` (owner pushed 17:17).
- Commits since the 2026-11-25 standup: `f2a580b2` (owner) trap object + anim / sprite, `DamageEffect` stub,
  `Runtime/SpawnMono/*` moved to `Runtime/SpawnMonoEffect/`; `b001d78e` `DamageEffect` through `IEffectReceiver`
  + 8 class damage-effect assets + GDD / docs; merge `381171a9` (DamageEffect GUID `0bdcbe2c…` referenced by all 8 assets). All off-plan.
- Planned Wed tasks: S24-02 / 03 / 01 / 08 / 09 all ⬜ — carried to Thu.
- Runtime: compile SKIPPED (Unity Editor open); Editor.log + Editor-prev.log 0 `error CS`, 0 Play Mode, no
  `Assets/Script/` exception. Fix-survival 7d: BUG-122 / 129 / 135 rows PRESENT.
- Inbox: 6 open, +1 (NOTE-20261126-1 S3 `Trap` layer-mask compare + unset `_context` in scene).
- Verdict: SLIPPED — Must burn 0 / 0.55d on Wed; Must ≈0.65d left over 1.5 days.

### 2026-11-27 (Fri) — standup
- Sync: SKIPPED — owner branch `origin/feature/fix-bug-sprint-24` (local, `fa206f82`) already an ancestor of `origin/sprint-24`.
- Commits since the 2026-11-26 standup (all off-plan, ≈1.0d): `85899a52` Delivery / Payload split + `SelfApplyEffect`;
  owner `8377ebaf` "coding" (`Trap.prefab`, `MainGamePlay.unity`, spawn effects, `Utility.cs`), `c973a8fb` "fix",
  `a9398fc5` rain arrows, `f75573c8` "update miss"; `83800891` A1 `StatModifier` magnitude + `impactType` serialized;
  `e52431d6` summon spawns once + RainArrows hold; `43bb14be` Consecrate clips; `6dd66791` / `fa206f82` spawn-effect controllers.
- Planned Thu tasks: S24-02 / 03 / 01 / F1 / 08 / 09 all ⬜ — Must items carried to Fri.
- Runtime: compile COMPILED `fa206f82` + 1 uncommitted path (`CombatTest.unity`, 35 s); Editor.log 0 `error CS`, 2 Play Mode;
  Editor-prev.log 0 `error CS`, 9 Play Mode; no `Assets/Script/` exception. Fix-survival 7d: BUG-129 / 135 rows PRESENT.
- Inbox: 6 open, +0 new; NOTE-20261123-1 updated (code fixed `83800891`, 8 of 9 modifier assets have no `impactType` key);
  NOTE-20261126-1 updated (`Trap.cs:17` mask compare unchanged after `8377ebaf`).
- Playtest sheet `production/qa/playtests/playtest-2026-11-28.md` generated.
- Verdict: SLIPPED — Must burn 0 / 0.25d on Thu; Must ≈0.65d left for Fri + Sat playtest.

### 2026-11-28 (Sat) — weekly wrap-up 22:00
- **Fri / Sat commits (after the Fri standup):** `bb142acc` status registry trial, `00cc9e56` statuses
  cleared only on death, pooled `SourceEffect` removed (B6, `ActiveStatus` / `ApplyStatus` on
  `EffectReceiverBase`), `78270214` / `83b8dd31` docs, `f9d672c3` / `386414b2` / `d3e5418e` animation
  review tool. All off-plan (≈1.0d). QA branch: test fixes `aeb4dbfc`, `baa295c7`.
- **Code review (STATIC):** 13 `.cs` files since `fa206f82`. Status registry reviewed: refresh by
  (AbilityInstance, asset) key, own-source removal, death / respawn clear, tick re-entry guard — no
  defect found. Observation only: ticks run at most once a frame, so a `perTime` shorter than a frame
  would lose ticks; no asset uses `RecoveryReductionPerTimeForDuration` today (latent, no note).
- **Automated QA:** last real run `20261127-0228` (HEAD `075711d9`): PASS 217 / FAIL 2 / KNOWN 13 /
  SKIP 19 / OBS 2, new FAIL 0. Sat 00:26 run SKIPPED (Editor open). Code after `075711d9`
  (status registry `00cc9e56`) is not runtime-tested.
- **Playtest:** NOT RUN — sheet empty; Editor logs 0 Play Mode entries today.
- **Triage:** 9 notes → BUG-136 (S3 Trap), BUG-135 + BUG-117 → FIXED (QA PASS), 1 duplicate, 2 resolved,
  2 by design, 4 needs owner (`production/qa/bug-triage-2026-11-28.md`). S1 count 6 → 5.
- **Fix survival (30 d):** 3 GONE — all explained (path move / refactor / intentional rename); no reopen.
- **Editor.log:** 0 `error CS` (22:14), `Editor-prev.log` 0 (04:27).

---

## Week close (wrap-up 2026-11-28)

| ID | Est. | Final status |
|----|------|--------------|
| S24-01 Playtest / smoke | 0.3d | ⬜ not done — **32nd carry** |
| S24-02 BUG-135 + QA rerun | 0.1d | ✅ BUG-135 FIXED (`e47f5c38`, run `20261127-0228`) |
| S24-03 BUG-122 + 086 + 065 | 0.2d | ⬜ not started — carry (BUG-065 19th) |
| S24-04 BUG-130 dash soft lock | 0.2d | 🟡 `b0cfe539` dash state wired; QA-PLY-06 still FAIL (3 classes) — carry |
| S24-05 BUG-126 | 0.05d | ✅ `070bc0a5` |
| Saturday playtest | 0.1d | ⬜ not run |
| S24-06 BUG-127 | 0.05d | ⬜ carry (QA-CMB-04.Mage KNOWN) |
| S24-07 One-liners | 0.15d | ⬜ carry |
| S24-08 BUG-087 design note | 0.3d | ⬜ carry (now also covers NOTE-20261122-1) |
| S24-09 BUG-096 remainder | 0.15d | ⬜ carry |
| S24-F1 Erika / dash VFX | ≤0.75d | ⬜ not started |

**Burn:** Must verified 0.15d / 0.95d; Should 0; Feature 0; off-plan ≈3.5d (status effects `df9e98fe`,
room TextAsset + build cleanup, `DamageEffect`, trap, Delivery / Payload split, A1 `StatModifier`,
anim fixes, B6 status registry, animation review tool). BUG-117 closed as a side effect of off-plan work.

### Handoff for Sunday `/weekly-kickoff`

- **Velocity:** 0.15 estimate-days completed (S24-02 0.1 + S24-05 0.05) / 4 = **0.04**. Must-only 0.15 / 0.95.
- **Carry-over:** S24-01 playtest (32nd) · S24-03 BUG-122 remainder + BUG-086 + BUG-065 · S24-04
  BUG-130 (runtime FAIL) · S24-06 BUG-127 · S24-07 one-liners · S24-08 BUG-087 note (+ NOTE-20261122-1)
  · S24-09 BUG-096 remainder · S24-F1.
- **New blocker bugs:** none. New BUG-136 (S3 Trap) is not a blocker unless the trap goes into the demo.
  S1 open: 5 (BUG-135 FIXED).
- **Playtest done: no.** Sheet empty at 22:00; no Play Mode session in today's Editor logs.
- **Automated QA:** rerun needed — the B6 status registry (`00cc9e56`) changed every timed effect after
  the last real run; the Sat 00:26 run was SKIPPED because the Editor was open.
- **Needs owner:** NOTE-20261115-7 (key C, 3rd), NOTE-20261117-4 (spawn state intent, 3rd),
  NOTE-20261122-1 (despawn on death), NOTE-20261123-1 data half (8 modifier assets with no `impactType`).
- **Tooling:** `.claude/` is untracked since `c40da7b8` and missing from the owner folder and PM
  worktree; this run read skill / protocol from git history and ran scripts from `D:/Fork/Game-RPG-pmko`.

### Retrospective

- **Went well:** BUG-135 (S1) fixed Tuesday and confirmed by the automated suite Thursday night
  (QA-ENT-08 x10, QA-VIT-15 PASS); BUG-117 closed by the A1 redesign with a runtime PASS; the automated
  run found and the QA branch fixed 3 test defects the same night.
- **Slipped:** for the fifth day running the Must set lost to off-plan ability work (≈3.5d); S24-03
  death → Game Over not started, the playtest carried a 32nd time, BUG-130 still soft-locks dash.
- **Improve (one):** open each working day with the one Must item for that day before any feature
  commit — if the day's Must is a runtime check, do it in the first Play Mode session.
- **Deferred for weeks:** S24-01 playtest (32), BUG-065 (19th carry), BUG-087 / BUG-096 remainder (10th).
