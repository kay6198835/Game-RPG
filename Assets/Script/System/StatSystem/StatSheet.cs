using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The runtime stats of ONE character, built from an authored <see cref="BaseStatsSO"/>.
/// Every number is calculated in exactly one method, <see cref="RecalculateAll"/>; reads only return what
/// it wrote. Writes (modifiers, points) change an input and call it again. A plain class: create one with
/// <c>new StatSheet(profile)</c> wherever stats are needed (character, Inspector preview, QA).
/// </summary>
// NEED CHECK: new 2026-11-09 (stat split) — replaces the per-character Instantiate()d BaseStatsSO clone.
public class StatSheet : IStatService
{
    private static readonly StatType[] AllTypes = (StatType[])Enum.GetValues(typeof(StatType));

    private readonly BaseStatsSO profile;
    private readonly Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
    private readonly Dictionary<StatType, StatsViewDTO> views = new Dictionary<StatType, StatsViewDTO>();

    /// <summary>Raised by RecalculateAll() once per stat whose FinalValue changed.</summary>
    public event Action<StatType> Changed;

    public BaseStatsSO Profile => profile;
    public int Level => profile != null ? profile.Level : 1;

    /// <summary>Every stat in StatType order (for debug views).</summary>
    public IEnumerable<Stat> All
    {
        get { foreach (StatType type in AllTypes) yield return stats[type]; }
    }

    public StatSheet(BaseStatsSO profile)
    {
        this.profile = profile;
        foreach (StatType type in AllTypes) stats[type] = new Stat(type);
        RecalculateAll();
    }

    // ------------------------- Read -------------------------

    public int GetLevel() => Level;
    public Stat GetStat(StatType statType) => stats[statType];
    public float GetStatValue(StatType statType) => stats[statType].FinalValue;

    public Dictionary<StatType, float> GetFullStat()
    {
        var result = new Dictionary<StatType, float>();
        foreach (StatType type in AllTypes) result[type] = stats[type].FinalValue;
        return result;
    }

    public StatsViewDTO GetViewStat(StatType statType)
    {
        if (!views.TryGetValue(statType, out StatsViewDTO view))
        {
            view = new StatsViewDTO(statType);
            views[statType] = view;
        }
        Stat stat = stats[statType];
        view.Update(stat.BaseValue, stat.LevelUpValue, stat.EquipmentValue);
        return view;
    }

    public Dictionary<StatType, StatsViewDTO> GetFullViewStats()
    {
        foreach (StatType type in AllTypes) GetViewStat(type);
        return views;
    }

    /// <summary>Points not spent yet: level − 1 − Σ primary LevelUpValue (level 1 gives no point).</summary>
    public int GetLevelUpStatsBonus()
    {
        float spent = 0f;
        foreach (StatType type in AllTypes)
        {
            if (type.IsPrimary()) spent += stats[type].LevelUpValue;
        }
        return Mathf.Max(0, Level - 1 - (int)spent);
    }

    // ------------------------- Write -------------------------

    /// <summary>
    /// Attaches a bundle of modifiers from one source (equipment, buff). Each authored modifier is copied and
    /// stamped with the source, so the shared list is never changed. Remove with the same source reference.
    /// </summary>
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
    {
        if (modifiers == null) return;
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier authored = modifiers[i];
            if (authored == null) continue;
            stats[authored.TargetStatType].ModifierList.Add(authored.WithSource(source));
        }
        RecalculateAll();
    }

    /// <summary>Removes every modifier whose source is this reference (unequip, buff expired).</summary>
    public void RemoveModifiersFromSource(object source)
    {
        foreach (StatType type in AllTypes)
        {
            stats[type].ModifierList.RemoveAll(m => ReferenceEquals(m.Source, source));
        }
        RecalculateAll();
    }

    /// <summary>Removes every modifier, whatever its source.</summary>
    public void ClearModifiers()
    {
        foreach (StatType type in AllTypes) stats[type].ModifierList.Clear();
        RecalculateAll();
    }

    /// <summary>Adds (or, with a negative amount, refunds) spent points on a primary stat.</summary>
    public void AddPrimaryPoint(StatType type, float amount)
    {
        if (!type.IsPrimary())
        {
            Debug.LogWarning($"[StatSheet] {type} is not a primary stat.");
            return;
        }
        stats[type].LevelUpValue += amount;
        RecalculateAll();
    }

    // ------------------------- The one calculation -------------------------

    /// <summary>
    /// Recomputes every stat from the profile, the spent points and the modifiers, then raises Changed for
    /// each stat whose FinalValue moved. Primary stats first, because derived formulas read them.
    /// </summary>
    public void RecalculateAll()
    {
        var before = new Dictionary<StatType, float>(stats.Count);
        foreach (StatType type in AllTypes) before[type] = stats[type].FinalValue;

        foreach (StatType type in AllTypes)
        {
            if (!type.IsPrimary()) continue;
            Stat stat = stats[type];
            stat.BaseValue = profile != null ? profile.GetPrimaryBase(type) : 0f;
            stat.PrimaryBonus = 0f;
            ApplyModifiers(stat);
        }

        foreach (StatType type in AllTypes)
        {
            if (type.IsPrimary()) continue;
            Stat stat = stats[type];
            DerivedStatFormula formula = profile != null ? profile.GetFormula(type) : null;
            float baseValue = 0f, levelUp = 0f, primaryBonus = 0f;
            if (formula != null)
            {
                baseValue = formula.baseConstant;
                levelUp = Level * formula.perLevel;
                if (formula.contributions != null)
                {
                    foreach (DerivedStatFormula.StatContribution part in formula.contributions)
                    {
                        Stat source = stats[part.sourceStat];
                        baseValue += source.BaseValue * part.coefficient;
                        levelUp += source.LevelUpValue * part.coefficient;
                        primaryBonus += source.EquipmentValue * part.coefficient;
                    }
                }
            }
            stat.BaseValue = baseValue;
            stat.LevelUpValue = levelUp;
            stat.PrimaryBonus = primaryBonus;
            ApplyModifiers(stat);
        }

        if (Changed == null) return;
        foreach (StatType type in AllTypes)
        {
            if (!Mathf.Approximately(before[type], stats[type].FinalValue)) Changed.Invoke(type);
        }
    }

    // ((Adjusted + ΣFlat) × (1 + ΣPercentAdd)) × Π(1 + PercentMult), then + PrimaryBonus.
    private static void ApplyModifiers(Stat stat)
    {
        float adjusted = stat.AdjustedValue;
        float flat = 0f, percentAdd = 0f, percentMult = 1f;
        foreach (StatModifier modifier in stat.Modifiers)
        {
            switch (modifier.Type)
            {
                case ModifierType.Flat: flat += modifier.Value; break;
                case ModifierType.PercentAdd: percentAdd += modifier.Value; break;
                case ModifierType.PercentMult: percentMult *= 1f + modifier.Value; break;
            }
        }
        float withModifiers = (adjusted + flat) * (1f + percentAdd) * percentMult;
        stat.ModifierBonus = withModifiers - adjusted;
        stat.FinalValue = withModifiers + stat.PrimaryBonus;
    }
}

/// <summary>Display copy of one stat for the stats UI (rounded to 2 decimals).</summary>
[Serializable]
public class StatsViewDTO
{
    public StatType StatType;
    public string Name;
    public float BaseValue;
    public float LevelUpValue;
    public float FinalValue;
    public float AdjustedValue;
    public float EquipmentValue;

    public StatsViewDTO(StatType statType)
    {
        StatType = statType;
        Name = GameConstants.StatTypeName.TryGetValue(statType, out string name) ? name : statType.ToString();
    }

    public void Update(float baseValue, float levelUpValue, float equipmentValue)
    {
        BaseValue = Round2(baseValue);
        LevelUpValue = Round2(levelUpValue);
        AdjustedValue = Round2(baseValue + levelUpValue);
        EquipmentValue = Round2(equipmentValue);
        FinalValue = Round2(baseValue + levelUpValue + equipmentValue);
    }

    private static float Round2(float value)
    {
        return (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
