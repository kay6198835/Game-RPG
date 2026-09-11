---
description: ScriptableObject data authoring standards — PlayerData, EntityData, EnemySO, AttackSO, ActivateSkill subclasses
globs: ["Assets/Script/**/*SO.cs", "Assets/Script/**/*Data.cs", "Assets/ScriptableObjects/**/*.asset"]
---

# ScriptableObject / Data Standards

## Single Source of Truth

> Corrected 2026-09-11. `WeaponMeleeStats` was renamed `MeleeWeaponStats` in the Sprint 8-10
> refactor, and `StatsCharacter` was superseded by `BaseStatsSO` in the Sprint 12 stat refactor
> (`b0512f4` deleted `StatsSO`; `1c0742e` added `BaseStatsSO`).

- All gameplay config belongs in SO assets — no magic numbers in MonoBehaviour code
- One SO type per concern: `PlayerData`, `EntityData`, `AttackSO`, `EnemySO`, `MeleeWeaponStats`, `ItemSO`, `DepotItem`
- Shared stats use the **`BaseStatsSO`** base (`EnemyStatSO : BaseStatsSO` for enemies) — do not duplicate stat fields across SOs
- ⚠️ `StatsCharacter.cs` is the legacy base and is no longer used by `Player` or `Entity`. Do not build on it

## Field Naming
- Fields must use `camelCase` matching their public property name: `attackDamege` stays as-is (preserve typos to avoid serialization breaks)
- New fields: `camelCase` with `[SerializeField]`, `[Header("Section")]` for inspector grouping
- Range validation: use `[Range(min, max)]` for all numeric fields to prevent invalid values in Editor

## Inheritance Rules
- Enemy SOs: `EnemySO` for spawn/drop data, `EntityData` for AI runtime — never merge them
- Skill SOs: subclass `ActivateSkill` and override the lifecycle methods — never modify the base class lifecycle
- Effect SOs: use `EffectSkillDuringTime` for duration effects, `EffectSkillOneTime` for instant

## Asset Naming (Unity convention)
- SO assets: `PascalCase` matching the class name + descriptor: `BatEntityData`, `SwordAttackSO`
- Prefab assets: `PascalCase`: `EnemyBat`, `WeaponSword`, `RoomBasic`
- Sprite assets: `PascalCase_Direction` or `PascalCase_00`: `Knight_Idle_00`, `Bat_Walk_01`

## Reborn / Reset Contract
- `PlayerData.Reborn()` is the canonical reset — it must restore `currentHealth = maxHealth` and any run-specific modifiers
- Never reset PlayerData by destroying and re-instantiating — call `Reborn()` only
- ⚠️ Status 2026-09-11: `Reborn()` still has **no caller** and `PlayerData.currentHealth` is still
  never written. Current health now lives in `VitalStatsComponent`. Closing Bug #6 means deciding
  which of the two owns run state, then wiring a `GameManager` to the reset

## Runtime vs Authored Serialization (added 2026-09-11)

The distinction that caused the project's only committed data-corruption incident:

| Field | Serialized? | Why |
|---|---|---|
| `Stat.modifiers` | **NO** | Holds *runtime* buffs. Serializing it writes live buffs into committed `.asset` files |
| `StatModifierGroup.authoredModifiers` | **YES** | Holds *designer* data. `SnS_Stat.asset` depends on it |

⚠️ **BUG-063 is open right now**: `Stat.modifiers` is re-serialized behind
`#if UNITY_EDITOR [SerializeField]` (`Stat.cs:63-65`), undoing the fix in `f5de65a`. Do not add
`[SerializeField]` to a field holding runtime state, under any preprocessor guard.
