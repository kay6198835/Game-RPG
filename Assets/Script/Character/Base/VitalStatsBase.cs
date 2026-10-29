using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared owner of a character's CURRENT stat values. Max values come from the sibling
/// <see cref="IStatService"/>; modifiers are applied to that service, current values are kept here.
/// </summary>
public abstract class VitalStatsBase<TCore> : CoreComponentBase<TCore>, IVitalComponent<TCore> where TCore : CoreBase
{
    protected Dictionary<VitalStatType, float> currentStats = new();
    protected IStatService statHandler;
    public event Action CurrentStatsChanged;

    protected override void Start()
    {
        base.Start();
    }

    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out statHandler);
        LoadVitalStats();
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        LoadVitalStats();
        UpdateStatField();
    }

    protected virtual void UpdateStatField()
    {
        CurrentStatsChanged?.Invoke();
    }

    void LoadVitalStats()
    {
        foreach (var stat in statHandler.GetFullStat())
        {
            VitalStatType vitalStatType = (VitalStatType)stat.Key;
            currentStats[vitalStatType] = Enum.IsDefined(typeof(VitalStatType), vitalStatType)
                ? statHandler.GetStatValue(stat.Key)
                : 0f;
        }
    }

    public float GetCurrentStatValue(VitalStatType vitalStatType)
    {
        if (currentStats.TryGetValue(vitalStatType, out float valueStat))
        {
            return valueStat;
        }
        return 0;
    }

    public void Recovery(VitalStatType vitalStatType, float amount)
    {
        StatType statType = (StatType)vitalStatType;
        if (!Enum.IsDefined(typeof(StatType), statType)) return;
        if (currentStats[vitalStatType] < statHandler.GetStatValue(statType))
        {
            currentStats[vitalStatType] += amount;
        }
        else
        {
            currentStats[vitalStatType] = statHandler.GetStatValue(statType);
        }
        UpdateStatField();
    }

    public void Reduction(VitalStatType vitalStatType, float amount)
    {
        if (currentStats[vitalStatType] - amount <= 0)
        {
            currentStats[vitalStatType] = 0;
        }
        else
        {
            currentStats[vitalStatType] -= amount;
        }
        UpdateStatField();
    }
}
