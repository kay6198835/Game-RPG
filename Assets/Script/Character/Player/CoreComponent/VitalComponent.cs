using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using VContainer;

//Multiple inheritance interface
//Affect max stat use modifier(statHandler)
//Affect current stats use StatType(Dictionary)
public class VitalStatsComponent : CoreComponent<Core>, IVitalComponent
{
    Dictionary<StatType, float> currentStats = new();
    StatHandler statHandler;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out statHandler);
        currentStats = statHandler.GetFullStat();
    }

    public float GetCurrentStatValue(StatType statType)
    {
        return currentStats[statType];
    }

    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statModifierGroup.Apply(statHandler.AddModifiersFromSource, this);
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
    }

    #region Reduction/Recovery per timer for duration
    public void ReductionPerTimerForDuration(StatType statType, float amount, float perTime, int duration)
    {
        var count = duration / perTime;
        if (duration % perTime > 0) count++;
        ReductionPerTimeForDuration(statType, amount, perTime, duration);
    }

    public void RecoveryPerTimerForDuration(StatType statType, float amount, float perTime, int duration)
    {
        var count = duration / perTime;
        if (duration % perTime > 0) count++;
        RecoveryPerTimeForDuration(statType, amount, perTime, duration);
    }

    IEnumerator RecoveryPerTimeForDuration(StatType statType, float amount, float perTime, int timeCount)
    {
        Recovery(statType, amount);
        timeCount--;
        if (timeCount <= 0) return;
        yield return new WaitForSeconds(perTime);
        RecoveryPerTimeForDuration(timeCount);
    }

    IEnumerator ReductionPerTimeForDuration(StatType statType, float amount, float perTime, int timeCount)
    {
        Reduction(statType, amount);
        timeCount--;
        if (timeCount <= 0) return;
        yield return new WaitForSeconds(perTime);
        ReductionPerTimeForDuration(timeCount);
    }
    #endregion

    #region Buff/Debuff for duration

    public void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration)
    {
        StartCoroutine(ApplyDebuffForDuration(statModifierGroup, duration));
    }
    IEnumerator ApplyDebuffForDuration(StatModifierGroup statModifierGroup, float duration)
    {
        statModifierGroup.Apply(statHandler.AddModifiersFromSource, this);
        yield return new WaitForSeconds(duration);
        statModifierGroup.Remmove(statHandler.RemoveModifiersFromSource, this);
    }
    #endregion
}