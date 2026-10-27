using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectReceiverBase<TCore> : CoreComponentBase<TCore>, IEffectReceiver where TCore : CoreBase
{
    protected IVitalComponent vital;
    protected IStatService statHandler;
    protected Dictionary<StatType, float> currentStats = new();
    protected GameObject effectSource;
    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out statHandler);
        effectSource = new GameObject();
    }

    void SetEffectSource(AbilityContext context)
    {
        //effectSource = context.Caster
    }
    public override void ReloadComponent()
    {
        base.ReloadComponent();
        currentStats = statHandler.GetFullStat();
    }
    public void AdjustMaxToCurrentStats(StatModifierGroup statModifierGroup, bool isIgnoreMax = true, object obj = null)
    {

        foreach (var modifier in statModifierGroup.Modifiers)
        {
            if (modifier.TargetStatType.IsPrimary()) continue;

            if (modifier.ImpactType == StatImpactType.Reduction)
            {
                vital.Reduction(modifier.TargetStatType, modifier.Value, isIgnoreMax);
            }
            else if (modifier.ImpactType == StatImpactType.Recovery)
            {
                vital.Recovery(modifier.TargetStatType, modifier.Value, isIgnoreMax);
            }
        }
    }

    public void AdjustMaxStats(StatModifierGroup statModifierGroup, object obj = null)
    {
        statHandler.AddModifiersFromSource(this, statModifierGroup.Modifiers);
        // UpdateStatField();
    }
    #region Reduction/Recovery per timer for duration
    // BUG-071: the iterators below are built but never started. Moved here unchanged.
    public void ReductionPerTimeForDuration(StatModifierGroup statModifierGroup, float perTime, float timeCount)
    {
        StartCoroutine(ReductionPerTime(statModifierGroup, perTime, timeCount));
    }

    IEnumerator ReductionPerTime(StatModifierGroup statModifierGroup, float perTime, float timeCount, object obj = null)
    {
        AdjustMaxToCurrentStats(statModifierGroup, true, obj);
        timeCount--;
        if (timeCount > 0)
        {
            yield return new WaitForSeconds(perTime);
            ReductionPerTimeForDuration(statModifierGroup, perTime, timeCount);
        }
    }
    #endregion

    #region Buff/Debuff for duration


    IEnumerator ApplyAdjustCurrentForDuration(StatModifierGroup statModifierGroup, float duration, object obj = null)
    {
        statHandler.AddModifiersFromSource(obj, statModifierGroup.Modifiers);
        yield return new WaitForSeconds(duration);
        statHandler.RemoveModifiersFromSource(obj);
    }

    public void AdjustCurrentForDuration(StatModifierGroup modifierGroup, float duration, object obj = null)
    {
        StartCoroutine(ApplyAdjustCurrentForDuration(modifierGroup, duration, obj));
    }

    public void AdjustCurrentPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, object obj = null)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}