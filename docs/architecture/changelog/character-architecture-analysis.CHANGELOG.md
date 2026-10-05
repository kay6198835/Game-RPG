# Changelog — `character-architecture-analysis.md`

Change log for [`docs/architecture/character-architecture-analysis.md`](../character-architecture-analysis.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Lookup-debt table: `bullet.cs:54` struck (deleted 2026-09-28)
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-24 — refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)
- **Commit:** `5007d3f6` (+11 / −8 lines)
- **Sections touched:** “Character Architecture — Phase 1 Analysis”, “2. Mapping the principles onto Player / Entity”, “4. Call sites depending on a concrete type where an …”
- **From → To:** (nothing) → `` > ``
- **From → To:** `` | `IGrid` (P1) | `ICharacter` — Stats, Vital, DamageReceiver, Core, Transform | `ICharacter` empty, zero implementers | `` → `` | `IGrid` (P1) | `ICharacter` — identity of the character root only (Amendment 1) | `ICharacter` empty, zero implementers | ``
- **From → To:** `` | `Weapons/Weapon.cs:87,100` | `weaponHolder.Core.Player.Data.Stats` — bypasses StatHandler entirely | `ICharacter.Stats` + … `` → `` | `Weapons/Weapon.cs:87,100` | `weaponHolder.Core.Player.Data.Stats` — bypasses StatHandler entirely | `GetComponentInParent<ICharacter>()` … ``
- … 2 more hunks — `git show 5007d3f6 -- docs/architecture/character-architecture-analysis.md`
- **Why:** commit message: “refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)”

## 2026-09-24 — feat(character): unified character contract (ADR-0005 steps 4-10) + docs
- **Commit:** `8c3c350a` (+120 / −0 lines)
- **Changed (from → to):** document created (+120 lines)
- **Why:** commit message: “feat(character): unified character contract (ADR-0005 steps 4-10) + docs”
