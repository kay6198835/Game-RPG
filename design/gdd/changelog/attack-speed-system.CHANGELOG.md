# Changelog — `attack-speed-system.md`

Change log for [`design/gdd/attack-speed-system.md`](../attack-speed-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+10 / −3 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** and `attack-speed-system.md` took `float` / `BaseStatsSO` / path corrections only.
- **Sections touched:** “Attack Speed System Design”, “Implementation Map”
- **From → To:** `` source: Assets/Script/Weapons/MeleeWeapon/, Assets/Script/StatSystem/ `` → `` source: Assets/Script/Weapons/MeleeWeapon/, Assets/Script/System/StatSystem/ ``
- **From → To:** (nothing) → `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** The `AttackSpeed` formula and its ``

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+43 / −22 lines)
- **Sections touched:** “Edge Cases”, “Dependencies”, “Tuning Knobs”, “Implementation Map”
- **From → To:** (nothing) → `` revised: 2026-08-20 (documentation audit — Implementation Map retargeted from the deleted ``
- **From → To:** `` | Stat changes mid-combo (card picked, buff expires) | Recomputed on the next `SetAnimation()` call. The currently playing clip is not … `` → `` | Stat changes mid-combo (card picked, buff expires) | Recomputed on the next `Weapon.OnAttackEnter()` call (was `SetAnimation()`). The … ``
- **From → To:** `` | Entity (enemy) attacks | Same model applies via `EntityWeaponMelee`; enemies read `AttackSpeed` from their own `StatsSO`. Enemy wiring is … `` → `` | Entity (enemy) attacks | Same model applies via `EntityWeaponMelee`; enemies read `AttackSpeed` from their own `StatsSO`. Enemy wiring is … ``
- … 11 more hunks — `git show e1cd01f4 -- design/gdd/attack-speed-system.md`
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-08-05 — design: add attack speed system GDD
- **Commit:** `b3a52e0c` (+247 / −0 lines)
- **Changed (from → to):** document created (+247 lines)
- **Why:** commit message: “design: add attack speed system GDD”
