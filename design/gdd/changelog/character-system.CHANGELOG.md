# Changelog — `character-system.md`

Change log for [`design/gdd/character-system.md`](../character-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+25 / −1 lines)
- **Sections touched:** “Character System Design”, “Dependencies”
- **From → To:** (nothing) → `` > **⚠️ Partially superseded 2026-09-11 — the health/stat model below predates the Sprint 12 refactor.** ``
- **From → To:** `` | **Skill/Ability** (`Assets/Script/Skill_Ability/`) | `ActivateSkill` SO provides ability lifecycle; `AbilityHolder` drives it | Character … `` → `` | **Skill/Ability** (`Assets/Script/System/Skill_Ability/`) | `ActivateSkill` SO provides ability lifecycle; `AbilityHolder` drives it | … ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-22 — chore: park the Skill Enhance ability framework under prototypes/
- **Commit:** `eda31a1e` (+1 / −1 lines)
- **Sections touched:** “Dependencies”
- **From → To:** `` | **Event Manager** (`EventManager.cs`) | Corrected 2026-08-20 — `ON_PLAYER_DEATH` and `ON_ENEMY_DEATH` **now exist** (the enum has 18 … `` → `` | **Event Manager** (`EventManager.cs`) | Corrected 2026-08-20 — `ON_PLAYER_DEATH` and `ON_ENEMY_DEATH` **now exist** (the enum has 20 … ``
- **Why:** commit message: “chore: park the Skill Enhance ability framework under prototypes/”

## 2026-08-21 — docs: correct my own EventID count - the enum has 18 values, not 19
- **Commit:** `01459440` (+1 / −1 lines)
- **Sections touched:** “Dependencies”
- **From → To:** `` | **Event Manager** (`EventManager.cs`) | Corrected 2026-08-20 — `ON_PLAYER_DEATH` and `ON_ENEMY_DEATH` **now exist** (the enum has 19 … `` → `` | **Event Manager** (`EventManager.cs`) | Corrected 2026-08-20 — `ON_PLAYER_DEATH` and `ON_ENEMY_DEATH` **now exist** (the enum has 18 … ``
- **Why:** commit message: “docs: correct my own EventID count - the enum has 18 values, not 19”

## 2026-08-21 — docs(gdd): sync the descriptive sections of five GDDs with source
- **Commit:** `e1cd01f4` (+32 / −22 lines)
- **Sections touched:** “Character System Design”, “Player States”, “Enemy (Entity) States”, “Damage Rule”, “Edge Cases”, “Dependencies” …
- **From → To:** `` **Status**: In Design `` → `` **Status**: In Design (implementation-status claims corrected 2026-08-20) ``
- **From → To:** `` **Attack gate:** `PlayerBasicState` checks `IsAttack && WeaponHolder.Weapon != null && Weapon.CheckCanAttack(player)` each frame. The … `` → `` **Attack gate:** `PlayerBasicState` checks `inputHandler.IsAttack && weaponHolder.Weapon != null && weaponHolder.CanAttack()` each frame … ``
- **From → To:** `` | **Death** | health ≤ 0 **[BUG — not triggered; see Edge Cases]** | despawn | frozen | `` → `` | **Death** | health ≤ 0 **[BUG — unreachable; see Edge Cases]** | despawn | frozen | ``
- … 13 more hunks — `git show e1cd01f4 -- design/gdd/character-system.md`
- **Why:** commit message: “docs(gdd): sync the descriptive sections of five GDDs with source”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+206 / −0 lines)
- **Changed (from → to):** document created (+206 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”
