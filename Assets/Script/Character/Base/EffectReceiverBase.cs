using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
public class EffectReceiverBase<TCore> : CoreComponentBase<TCore>, IEffectReceiver where TCore : CoreBase
{
    protected IVitalComponent<TCore> vital;
    protected IStatService statHandler;
    [SerializeField] protected SourceEffect sourceEffect;
    protected IObjectResolver _resolver;
    protected IObjectPoolService _objectPoolManager;
    [Inject]
    public void Construct(IObjectResolver resolver, IObjectPoolService objectPoolManager)
    {
        _resolver = resolver;
        _objectPoolManager = objectPoolManager;
    }
    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out statHandler);
    }

    GameObject SetEffectSource(AbilityContext context, float lifetime, Action callback)
    {
        Vector2 position = context != null ? context.Caster.Transform.position : Vector2.zero;
        GameObject instance = _objectPoolManager.Spawn(sourceEffect.gameObject, position, Quaternion.identity);
        instance.GetComponent<SourceEffect>().SetSourceEffect(new SourceEffectConfig
        {
            callback = callback,
            lifetime = lifetime
        });
        _resolver?.Inject(instance);
        return instance;
    }
    public override void ReloadComponent()
    {
        base.ReloadComponent();
    }
    public void AdjustCurrentStats(StatModifierGroup statModifierGroup, AbilityContext context, Action callback = null)
    {
        foreach (var modifier in statModifierGroup.GetVitalModifiers)
        {
            if (modifier.ImpactType == StatImpactType.Reduction)
            {
                vital.Reduction(modifier.TargetStatType, Math.Abs(modifier.Value));
            }
            else if (modifier.ImpactType == StatImpactType.Recovery)
            {
                vital.Recovery(modifier.TargetStatType, Math.Abs(modifier.Value));
            }
        }
    }

    public void AdjustMaxStats(StatModifierGroup statModifierGroup, AbilityContext context = null, Action callback = null)
    {
        statHandler.AddModifiersFromSource(SetEffectSource(context, 0f, callback), statModifierGroup.GetStatsModifiers);
    }
    #region Reduction/Recovery per timer for duration
    // BUG-071: the iterators below are built but never started. Moved here unchanged.
    public void ReductionPerTimeForDuration(StatModifierGroup statModifierGroup, float perTime, float timeCount, AbilityContext context, Action action = null)
    {
        StartCoroutine(ReductionPerTime(statModifierGroup, perTime, timeCount, context));
    }

    IEnumerator ReductionPerTime(StatModifierGroup statModifierGroup, float perTime, float timeCount, AbilityContext context)
    {
        AdjustCurrentStats(statModifierGroup, context);
        timeCount--;
        if (timeCount > 0)
        {
            yield return new WaitForSeconds(perTime);
            ReductionPerTimeForDuration(statModifierGroup, perTime, timeCount, context);
        }
    }
    #endregion

    #region Buff/Debuff for duration


    IEnumerator ApplyAdjustCurrentForDuration(StatModifierGroup statModifierGroup, float duration, AbilityContext context, Action callback = null)
    {
        var sourceEffect = SetEffectSource(context, duration, callback);
        statHandler.AddModifiersFromSource(sourceEffect, statModifierGroup.GetStatsModifiers);
        yield return new WaitForSeconds(duration);
        statHandler.RemoveModifiersFromSource(sourceEffect);
    }

    public void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context, Action callback = null)
    {
        StartCoroutine(ApplyAdjustCurrentForDuration(modifierGroup, duration, context, callback));
    }

    public void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context, Action callback = null)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}