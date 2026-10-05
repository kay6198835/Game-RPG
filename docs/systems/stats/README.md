# Stats

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` (no code change since 2026-09-24) · **ADR:** ADR-0001
> History: [CHANGELOG.md](CHANGELOG.md) · GDD: `design/gdd/stat-system.md` · Numbers: `ToolExcel/stat_system_formula_reference.xlsx`

## Purpose

RPG stat model: primary stats feed derived stats through formulas; modifiers from equipment, buffs
and abilities stack on top. Max values live here; current values live in the vitals component
(see [character](../character/README.md)).

## Code — `Assets/Script/System/StatSystem/` (9 files)

| File | Contents |
|------|----------|
| `StatType.cs` | Primary STR/DEX/INT/VIT/LUK (0-4); derived HP, Mana, PhysicalDamage, MagicDamage, Defense, AttackSpeed, CritChance, CritDamage, MoveSpeed, HPRegen, ManaRegen, Evasion (100-111) |
| `Stat.cs` | BaseValue / LevelUpValue / EquipmentValue / EquipmentByPrimaryValue / AdjustedValue / FinalValue + runtime modifier list |
| `StatModifier.cs` | Authored `targetStat` / `type` / `value` + runtime `Source` (`[NonSerialized]`, stamped by `WithSource()`) |
| `StatModifierGroup.cs` | `[Serializable]` bundle (`authoredModifiers`), embedded in `WeaponStats` and effects |
| `DerivedStatFormula.cs` | `baseConstant + level × perLevel + Σ(primary × coefficient)` |
| `BaseStatsSO.cs` | The stat profile SO: `Level`, `Get/GetStat/GetStatValue`, `AddModifiersFromSource` / `RemoveModifiersFromSource`, `AddPrimaryPoint`, `OnStatChanged`, `SeedAllStats`, `FullStatsValue`, `FullStatView`; declares `StatsViewDTO` |
| `EnemyStatSO.cs` | `: BaseStatsSO` |
| `StatPointAllocator.cs` | Allocation session (accept / revert / restore) → `ON_RESET_STATS_UI_SESSION` |
| `StatModifierTester.cs` | Debug MonoBehaviour (+ `Assets/Editor/StatModifierTesterEditor.cs`) |

Access: `IStatService` (`Interface/IStatService.cs`) implemented by `StatHandlerBase<T>`;
`IPlayerStatService : IStatService` is resolved from `PlayerManager.StatService`.
Assets: `Assets/SO/Stat/PlayerStats.asset`, `Enemy/*Stats.asset`, `Enemy/Boss/BossStats.asset`.

## Rules

- `Stat.modifiers` holds runtime state and must not be serialized; `StatModifierGroup.authoredModifiers` must be.
- Max values via `IStatService.GetStatValue()`; current values via `IVitalComponent` — never mix.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-063 | ACCEPTED (deferred to demo prep): `Stat.modifiers` re-serialized under `#if UNITY_EDITOR` (`Stat.cs:63-66`). Check `git status` for dirty `Assets/SO/Stat/*.asset` after Play Mode |
| TD-018 | `TalentManagger` hardcodes stats outside this system |
