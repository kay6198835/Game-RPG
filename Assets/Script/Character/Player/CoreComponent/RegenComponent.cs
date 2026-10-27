using System.Collections.Generic;
using UnityEngine;

public class RegenComponent : MovementBase<Core>, IRegenResource
{
    StatHandler statHandler;
    VitalStatsComponent vitalStats;
    public Dictionary<StatType, RegenData> _cacheDatas;

    public override void Setup()
    {
        base.Setup();
        Core.GetCoreComponent(out statHandler);
        Core.GetCoreComponent(out vitalStats);
        SetData();
    }


    public void Regenning()
    {
        foreach (var stat in _cacheDatas)
        {
            if (stat.Value.nextTimeRegen >= Time.time
            || stat.Value.currentValue == stat.Value.maxValue) continue;
            vitalStats.Recovery(stat.Key, stat.Value.regenValue);
            stat.Value.currentValue = vitalStats.GetCurrentStatValue(stat.Key);
            stat.Value.nextTimeRegen = Time.time + stat.Value.eachTime;
        }
    }

    public void SetRegenDuration(StatType statType, float duration)
    {
        _cacheDatas[statType].eachTime = duration;
    }

    void OnChangeStatMax(StatType statType)
    {
        statHandler.GetStatValue(statType);
        _cacheDatas[statType].maxValue = statHandler.GetStatValue(statType);
    }

    void SetData()
    {
        SeedRegenData();
        foreach (var data in _cacheDatas)
        {
            statHandler.ResignChangeStat(OnChangeStatMax);
        }
    }    

    void SeedRegenData()
    {
        _cacheDatas.Add(StatType.Mana, new RegenData
        {
            maxValue = statHandler.GetStatValue(StatType.Mana),
            currentValue = statHandler.GetStatValue(StatType.Mana),
            eachTime = 1, // default 1s
            nextTimeRegen = 0,
            regenValue = statHandler.GetStatValue(StatType.ManaRegen)
        });
        _cacheDatas.Add(StatType.HP, new RegenData
        {
            maxValue = statHandler.GetStatValue(StatType.HP),
            currentValue = statHandler.GetStatValue(StatType.HP),
            eachTime = 1, // default 1s
            nextTimeRegen = 0,
            regenValue = statHandler.GetStatValue(StatType.HPRegen)
        });
    }

    public void OnChangeStatMax()
    {
        throw new System.NotImplementedException();
    }

    void IRegenResource.SetData()
    {
        SetData();
    }
}

public class RegenData
{
    public float maxValue;
    public float currentValue;
    public float eachTime;
    public float nextTimeRegen;
    public float regenValue;
}