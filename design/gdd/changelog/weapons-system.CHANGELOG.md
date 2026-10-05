# Changelog — `weapons-system.md`

Change log for [`design/gdd/weapons-system.md`](../weapons-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** Ranged section still describes `bullet.cs` + `BulletDataSO` (`:125`, `:187-188`, `:243`, `:258`, `:292-293`) — both deleted in `5b035b73` / `b7a0af5e` (2026-09-28); the ranged path is now `ProjectileBody` + `ProjectileConfig` + `RangeWeapon.OnHit()`
  - **From → To:** `attackDamege` (`:113`, `:175`, `:197`, `:209`, `:235`, `:281`) — the field is `attackDamage` since `ddcc0a5c` (2026-09-01); see BUG-095
  - **From → To:** `AbilityWeapon` / `SkillWeapon` (`:18`, `:254-255`) — deleted from `WeaponStats` in `b7a0af5e`; E key no longer bound (abilities on keys 1-4)
  - **From → To:** Melee damage formula `finalDamage = attackDamege` (raw) → `PhysicalDamage + attackDamage (+ CritDamage)` from `WeaponHolderBase.CalculateCurrentDamage()`; ranged cooldown edge case 'enforced' → 'not enforced (BUG-093)'; acceptance criteria rewritten for `ProjectileBody`, enemy weapons and weapon-independent abilities
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+17 / −0 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** five map bugs re-confirmed open, and the `StatusAnimation` handoff unchanged. `weapons-system.md`
- **Sections touched:** “Weapons System Design”
- **From → To:** (nothing) → `` > **Re-verified 2026-09-11 against HEAD `6d6a8e4`.** The weapon lifecycle, combo-stage model and ``

## 2026-09-01 — coding
- **Commit:** `ddcc0a5c` (+1 / −1 lines)
- **Sections touched:** “Attack Stages — Shared By Both Weapon Types [IMPLEMENTED]”
- **From → To:** `` 5. Animator fires `AnimtionFinishTrigger` → `Weapon.OnDeactivate()`, then either chains (if input is held/buffered and `CanChain()`) or … `` → `` 5. Animator fires `AnimationFinishTrigger` → `Weapon.OnDeactivate()`, then either chains (if input is held/buffered and `CanChain()`) or … ``
- **Why:** commit message: “coding”

## 2026-08-13 — refactor(weapons): wrap stage index so it is always in range
- **Commit:** `6aba70c0` (+8 / −5 lines)
- **Sections touched:** “Attack Stages — Shared By Both Weapon Types [IMPLEMENTED]”, “Stage selection (shared)”, “Chain permission”
- **From → To:** `` 1. Each attack input plays `AttackStages[CurrentStageIndex]`, then increments the index `` → `` 1. Each attack input plays `AttackStages[CurrentStageIndex]`, then advances the index modulo `StageCount` ``
- **From → To:** (nothing) → `` CurrentStageIndex = (CurrentStageIndex + 1) % StageCount ``
- **From → To:** `` Weapon.CanChain() = CanAttack() && CurrentStageIndex < StageCount `` → `` Weapon.CanChain() = CanAttack() && CurrentStageIndex != 0 ``
- **Why:** commit message: “refactor(weapons): wrap stage index so it is always in range”

## 2026-08-13 — refactor(weapons): unify melee and ranged behind one attack state
- **Commit:** `797a5628` (+148 / −92 lines)
- **Sections touched:** “Weapons System Design”, “Attack Stages — Shared By Both Weapon Types [IMPLEMENTED]”, “Weapon Skill Slots”, “Block Mechanic”, “Melee hitbox center”, “Stage selection (shared)” …
- **From → To:** `` date: 2026-05-19 `` → `` date: 2026-08-13 ``
- **From → To:** `` **Status**: In Design `` → `` **Status**: Implemented — melee and ranged share one attack state ``
- **From → To:** `` ### Melee Combat — 3-Hit Combo `` → `` ### Attack Stages — Shared By Both Weapon Types [IMPLEMENTED] ``
- … 32 more hunks — `git show 797a5628 -- design/gdd/weapons-system.md`
- **Why:** commit message: “refactor(weapons): unify melee and ranged behind one attack state”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+231 / −0 lines)
- **Changed (from → to):** document created (+231 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”
