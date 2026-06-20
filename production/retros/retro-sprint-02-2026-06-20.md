# Sprint 2 Retrospective

> **Sprint**: 2026-06-15 to 2026-06-19
> **Retro date**: 2026-06-20 (Saturday wrap-up)
> **Facilitator**: Automated PM (weekly-wrapup)
> **Branch**: origin/fix-player-control @ 6452127

---

## Sprint Goal Reminder

> Pay down Player combat architecture debt: decouple Weapon from WeaponHolder/AbilityHolder, introduce `Core.GetCoreComponent<T>()`, and fold in Bug #9 (animation event) + Bug #4 (empty melee damage) so the equipped-weapon → attack → skill loop becomes playtestable by Friday.

---

## What Went Well

### 1. PlayerState architecture is cleaner
`Enter()` now centralizes all `GetCoreComponent()` calls instead of repeating them per-subclass. The `StatusAnimation` enum (`Start` / `StartRangeTrigger` / `OnActivate` / `OffActivate` / `EndRangeTrigger` / `End`) replaces the old `animFinish` bool and enables precise frame-window control — the foundation for the combo cancel system.

### 2. Combo-cancel architecture landed
`PlayerAttackState.LogicUpdate()` now switches on `StatusAnimation` and correctly handles `EndRangeTrigger` (buffer consume) and `None` (exit). `PlayerInputHandle.OnAttack()` implements the cancel-window check: buffered attack only fires when inside `StartRangeTrigger..EndRangeTrigger`. This is the right model.

### 3. Input buffer implemented
`BufferIsAttack` + `SetBufferAttack()` is a clean, explicit single-bit buffer. No frame-race conditions from implicit timing.

### 4. CoreComponent self-register works
`CoreComponent.Awake()` now calls `core.AddCoreComponent(this)` — removing the Inspector wiring requirement for the component list. Half of S2-03 is done.

### 5. Combat system completed in parallel (post-sprint)
Three commits (e314b88 / 5fa5e27 / 81c95de) in a parallel session — landed on Jun 19 outside the sprint branch — delivered the full melee combo system: Bug #4 fixed, animation event wiring, `damageMultiplier` in `AttackSO`, `ON_COMBO_HIT` event, `ComboCounterUI`. First real combat milestone in the project. Needs merge review.

---

## What Went Wrong

### 1. Sprint commitments missed again (3rd consecutive sprint)
| Task | Target | Actual |
|------|--------|--------|
| S2-01 Commit clean base | Done | ✅ Done |
| S2-02 Decouple Weapon↔WeaponHolder | 1.0d | ✂️ Cut |
| S2-03 Core.GetCoreComponent foreach | 1.0d | 🟡 Partial (LINQ still present) |
| S2-04 Fix Bug #9 (AnimController) | 0.25d | ⬜ Not started |
| S2-05 Fix Bug #4 (WeaponMelee.Attack) | 0.25d | ⬜ Not on sprint branch |
| S2-06 EditMode test | 0.5d | ✂️ Cut |

Velocity: **~0.5d / 3.5d estimated = 14%** on the sprint branch. If the parallel branch work (e314b88) is counted, effective velocity rises to ~1.25d / 3.5d = 36% — still below the 80% target.

### 2. Bug #9 (AnimationPlayerController) skipped again
`AnimationPlayerController.cs` was not touched this sprint. Bug #9 was rated Critical in both Sprint 1 and Sprint 2 plans. It has been open for 6+ weeks. The 2-line fix (change `StartAnimation` → `EndAnimation` on lines 21 and 29) requires ~5 minutes. The fact that it continues to be skipped suggests it is not being included in the actual daily work sequence.

### 3. LINQ not removed from Core.GetCoreComponent
S2-03 acceptance criteria explicitly required replacing LINQ with `foreach`. The API was created and self-register was wired, but `coreComponents.OfType<T>().FirstOrDefault()` remains. This means 5 LINQ calls on every state entry.

### 4. Editor namespace in AbilityHolder (build-breaking)
`using UnityEditor.Experimental.GraphView` and `using UnityEngine.UIElements` in `AbilityHolder.cs` will cause a Player build failure. This is a regression from the sprint's own acceptance criteria (S2-02 required removing `UnityEditor.*` imports). No Player build was attempted this sprint so the break is latent.

### 5. Parallel branch visibility gap
The most significant combat work (e314b88) happened in a separate Claude Code session and was committed to a branch not tracked by the sprint board. The sprint's daily plan and standup logs had no visibility into this work until the Saturday wrap-up.

---

## Root Causes

| Issue | Root Cause |
|-------|-----------|
| Bug #9 never fixed | It is not appearing on the developer's actual task queue for the day, despite being in sprint docs. The sprint board entry exists but is not being read at the start of each session. |
| S2-03 LINQ remaining | The task was marked "in progress" but the definition-of-done (foreach, no LINQ) was not checked at completion. |
| Editor namespace regression | AbilityHolder.cs opened during refactor; imports auto-added by IDE; not caught without a build check. |
| Parallel branch gap | Claude Code sessions without a session-start `git pull` + `git switch sprint-branch` create divergent work. |

---

## Action Items for Sprint 3

| # | Action | Owner | Priority |
|---|--------|-------|----------|
| 1 | Merge e314b88 / 5fa5e27 / 81c95de into sprint branch before planning | Developer | Before Sprint 3 kickoff |
| 2 | Fix BUG-09 (AnimationPlayerController lines 21, 29) as Sprint 3 **Day 1 first commit** — hard time-box 30 min | Developer | Sprint 3 Day 1 |
| 3 | Fix BUG-AH-1 (AbilityHolder Editor imports) before any Player build | Developer | Sprint 3 Day 1 |
| 4 | Complete S2-03: replace LINQ in `Core.GetCoreComponent<T>()` with `foreach` | Developer | Sprint 3 first half |
| 5 | Add `CancelInvoke(nameof(ChangeIsTakeDamage))` before Invoke in `PlayerInputHandle.cs:269` | Developer | Sprint 3 |
| 6 | Define process: each session starts on the sprint branch (not a new branch), to avoid parallel-branch visibility gaps | Process | Standing |

---

## Metrics

| Metric | Sprint 1 | Sprint 2 |
|--------|----------|----------|
| Must Have completion | 0% | 14% (sprint branch) / 36% (incl. parallel) |
| P1 bugs closed | 0 | 1 pending merge (BUG-04) |
| New P1/P2 bugs found | 6 | 4 |
| Playtest sessions | 0 (code-only verify) | 0 (code-only verify) |
| Days with code commits | 1 | 3 (Mon/Tue/Wed) |
| Zero-commit days | 4 | 2 (Thu/Fri sprint branch) |

---

## Carry-Over to Sprint 3

From Sprint 2 Must Have (not done):
- S2-02: Decouple Weapon ↔ WeaponHolder/AbilityHolder (push-on-equip)
- S2-03: Complete `Core.GetCoreComponent<T>()` foreach (LINQ removal)
- S2-04: Fix Bug #9 — AnimationPlayerController double-registration ← **Priority 1**

From Sprint 1 (still outstanding):
- Fix Bug #6 (player death)
- Fix Bug #5/#7/#8 (enemy death chain)
- Room-clear condition

New for Sprint 3:
- Merge + verify parallel branch combat commits
- Fix BUG-AH-1 (AbilityHolder Editor imports)
- Fix BUG-CORE-1 (Core.cs [SerializeField] on auto-property)
- Fix BUG-PIH-1 (CancelInvoke missing)

---

## Sprint 3 Outlook

The parallel branch's e314b88 gives Sprint 3 a rare advantage: working melee combat to build on instead of starting from an untestable state. Sprint 3 should:
1. Open with the merge + Bug #9 fix, making combat fully functional for the first time
2. Run a live playtest on day 1 or 2 to validate the combo feel
3. Scope S2-02 (Weapon decoupling) only if combat is stable — carry to Sprint 4 if playtesting reveals bigger issues
4. Stats system (planned Sprint 3 theme) should be de-prioritized until BUG-09 is confirmed fixed
