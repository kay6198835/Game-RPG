# Changelog — `ui-ux-flow.md`

Change log for [`docs/ui/ui-ux-flow.md`](../ui-ux-flow.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-10 — Scene path; ConfirnPanel noted (doc-sync --auto)
- **Commits:** `221d54be` "logic open door at start room", merged into `sprint-18` by `42a81260` "update flow" (`c6cbc57a`)
- **Changed:** banner, build-index table
- **From → To:** `Main/Test/LoadRandomMap` → `Main/LoadRandomMap`; banner notes the new `ConfirnPanel`.
- **Why:** the scene was moved and the panel was added in code.
- **⚠️ Out of date against code (found by doc-sync, not yet fixed):** sections 6-8 do not describe `ConfirnPanel` / `UIEvents.OnOpenConfirmPanel` — the panel has an open defect (BUG-102) and its labels are swapped, so a description now would document a bug.

## 2026-10-05 — Mock removal
- **Changed:** banner, scene flow, section 3 (flags → configuration), 6.2, 6.3, section 7 (removed), section 8 table, findings 6 and 8, "Working on it", file map
- **From → To:** five flags with a mock path → `skipLogin` + timings only; `GameplayMock` harness documented → removed; real-vs-mock table → what is wired today; finding 8 (stale provider after rebuild) → resolved; `WorldHealthBar` / `DamageNumber` kept but now have no caller (finding 6 still open)
- **Why:** owner request to remove mock and test-only flows before building the UI/UX on the real game
- **Bugs:** none

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
