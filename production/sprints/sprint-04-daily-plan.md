# Sprint 4 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-07 (Mon) → 2026-07-11 (Fri)
> **Companion to**: `sprint-04.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes yesterday from git + this tracker, updates statuses, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the week's `.cs`, playtest log, bug-triage, light retro; finalizes verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates upcoming week's sprint plan.
> **Last updated**: 2026-07-08 (Tue) — Day 2 standup

---

## Status Verdict: 🟡 AT RISK (conditional) — S4-01→S4-04 are actually DONE, but stranded on an unmerged branch (`origin/feature/enhance-stats-system`), not on `sprint-04`. Zero P1 progress landed on `sprint-04` itself.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 2.5 days (Must + Should) |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 2 (Day 2 morning) |
| Days remaining | 4 |
| Work committed/done on `sprint-04` | 0 (S4-01→S4-04 still Not Started **on this branch**) |
| Work done but unmerged | 1.0d — S4-01, S4-02, S4-03, S4-04 all verified complete on `origin/feature/enhance-stats-system` (commits `87acd8f`, `3987933`) |
| Work remaining | 1.5 days (Should + merge overhead) once merged |
| Velocity | 0% landed on sprint branch; 100% of Must-Have done in absolute terms, blocked on merge |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S4-01 | Fix BUG-AH-1 — remove `UnityEditor` imports from `AbilityHolder.cs` + all runtime scripts | 0.25 | Must | 🟡 Done on `feature/enhance-stats-system`, unmerged into `sprint-04` |
| S4-02 | Fix Bug #9 — `AnimationPlayerController` double-registration | 0.25 | Must | 🟡 Done on `feature/enhance-stats-system`, unmerged |
| S4-03 | `Core.GetCoreComponent<T>()` LINQ → foreach + lazy cache | 0.25 | Must | 🟡 Done on `feature/enhance-stats-system`, unmerged |
| S4-04 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach (add TakeDamage) | 0.25 | Must | 🟡 Done on `feature/enhance-stats-system`, unmerged |
| S4-05 | Fix BUG-PIH-1 — `CancelInvoke` pairing in `PlayerInputHandle` | 0.25 | Should | ⬜ Not started |
| S4-06 | Stats system — `TalentManager` prototype → SO-driven | 1.0 | Should | ⬜ Not started |
| S4-07 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | 1.0 | Nice | ⬜ Not started |
| S4-08 | EditMode test for `Core.GetCoreComponent<T>()` | 0.5 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/07 — PLAN
**Goal: Land all 4 P1 Must-Have fixes (S4-01 → S4-04). These are the ONLY tasks until done.**

> These four items have been carried 8 sprints. Each is ≤0.25d. A single focused session (2–3h) closes all of them.

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-01** — `AbilityHolder.cs` line 4: remove `using UnityEditor.Experimental.GraphView;`. Then grep full `Assets/Script/` for `using UnityEditor` and fix all runtime hits. | 0.25d | 🔴 Must |
| 2 | **S4-02** — `AnimationPlayerController.cs` line 21: change second `StartAnimation` → `EndAnimation`; line 29 OnDisable mirror. Verify in Play Mode weapon state exits. | 0.25d | 🔴 Must |
| 3 | **S4-03** — `Core.cs` `GetCoreComponent<T>()`: replace LINQ `OfType<T>().FirstOrDefault()` with `foreach`. Add `??=` lazy-cache so loop runs once per type. | 0.25d | 🔴 Must |
| 4 | **S4-04** — `WeaponMelee.cs` Attack() foreach body: add `INegativeReceiver dmg = enemy.GetComponentInChildren<INegativeReceiver>(); if (dmg != null) dmg.TakeDamage(currrentSA.attackDamege, transform.position);` | 0.25d | 🔴 Must |
| 5 | **Smoke check** — Enter Play Mode: equip → LMB attack → confirm enemy Health decreases in Inspector | — | Advisory |

*If all 4 land Mon: combat is testable for the first time in 8 weeks.*

---

### Tue 08/07 — PLAN (revised at Day 2 standup)
**Goal: Land the already-completed P1 block onto `sprint-04`, then S4-05 + start S4-06**

> Original plan assumed S4-01→S4-04 would start today. They're actually already done — just on the wrong branch. Priority 0 today is getting them onto `sprint-04`.

| # | Task | Est | Priority |
|---|------|-----|----------|
| 0 | **Merge `origin/feature/enhance-stats-system` → `sprint-04`** — confirmed clean fast-forward (`sprint-04` HEAD `60fcfc9` is the merge-base, feature branch is 8 commits strictly ahead, `git merge-tree` shows zero conflicts). Brings in S4-01→S4-04 fixes + StatModifierTester + stat-system GDD/balance docs + enemy/boss stats data. | 0.25d | 🔴 Blocker for everything else |
| 1 | **Re-verify acceptance criteria in Play Mode post-merge** — equip → LMB attack → enemy Health decreases; ability exits cleanly (no stuck state); no Console warnings from `Core.GetCoreComponent<T>()` | 0.25d | 🔴 Must |
| 2 | **S4-05** — `PlayerInputHandle.cs`: audit all `Invoke`/`InvokeRepeating` calls; pair each with `CancelInvoke` in `OnDisable` | 0.25d | Should |
| 3 | **S4-06 start** — Read `TalentManager.cs`; create or extend `StatsCharacter` SO with the 5 fields (`strength`, `dex`, `int`, `cha`, `skillPoint`); Inspector-assignable | 0.5d | Should |

---

### Wed 09/07 — PLAN
**Goal: Complete S4-06 + begin S4-07 if capacity allows**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-06 finish** — Wire `TalentManager` to SO; remove hardcoded `Awake` assignments; add `[Range]` validators on SO fields | 0.5d | Should |
| 2 | **S4-07 start** — Decouple `Weapon.Interact()` / `WeaponHolder.Equip()` (time-box 0.5d; carry if over) | 0.5d | Nice |

---

### Thu 10/07 — PLAN
**Goal: S4-07 finish OR S4-08 EditMode test + first playtest session**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-07 finish** OR **S4-08** EditMode test for `Core.GetCoreComponent<T>()` | 0.5–1.0d | Nice |
| 2 | **First playtest session** — if S4-04 landed Mon: run combat loop, log findings under `production/qa/playtests/` | — | Advisory |

---

### Fri 11/07 — WRAPUP DAY
**Goal: Smoke-check + run `/weekly-wrapup` → close sprint, code-review, retrospective**

| # | Task | Est |
|---|------|-----|
| 1 | Final smoke-check: full demo loop (equip → melee hit deals damage → ability run + exit → player takes damage) | — |
| 2 | `/weekly-wrapup` — code review `.cs` changes, playtest log if session happened, bug triage, retro | — |
| 3 | Record carry-over + velocity in `sprint-04.md` Sprint Close section | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| P1 fixes stranded on unmerged branch | 🟡 NEW (Day 2) — S4-01→S4-04 all complete on `origin/feature/enhance-stats-system`, zero of it on `sprint-04`. Confirmed clean fast-forward, no conflicts. If this merge doesn't happen today, `sprint-04` enters Day 3 still showing 0% on its own branch despite the work existing. | Merge `feature/enhance-stats-system` → `sprint-04` as Priority 0 today; re-verify acceptance criteria in Play Mode post-merge before starting S4-05/S4-06 |
| Off-plan work displacing Must-Haves (pattern from 8 prior sprints) | 🟡 PARTIALLY BROKEN — Must-Haves *are* done (just not merged); but Kay's own `sprint-04` commit (`60fcfc9`, Mon) was still off-plan StatSystem work, and the feature branch itself carries substantial off-plan additions (GDD, balance Excel, StatModifierTester, enemy/boss stats data) riding along with the P1 fixes | Merge is still required (P1 fixes are entangled with the off-plan content on that branch) — accept the bundle, but hold new off-plan work until S4-05/S4-06 land |
| BUG-AH-1 scope wider than `AbilityHolder.cs` alone | ✅ Resolved on feature branch — 0 runtime `using UnityEditor` hits confirmed | Re-verify after merge to `sprint-04` |
| Developer absence / zero-commit days | 🟢 Lower — 2 commits landed 07-07 (`60fcfc9` sprint-04, plus feature-branch activity) | Continue monitoring |
| S4-03 lazy-cache breaks call sites | ✅ Resolved on feature branch — `Core.cs` foreach + `Dictionary<Type,CoreComponent>` cache, public signature unchanged | Re-verify no Console warnings after merge + in Play Mode |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-05 (Sun)**: Sprint-04 opened. Branch `sprint-04` created from `sprint-03`. All items carry from Sprint 3 (0% velocity, 8th consecutive sprint with same P1 backlog). Sprint-03 formally closed. 22:00 scheduled wrapup confirmed: bug-triage-2026-07-05.md, retro-sprint-03-2026-07-05.md, sprint-03.md close — all committed @ 2fb9211. No new CS changes since 20:08 close. Sprint-04 opens Mon 2026-07-07.
- **2026-07-06 (Sun)**: Pre-sprint day. **Zero sprint task progress** — all S4-01→S4-08 remain Not Started. ⚠️ RISK DETECTED: uncommitted changes found in `Assets/Skill Enhance/` (off-plan folder): `IAbilityOwner.cs` (3 interface members commented out), `SpiritDoTBehaviour.cs` (entire DoT implementation commented out). Also untracked `StatsSO.cs.meta`. None of these belong to S4 Must-Have tasks. Pattern warning: off-plan work occurring before sprint Day 1 — exact same pattern as 8 prior sprints. Sprint Day 1 (Mon 07/07) must focus exclusively on S4-01→S4-04.
- **2026-07-07 (Mon, Day 1) — 02:00 standup**: The pre-sprint uncommitted work from 07-06 was committed anyway, off-plan: `c181bd2` (StatType/Stat serialization), `6cd30f1` "coding" (StatsSO + new `Assets/.../Abilities/` files: `Conditions/`, `Core/AbilityInstance.cs`, `Effects/`, `Runtime/SpiritDoTBehaviour.cs`, `SpiritOrbProjectile.cs` — a system not referenced anywhere in `CLAUDE.md`), `c16ee77` (ADR-0001 for the StatSystem dual data structure), `dac227d` "fix base calculate stats" (StatsSO/Stat/DerivedStatFormula). **Zero commits touch S4-01→S4-04.** Verified directly against acceptance criteria: `grep -r "using UnityEditor" Assets/Script/` still returns 7 runtime hits (S4-01 open); `AnimationPlayerController.cs` line 21 still registers `StartAnimation` twice instead of `EndAnimation` (S4-02 open); `Core.cs` line 24 still uses `coreComponents.OfType<T>().FirstOrDefault()` (S4-03 open); `WeaponMelee.cs` `Attack()` foreach body (line 30-32) is still empty (S4-04 open). This is now day 1 of the 9th sprint carrying the exact same pattern flagged in the sprint's own risk register. Separately, a WIP uncommitted change to `Assets/Script/Utility/GameConstants.cs` exists on branch `feature/stats-system` (stashed during this standup, not evaluated) — also off S4-plan.
- **2026-07-08 (Tue, Day 2) — 02:00 standup**: Two things happened in parallel on 07-07, on two different branches. (1) On `sprint-04` itself: Kay committed `60fcfc9` "fix bug" (11:23) — StatSystem work only (`StatsSO` lookup changed from `Dictionary<StatType,float>` to `Dictionary<StatType,Stat>`, `DerivedStatFormula.Evaluate` signature change, new `GameConstants.StatsTypeNames` map). Off-plan, zero S4-01→S4-04 progress on this branch. (2) On the separate, unmerged branch `origin/feature/enhance-stats-system` (diverged from `sprint-04` at exactly `60fcfc9`, so it is 8 commits strictly *ahead*, not diverged in the conflicting sense): Claude landed `87acd8f` "land P1 combat fixes S4-01, S4-02, S4-04" and `3987933` "S4-03 — Core.GetCoreComponent LINQ to foreach + lazy cache". **Verified directly**: feature branch has 0 runtime `using UnityEditor` hits, `Core.cs` uses `foreach` (no `OfType`/LINQ), `WeaponMelee.Attack()` now resolves `INegativeReceiver` and calls `TakeDamage()`. All 4 Must-Haves are objectively complete — for the first time in 9 sprints — but not on `sprint-04`. The same feature branch also carries substantial off-plan additions: `StatModifierTester` editor tool, `design/gdd/stat-system.md`, leveled stat-system Excel (perLevel + rank tiers Lv1-20), and enemy/boss stats data-drive wiring. `git merge-tree` confirms a clean fast-forward, zero conflicts, merging feature→sprint-04. **Action**: flagged merge as top priority for today; tracker plan revised accordingly.
- **2026-07-07 (Mon)**: **S4-01, S4-02, S4-04 landed** — first P1 progress in 8 sprints. S4-01: removed `using UnityEditor.*` from 7 runtime scripts (`WeaponMeleeStats`, `StatsCharacter`, `EntityData`, `EnemySO`, `DualAbility`, `AnimationEventManager`, `AbilityHolder`); `StatsCharacter.animator` retyped `AnimatorController` → `RuntimeAnimatorController` (build-safe base). `grep "using UnityEditor" Assets/Script/` now returns 0 runtime hits. S4-02: `AnimationPlayerController` line 21 `StartAnimation` → `EndAnimation` registration + `OnDisable` mirror. S4-04: `WeaponMelee.Attack()` foreach now calls `INegativeReceiver.TakeDamage()` (mirrors `EntityWeaponMelee`). Play Mode smoke check pending in Editor (no Unity CLI in this environment).
- **2026-07-07 (Mon, cont.)**: **S4-03 landed — all four Must-Have P1 items now complete.** `Core.GetCoreComponent<T>()` rewritten: removed `using System.Linq`, replaced `OfType<T>().FirstOrDefault()` with a `foreach` + `is T` pattern match, added a `Dictionary<Type,CoreComponent>` lazy cache so the loop runs once per component type (subsequent calls are O(1), zero-alloc). Method signature unchanged — all 3 call sites (`WeaponMelee.Equid`, `AbilityHolder.Start`, `PlayerState.Enter`) compile unchanged. `grep "OfType|System.Linq" Core.cs` → 0 hits. Definition of Done for the Must-Have block met; combat is testable for the first time in 8 sprints (pending Editor Play Mode smoke + Profiler GC check).
