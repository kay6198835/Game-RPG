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
    protected IStatService statHandler;

    protected override void Start()
    {
        base.Start();
        Reborn();
    }

    public virtual void Reborn()
    {
        if (statHandler == null) Core.TryGetCapability(out statHandler);
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
    // BUG-071: the iterators below are built but never started. Moved here unchanged.
    public void ReductionPerTimeForDuration(StatType statType, float amount, float perTime, float duration)
    {
        var count = duration / perTime;
        if (duration % perTime > 0) count++;
        StartCoroutine(ReductionPerTime(statType, amount, perTime, duration));
    }

    public void RecoveryPerTimeForDuration(StatType statType, float amount, float perTime, float duration)
    {
        var count = duration / perTime;
        if (duration % perTime > 0) count++;
        StartCoroutine(RecoveryPerTime(statType, amount, perTime, duration));
    }

    IEnumerator RecoveryPerTime(StatType statType, float amount, float perTime, float timeCount)
    {
        Recovery(statType, amount);
        timeCount--;
        if (timeCount > 0)
        {
            yield return new WaitForSeconds(perTime);
            RecoveryPerTimeForDuration(statType, amount, perTime, timeCount);
        }
    }

    IEnumerator ReductionPerTime(StatType statType, float amount, float perTime, float timeCount)
    {
        Reduction(statType, amount);
        timeCount--;
        if (timeCount > 0)
        {
            yield return new WaitForSeconds(perTime);
            ReductionPerTimeForDuration(statType, amount, perTime, timeCount);
        }
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
