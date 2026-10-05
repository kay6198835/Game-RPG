# Changelog — `character-architecture-diagrams.md`

Change log for [`docs/diagrams/character-architecture-diagrams.md`](../character-architecture-diagrams.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Fix Mermaid parse error in §10
- **Commit:** not yet committed
- **Changed:** §10 "Flow — enemy casts an ability", one sequence message
- **From → To:** `` AH-->>BS: true (first ready slot; animator override applied) `` → `` AH-->>BS: true (first ready slot, animator override applied) ``
- **Why:** in a Mermaid `sequenceDiagram` a `;` ends the statement, so the message was cut in two and the diagram failed to render ("Parse error on line 8 … got 'NEWLINE'"). A scan of every `sequenceDiagram` block in the repo's `.md` files found no other `;` in a message
- **Bugs:** none

## 2026-09-25 — docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData
- **Commit:** `ece32573` (+45 / −0 lines)
- **Sections touched:** “10. Flow — enemy casts an ability”
- **From → To:** (nothing) → `` ## 11. Amendment 3 — shared character data and "no weapon, no attack" ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 3 — no weapon no attack, CharacterData”

## 2026-09-25 — docs(character): ADR-0005 Amendment 2 — shared component bases
- **Commit:** `be32d404` (+64 / −0 lines)
- **Sections touched:** “8. Flow — DI and pooled enemy spawn”
- **From → To:** (nothing) → `` ## 9. Component bases (ADR-0005 Amendment 2) ``
- **Why:** commit message: “docs(character): ADR-0005 Amendment 2 — shared component bases”

## 2026-09-24 — refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)
- **Commit:** `5007d3f6` (+52 / −43 lines)
- **Sections touched:** “Character Architecture — Diagrams”, “2. Class diagram — after (ADR-0005 + Amendment 1)”, “Lookup rule”, “3. Correspondence with the Map precedent”, “4. Flow — stat effect on a target (player or enemy, same …”, “5. Flow — damage (signature unchanged)” …
- **From → To:** `` after = branch `demo-architeture-1`. `` → `` after = branch `demo-architeture-1`, including ADR-0005 Amendment 1 (2026-09-24). ``
- **From → To:** `` ## 2. Class diagram — after `` → `` ## 2. Class diagram — after (ADR-0005 + Amendment 1) ``
- **From → To:** `` class ICharacter { <<interface>> +Transform +Core +Stats +Vital +DamageReceiver } `` → `` class ICharacter { <<interface>> +Transform } ``
- … 24 more hunks — `git show 5007d3f6 -- docs/diagrams/character-architecture-diagrams.md`
- **Why:** commit message: “refactor(character): ICharacter is identity only (ADR-0005 Amendment 1)”

## 2026-09-24 — feat(character): unified character contract (ADR-0005 steps 4-10) + docs
- **Commit:** `8c3c350a` (+211 / −0 lines)
- **Changed (from → to):** document created (+211 lines)
- **Why:** commit message: “feat(character): unified character contract (ADR-0005 steps 4-10) + docs”
