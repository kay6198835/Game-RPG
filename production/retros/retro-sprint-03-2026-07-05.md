# Sprint 3 Retrospective

> **Sprint**: 2026-06-23 to 2026-06-27
> **Retro date**: 2026-07-05 (Saturday — 8 days late; wrapup routine missed its window)
> **Facilitator**: Automated PM (weekly-wrapup)
> **Branch**: sprint-03 (merged into sprint-04; final HEAD e50643c)
> **Note on activity windows**: The formal sprint window (Mon–Fri, 2026-06-23 to 2026-06-27) had 0 code commits. A post-sprint burst (2026-06-30 to 2026-07-04) delivered significant new work on the sprint-03 branch before it was merged into sprint-04.

---

## Sprint Goal Reminder

> Clear the P1 debt blocking combat verification: fix BUG-AH-1, Bug #9, S2-03 (Core LINQ), and Bug #4 — so the equipped-weapon → attack → damage loop is playable for the first time. If capacity allows, start promoting TalentManager to SO-driven stats.

---

## Verdict: CONCERNS — 0% Must-Have, ~1.5d total work (post-sprint window)

**Must-Have velocity: 0%** — all 4 Must-Have tasks (S3-01 to S3-04) carry to Sprint 4 unresolved.
**Total sprint-period work**: ~1.5 estimated days of new code delivered, all in the post-sprint window (2026-06-30 to 2026-07-04).

The verdict is CONCERNS rather than FAIL because real engineering work landed — but it was wrong-priority work and it landed after the sprint formally ended.

---

## What Was Delivered

### During Sprint Window (2026-06-23 to 2026-06-27)
| Work | Committed | Notes |
|------|-----------|-------|
| Daily standup log (Mon 2026-06-23) | 406e41b | PM documentation only — no code |

### Post-Sprint Burst (2026-06-30 to 2026-07-04)

| Work | Commits | Notes |
|------|---------|-------|
| StatSystem SO foundation | 9d1ecbf | `StatsSO`, `Stat`, `StatModifier`, `StatType`, `DerivedStatFormula` — SO-driven stat architecture for S3-06 scope |
| Stats formula emulator tool | 0bf02f0 | Excel-based formula calculator + stat presets for 5 player types and boss/creep variants |
| General coding | 34845f9, e69a485, c471416 | Vague commit labels — content partially in StatSystem and Skill Enhance |
| BUG-RC-1 FIXED | 05b76cc | `RoomCell.GetDoor()` now returns `null`; DoorControllers created via `Instantiate(doorPrefab)` |
| Skill Enhance system | (via merge) | New `Assets/Skill Enhance/Scripts/Abilities/` — data-driven ability pipeline separate from existing `ActivateSkill` |
| NegativeReciver.cs | (in coding) | New CoreComponent (filename typo inherited from interface) |
| Docs updated | c65b391, 50d1a28, b098f6f | CLAUDE.md sync, map-system.md GDD update, tech-debt-register.md TD-021–TD-032 |

---

## What Was Not Delivered (Must-Have)

| ID | Task | Est | Status |
|----|------|-----|--------|
| S3-01 | Fix BUG-AH-1 — AbilityHolder UnityEditor imports | 0.25d | Not done |
| S3-02 | Fix Bug #9 — AnimationPlayerController double-registration | 0.25d | Not done |
| S3-03 | Complete S2-03 — Core.GetCoreComponent LINQ → foreach | 0.25d | Not done (LINQ confirmed still present) |
| S3-04 | Fix Bug #4 — WeaponMelee.Attack() empty foreach | 0.25d | Not done (foreach still empty) |

Combined total: 1 hour of fix work. All 4 items have been on the Must-Have list since Sprint 1 or 2.

---

## What Went Well

### 1. StatSystem is a solid foundation
`StatsSO` / `Stat` / `StatModifier` / `DerivedStatFormula` follow the `scriptableobject-data.md` pattern: data in SOs, modifier stack with source tracking, event-driven `OnStatChanged`. This is production-grade architecture — far ahead of the hardcoded `TalentManager` prototype it was meant to replace. It addresses S3-06 in scope and quality.

### 2. BUG-RC-1 closed
`RoomCell.GetDoor()` was fixed (commit 05b76cc) — returns `null` instead of incorrectly constructing `new DoorController()` via `new`. One P1 bug resolved for the first time since Sprint 2.

### 3. Skill Enhance — composable ability architecture
The `Skill Enhance` system introduces a data-driven ability pipeline (Conditions + Effects as separate SO components composited into `AbilityDefinition`). This is architecturally more flexible than `ActivateSkill` subclassing. If adopted, it reduces boilerplate for new skill types significantly.

### 4. Stats formula tool (design support)
An Excel-based emulator with 5 player type presets and formula validation was produced alongside the StatSystem. This gives the designer a sandbox for balance tuning without needing Unity Play Mode.

### 5. Combo attack architecture (from pre-sprint commits) is working
`StatusAnimation` switch + `BufferIsAttack` buffer — correct model for combo chains. When BUG-04 is eventually fixed, the combo extend will work naturally.

---

## What Went Wrong

### 1. Sprint window had zero code commits (8th week P1 backlog pattern)
The formal sprint (Mon–Fri, 2026-06-23 to 2026-06-27) produced no code. Zero commits in a 5-day window where the 4 Must-Have items total ~1 hour of fix work. The developer was either absent or worked on something else entirely.

### 2. Must-Have P1 fixes skipped for the 3rd+ sprint
BUG-AH-1, BUG-09, BUG-CORE-1, BUG-04 have been on the sprint Must-Have list for 3 consecutive sprints. Each is a 1-2 line fix. None has been touched. The pattern is consistent: the developer chooses larger feature work over the small critical fixes.

### 3. Post-sprint work was the wrong priority order
The post-sprint burst (2026-06-30 to 2026-07-04) delivered StatSystem (S3-06, a Should-Have) and a new Skill Enhance architecture (unplanned) — but did not touch any of the 4 Must-Have fixes that have been accumulating for 7+ sprints. BUG-04 (empty foreach: 1 line) and BUG-09 (wrong string: 2 lines) were skipped again while a complete new stat engine was written.

### 4. Wrapup and kickoff routines missed
The Sprint 3 wrapup (due 2026-06-28 Saturday) never ran. This triage is 8 days late. The Sprint 4 kickoff ran today (2026-07-05) opening sprint-04 branch without a formal Sprint 3 close record. Both routines require an active Claude Code session — if the developer is absent, they silently miss.

### 5. New architecture added without integration plan
Two new systems were created (`StatSystem` and `Skill Enhance`) that are not yet connected to the existing player/entity pipeline. `StatsSO` replaces the need for `TalentManager.cs` (prototype) but `TalentManager` has not been removed or wired. The `Skill Enhance` ability system is architecturally parallel to `ActivateSkill` — both exist without a decision on which survives. This increases codebase ambiguity.

### 6. Mutable shared SO (BUG-SS-2)
`StatsSO` holds a mutable `Dictionary<StatType, Stat>` at the SO level. Because SOs are shared assets, this runtime state persists across Play Mode sessions in the Editor and could be shared if multiple characters reference the same SO. This is a design flaw in the new StatSystem that needs an architecture decision before Sprint 4 stats wiring begins.

---

## Root Causes

| Issue | Root Cause |
|-------|-----------|
| Must-Haves skipped sprint after sprint | Absence during sprint window + feature-work preference over short critical fixes when sessions do open |
| Wrapup missed | No active Claude Code session on Saturday 2026-06-28 |
| Wrong priority order | Developer works on what is interesting (new systems), not what is critical (1-line fixes) |
| New systems without integration path | Work happens in isolation (parallel branches) without coordination on adoption vs. integration vs. replacement |

---

## Action Items for Sprint 4

| # | Action | Priority | Est |
|---|--------|----------|-----|
| 1 | **S4-01** Fix BUG-AH-1 — delete `using UnityEditor.Experimental.GraphView` from `AbilityHolder.cs` | P1 — Day 1 | 5 min |
| 2 | **S4-02** Fix BUG-09 — `AnimationPlayerController.cs` lines 21 + 29: `StartAnimation` → `EndAnimation` | P1 — Day 1 | 5 min |
| 3 | **S4-03** Fix BUG-CORE-1 — `Core.GetCoreComponent<T>()`: replace LINQ with `foreach` | P1 | 15 min |
| 4 | **S4-04** Fix BUG-04 — `WeaponMelee.Attack()`: add `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)` | P1 | 5 min |
| 5 | **S4-05** Fix BUG-PIH-1 — add `CancelInvoke(nameof(ChangeIsTakeDamage))` before `Invoke()` in `PlayerInputHandle.cs:264` | P2 | 5 min |
| 6 | **S4-06** Fix BUG-SS-2 — `StatsSO`: move mutable `stats` dictionary to an instance-per-runtime component; SO becomes config-only | P2 | 1d |
| 7 | **ADR** — write Architecture Decision Record: adopt `Skill Enhance` pipeline vs. extend `ActivateSkill` — pick one and remove the other | P2 | 0.5d |
| 8 | **Remove `TalentManager.cs`** prototype if `StatsSO` replaces it, or wire `StatsSO` to the player — close the gap | P2 | 0.25d |
| 9 | **First playtest** after S4-01 → S4-04 land — log in `production/qa/playtests/` | Advisory | — |

---

## Metrics

| Metric | Sprint 1 | Sprint 2 | Sprint 3 |
|--------|----------|----------|----------|
| Must Have completion | 0% | 14% | 0% |
| Total work delivered | Unknown | ~36% (incl. parallel) | ~1.5d (post-sprint burst) |
| P1 bugs closed | 0 | 1 pending merge | 1 (BUG-RC-1) |
| New bugs found | 6 | 4 | 4 |
| Playtest sessions | 0 | 0 | 0 |
| Commits in sprint window | Unknown | 3 days | 0 |
| Consecutive sprints, P1 backlog unresolved | 1 | 2 | 3 (same 4 items since S1) |

---

## Carry-Over to Sprint 4

From Sprint 3 Must Have (0 of 4 done — **3rd consecutive sprint carry**):
- S3-01 → S4-01: Fix BUG-AH-1
- S3-02 → S4-02: Fix Bug #9
- S3-03 → S4-03: Fix BUG-CORE-1
- S3-04 → S4-04: Fix Bug #4

From Sprint 3 Should Have:
- S3-05 → S4-05: Fix BUG-PIH-1

New Sprint 4 work from post-sprint findings:
- Fix BUG-SS-2 (StatsSO mutable shared state)
- ADR for Skill Enhance vs ActivateSkill
- Wire StatsSO to player character

Still deferred from Sprint 1:
- Bug #6 (player death)
- Bug #5/#7/#8 (enemy death chain)
- Room-clear condition

---

## Playtest Status

No playtest this sprint. Most recent: `production/qa/playtests/playtest-2026-06-12-weekly-wrapup.md`.
Playtest is blocked until BUG-04 (melee damage) and BUG-09 (state exit) are fixed. Both remain open.
Target: first playtest within Sprint 4 Day 1–2 once S4-01 → S4-04 land.

---

## Reference Files

- Bug triage: `production/qa/bug-triage-2026-07-05.md`
- Daily plan tracker: `production/sprints/sprint-03-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-02-2026-06-20.md`
