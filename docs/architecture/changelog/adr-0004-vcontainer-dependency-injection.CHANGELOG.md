# Changelog — `adr-0004-vcontainer-dependency-injection.md`

Change log for [`docs/architecture/adr-0004-vcontainer-dependency-injection.md`](../adr-0004-vcontainer-dependency-injection.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Registration table (`:69-74`) predates 2026-09-28: `Player`, `StatHandler`, `AbilityHolder` are no longer registered; `IPlayerStatService` is a factory over `PlayerManager.StatService`; `RoomGridController` and `LevelManager` were added (`0bc36406`). TD-023 (`:48`, `:103`) is resolved. → Decision left intact; status banner + **Amendment 1 (2026-10-05)** appended with the new registration table, runtime player spawn, weapon injection on equip and the two concrete-type injections
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+167 / −0 lines)
- **Changed (from → to):** document created (+167 lines)
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”
