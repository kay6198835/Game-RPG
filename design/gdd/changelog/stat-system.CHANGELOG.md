# Changelog — `stat-system.md`

Change log for [`design/gdd/stat-system.md`](../stat-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+32 / −14 lines)
- **Sections touched:** “Stat System Design”, “Overview”, “Detailed Rules”, “Formulas”, “Edge Cases”, “Dependencies” …
- **From → To:** `` source: Assets/Script/StatSystem/, ToolExcel/stat_system_formula_reference.xlsx `` → `` source: Assets/Script/System/StatSystem/, ToolExcel/stat_system_formula_reference.xlsx ``
- **From → To:** (nothing) → `` > **⚠️ Renamed 2026-09-11 — `StatsSO` is now `BaseStatsSO`.** The class was deleted in `b0512f4` ``
- **From → To:** `` in ScriptableObjects and read at runtime through `StatsSO`. `` → `` in ScriptableObjects and read at runtime through `BaseStatsSO`. ``
- … 9 more hunks — `git show bbfb3028 -- design/gdd/stat-system.md`
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-26 — coding
- **Commit:** `6afefdec` (+2 / −2 lines)
- **Sections touched:** “Detailed Rules”, “Dependencies”
- **From → To:** `` - *Flat / resource* (MaxHP, MaxMana, PhysicalDamage, MagicDamage, Defense, MagicDefense, `` → `` - *Flat / resource* (HP, Mana, PhysicalDamage, MagicDamage, Defense, MagicDefense, ``
- **From → To:** `` | Damage & Health | Consumes MaxHP / Defense / damage stats; damage application should apply Defense (currently `finalDamage = rawDamage` — … `` → `` | Damage & Health | Consumes HP / Defense / damage stats; damage application should apply Defense (currently `finalDamage = rawDamage` — … ``
- **Why:** commit message: “coding”

## 2026-08-21 — docs(stats): record the ratified StatModifierGroup shape and the C1 fix
- **Commit:** `610551f3` (+28 / −20 lines)
- **Sections touched:** “Detailed Rules”
- **From → To:** `` revised: 2026-08-20 (documentation audit — recorded the StatModifierGroupSO -> StatModifierGroup `` → `` revised: 2026-08-21 (owner ratified the plain-class StatModifierGroup shape and the field renames; ``
- **From → To:** `` equipment, one buff, one upgrade card) is authored as a `StatModifierGroupSO` asset and attached `` → `` equipment, one buff, one upgrade card) is authored as a `StatModifierGroup` and attached ``
- **From → To:** `` > ⚠️ **Implementation defect (audit 2026-08-20, re-verified 2026-08-21 — TD-038).** `` → `` > ✅ **Owner decision 2026-08-21 — the shipped shape is ratified.** This GDD originally specified a ``
- … 2 more hunks — `git show 610551f3 -- design/gdd/stat-system.md`
- **Why:** commit message: “docs(stats): record the ratified StatModifierGroup shape and the C1 fix”

## 2026-08-21 — docs: correct three audit findings overtaken by sprint-10
- **Commit:** `f5042d67` (+20 / −15 lines)
- **Sections touched:** “Detailed Rules”, “Acceptance Criteria”
- **From → To:** `` > ⚠️ **Two implementation defects break this section today (audit 2026-08-20, TD-038).** `` → `` > ⚠️ **Implementation defect (audit 2026-08-20, re-verified 2026-08-21 — TD-038).** ``
- **From → To:** `` - [ ] Every derived stat recalculates when a primary stat or the level changes. **Currently `` → `` - [x] Every derived stat recalculates when a primary stat or the level changes — fixed on ``
- **Why:** commit message: “docs: correct three audit findings overtaken by sprint-10”

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+19 / −1 lines)
- **Sections touched:** “Detailed Rules”, “Acceptance Criteria”
- **From → To:** (nothing) → `` revised: 2026-08-20 (documentation audit — recorded the StatModifierGroupSO -> StatModifierGroup ``
- **From → To:** (nothing) → `` > ⚠️ **Two implementation defects break this section today (audit 2026-08-20, TD-038).** ``
- **From → To:** `` - [ ] Every derived stat recalculates when a primary stat or the level changes. `` → `` - [ ] Every derived stat recalculates when a primary stat or the level changes. **Currently ``
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-08-14 — feat(stats): add modifiers in bulk by source, mirroring bulk removal
- **Commit:** `1941fb23` (+11 / −2 lines)
- **Sections touched:** “Detailed Rules”
- **From → To:** `` - **Modifiers**: buffs, equipment, and effects attach as `StatModifier`s on top of the `` → `` - **Modifiers** `[IMPLEMENTED]`: buffs, equipment, and effects attach as `StatModifier`s on top of ``
- **Why:** commit message: “feat(stats): add modifiers in bulk by source, mirroring bulk removal”

## 2026-07-07 — docs(statsystem): author stat-system GDD; move formula reference into ToolExcel
- **Commit:** `76803221` (+136 / −0 lines)
- **Changed (from → to):** document created (+136 lines)
- **Why:** commit message: “docs(statsystem): author stat-system GDD; move formula reference into ToolExcel”
