# Changelog — `ui-ux-flow.md`

Change log for [`docs/ui/ui-ux-flow.md`](../ui-ux-flow.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Game-completion phase (merged into the doc-sync layout)
- **Commit:** `cb0de496` (content); merge of `7d1b5c79` (change-log link)
- **Changed:** scope banner, flags, scenes, real-vs-mock table, findings
- **From → To:** flags default to mock → real game (only `skipLogin` on); character creation → removed; `StartScene` / `UISample` / pre-2026 UI assets → deleted; findings 1, 2, 5, 7 in section 9 → resolved. Merge conflict at the banner resolved by keeping the change-log link above `cb0de496`'s scope text and dropping the older `HEAD ae8be5e` scope line
- **Why:** "the UIFlow menu now starts the real dungeon and the HUD shows live player data" (commit message)
- **Bugs:** none

## 2026-10-05 — docs(ui): add end-to-end UI/UX flow reference for UIFlow
- **Commit:** `f8f180d0` (+372 / −0 lines)
- **Why:** Doc sync against HEAD `93ba6d8e` (38 commits, sprints 16-17) + per-system layout (`docs/CHANGELOG-DOCS.md`)
- **Changed:** 5 ADRs + 2 architecture notes, 2 diagrams, `ui-ux-flow.md`, `skill-reference.md`,
