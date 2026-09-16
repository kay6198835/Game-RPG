using System.Collections.Generic;
using UnityEngine;

public abstract class StatsEffectBase : AbilityEffectDefinition
{
    [field: SerializeField] protected StatModifierGroup statModifier;
    [field: SerializeField] protected StatImpactType impactType;

    public override void Apply(AbilityContext context)
    {
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
            float currentStatVital = context.Services.Vital.GetCurrentStatValue(currentStatTypeKey);
            float impactValue = Utility.ModifierStatsCalculate(kvp.Value, currentStatVital);

            ApplyImpact(context, currentStatTypeKey, impactValue);
        }
    }

    protected abstract void ApplyImpact(AbilityContext context, StatType statType, float impactValue);
}