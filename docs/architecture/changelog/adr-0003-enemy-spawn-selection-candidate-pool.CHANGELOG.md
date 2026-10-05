# Changelog — `adr-0003-enemy-spawn-selection-candidate-pool.md`

Change log for [`docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md`](../adr-0003-enemy-spawn-selection-candidate-pool.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-08-21 — docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic
- **Commit:** `66b7604c` (+91 / −1 lines)
- **Sections touched:** “Status”, “Related Decisions”
- **From → To:** `` Accepted (2026-07-23 — see Amendment section below; Data Model amended to match shipped code, algorithm/selection-flow unchanged from … `` → `` Accepted ``
- **From → To:** (nothing) → `` --- ``
- **Why:** commit message: “docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic”

## 2026-07-23 — fix(sprint-06): close S6-01/02/03/04 bugs, resolve S6-09 data-model decision
- **Commit:** `d3b29d9d` (+59 / −21 lines)
- **Sections touched:** “Status”, “Data Model”, “Formulas”, “Key Interfaces”, “Risks”, “Migration Plan” …
- **From → To:** `` Proposed `` → `` Accepted (2026-07-23 — see Amendment section below; Data Model amended to match shipped code, algorithm/selection-flow unchanged from … ``
- **From → To:** `` `EnemyModal` gains a `RarityTier` enum field. `weight` (Cost) and `rarityTier` are **fully independent by design** — there is no … `` → `` **[AMENDED 2026-07-23 — see Amendment section below]** The SO-extending-`EntityModel` shape originally specified here was not built. … ``
- **From → To:** (nothing) → `` `EnemyModal` carries a `RarityTier` enum field. `weight` (Cost) and `rarityTier` are **fully independent by design** — there is no … ``
- … 12 more hunks — `git show d3b29d9d -- docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md`
- **Why:** commit message: “fix(sprint-06): close S6-01/02/03/04 bugs, resolve S6-09 data-model decision”

## 2026-07-13 — docs(adr): ADR-0003 enemy spawn selection — Candidate Pool + RarityTier (Option C)
- **Commit:** `0c1a7b49` (+238 / −0 lines)
- **Changed (from → to):** document created (+238 lines)
- **Why:** commit message: “docs(adr): ADR-0003 enemy spawn selection — Candidate Pool + RarityTier (Option C)”
