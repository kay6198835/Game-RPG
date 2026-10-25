using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared owner of a character's CURRENT stat values. Max values come from the sibling
/// <see cref="IStatService"/>; modifiers are applied to that service, current values are kept here.
/// </summary>
public abstract class VitalStatsBase<TCore> : CoreComponentBase<TCore>, IVitalComponent where TCore : CoreBase
{
    protected Dictionary<StatType, float> currentStats = new();

    public event System.Action CurrentStatsChanged;

    protected override void Start()
    {
        base.Start();
    }

    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out statHandler);
        currentStats = statHandler.GetFullStat();
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        currentStats = statHandler.GetFullStat();
        UpdateStatField();
    }

    protected virtual void UpdateStatField()
    {
        _currentHP = GetCurrentStatValue(StatType.HP);
        CurrentStatsChanged?.Invoke();
    }

    public float GetCurrentStatValue(StatType statType)
    {
        if (currentStats.TryGetValue(statType, out float valueStat))
        {
            return valueStat;
        }
        return statHandler.GetStatValue(statType);
    }

    public void Recovery(StatType statType, float amount)
    {
        if (statType.IsPrimary()) return;
        if (currentStats[statType] + amount >= statHandler.GetStatValue(statType))
        {
            currentStats[statType] = statHandler.GetStatValue(statType);
        }
        else
        {
            currentStats[statType] += amount;
        }
        UpdateStatField();
    }

    public void Reduction(StatType statType, float amount)
    {
        if (statType.IsPrimary()) return;
        if (currentStats[statType] - amount <= 0)
        {
            currentStats[statType] = 0;
        }
        else
        {
            currentStats[statType] -= amount;
        }
        UpdateStatField();
    }
}
