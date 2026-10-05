# Changelog — `character-migration-plan.md`

Change log for [`docs/architecture/character-migration-plan.md`](../character-migration-plan.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Follow-ups: `bullet.cs` struck (deleted 2026-09-28); `FormerlySerializedAs` follow-up annotated 'still not done — BUG-095'
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-25 — docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData
- **Commit:** `ece32573` (+5 / −0 lines)
- **Sections touched:** “Steps”, “Follow-ups not in this migration”
- **From → To:** (nothing) → `` | 17 | **Amendment 3.** `CharacterData` base SO; `PlayerData`/`EntityData` rebased; `CoreBase.Data`; stat/ability bases read it by default … ``
- **From → To:** (nothing) → `` - Add `FormerlySerializedAs("attackDamege")` to `AttackSO.attackDamage` (player stage damage reads 0). ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData”

## 2026-09-25 — docs(character): ADR-0005 Amendment 2 — shared component bases
- **Commit:** `be32d404` (+5 / −0 lines)
- **Sections touched:** “Steps”
- **From → To:** (nothing) → `` | 12 | `CharacterInputBase<TCore>` + `ICharacterInput`; `PlayerInputHandler`, `EntityInput` rebased | … ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 2 — shared component bases”

## 2026-09-24 — refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)
- **Commit:** `5007d3f6` (+4 / −2 lines)
- **Sections touched:** “Steps”, “Follow-ups not in this migration”
- **From → To:** `` | 2 | `ICore.TryGetCapability<T>` + `ICore.Character`, implemented in `CoreBase` | `ICore.cs`, `CoreBase.cs` | none | compile; S | `` → `` | 2 | `ICore.TryGetCapability<T>` + `ICore.Character` (the latter removed in step 11), implemented in `CoreBase` | `ICore.cs`, … ``
- **From → To:** (nothing) → `` | 11 | **Amendment 1** (owner review 2026-09-24): `ICharacter` reduced to `Transform`; `ICharacter<TCore>`, `ICore.Character`, … ``
- **From → To:** `` - Move `bullet.cs` / `Projectile.cs` / `Spell.cs` to `CharacterLookup`. `` → `` - Move `bullet.cs` / `Projectile.cs` / `Spell.cs` from `GetComponentInChildren` to `TryGetComponent` on the hurtbox. ``
- **Why:** commit message: “refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)”

## 2026-09-24 — feat(character): unified character contract (ADR-0005 steps 4-10) + docs
- **Commit:** `8c3c350a` (+50 / −0 lines)
- **Changed (from → to):** document created (+50 lines)
- **Why:** commit message: “feat(character): unified character contract (ADR-0005 steps 4-10) + docs”
