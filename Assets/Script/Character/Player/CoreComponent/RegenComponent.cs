using System.Collections.Generic;
using UnityEngine;

// NEED CHECK: also implements IRegenResource — IRegenResource<Core> is empty and does not extend it, so
// Regenning() was unreachable through the interface.
public class RegenComponent : CoreComponent<Core>, IRegenResource<Core>
{
    IStatService<Core> statHandler;
    IVitalComponent<Core> vitalStats;
    // NEED CHECK: holds timing only. Max, regen amount and current value are read live each tick (was cached
    // maxValue / regenValue, stale after a class change because a new stat clone raises no StatChanged).
    public Dictionary<VitalStatType, RegenData> _cacheDatas = new()
    {
        { VitalStatType.HP, new RegenData { eachTime = 1 } },   // default 1s
        { VitalStatType.Mana, new RegenData { eachTime = 1 } },
    };

    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out statHandler);
        Core.TryGetCapability(out vitalStats);
    }

    /// <summary>Recovers HP / Mana by their regen stat, once per eachTime, while below the max stat.</summary>
    public void Regenning()
    {
        if (statHandler == null || vitalStats == null) return;
        foreach (var stat in _cacheDatas)
        {
            if (stat.Value.nextTimeRegen >= Time.time) continue;
            StatType maxType = (StatType)(int)stat.Key;
            StatType regenType = stat.Key == VitalStatType.HP ? StatType.HPRegen : StatType.ManaRegen;
            if (vitalStats.GetCurrentStatValue(stat.Key) >= statHandler.GetStatValue(maxType)) continue;
            vitalStats.Recovery(stat.Key, statHandler.GetStatValue(regenType));
            stat.Value.nextTimeRegen = Time.time + stat.Value.eachTime;
        }
    }
}

public class RegenData
{
    // NEED CHECK: removed — read live in Regenning().
    //public float maxValue;
    //public float regenValue;
    public float eachTime;
    public float nextTimeRegen;
}
