# Changelog — `adr-0005-unified-character-contract.md`

Change log for [`docs/architecture/adr-0005-unified-character-contract.md`](../adr-0005-unified-character-contract.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-09 — ⚠️ Out of date against code (found by doc-sync, not yet fixed)
- **Commit:** branch `pm/doc-sync-2026-10-09` (base `sprint-17` `cdf68555`)
- **Stale sentence:** Context, `:33`: "`StatModifierGroup.Apply/Remmove` becomes the one way to attach a modifier bundle to a character".
- **Code at `sprint-17`:** `7f632021` (2026-10-07, "coding start room") deleted `StatModifierGroup.Apply()` and `.Remmove()`. `VitalStatsBase` and `Weapon` now call `AddModifiersFromSource(this, group.Modifiers)` / `RemoveModifiersFromSource(this)` directly.
- **Why not fixed:** whether this changes the ADR's intent is an owner decision (an Amendment), not a doc-sync edit. A note under the Status banner points here.
- **Related:** BUG-100 (one modifier source shared by permanent and timed buffs) sits on the same code path.

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Residuals list: `attackDamege` migration gap → annotated 'filed as BUG-095, still open'; `bullet.cs` lookup debt → annotated 'deleted 2026-09-28, replaced by `ProjectileBody` using `TryGetComponent`'
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-25 — docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData
- **Commit:** `ece32573` (+42 / −2 lines)
- **Sections touched:** “Amendment 2 — 2026-09-25 (owner request): shared bases for …”, “Amendment 3 — 2026-09-25 (owner request): no weapon, no …”, “Residuals (out of scope)”
- **From → To:** `` Otherwise it keeps `EntityAttack`. Content requirement: enemy `AttackSO`s need animator overrides built on `` → `` (Superseded by Amendment 3: there is no `EntityAttack` fallback.) Content requirement: enemy `AttackSO`s need animator overrides built on ``
- **From → To:** (nothing) → `` ## Amendment 3 — 2026-09-25 (owner request): no weapon, no attack; shared character data ``
- **From → To:** `` - `EntityEffectStats` (Abilities v1) still writes `Core.Entity.Data.StatsSO`, the shared asset. `` → `` - `EntityEffectStats` (Abilities v1) still writes `Core.Entity.Data.Stats`, the shared asset. ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData”

## 2026-09-25 — docs(character): ADR-0005 Amendment 2 — shared component bases
- **Commit:** `be32d404` (+30 / −1 lines)
- **Sections touched:** “Date”, “Amendment 2 — 2026-09-25 (owner request): shared bases for …”
- **From → To:** `` 2026-09-23 · **Amendment 1: 2026-09-24** (owner review — see the end of this document) `` → `` 2026-09-23 · **Amendment 1: 2026-09-24** · **Amendment 2: 2026-09-25** (see the end of this document) ``
- **From → To:** (nothing) → `` ## Amendment 2 — 2026-09-25 (owner request): shared bases for the remaining components ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 2 — shared component bases”

## 2026-09-24 — refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)
- **Commit:** `5007d3f6` (+70 / −32 lines)
- **Sections touched:** “Date”, “1. One character identity, one set of component interfaces”, “2. Interfaces and who implements them”, “3. Shared bases (P2 + P3)”, “4. Core hubs stay separate”, “5. Lookup paths” …
- **From → To:** `` 2026-09-23 `` → `` 2026-09-23 · **Amendment 1: 2026-09-24** (owner review — see the end of this document) ``
- **From → To:** `` ### 1. One character contract `` → `` ### 1. One character identity, one set of component interfaces ``
- **From → To:** `` public interface ICharacter // coordination — the IGrid role `` → `` public interface ICharacter // identity of the character root — the IGrid role ``
- … 9 more hunks — `git show 5007d3f6 -- docs/architecture/adr-0005-unified-character-contract.md`
- **Why:** commit message: “refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)”

## 2026-09-24 — feat(character): unified character contract (ADR-0005 steps 4-10) + docs
- **Commit:** `8c3c350a` (+178 / −0 lines)
- **Changed (from → to):** document created (+178 lines)
- **Why:** commit message: “feat(character): unified character contract (ADR-0005 steps 4-10) + docs”
