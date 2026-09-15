using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery Stat Effect")]
public class RecoveryStatEffect : AbilityEffectDefinition
{
    [field: SerializeField] private StatModifierGroup statModifier;

    public override void Apply(AbilityContext context)
    {

        Dictionary<StatType, List<StatModifier>> keyValuePairs = new();

        foreach (var modifier in statModifier.Modifiers)
        {
            if (!keyValuePairs.TryGetValue(modifier.TargetStat, out List<StatModifier> list))
            {
                List<StatModifier> listStatModifiers = new List<StatModifier>();
                keyValuePairs.Add(modifier.TargetStat, listStatModifiers);
                keyValuePairs[modifier.TargetStat].Add(modifier);
            }
            else
            {
                keyValuePairs[modifier.TargetStat].Add(modifier);
            }
        }
        foreach (var keyValuePair in keyValuePairs)
        {
            StatType currentStatTypeKey = keyValuePair.Key;
            float currentStatVital = context.Services.Vital.GetCurrentStatValue(currentStatTypeKey);
            context.Services.Vital.ReceiverRecovery(currentStatTypeKey,
             Utility.ModifierStatsCalculate(keyValuePair.Value, currentStatVital));
        }
    }
}