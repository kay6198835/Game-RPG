using System.Collections.Generic;
using UnityEngine;

public abstract class StatsEffectBase : AbilityEffectDefinition
{
    [field: SerializeField] protected StatModifierGroup statModifier;
    [field: SerializeField] protected StatImpactType impactType;
    [SerializeField] protected EffectRecipient recipient = EffectRecipient.Caster;

    public override void Apply(AbilityContext context)
    {
        // Same path for a player or an enemy: only the ICharacter contract is touched.
        ICharacter character = context.ResolveRecipient(recipient);
        if (character == null) return;
        IVitalComponent vital = character.Vital;

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

            ApplyImpact(vital, currentStatTypeKey, impactValue);
        }
    }

    protected abstract void ApplyImpact(IVitalComponent vital, StatType statType, float impactValue);
}
