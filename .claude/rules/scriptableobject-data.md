---
description: ScriptableObject data authoring standards — PlayerData, EntityData, EnemySO, AttackSO, ActivateSkill subclasses
globs: ["Assets/Script/**/*SO.cs", "Assets/Script/**/*Data.cs", "Assets/ScriptableObjects/**/*.asset"]
---

# ScriptableObject / Data Standards

## Single Source of Truth
- All gameplay config belongs in SO assets — no magic numbers in MonoBehaviour code
- One SO type per concern: `PlayerData`, `EntityData`, `AttackSO`, `EnemySO`, `WeaponMeleeStats`
- Shared stats use `StatsCharacter` SO base — do not duplicate fields across SOs

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
