using System.Collections;
using UnityEngine;

public class EffectReceiverBase<TCore> : CoreComponentBase<TCore>, IEffectReceiver where TCore : CoreBase
{
    protected IVitalComponent vital;
    protected IStatService statHandler;
    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out statHandler);
    }
    public void AdjustMaxToCurrentStats(StatModifierGroup statModifierGroup, bool isAdjustmentMaxStats = false, object obj = null)
    {
        // if (statType.IsPrimary()) return;
        // if (currentStats[statType] - amount <= 0)
        //     foreach (var modifier in statModifierGroup.Modifiers)
        //     {
        //         currentStats[statType] = 0;
        //         if (modifier.statType.IsPrimary()) continue;

        //         if (currentStats[modifier.statType] + amount >= statHandler.GetStatValue(modifier.statType))
        //         {
        //             if (isAdjustmentMaxStats)
        //             {
        //                 currentStats[modifier.statType] += amount;
        //             }
        //             else
        //             {
        //                 currentStats[modifier.statType] = statHandler.GetStatValue(modifier.statType);
        //             }
        //         }
        //         else if (currentStats[modifier.statType] + amount <= 0)
        //         {
        //             currentStats[modifier.statType] = 0;
        //         }
        //         else
        //         {
        //             currentStats[modifier.statType] += amount;
        //         }
        //         // UpdateStatField();
        //     }
        // else
        //     if (isAdjustmentMaxStats)
        //     {
        //         currentStats[statType] -= amount;
        //         ModifierMaxStats(statModifierGroup, obj);
        //     }
        // UpdateStatField();
    }






































    
    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statHandler.AddModifiersFromSource(this, statModifierGroup.Modifiers);
        // UpdateStatField();
    }
    #region Reduction/Recovery per timer for duration
    // BUG-071: the iterators below are built but never started. Moved here unchanged.
    public void ReductionPerTimeForDuration(StatType statType, float amount, float perTime, float timeCount)
    {
        StartCoroutine(ReductionPerTime(statType, amount, perTime, timeCount));
    }

    public void RecoveryPerTimeForDuration(StatType statType, float amount, float perTime, float timeCount)
    {
        StartCoroutine(RecoveryPerTime(statType, amount, perTime, timeCount));
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
        statHandler.AddModifiersFromSource(this, statModifierGroup.Modifiers);
        yield return new WaitForSeconds(duration);
        statHandler.RemoveModifiersFromSource(this);
    }

    public void Recovery(StatType statType, float amount)
    {
        throw new System.NotImplementedException();
    }

    public void Reduction(StatType statType, float amount)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}