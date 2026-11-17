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
    protected IStatService<TCore> statHandler;
    public event Action<VitalStatType> CurrentStatsChanged;

    protected override void Start()
    {
        base.Start();
    }

    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out statHandler);
        statHandler?.ResignStatsChange(OnMaxStatChanged);
        LoadVitalStats();
    }

    protected virtual void OnDestroy()
    {
        statHandler?.UnResignStatsChange(OnMaxStatChanged);
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        LoadVitalStats();
    }

    protected virtual void UpdateStatField(VitalStatType vitalStatType)
    {
        CurrentStatsChanged?.Invoke(vitalStatType);
    }

    // Seeds only the vital stats backed by a StatType (HP, Mana); Coin has no StatType.
    void LoadVitalStats()
    {
        currentStats.Clear();
        foreach (VitalStatType vitalStatType in Enum.GetValues(typeof(VitalStatType)))
        {
            StatType statType = (StatType)(int)vitalStatType;
            if (!Enum.IsDefined(typeof(StatType), statType)) continue;
            currentStats[vitalStatType] = statHandler.GetStatValue(statType);
            UpdateStatField(vitalStatType);
        }
    }

    // A lowered max clamps the current value down; a raised max leaves it as is.
    void OnMaxStatChanged(StatType statType)
    {
        VitalStatType vitalStatType = (VitalStatType)(int)statType;
        if (!currentStats.TryGetValue(vitalStatType, out float current)) return;
        float max = statHandler.GetStatValue(statType);
        if (current > max) currentStats[vitalStatType] = max;
        UpdateStatField(vitalStatType);
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
        if (currentStats.TryGetValue(vitalStatType, out float current))
        {
            current += amount;
            StatType statType = (StatType)vitalStatType;
            if (!Enum.IsDefined(typeof(StatType), statType))
            {
                currentStats[vitalStatType] = current;
            }
            else
            {
                currentStats[vitalStatType] = Mathf.Min(current, statHandler.GetStatValue(statType));
            }
            UpdateStatField(vitalStatType);
        }
    }

    public void Reduction(VitalStatType vitalStatType, float amount)
    {
        if (currentStats.TryGetValue(vitalStatType, out float current))
        {
            current -= amount;
            currentStats[vitalStatType] = Mathf.Max(current, 0);
            UpdateStatField(vitalStatType);
        }
    }
}
