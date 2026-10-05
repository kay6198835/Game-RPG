# Changelog — `adr-0001-statsystem-dual-data-structure.md`

Change log for [`docs/architecture/adr-0001-statsystem-dual-data-structure.md`](../adr-0001-statsystem-dual-data-structure.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+64 / −0 lines)
- **Sections touched:** “Status”, “Related Decisions”
- **From → To:** (nothing) → `` > **⚠️ Amended 2026-09-11 — the class this ADR is written about no longer exists.** ``
- **From → To:** (nothing) → `` --- ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-21 — docs(stats): record the ratified StatModifierGroup shape and the C1 fix
- **Commit:** `610551f3` (+14 / −1 lines)
- **Sections touched:** “Constraints”
- **From → To:** `` its `Source` is runtime-only. `Stat.modifiers` is therefore `[NonSerialized]` so runtime buffs are `` → `` its `Source` is runtime-only. `Stat.modifiers` is therefore **not serialized**, so runtime buffs are ``
- **From → To:** (nothing) → `` > **Clarification 2026-08-21.** This constraint was violated in shipped code for some time — ``
- **Why:** commit message: “docs(stats): record the ratified StatModifierGroup shape and the C1 fix”

## 2026-08-14 — feat(stats): add modifiers in bulk by source, mirroring bulk removal
- **Commit:** `1941fb23` (+4 / −2 lines)
- **Sections touched:** “Constraints”, “Key Interfaces”
- **From → To:** `` - `StatModifier` is runtime-only and not Unity-serializable (see `Stat.modifiers`). `` → `` - `StatModifier` is Unity-serializable for its authored part (`targetStat` / `type` / `value`), but ``
- **From → To:** `` - `void StatsSO.AddModifier / RemoveModifiersFromSource / AddPrimaryPoint` — write through `List`, then resync. `` → `` - `void StatsSO.AddModifier / AddModifiersFromSource / RemoveModifiersFromSource / AddPrimaryPoint` — write through `List`, then resync. ``
- **Why:** commit message: “feat(stats): add modifiers in bulk by source, mirroring bulk removal”

## 2026-07-09 — fix doc
- **Commit:** `65c8228e` (+1 / −1 lines)
- **Sections touched:** “Engine Compatibility”
- **From → To:** `` | Engine | Unity 2022.3.62f1 LTS | `` → `` | Engine | Unity 2022.3.62f3 LTS | ``
- **Why:** commit message: “fix doc”

## 2026-07-07 — docs(statsystem): author stat-system GDD; move formula reference into ToolExcel
- **Commit:** `76803221` (+1 / −1 lines)
- **Sections touched:** “GDD Requirements Addressed”
- **From → To:** `` | stat-system.md (not yet authored) | Runtime stat lookup + author-time editing | Records the storage decision; will be linked when the GDD … `` → `` | design/gdd/stat-system.md | Runtime stat lookup + author-time editing | Records the storage decision behind `StatsSO`'s List + Dictionary … ``
- **Why:** commit message: “docs(statsystem): author stat-system GDD; move formula reference into ToolExcel”

## 2026-07-06 — docs(adr): ADR-0001 StatSystem dual data structure (List + Dictionary)
- **Commit:** `c16ee770` (+118 / −0 lines)
- **Changed (from → to):** document created (+118 lines)
- **Why:** commit message: “docs(adr): ADR-0001 StatSystem dual data structure (List + Dictionary)”
