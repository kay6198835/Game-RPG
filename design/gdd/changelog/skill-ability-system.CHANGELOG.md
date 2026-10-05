# Changelog — `skill-ability-system.md`

Change log for [`design/gdd/skill-ability-system.md`](../skill-ability-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Header table and Dependencies (`:22`, `:245`) list `WeaponStats.AbilityWeapon` / `.SkillWeapon`, `AttackSO.ability` and `Weapon` as v1 users — all deleted 2026-09-28; only `EntityWeapon.currentAbilitySO` remains. Banner update added: RMB/E weapon inputs no longer exist; v2 on keys 1-4
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-21 — update doc for ability system, update 1-7 dir animation clip, done animation for ability consecrae
- **Commit:** `ac347d89` (+8 / −3 lines)
- **Why:** Abilities v2 re-synchronisation + Paladin direction renumber (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `design/gdd/skill-ability-system.md` | v1/v2 comparison table: lifecycle corrected to `AbilityState: Start → Cast → Do → Exit`; live v2 assets updated to the Paladin set. Cross-system table: the "`PlayerInputHandle` provides a `SkillState` enum" row was wrong and is corrected.
- **Sections touched:** “Skill & Ability System Design”, “Dependencies”
- **From → To:** `` > | Lifecycle | `Enter → Activate → Cast → Do → Exit` | `SkillState`: `None → Start → Cast → Do → Exit` | `` → `` > | Lifecycle | `Enter → Activate → Cast → Do → Exit` | `AbilityState`: `Start → Cast → Do → Exit` | ``
- **From → To:** `` > | Live SO assets | `SO/Skill/{Dash,Slash,Block,Dual} Ability.asset` | `SO/Skill/ShootSpirit/*.asset`, `SO/Skill/Conditions/*.asset` | `` → `` > | Live SO assets | `SO/Skill/{Dash,Slash,Block,Dual} Ability.asset` | `SO/Skill/Paladin/Ability/**`, `SO/Skill/ShootSpirit/*.asset`, … ``

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+26 / −1 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** Until this is decided, `design/gdd/skill-ability-system.md` cannot be made authoritative and
- **Sections touched:** “Skill & Ability System Design”
- **From → To:** `` source: Assets/Script/Skill_Ability/ `` → `` source: Assets/Script/System/Skill_Ability/ ``
- **From → To:** (nothing) → `` > **⚠️ SCOPE CHANGED 2026-09-11 — this GDD now describes only ONE of two live frameworks.** ``

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+3 / −3 lines)
- **Sections touched:** “Block Skill (RMB hold) — **[GAP — BlockAbility is a stub]**”, “Dependencies”, “Active Abilities”
- **From → To:** `` - **On hit during block**: Incoming damage reduced by `WeaponMeleeStats.blockDamage` `` → `` - **On hit during block**: Incoming damage reduced by `MeleeWeaponStats.blockDamage` (class renamed from `WeaponMeleeStats` during the … ``
- **From → To:** `` | **Weapons** (`WeaponMeleeStats`) | Carries `abilityWeapon` and `skillWeapon` SO refs; wires them to `AbilityHolder` on equip via … `` → `` | **Weapons** (`WeaponStats`) | Carries `AbilityWeapon` and `SkillWeapon` SO refs — corrected 2026-08-20: these moved up from … ``
- **From → To:** `` | Block damage reduction | `blockDamage` | `WeaponMeleeStats` SO | Flat damage absorbed while blocking | `` → `` | Block damage reduction | `blockDamage` | `MeleeWeaponStats` SO | Flat damage absorbed while blocking. ⚠️ **Cross-GDD conflict (audit … ``
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+288 / −0 lines)
- **Changed (from → to):** document created (+288 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”
