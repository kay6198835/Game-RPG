# Changelog — `adr-0002-enemymanager-singleton-exception.md`

Change log for [`docs/architecture/adr-0002-enemymanager-singleton-exception.md`](../adr-0002-enemymanager-singleton-exception.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Refers to TD-023 (`LevelManager` singleton) as a live violation (`:79`, `:126`, `:348`, `:418`) — resolved 2026-10-02 (`0bc36406`). Status note added at the top; historical mentions left as written. `EnemyManager` and `MazeController` are now the only singletons
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+10 / −0 lines)
- **Sections touched:** “Status”
- **From → To:** (nothing) → `` > ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-21 — docs(adr): amend ADR-0002 to describe the EnemyManager that actually shipped
- **Commit:** `3f3bda85` (+81 / −0 lines)
- **Sections touched:** “Status”, “Related Decisions”
- **From → To:** (nothing) → `` > **⚠️ Amended 2026-08-21 — read the Amendment section at the end before acting on this ADR.** ``
- **From → To:** (nothing) → `` --- ``
- **Why:** commit message: “docs(adr): amend ADR-0002 to describe the EnemyManager that actually shipped”

## 2026-07-11 — docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05
- **Commit:** `e600eb24` (+5 / −5 lines)
- **Sections touched:** “Negative”, “Migration Plan”
- **From → To:** `` (`.claude/rules/manager-event-code.md` already updated; `engine-code.md` and `` → `` (**Update 2026-07-13: done.** `.claude/rules/manager-event-code.md`, `engine-code.md`, and ``
- **From → To:** `` 3. Update `.claude/rules/engine-code.md` and `.claude/rules/map-code.md` to name `` → `` 3. ✅ Done (confirmed 2026-07-13) — `.claude/rules/engine-code.md` and `.claude/rules/map-code.md` ``
- **Why:** commit message: “docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05”

## 2026-07-09 — fix doc
- **Commit:** `65c8228e` (+245 / −425 lines)
- **Sections touched:** “ADR-0002: EnemyManager is a permitted singleton exception”, “Engine Compatibility”, “ADR Dependencies”, “Problem Statement”, “Constraints”, “Requirements” …
- **From → To:** `` # ADR-0002: EnemyManager as a Time-Boxed Singleton Exception `` → (removed)
- **From → To:** `` | Engine | Unity 2022.3.62f1 LTS | `` → `` | Engine | Unity 2022.3.62f3 LTS | ``
- **From → To:** `` | Verification Required | Confirm at implementation time (Sprint 6) that `EnemyManager`'s Awake() duplicate-instance guard includes … `` → `` | Verification Required | At implementation time (Sprint 6) confirm `EnemyManager.Awake()`'s duplicate-instance guard includes `return` … ``
- … 28 more hunks — `git show 65c8228e -- docs/architecture/adr-0002-enemymanager-singleton-exception.md`
- **Why:** commit message: “fix doc”

## 2026-07-09 — fix conflic doc
- **Commit:** `3b64fca0` (+0 / −18 lines)
- **Sections touched:** “ADR-0002: EnemyManager as a Time-Boxed Singleton Exception”, “ADR-0002: EnemyManager is a permitted singleton exception”, “Engine Compatibility”, “ADR Dependencies”, “Problem Statement”, “Alternative 2: ScriptableObject-event-based indirection” …
- **From → To:** `` <<<<<<< HEAD `` → (removed)
- **From → To:** `` ======= `` → (removed)
- **From → To:** `` >>>>>>> origin/claude/enemy-spawn-manager-review-7aq2wa `` → (removed)
- … 15 more hunks — `git show 3b64fca0 -- docs/architecture/adr-0002-enemymanager-singleton-exception.md`
- **Why:** commit message: “fix conflic doc”

## 2026-07-09 — docs(enemy-spawn): reverse-sync GDD to prototype code + propagate
- **Commit:** `950ae319` (+7 / −4 lines)
- **Sections touched:** “Migration Plan”
- **From → To:** `` No existing code to migrate — `EnemyManager` is not yet implemented. If a future `` → `` `EnemyManager` is not yet implemented. The current prototype (2026-07-09) drives ``
- **Why:** commit message: “docs(enemy-spawn): reverse-sync GDD to prototype code + propagate”

## 2026-07-09 — docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002
- **Commit:** `3dc2394d` (+206 / −0 lines)
- **Changed (from → to):** document created (+206 lines)
- **Why:** commit message: “docs(enemy-spawn): sync spawn architecture across GDDs + add ADR-0002”

## 2026-07-09 — docs(architecture): ratify EnemyManager singleton, resolve enemy-spawn seed source
- **Commit:** `8a7b481a` (+340 / −0 lines)
- **Changed (from → to):** document created (+340 lines)
- **Why:** commit message: “docs(architecture): ratify EnemyManager singleton, resolve enemy-spawn seed source”
