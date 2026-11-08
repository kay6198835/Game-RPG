# Stats

> **Status:** live · **Last verified:** 2026-11-09 (stat split, working tree — not committed, not yet compiled in the Editor) · **ADR:** ADR-0001
> History: [CHANGELOG.md](CHANGELOG.md) · GDD: `design/gdd/stat-system.md` · Numbers: `ToolExcel/stat_system_formula_reference.xlsx`

## Purpose

RPG stat model: primary stats feed derived stats through formulas; modifiers from equipment, buffs
and abilities stack on top. Max values live here; current values live in the vitals component
(see [character](../character/README.md)).

## Design — data asset + one calculator

| Part | Type | Job |
|------|------|-----|
| Profile | `BaseStatsSO` (asset) | **Data only:** `level`, `primaryStats` (base value of STR/DEX/INT/VIT/LUK), `derivedStats` (one `DerivedStatFormula` per derived stat). No runtime state, no calculation |
| Calculator | `StatSheet` (plain class, `: IStatService`) | Built with `new StatSheet(profile)`. Holds one runtime `Stat` per `StatType`, the modifiers and the spent points. **Every number is computed in `StatSheet.RecalculateAll()`** |
| Character façade | `StatHandlerBase<T>` (`StatHandler`, `EntityStatsHandler`) | Owns one `StatSheet` per character, relays `Changed` as `StatChanged`, drops the sheet on `ReloadComponent()` (respawn / pool) |

Flow: designer edits the asset → a character reads a stat → `StatHandlerBase` builds its `StatSheet`
→ `RecalculateAll()` → gameplay reads `FinalValue` → a buff or a point changes an input →
`RecalculateAll()` again → `Changed(StatType)` for every stat whose value moved → vitals / UI / AI.

### The calculation (`RecalculateAll`)

```
Primary:  Final = ((Base + LevelUp + ΣFlat) × (1 + ΣPercentAdd)) × Π(1 + PercentMult)
Derived:  Base         = baseConstant + Σ(primary Base × coefficient)
          LevelUp      = level × perLevel + Σ(primary LevelUp × coefficient)
          PrimaryBonus = Σ(primary EquipmentValue × coefficient)
          Final        = ((Base + LevelUp + ΣFlat) × (1 + ΣPercentAdd)) × Π(1 + PercentMult) + PrimaryBonus
```

Each `Stat` exposes the parts for inspection: `BaseValue`, `LevelUpValue`, `AdjustedValue`,
`PrimaryBonus` (was `equipmentByPrimaryValue`), `ModifierBonus`, `EquipmentValue`
(= `ModifierBonus + PrimaryBonus`), `FinalValue`. Only `StatSheet` writes them.

Where to look while debugging:
- a breakpoint in `StatSheet.RecalculateAll()`;
- Play Mode: the read-only **Debug** list on the character's `StatHandler` / `EntityStatsHandler`;
- the stat asset Inspector (`BaseStatsSOEditor`): a preview table from a fresh sheet (no buffs);
- the `EntityData` Inspector (`EntityDataEditor`): view range / angle / idle / move from the profile.

## Code — `Assets/Script/System/StatSystem/`

| File | Contents |
|------|----------|
| `StatType.cs` | Primary STR/DEX/INT/VIT/LUK (0-4); derived HP … Evasion (100-111), MaxRangeAttack (112), MinRangeAttack (113) |
| `BaseStatsSO.cs` | Data-only profile; `GetPrimaryBase()`, `GetFormula()`; `OnValidate` keeps exactly one row per primary stat |
| `PrimaryStatEntry.cs` | One authored primary row (`Type`, `baseValue`) |
| `DerivedStatFormula.cs` | `targetStat`, `baseConstant`, `perLevel`, `contributions[]` — data only |
| `StatSheet.cs` | The calculator; also declares `StatsViewDTO` |
| `Stat.cs` | Runtime result record + modifier list (not serialized) |
| `StatModifier.cs` | Authored `targetStatType` / `type` / `value` + runtime `Source` (stamped by `WithSource()`) |
| `StatModifierGroup.cs` | `[Serializable]` bundle (`statsModifiers`, `vitalModifiers`), embedded in `WeaponStats` and effects |
| `EnemyStatSO.cs` | `: BaseStatsSO` |
| `StatPointSession.cs` / `StatPointAllocator.cs` | Point allocation session (accept / revert / restore) over `IPlayerStatService` |
| `StatModifierTester.cs` | Debug MonoBehaviour working on its own `StatSheet` (+ `Assets/Editor/StatModifierTesterEditor.cs`) |

Access: `IStatService` (`Interface/IStatService.cs`) implemented by `StatSheet` and `StatHandlerBase<T>`;
`IPlayerStatService : IStatService` is resolved from `PlayerManager.StatService`.
Assets: `Assets/SO/Stat/Player/{Paladin,Archer,Mage,Base}Stats.asset`, `Enemy/*Stats.asset`, `Enemy/Boss/BossStats.asset`.

## Rules

- Never write to a `BaseStatsSO` at runtime. Buffs, points and level-ups go through `IStatService` (the character's sheet).
- Spent points (`LevelUpValue`) live only in the sheet: they reset on respawn and New Game.
- A derived stat has no authored value: change its formula or the primary stats it reads.
- Formula `contributions` must reference primary stats only.
- Max values via `IStatService.GetStatValue()`; current values via `IVitalComponent` — never mix.
- Enemy AI values derived from stats (`EntityData.RangeView/AngleView/IdleDurationTime/MoveDurationTime`) take the enemy's own `IStatService`.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-063 | Addressed in code by the 2026-11-09 split (`Stat` is no longer serialized); status left for triage |
| BUG-115 | Runtime path no longer writes the asset; the 2026-11-09 split also makes spent points runtime-only. Status left for triage |
| TD-018 | `TalentManagger` hardcodes stats outside this system |
