using System.Collections.Generic;

/// <summary>
/// One stat of one character at runtime. Inputs are <see cref="LevelUpValue"/> (spent points, primary
/// stats only) and the modifier list. Every other value is a RESULT written by
/// <see cref="StatSheet.RecalculateAll"/>; nothing else sets them.
///
///   AdjustedValue = BaseValue + LevelUpValue
///   FinalValue    = AdjustedValue + ModifierBonus + PrimaryBonus
/// </summary>
// NEED CHECK: 2026-11-09 (stat split) — was a serialized class that also calculated itself (dirty flag,
// SetDirty → Recaulate, six stored tiers). It is no longer serialized anywhere, so runtime modifiers can
// never reach an .asset file (BUG-063 by construction).
public class Stat
{
    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    public StatType Type { get; }

    /// <summary>Primary: authored base. Derived: baseConstant + Σ(primary base × coefficient).</summary>
    public float BaseValue { get; internal set; }

    /// <summary>Primary: spent points (input). Derived: level × perLevel + Σ(primary level-up × coefficient).</summary>
    public float LevelUpValue { get; internal set; }

    /// <summary>Derived only: what buffs on primary stats add, Σ(primary EquipmentValue × coefficient). Was equipmentByPrimaryValue.</summary>
    public float PrimaryBonus { get; internal set; }

    /// <summary>What this stat's own modifiers add on top of AdjustedValue (Flat → PercentAdd → PercentMult).</summary>
    public float ModifierBonus { get; internal set; }

    /// <summary>The value gameplay reads.</summary>
    public float FinalValue { get; internal set; }

    public float AdjustedValue => BaseValue + LevelUpValue;

    /// <summary>Total bonus over AdjustedValue (ModifierBonus + PrimaryBonus). Read by the stats UI.</summary>
    public float EquipmentValue => ModifierBonus + PrimaryBonus;

    public IReadOnlyList<StatModifier> Modifiers => modifiers;

    internal List<StatModifier> ModifierList => modifiers;

    public Stat(StatType type)
    {
        Type = type;
    }
}
