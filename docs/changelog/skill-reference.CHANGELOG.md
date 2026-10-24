# Changelog — `skill-reference.md`

Change log for [`docs/skill-reference.md`](../skill-reference.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-09 — PM routines redesigned (bug inbox, weekly playtest, doc-truth routine)

- **Commit:** none yet (working tree on `origin/feature/update-bug-document`, HEAD `221d54be`)
- **Changed:** §9 rows for `/doc-sync`, `/daily-standup`, `/weekly-kickoff`, `/weekly-wrapup`,
  `/weekly-sprint`; new "Weekly PM cycle" note; header note on 81 skills / 39 agents.
- **From → To:** "Daily 10:00 standup" → "Mon–Fri 02:00 … compile check + Unity log scan …";
  wrap-up "playtest log, bug triage" → "reads the owner's playtest sheet, fix-survival sweep,
  bug-inbox triage (the only bug-ID allocator)"; `/weekly-sprint` "superseded in practice" →
  "No routine calls it since 2026-10-09".
- **Why:** owner decision 2026-10-09 to close four review-process gaps (bug-ID authority, doc-vs-code
  check, regression sweep, runtime evidence) and let `.md`-only routines run unattended. Sources:
  `production/qa/reviews/skill-audit-2026-10-09.md`, `production/review-flow.md` v3.

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+3 / −3 lines)
- **Sections touched:** “Studio Skill Reference”, “3. Architecture”
- **From → To:** `` > development phase. Generated 2026-06-08 from `.claude/skills/`; re-synced 2026-08-21 `` → `` > development phase. Generated 2026-06-08 from `.claude/skills/`; re-synced **2026-09-11** ``
- **From → To:** `` | `/architecture-decision` | Creates one ADR (Architecture Decision Record) documenting a significant technical decision | Every major … `` → `` | `/architecture-decision` | Creates one ADR (Architecture Decision Record) documenting a significant technical decision | Every major … ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-21 — docs: second-pass audit of the four documents the first pass never opened
- **Commit:** `3e0ee09d` (+37 / −15 lines)
- **Sections touched:** “Studio Skill Reference”, “3. Architecture”, “7. Asset & UX/UI”, “8. Performance & Tech Debt”, “9. Recurring Reviews & Process”, “11. Team Orchestration (multi-agent, parallel coordination)” …
- **From → To:** `` > development phase. Generated 2026-06-08 from `.claude/skills/`. `` → `` > development phase. Generated 2026-06-08 from `.claude/skills/`; re-synced 2026-08-21 ``
- **From → To:** `` | `/architecture-decision` | Creates one ADR (Architecture Decision Record) documenting a significant technical decision | Every major … `` → `` | `/architecture-decision` | Creates one ADR (Architecture Decision Record) documenting a significant technical decision | Every major … ``
- **From → To:** (nothing) → `` | `/ui-screen` | Builds runnable UI Toolkit screens (UXML + USS + controller + wired scene) from a mockup, Figma description, or text flow … ``
- … 4 more hunks — `git show 3e0ee09d -- docs/skill-reference.md`
- **Why:** commit message: “docs: second-pass audit of the four documents the first pass never opened”

## 2026-06-08 — chore: add studio skill reference doc
- **Commit:** `527a00dd` (+155 / −0 lines)
- **Changed (from → to):** document created (+155 lines)
- **Why:** commit message: “chore: add studio skill reference doc”

## 2026-10-05 — Doc sync against HEAD `93ba6d8e` (38 commits, sprints 16-17) + per-system layout (no matching commit on that date)
- **Commit:** none found for this date in `git log --follow` (likely committed on a later date or as part of a merge)
- **Changed (from → to):** 5 ADRs + 2 architecture notes, 2 diagrams, `ui-ux-flow.md`, `skill-reference.md`,
- **Why:** see that section of `docs/CHANGELOG-DOCS.md`
