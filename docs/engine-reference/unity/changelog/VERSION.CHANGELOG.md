# Changelog — `VERSION.md`

Change log for [`docs/engine-reference/unity/VERSION.md`](../VERSION.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+41 / −4 lines)
- **Sections touched:** “Unity — Version Reference”, “Pinned Package Versions”, “VContainer 1.19.0 (added 2026-09-11)”, “UI Toolkit vs UGUI”, “Migration Notes”
- **From → To:** `` | **Last Docs Verified** | 2026-05-19 | `` → `` | **Last Docs Verified** | 2026-09-11 | ``
- **From → To:** (nothing) → `` > **Table corrected 2026-09-11.** VContainer was added on 2026-08-22 (`aa4e620`) and was missing ``
- **From → To:** `` | 2D Feature Pack | current | 2D Renderer (URP), Tilemap, Sprite Atlas | `` → `` | 2D Feature Pack | 2.0.1 | 2D Renderer (URP), Tilemap, Sprite Atlas | ``
- … 4 more hunks — `git show bbfb3028 -- docs/engine-reference/unity/VERSION.md`
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-07-09 — fix doc
- **Commit:** `65c8228e` (+2 / −2 lines)
- **Sections touched:** “Unity — Version Reference”
- **From → To:** `` | **Engine Version** | Unity 2022.3.62f1 LTS | `` → `` | **Engine Version** | Unity 2022.3.62f3 LTS | ``
- **Why:** commit message: “fix doc”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+68 / −0 lines)
- **Changed (from → to):** document created (+68 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”

## 2026-10-05 — Doc sync against HEAD `93ba6d8e` (38 commits, sprints 16-17) + per-system layout (no matching commit on that date)
- **Commit:** none found for this date in `git log --follow` (likely committed on a later date or as part of a merge)
- **Changed (from → to):** `tech-debt-register.md`, `VERSION.md`) kept their paths and got `changelog/<doc>.CHANGELOG.md`
- **Why:** see that section of `docs/CHANGELOG-DOCS.md`
