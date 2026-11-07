# Sprint 21 — Daily Plan & Progress Tracker

> **Sprint**: 2026-11-02 (Mon) -> 2026-11-06 (Fri) · playtest Sat 2026-11-07
> **Companion to**: `sprint-21.md` — day-by-day breakdown + live tracker
> **Routines: Mon–Fri 02:00 /daily-standup · Sat 22:00 /weekly-wrapup · Sat 23:00 /doc-sync --auto · Sun 22:00 /weekly-kickoff**
> **Opened**: 2026-11-01 (kickoff, autonomous). Branch `sprint-21` from `sprint-20` tip (`ed2f984b`).
> Sprint 20 closed SLIPPED (Must-Have 0 / 1.3d, planned velocity 0.13, ≈1.5d off-plan output,
> playtest done: no). See `sprint-21.md` › Sprint 20 Closure.

---

## Status Verdict

**SLIPPED — start of day 5, last working day (Fri 2026-11-06 standup)** (evidence: COMPILED + LOG + STATIC).
Thursday's owner output is mostly unplanned-but-useful: death now read from `ICharacterInput.IsDeath`
(`4097be4c`, `1c3a34fc` — targets NOTE-20261101-1/2), serialized-key data fix (`fbab752a` — BUG-105
sword damage and the BUG-121 `targetStatType` half), player body collider layer (`6f5322f6`), new
`Maw` art, and seven QA-automation runs (Play Mode, automated). S21-08 moved to 🟡; S21-01 owner
playtest still not run (0 Editor Play Mode entries). 1.0 working day left / ≈2.3d planned work
remaining (Must ≈0.85d + Sat 0.1d, Should ≈0.9d, S21-F1 0.5d) — only Must Have can still fit.
*Was (Thu):* SLIPPED — start of day 4; 2.0 days left / ≈2.4d remaining.
Wednesday's owner output `6094aa10` (1830 files, 36 `.cs`) is almost entirely off-plan: enemy role
scaffolding + assets, rooms, deletion of Abilities v1, and code fixes for five inbox notes. No
planned Wed task moved to ✅; S21-01 playtest still not run (0 Play Mode entries in the Editor log).
2.0 working days left (Thu–Fri) / ≈2.4d planned work remaining (Must ≈1.0d, Should ≈0.9d,
S21-F1 0.5d) — Should Have and S21-F1 will not all fit.
*Was (Wed):* SLIPPED, narrowing — start of day 3; 3.0 days left / ≈2.6d remaining.
*Was (Tue evening):* SLIPPED, narrowing — end of day 2. Owner commit `1370136c` closes S21-02 (BUG-116) and S21-03 (BUG-121 list key) in
code and makes partial progress on S21-05, S21-06 and S21-12; S21-09 done. S21-01 playtest still
not done (23rd carry). 3.0 working days left (Wed–Fri) / ≈2.6d planned work remaining.
*Was (Tue morning):* SLIPPED — day 2, 0 / 0.5d burned, batchmode exit 1.
*Was (2026-11-02):* ON-TRACK — day 1, nothing due yet.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have (incl. Sat playtest 0.1d) | 1.45d | 0.3d (S21-02, S21-03, S21-09) + partials (S21-05, S21-06, S21-08) | ≈0.95d |
| Should Have | 0.95d | 0 (S21-11 death inputs touched by `4097be4c`) | ≈0.9d |
| Feature story S21-F1 (capped, gated by S21-01 + S21-03) | 0.5d | 0 | 0.5d |
| Off-plan (Wed `6094aa10`: enemy roles, rooms, v1 deletion, note fixes) | — | ≈1d | — |
| Off-plan (Thu: death-flag flow, `Maw` art, QA-automation enemy suite, collider layer) | — | ≈0.7d | — |
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
| S21-01 Playtest / smoke through the menu | 0.3d | ⬜ → carried to Tue | playtest done: no; 23rd carry; QA automation now covers part of the sheet |
| S21-02 BUG-116 | 0.1d | ✅ in code Tue (`1370136c`) | `using UnityEditor.Animations` removed; field is `RuntimeAnimatorController`. Not build-verified |
| S21-03 BUG-121 | 0.1d | ✅ in code Tue (`1370136c`) | `[FormerlySerializedAs("authoredModifiers")]` + 4 assets re-saved. ⚠️ `targetStat` → `targetStatType` half still open (NOTE-20261103-2) |
| (off-plan) Archer/Mage ability GDD + temporary assets | — | ✅ committed `ae0d6e82`, `6c61ccdb` | Not in sprint plan; burned from buffer |

### Tue 2026-11-03 — Dungeon S1 blockers
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-04 BUG-108 finish + Knight check | 0.1d | ⬜ (new `Knight.asset` + `KnightWeapon` in `1370136c`; old `Enemy - Knight*` keys not checked) | S1 — same pattern as S21-03, do it while the context is warm |
| S21-05 BUG-096 | 0.1d | 🟡 `1370136c` | Shop / Rest / Treasure now emit `ON_OPEN_DOOR` on load (`RoomGeneraterController.cs` `AutoOpenRoomTypes`); Buff / Boss not in the set |
| S21-07 BUG-097 | 0.1d | ⬜ | Run starts in the boss room |

### Wed 2026-11-04 — Weapon session
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-06 BUG-106 / 107 / 110 | 0.3d | 🟡 `1370136c` (BUG-107 null-`Weapon` guard + `UnEquid` before release); `6094aa10` touched only the crit line → carried to Thu | One file (`WeaponHolderBase`), three bugs; enemies cannot attack |
| S21-08 Editor / data block | 0.15d | 🟡 Thu `fbab752a` (BUG-105 `attackDamege` keys renamed in `SnS_State1-3`; BUG-095 collider / BUG-072 `layerMask` not checked; owner to re-save the eight assets) | BUG-105 drops sword stage damage to 0 |
| (off-plan) `6094aa10` enemy roles + rooms + v1 deletion + fixes for NOTE-20261101-2, NOTE-20261104-1/2/5/6 | — | ✅ committed (not Play-Mode verified) | Burned from buffer; bug/note status for Saturday triage |

### Thu 2026-11-05 — Process gate + effect bugs
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-09 Process gate decision | 0.1d | ✅ 2026-11-03 | Decided C + B; `.githooks/pre-push` landed — owner runs `git config core.hooksPath .githooks` once |
| S21-10 BUG-117 + BUG-120 | 0.3d | ⬜ → carried to Fri | Heal / damage and stat items are no-ops |
| S21-F1 Effect-source work (cap 0.5d) | ≤0.5d | ⬜ | Only after S21-01 + S21-03 |
| (off-plan) death read from `ICharacterInput.IsDeath` (`4097be4c`, `1c3a34fc`); data key fix (`fbab752a`, also closes S21-03's `targetStat` half in data); body collider layer (`6f5322f6`); `Maw` art (`06204474`, `dd1ec12d`) | — | ✅ committed (not owner Play-Mode verified) | Targets NOTE-20261101-1/2 and NOTE-20261105-4; status for Saturday triage |

### Fri 2026-11-06 — Death path + cleanup
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S21-11 BUG-065 + BUG-086 | 0.2d | ⬜ (death states' inputs reworked Thu `4097be4c`; `PlayerDeathState` itself not touched) | Death path; read NOTE-20261101-1/2 first |
| S21-12 BUG-115 | 0.3d | 🟡 `1370136c` (`CharacterData.CreateRuntimeStats()` clone used by `StatHandlerBase`; asset revert not checked) | Every Play Mode session can dirty committed stat assets |
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

### 2026-11-03 (Tue) — standup ~09:00 (fired late, slot is 02:00), evidence STATIC + LOG
- **Sync**: SKIPPED — owner branch `sprint-21` (`6c61ccdb`) is already in `origin/sprint-21`.
- **Yesterday**: two owner commits, both off-plan — `ae0d6e82` design(gdd) Archer/Mage class
  abilities (Phase 1), `6c61ccdb` temporary Archer/Mage ability assets (Phase 2, 30 `.asset`/`.meta`).
  No sprint task moved: S21-01/02/03 (0.5d) carried. Burned 0 / 0.5d planned → **SLIPPED**.
- **Compile**: `compile-check.sh` exit 1 (65 s) on `6c61ccdb`, committed state only — 0 `error CS`
  (C# compiled); the exit comes from 12 unparseable `.meta` files of the Phase 2 assets →
  NOTE-20261103-1 (S2). The owner's working tree changed while Unity ran (owner active ~09:00); not
  reverted, owner to judge.
- **Editor log**: 0 `error CS`, 0 Play Mode entries, 0 `Assets/Script/` exceptions (Editor.log
  2026-11-03 07:50, Editor-prev.log 2026-11-02 17:02). Inbox: 8 open notes (+1).
- **Fix-survival**: not run — yesterday's commits touched no `.cs`.
- **Today** (Tue plan + Monday carry): S21-02 BUG-116 → S21-03 BUG-121 → fix NOTE-20261103-1 metas
  (owner call) → S21-01 playtest → S21-04 / S21-05 / S21-07 as time allows. Total 0.8d+ on a 1d day.

### 2026-11-03 (Tue) — second standup firing ~23:10 (evening update), evidence COMPILED + LOG
- **Duplicate firing:** the 02:00 slot already ran (~09:00, `f9d19cc7`). Kept as an evening update,
  not a new day; Wednesday's 02:00 standup still owns the Wed plan.
- **Sync**: SKIPPED — owner branch `origin/feature/add-effect-component` (`f1549094`) is already in
  `origin/sprint-21` (owner merged it in `f1549094`).
- **Today (owner)**: `1370136c` "fix bug add skill" (47 files, 29 `.cs`): BUG-116 fixed in code
  (S21-02 ✅), BUG-121 list key fixed in code (S21-03 ✅, `targetStat` half open — NOTE-20261103-2
  addendum), BUG-096 partial (Shop/Rest/Treasure auto-open; Buff/Boss not), BUG-107 item 2 guard,
  BUG-113 (`GetStatValue()` → `FinalValue`), BUG-115 runtime stat clone, new Knight enemy + weapon
  assets. Bug files not updated — Saturday triage. Process: S21-09 gate (`3e3f6f10`, `c7fbd69e`, `01717ace`).
- **Compile**: `compile-check.sh` COMPILED (58 s) on `f1549094` + 17 uncommitted owner paths
  (`Entity`, `EntityData`, `EntityAbilityState`, `EntityAttackState`, `EnemyRoleScaffolder`, enemy role
  assets — in-progress, not counted). 0 `Unable to parse file` lines this time (NOTE-20261103-1 addendum).
- **Editor log**: 0 `error CS`, 0 Play Mode entries, 0 `Assets/Script/` exceptions. Inbox: 10 open, 0 new.
- **Fix-survival**: `fix-survival.sh 7` → no fix commits in window.
- **Burn**: Must 0.3d / 1.45d done + partials. Still no Play Mode run this sprint — S21-01 is the first
  thing Wednesday, before S21-06 weapon work.

### 2026-11-04 (Wed) — standup ~02:13, evidence COMPILED + LOG
- **Sync**: SKIPPED — owner branch `origin/feature/add-effect-component` (`f1549094`) is already an
  ancestor of `origin/sprint-21` (`bf757c74`); `log origin/sprint-21..<branch>` is empty.
- **Since the Tue evening update**: no new commits on any branch (only `bf757c74`, the evening
  standup). Tuesday's full owner output is `1370136c` (recorded above). Tue plan: S21-05 🟡, S21-04 ⬜,
  S21-07 ⬜ → S21-04 / S21-07 carried to Wed. Day-2 burn 0.3d done + partials vs 0.3d Tue plan.
- **Compile**: `compile-check.sh` COMPILED (80 s) on `f1549094` + 17 uncommitted owner paths
  (`Entity`, `EntityData`, `EntityAbilityState`, `EntityAttackState`, `EnemyRoleScaffolder`, enemy role
  assets) — in progress, not counted.
- **Editor log**: `Editor.log` (2026-11-03 16:49) and `Editor-prev.log` (07:50): 0 `error CS`,
  0 Play Mode entries, 0 `Assets/Script/` exceptions. Inbox: 10 open, 0 new.
- **Fix-survival**: not re-run — no `.cs` commit since the Tue evening run (no fix commits in window).
- **Today** (Wed plan + Tue carry): S21-01 playtest (0.3d) → S21-06 weapon trio remainder (≈0.2d) →
  S21-04 BUG-108 Knight keys (0.1d) → S21-07 BUG-097 (0.1d) → S21-08 data block (0.15d). ≈0.85d on a 1d day.

### 2026-11-05 (Thu) — standup ~02:15, evidence COMPILED + LOG
- **Sync**: SKIPPED — owner branch `sprint-21` (`ea00b3a4`) is already in `origin/sprint-21`
  (owner merged and pushed Wed 21:26).
- **Yesterday**: `6094aa10` "fix bug, add asset room, enemy, data" (1830 files, +149.7k/−1.4k;
  36 `.cs`), `1da76e9a` pre-push gate now blocks pushes to `main` only (owner decision), merge
  `ea00b3a4`. Code highlights: `NegativeReceiverBase` lethal-hit → death state (NOTE-20261101-2);
  `MovementBase.MoveSpeed` from `StatType.MoveSpeed` (NOTE-20261104-1); crit as percent
  (NOTE-20261104-2); `RegenComponent` base fixed (NOTE-20261104-5); `EntityFindTarget` range from
  `EntityData.rangeCheckFieldOfView` + new `StatType.MaxRangeAttack` / `MinRangeAttack` (112/113)
  (NOTE-20261104-6); `EnemyRoleScaffolder` editor tool + role assets; **Abilities v1
  (`System/Skill_Ability/`) deleted**, `WeaponSO` moved to `System/Item/`; `EntityWeapon`,
  `EntityEffectStats`, `Spell`, `IEffectable` deleted; `[FormerlySerializedAs("aima")]` on
  `CharacterData.animationController`. Planned Wed tasks: none moved to ✅ → S21-01, S21-04,
  S21-06, S21-07, S21-08 carried. Burned ≈0 / 0.85d planned (≈1d off-plan) → **SLIPPED**.
- **Compile**: `compile-check.sh` COMPILED (82 s) on `ea00b3a4`, committed state only.
- **Editor log**: `Editor.log` (2026-11-04 21:30), `Editor-prev.log` (11-03 16:49): 0 `error CS`,
  0 Play Mode entries, 0 `Assets/Script/` exceptions. Inbox: 12 open (+1 today, NOTE-20261105-1).
- **Fix-survival**: `fix-survival.sh 7` → no fix commits in window.
- **Spot check**: no asset under `Assets/` references the GUIDs of deleted `EffectSkillSO` /
  `SlashAbility`; all new enemy stat assets carry 112/113 entries.
- **Today** (Thu plan + Wed carry): S21-01 playtest (0.3d, now also verifies `6094aa10`) →
  S21-06 remainder (0.2d) → S21-04 (0.1d) → S21-07 (0.1d) → S21-08 (0.15d) → S21-10 (0.3d) if
  time. S21-F1 stays gated. ≈1.15d on a 1d day.

### 2026-11-06 (Fri) — standup ~13:00 (fired late, slot is 02:00), evidence COMPILED + LOG
- **Sync**: SKIPPED — the owner's folder is on `pm/doc-sync-2026-11-05` (a `pm/*` branch), and its tip
  `26492eea` is already `origin/sprint-21` (owner merged `feature/new-asset-enemy` and the doc-sync
  branch and pushed).
- **Yesterday**: owner + interactive sessions — `4097be4c` + `1c3a34fc` death read from
  `ICharacterInput.IsDeath` (states own the transition; targets NOTE-20261101-1/2), `fbab752a`
  serialized-key data fix (BUG-105 `attackDamege` → `attackDamage` in `SnS_State1-3`; BUG-121
  `targetStat` → `targetStatType`; Blessing `modifiers` → `statsModifiers`; owner to re-save the
  eight assets in Unity), `6f5322f6` `PlayerTest.prefab` body collider Player → Default layer
  (NOTE-20261105-4, owner check needed), `06204474` / `dd1ec12d` new `Maw` art, doc-sync `7f590cec`,
  and seven QA-automation runs (QA-ENT-02…08, QA-STATIC-07/08). Thu plan: S21-08 🟡; S21-01, S21-04,
  S21-06, S21-07, S21-10 not moved → carried. Burned ≈0.1d planned / 1.15d (≈0.7d off-plan) → **SLIPPED**.
- ⚠️ **Death fix not yet confirmed**: QA run `20261105-1417` (on `8140a56d`, after `4097be4c` +
  `1c3a34fc`) still FAILs QA-VIT-07 = NOTE-20261101-1 (player death, 9 of 9 runs). Evidence is in the
  note; triage Saturday.
- **Compile**: `compile-check.sh` COMPILED (222 s) on `26492eea`, committed state only.
- **Editor log**: `Editor.log` (2026-11-05 16:01), `Editor-prev.log` (13:24): 0 `error CS`,
  0 Play Mode entries, 0 `Assets/Script/` exceptions. Inbox: 20 rows under Open notes, 0 new today.
- **Fix-survival**: `fix-survival.sh 7` → no fix commits in window (bug-titled fix commits only).
- **Playtest sheet**: `production/qa/playtests/playtest-2026-11-07.md` written, with part B2 added for
  Thursday's four owner fixes.
- **Today** (last working day): S21-01 playtest via the sheet (0.3d — now also confirms `4097be4c`,
  `fbab752a`, `6f5322f6`) → S21-06 remainder (0.2d) → S21-04 (0.1d) → S21-07 (0.1d) → S21-08
  remainder (0.05d). Should Have and S21-F1 go to carry-over unless the Must list finishes early.
