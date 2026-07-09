# Sprint 4 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-07 (Mon) → 2026-07-11 (Fri)
> **Companion to**: `sprint-04.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes yesterday from git + this tracker, updates statuses, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the week's `.cs`, playtest log, bug-triage, light retro; finalizes verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates upcoming week's sprint plan.
> **Last updated**: 2026-07-09 (Wed) — Day 3 standup: design track substantially closed, but Wed's actual work was off-plan code, not the planned S4-D3 wrap-up

---

## Status Verdict: 🟡 SLIPPED (plan-adherence) — Must-Have P1 block still closed and merged (`204be85`). GDD design work (S4-D1/D2/D4) is now substantively done — the design-review revision on 2026-07-08 resolved all 6 open questions in-session and the GDD is marked **Approved** in `systems-index.md` (header field on the GDD itself is stale, still reads "In Design" — doc-sync needed). **But** the Day-2 pivot decision was "design only, no code this sprint," and last night's actual commits (`a420d5e`, `9f1d96b`, 2026-07-09 00:20–00:23) are code — new `Assets/Script/Database-SO/` model classes, `Assets/SO/Database/` enemy assets/prefabs, and edits to `Assets/Script/LevelEdit/LevelManager.cs` — none of it under `prototypes/` per `.claude/rules/prototype-code.md`. S4-D3 (epic/story breakdown) is still not done — no file under `production/epics/`.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 2.5 days (Must + Should) + 2.25d Design track |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 3 (Day 3 morning) |
| Days remaining | 2 (Thu, Fri) |
| Work committed/done on `sprint-04` | 1.0d Must-Have (merged `204be85`) + ~1.75d Design (S4-D1 GDD, S4-D2 open-Qs, S4-D4 review — all done 2026-07-08) |
| Work done but unmerged | 0 |
| Work remaining (in-plan) | S4-D3 epic/story breakdown (0.5d, not started) |
| Off-plan this cycle | Enemy-spawn code prototyping started early (`a420d5e`, `9f1d96b`) — not estimated, not in this sprint's Design-Track scope, and not isolated under `prototypes/` |
| Velocity | Must-Have 100% done; Design-Track ~75% done (3 of 4 sub-tasks); off-plan code risk re-emerging (same pattern flagged in Risks since Sprint 3) |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S4-01 | Fix BUG-AH-1 — remove `UnityEditor` imports from `AbilityHolder.cs` + all runtime scripts | 0.25 | Must | ✅ Done — merged to `sprint-04`, verified `grep "using UnityEditor" Assets/Script/` = 0 hits |
| S4-02 | Fix Bug #9 — `AnimationPlayerController` double-registration | 0.25 | Must | ✅ Done — merged to `sprint-04`, verified lines 17-29 register/unregister `StartAnimation` + `EndAnimation` distinctly |
| S4-03 | `Core.GetCoreComponent<T>()` LINQ → foreach + lazy cache | 0.25 | Must | ✅ Done — merged to `sprint-04`, verified `foreach` + `Dictionary<Type,CoreComponent> _cache` in `Core.cs` |
| S4-04 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach (add TakeDamage) | 0.25 | Must | ✅ Done — merged to `sprint-04`, verified `INegativeReceiver.TakeDamage()` call present |
| S4-D1 | Author GDD `design/gdd/enemy-spawn-system.md` (8 sections) from owner spec — 4 SOs + `GetHybridEnemySet` | 1.0 | Design | ✅ Done (2026-07-08) — GDD authored, all 8 sections; Approach B locked (EnemyManager singleton); supersedes map-system.md EncounterSO plan |
| S4-D2 | Resolve 6 open design questions inside the GDD | 0.5 | Design | ✅ Done (2026-07-08) — resolved in-session during `/design-review` revision: `weight ≥ 1` invariant + termination guarantee, pinned tie-break, seed-injection param, concrete 50-seed variety AC, RoomType→RoomData table, entry-safety/jitter rules. Verified inline in GDD (e.g. "Open Q#3 — RESOLVED 2026-07-08" at line 177) |
| S4-D3 | Decompose into epic + per-sprint stories + dependency map (`/map-systems` → `/create-epics`) | 0.5 | Design | 🟡 In progress — `systems-index.md` updated (row 20, status Approved) satisfies the `/map-systems` half; `/create-epics enemy-spawn` not yet run, no file under `production/epics/` |
| S4-D4 | `/design-review` the GDD → APPROVED before hand-off | 0.25 | Design | ✅ Done (2026-07-08) — verdict NEEDS REVISION → revised same session → **Approved** (see `design/gdd/reviews/enemy-spawn-system-review-log.md`); note `enemy-spawn-system.md` line 12 header still says "In Design" — stale, needs a one-line doc-sync fix |
| S4-05 | Fix BUG-PIH-1 — `CancelInvoke` pairing in `PlayerInputHandle` | 0.25 | Should | ⏸️ PENDED → Sprint 5 — not on enemy-spawn critical path (`Invoke(nameof(ChangeIsTakeDamage), 0.2f)` at line 264, no matching `CancelInvoke`; actual path `Assets/Script/Character/Player/CoreComponent/PlayerInputHandle.cs`) |
| S4-06 | Stats system — `TalentManager` prototype → SO-driven | 1.0 | Should | ⏸️ PENDED → Sprint 5 — not on enemy-spawn critical path (`TalentManagger.Awake()` still hardcodes stats, no SO) |
| S4-07 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | 1.0 | Nice | ⬜ Deferred |
| S4-08 | EditMode test for `Core.GetCoreComponent<T>()` | 0.5 | Nice | ⬜ Deferred |

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

### Wed 09/07 — PLAN vs ACTUAL
**Planned goal: S4-D1 GDD authoring (stale line — S4-D1/D2/D4 were actually already finished 07-08 during same-session design-review revision).**
**Actual (git, 00:20–00:23 07-09)**: no design-doc commits. Owner instead started coding the enemy-spawn prototype directly: `Assets/Script/Database-SO/Modal/{EnemyModal,EntityModel,MapModel,RoomModel}.cs`, `Assets/SO/Database/**` (enemy SO assets + 6 prefabs), `Assets/SO/Room/RoomData.asset`, and edits to `Assets/Script/LevelEdit/LevelManager.cs` (`SpawnRoomEnemies()`, door-tile TODO stub). This is real progress toward Sprint 5/6 scope, but it is (a) off this sprint's "design only" decision and (b) not isolated under `prototypes/` per `prototype-code.md`, and (c) the `RoomModel.GetHybridEnemySet()` Phase-2 pick uses `Random.Range` among all in-cap candidates, not the GDD's locked `argmin(|weight-remaining|)` tie-break, and takes no seed parameter (Open Q#5 asked for a seedable RNG hook) — worth a quick compare-to-GDD pass before this goes further.

---

### Thu 10/07 — PLAN (revised — Day 3 standup)
**Goal: Close S4-D3 (the one real remaining design gap) and reconcile the code that's already started**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S4-D3** — `/create-epics enemy-spawn` (systems-index half already done); map stories to S5 (data+algorithm) / S6 (runtime+room-clear), note enemy-death dependency | 0.5d | 🔴 Design |
| 2 | **Reconcile prototype vs GDD** — 🟡 partially done (2026-07-09): owner chose **doc-follows-code**, so the GDD now reverse-documents the prototype ("Current Implementation Status") and logs every gap ("Prototype Deviations" D1–D8, `TD-031`). **Still open (owner decision):** (a) move `Assets/Script/Database-SO/**` under `prototypes/enemy-spawn/` with a README per `prototype-code.md`, **or** (b) treat it as an early Sprint-5 start and harden `RoomModel` toward the GDD's injected-seed + `argmin` formula | — | Owner decision |
| 3 | ✅ Done (2026-07-09) — stale header `**Status**: In Design` → `Approved`; `EncounterSO`/`RoomEnemySpawner` refs synced across CLAUDE.md / project_state / tech-debt / map-system | — | Housekeeping |

---

### Fri 11/07 — WRAPUP DAY
**Goal: Smoke-check + run `/weekly-wrapup` → close sprint, code-review, retrospective**

| # | Task | Est |
|---|------|-----|
| 1 | `/weekly-wrapup` — review the week's `.cs` (including the new enemy-spawn prototype code), playtest log if any, bug triage, light retro | — |
| 2 | Record carry-over + velocity in `sprint-04.md` Sprint Close section (S4-05/S4-06 → Sprint 5; enemy-spawn design done; note prototype code carry-over) | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| P1 fixes stranded on unmerged branch | ✅ RESOLVED (Day 2, 11:19) — merge commit `204be85` lands `feature/enhance-stats-system` onto `sprint-04`. All 4 Must-Haves verified directly on `sprint-04` HEAD. | Closed. |
| Off-plan work displacing Must-Haves (pattern from 9 prior sprints) | 🟡 STILL LIVE — the merge itself carries substantial off-plan content (StatModifierTester editor tool, 6 enemy/boss `Stat` SO assets, GDD, balance Excel) riding in alongside the P1 fixes; net effect is P1 debt is closed, but off-plan volume keeps growing | Hold new off-plan work until S4-05/S4-06 land; do not let StatSystem expansion continue unbounded |
| BUG-AH-1 scope wider than `AbilityHolder.cs` alone | ✅ Resolved and re-verified on `sprint-04` post-merge — 0 runtime `using UnityEditor` hits | Closed. |
| Developer absence / zero-commit days | 🟢 Lower — active commit history 07-07→07-08 including the merge | Continue monitoring |
| S4-03 lazy-cache breaks call sites | ✅ Resolved and re-verified on `sprint-04` — `Core.cs` foreach + `Dictionary<Type,CoreComponent>` cache, public signature unchanged | Closed pending Play Mode confirmation (no Unity CLI available in this environment — flag for manual smoke check) |
| S4-05 scope confirmed | 🟡 NEW — audit of `PlayerInputHandle.cs` (actual path: `Assets/Script/Character/Player/CoreComponent/PlayerInputHandle.cs`, not `Input/` as documented in CLAUDE.md) found exactly 1 `Invoke` call (line 264, `ChangeIsTakeDamage`) with no `CancelInvoke` pairing in `OnDisable` | Small, well-scoped fix — should close in the 0.25d estimate; also flag CLAUDE.md path drift for correction |
| **2 enemy-spawn decisions still OPEN — decide this sprint** | 🟡 LIVE (opened 2026-07-08 at `/design-review`) — GDD Open Questions #1 & #2: (1) **`EnemyManager` singleton ADR** — gates the PlayMode lifecycle test harness (AC-L1…L6 can't be authored until architecture is ratified); (2) **runtime shuffle-seed source** (per-room-from-run-seed vs unseeded). Both blocking downstream. | **DAILY NUDGE**: raise both at each standup until closed. #1 → run `/architecture-decision`. #2 → owner pick; recommend per-room-from-run-seed. Neither blocks the algorithm's EditMode tests, only runtime/integration. |
| **Design-only decision broken same night it was made** | 🔴 NEW (07-09 00:20) — Day-2 pivot explicitly said "no code this sprint" for enemy-spawn; commits `a420d5e`/`9f1d96b` are code (new SO classes, assets, prefabs, `LevelManager.cs` edits), landed ~4h after the pivot was committed. Same off-plan-work pattern flagged every standup since Sprint 3. | Not blocking — the code is directionally useful for S5 — but needs an explicit owner call: keep going as an early S5 start (then align it to the GDD's locked algorithm spec and update `sprint-04.md`/`sprint-05` scope), or move it to `prototypes/` per the isolation rule. Raise at Thu standup if undecided. |
| **`EnemyManager` singleton pattern showing up early** | 🟡 NEW — `LevelManager.cs` (touched in last night's commits) already uses a bare `public static Instance` singleton (line 10), same pattern flagged as Bug #12 (ARCH) in `CLAUDE.md` and the exact pattern Open Q#1's ADR was meant to gate before code depends on it. | The ADR (Open Q#1) should land before more code takes a hard dependency on the singleton shape. |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-05 (Sun)**: Sprint-04 opened. Branch `sprint-04` created from `sprint-03`. All items carry from Sprint 3 (0% velocity, 8th consecutive sprint with same P1 backlog). Sprint-03 formally closed. 22:00 scheduled wrapup confirmed: bug-triage-2026-07-05.md, retro-sprint-03-2026-07-05.md, sprint-03.md close — all committed @ 2fb9211. No new CS changes since 20:08 close. Sprint-04 opens Mon 2026-07-07.
- **2026-07-06 (Sun)**: Pre-sprint day. **Zero sprint task progress** — all S4-01→S4-08 remain Not Started. ⚠️ RISK DETECTED: uncommitted changes found in `Assets/Skill Enhance/` (off-plan folder): `IAbilityOwner.cs` (3 interface members commented out), `SpiritDoTBehaviour.cs` (entire DoT implementation commented out). Also untracked `StatsSO.cs.meta`. None of these belong to S4 Must-Have tasks. Pattern warning: off-plan work occurring before sprint Day 1 — exact same pattern as 8 prior sprints. Sprint Day 1 (Mon 07/07) must focus exclusively on S4-01→S4-04.
- **2026-07-07 (Mon, Day 1) — 02:00 standup**: The pre-sprint uncommitted work from 07-06 was committed anyway, off-plan: `c181bd2` (StatType/Stat serialization), `6cd30f1` "coding" (StatsSO + new `Assets/.../Abilities/` files: `Conditions/`, `Core/AbilityInstance.cs`, `Effects/`, `Runtime/SpiritDoTBehaviour.cs`, `SpiritOrbProjectile.cs` — a system not referenced anywhere in `CLAUDE.md`), `c16ee77` (ADR-0001 for the StatSystem dual data structure), `dac227d` "fix base calculate stats" (StatsSO/Stat/DerivedStatFormula). **Zero commits touch S4-01→S4-04.** Verified directly against acceptance criteria: `grep -r "using UnityEditor" Assets/Script/` still returns 7 runtime hits (S4-01 open); `AnimationPlayerController.cs` line 21 still registers `StartAnimation` twice instead of `EndAnimation` (S4-02 open); `Core.cs` line 24 still uses `coreComponents.OfType<T>().FirstOrDefault()` (S4-03 open); `WeaponMelee.cs` `Attack()` foreach body (line 30-32) is still empty (S4-04 open). This is now day 1 of the 9th sprint carrying the exact same pattern flagged in the sprint's own risk register. Separately, a WIP uncommitted change to `Assets/Script/Utility/GameConstants.cs` exists on branch `feature/stats-system` (stashed during this standup, not evaluated) — also off S4-plan.
- **2026-07-08 (Tue, Day 2) — 02:00 standup**: Two things happened in parallel on 07-07, on two different branches. (1) On `sprint-04` itself: Kay committed `60fcfc9` "fix bug" (11:23) — StatSystem work only (`StatsSO` lookup changed from `Dictionary<StatType,float>` to `Dictionary<StatType,Stat>`, `DerivedStatFormula.Evaluate` signature change, new `GameConstants.StatsTypeNames` map). Off-plan, zero S4-01→S4-04 progress on this branch. (2) On the separate, unmerged branch `origin/feature/enhance-stats-system` (diverged from `sprint-04` at exactly `60fcfc9`, so it is 8 commits strictly *ahead*, not diverged in the conflicting sense): Claude landed `87acd8f` "land P1 combat fixes S4-01, S4-02, S4-04" and `3987933` "S4-03 — Core.GetCoreComponent LINQ to foreach + lazy cache". **Verified directly**: feature branch has 0 runtime `using UnityEditor` hits, `Core.cs` uses `foreach` (no `OfType`/LINQ), `WeaponMelee.Attack()` now resolves `INegativeReceiver` and calls `TakeDamage()`. All 4 Must-Haves are objectively complete — for the first time in 9 sprints — but not on `sprint-04`. The same feature branch also carries substantial off-plan additions: `StatModifierTester` editor tool, `design/gdd/stat-system.md`, leveled stat-system Excel (perLevel + rank tiers Lv1-20), and enemy/boss stats data-drive wiring. `git merge-tree` confirms a clean fast-forward, zero conflicts, merging feature→sprint-04. **Action**: flagged merge as top priority for today; tracker plan revised accordingly.
- **2026-07-07 (Mon)**: **S4-01, S4-02, S4-04 landed** — first P1 progress in 8 sprints. S4-01: removed `using UnityEditor.*` from 7 runtime scripts (`WeaponMeleeStats`, `StatsCharacter`, `EntityData`, `EnemySO`, `DualAbility`, `AnimationEventManager`, `AbilityHolder`); `StatsCharacter.animator` retyped `AnimatorController` → `RuntimeAnimatorController` (build-safe base). `grep "using UnityEditor" Assets/Script/` now returns 0 runtime hits. S4-02: `AnimationPlayerController` line 21 `StartAnimation` → `EndAnimation` registration + `OnDisable` mirror. S4-04: `WeaponMelee.Attack()` foreach now calls `INegativeReceiver.TakeDamage()` (mirrors `EntityWeaponMelee`). Play Mode smoke check pending in Editor (no Unity CLI in this environment).
- **2026-07-07 (Mon, cont.)**: **S4-03 landed — all four Must-Have P1 items now complete.** `Core.GetCoreComponent<T>()` rewritten: removed `using System.Linq`, replaced `OfType<T>().FirstOrDefault()` with a `foreach` + `is T` pattern match, added a `Dictionary<Type,CoreComponent>` lazy cache so the loop runs once per component type (subsequent calls are O(1), zero-alloc). Method signature unchanged — all 3 call sites (`WeaponMelee.Equid`, `AbilityHolder.Start`, `PlayerState.Enter`) compile unchanged. `grep "OfType|System.Linq" Core.cs` → 0 hits. Definition of Done for the Must-Have block met; combat is testable for the first time in 8 sprints (pending Editor Play Mode smoke + Profiler GC check).
- **2026-07-08 (Tue, Day 2) — pivot decision**: With the P1 backlog closed, owner directed the remaining sprint days to **design-only** work on a new **data-driven room-based enemy-spawn system** (4 SOs: `EnemyData`/`EnemyDatabase`/`MapEnemyDatabase`/`RoomData` + `GetHybridEnemySet` two-phase algorithm + `EnemyManager` singleton). Codebase audit confirmed the system is build-from-scratch (no existing spawn code; `Tile_Spawn` is a dead constant; `EnemySO` has no id/weight) and its room-clear half is hard-blocked on the unimplemented enemy-death chain (Bugs #7/#8, no `ON_ENEMY_DEATH`, `ON_CLEAR_ENEMY` never emitted). Decision: Sprint 4 produces the GDD + multi-sprint roadmap (S4-D1→S4-D4, ≈2.25d); implementation split across Sprint 5 (data+algorithm, +death chain in parallel) and Sprint 6 (runtime+room-clear). Non-Must-Have S4-05/S4-06 pended → Sprint 5 (not on the enemy-spawn critical path). Sprint files updated accordingly.
- **2026-07-08 (Tue, Day 2) — 11:19 update (post-standup)**: **Merge landed.** `204be85` merges `origin/feature/enhance-stats-system` into `sprint-04` (created ~30 min after the 10:49 standup commit `830af8d`). Not a pure fast-forward — a real merge commit, since `sprint-04-daily-plan.md` itself had diverged by 2 lines between branches. Re-verified all 4 acceptance criteria directly on `sprint-04` HEAD: (1) `grep -rn "using UnityEditor" Assets/Script/` → 0 hits; (2) `AnimationPlayerController.cs` lines 17-29 → `StartAnimation`/`EndAnimation` registered and unregistered distinctly; (3) `Core.cs` → `foreach` loop + `Dictionary<Type,CoreComponent> _cache`, zero `OfType`/LINQ; (4) `WeaponMelee.cs` lines 31-34 → resolves `INegativeReceiver` via `GetComponentInChildren` and calls `TakeDamage()`. **First time in 9 sprints the full P1 block is both done and on the sprint branch.** The merge also pulled in off-plan content riding along: `StatModifierTester` editor tool, 6 new `Stat` SO assets (`PlayerStats`, `Assasin`, `Boss`, `FastSwarmStats`, `RangedCaster`, `TankStats`), `design/gdd/stat-system.md`, balance Excel/design doc, and an `Assets/Scenes/Main/SetLevel.unity` scene rewrite (972-line diff — not evaluated, out of standup scope). Play Mode re-verification (equip → LMB attack → enemy Health decreases; ability exits cleanly) still pending — no Unity CLI in this environment, needs manual confirmation in-Editor. Checked S4-05/S4-06 readiness for today's remaining work: `PlayerInputHandle.cs` (actual location `Assets/Script/Character/Player/CoreComponent/`, not `Input/` per CLAUDE.md) has exactly one un-paired `Invoke` call (line 264); `TalentManagger.cs` (class name has a typo) still hardcodes stat values in `Awake()`, no SO wiring yet. Both confirmed Not Started, ready to pick up.
- **2026-07-08 (Tue, Day 2, 20:14) — `ddbd54d` "plan enemy spawn"**: Tracker/plan files updated for the design pivot (`sprint-04.md`, `sprint-04-daily-plan.md`, `systems-index.md`, new `production/sprint-status.yaml`). At this same commit S4-D1 was already recorded Done — meaning the GDD authoring plus the `/design-review` revision (see `design/gdd/reviews/enemy-spawn-system-review-log.md`, verdict NEEDS REVISION → revised same session → Approved) both actually happened on 07-08, resolving S4-D2's 6 open questions and S4-D4's review gate in the same pass. The Wed/Thu/Fri day-plan text wasn't fully reconciled with that at the time (Wed still read "Author the GDD") — fixed in this standup.
- **2026-07-09 (Wed, Day 3) — 02:00 standup**: **Off-plan code, not design work.** `a420d5e` "prototypr spawn enemy" (00:20) and `9f1d96b` "miss" (00:23) — both ~4h after the design-only pivot was committed. Added: `Assets/Script/Database-SO/Modal/{EnemyModal,EntityModel,MapModel,RoomModel}.cs` (the `GetHybridEnemySet` algorithm, implemented but diverging from the GDD's locked formula — `Random.Range` pick instead of `argmin(|weight-remaining|)` tie-break, no seed parameter despite Open Q#5), `Assets/SO/Database/**` (enemy SO assets + 6 prefabs: Assasin/Boss/FastSwarm/RangeCaster/Tank/TrashMelee), `Assets/SO/Room/RoomData.asset`, and `Assets/Script/LevelEdit/LevelManager.cs` (+41 lines: `SpawnRoomEnemies()`, a still-empty door-tile-swap TODO block). Also renamed/restructured several `Stat` SO assets and deleted two now-superseded scenes (`DungeonStart.unity`, `RenderRadomDungeon.unity`). None of this is under `prototypes/` per `.claude/rules/prototype-code.md` (no README, no Hypothesis/Result/Decision). Verified: `production/epics/` still has no enemy-spawn file, so S4-D3 (the one genuinely unfinished design task) is still open. Tracker updated: S4-D2/D4 retroactively marked Done (07-08), S4-D3 marked In Progress, two new risks logged (design-only decision broken same night; `LevelManager` singleton pattern pre-empting the Open Q#1 ADR).
