using System.Collections.Generic;
using UnityEngine;

public abstract class StatsEffectBase : AbilityEffectDefinition
{
    [field: SerializeField] protected StatModifierGroup statModifier;
    [field: SerializeField] protected StatImpactType impactType;
    [SerializeField] protected EffectRecipient recipient = EffectRecipient.Caster;
    
    public override void Apply(AbilityContext context)
    {
        // Same path for a player or an enemy: only IVitalComponent is touched.
        if (!context.TryGetRecipientComponent(recipient, out IVitalComponent vital)) return;
        if (!context.TryGetRecipientComponent(recipient, out IEffectReceiver effectReceiver)) return;

        Dictionary<StatType, List<StatModifier>> grouped = new();

        foreach (var modifier in statModifier.Modifiers)
        {
            if (!grouped.TryGetValue(modifier.TargetStat, out List<StatModifier> list))
            {
                list = new List<StatModifier>();
                grouped.Add(modifier.TargetStat, list);
            }
            list.Add(modifier);
        }

        foreach (var kvp in grouped)
        {
            StatType currentStatTypeKey = kvp.Key;
            float currentStatVital = vital.GetCurrentStatValue(currentStatTypeKey);
            float impactValue = Utility.ModifierStatsCalculate(kvp.Value, currentStatVital);

            ApplyImpact(effectReceiver, currentStatTypeKey, impactValue);
        }
    }

    protected abstract void ApplyImpact(IEffectReceiver effectReceiver, StatType statType, float impactValue);
}
