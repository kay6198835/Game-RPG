# Changelog — `animation-system.md`

Change log for [`design/gdd/animation-system.md`](../animation-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-21 — update doc for ability system, update 1-7 dir animation clip, done animation for ability consecrae
- **Commit:** `ac347d89` (+47 / −1 lines)
- **Why:** Abilities v2 re-synchronisation + Paladin direction renumber (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `design/gdd/animation-system.md` | **New section "8-direction index convention"** under Formulas — the `DirectionResolver` index table (0 = down-left, clockwise), the asset classes it binds, and the Paladin renumber precedent. This convention had never been written down, which is how the Paladin set was imported against a different one. Header re-verification banner updated.
- **Sections touched:** “Animation System”, “8-direction index convention [ADDED 2026-09-21]”
- **From → To:** `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** No drift found. The `StatusAnimation` handoff `` → `` > **Re-verified 2026-09-21 against HEAD `15242e6`.** The `StatusAnimation` handoff is unchanged. ``
- **From → To:** (nothing) → `` ### 8-direction index convention [ADDED 2026-09-21] ``

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+7 / −0 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** **Changed:** verification banners. `map-system.md` and `animation-system.md` had **no drift** — all
- **Sections touched:** “Animation System”
- **From → To:** (nothing) → `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** No drift found. The `StatusAnimation` handoff ``

## 2026-09-01 — coding
- **Commit:** `ddcc0a5c` (+1 / −1 lines)
- **Sections touched:** “Core Rules”
- **From → To:** `` | `EndRangeTrigger` | `AnimtionFinishTrigger()` *(typo intentional)* | Chain to the next stage, or fall through to exit | `` → `` | `EndRangeTrigger` | `AnimationFinishTrigger()` | Chain to the next stage, or fall through to exit | ``
- **Why:** commit message: “coding”

## 2026-08-21 — docs(animation): rewrite the GDD against StatusAnimation, and flag the dead event bus
- **Commit:** `7911a960` (+149 / −96 lines)
- **Sections touched:** “Animation System”, “Overview”, “Core Rules”, “AnimationName constants”, “States and Transitions”, “Interactions with Other Systems” …
- **From → To:** `` > **Last Updated**: 2026-05-19 (implementation-status claims corrected 2026-08-20) `` → `` > **Last Updated**: 2026-08-21 (rewritten against the shipped `StatusAnimation` enum) ``
- **From → To:** `` The animation system is the timing bridge between Unity's Animator state machine and the gameplay logic layers. It translates animation … `` → `` The animation system is the timing bridge between Unity's Animator and the gameplay logic layers. ``
- **From → To:** `` All character combat timing in this project depends on this system: a melee attack deals damage exactly when the animation's hit frame … `` → `` | Piece | Where | Role | ``
- … 16 more hunks — `git show 7911a960 -- design/gdd/animation-system.md`
- **Why:** commit message: “docs(animation): rewrite the GDD against StatusAnimation, and flag the dead event bus”

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+34 / −14 lines)
- **Sections touched:** “Animation System”, “Core Rules”, “Edge Cases”, “Acceptance Criteria”
- **From → To:** `` > **Last Updated**: 2026-05-19 `` → `` > **Last Updated**: 2026-05-19 (implementation-status claims corrected 2026-08-20) ``
- **From → To:** `` **⚠️ Bug #9 — `AnimationPlayerController.OnEnable()` line 21:** `` → `` **✅ Bug #9 — FIXED (verified 2026-08-20).** ``
- **From → To:** `` **Correct registration table:** `` → `` **Registration table (matches source):** ``
- … 7 more hunks — `git show e1cd01f4 -- design/gdd/animation-system.md`
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-05-19 — Add animation system GDD and update systems index
- **Commit:** `f8321fbb` (+174 / −0 lines)
- **Changed (from → to):** document created (+174 lines)
- **Why:** commit message: “Add animation system GDD and update systems index”
