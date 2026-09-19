using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Stats Effect")]
public class RecoveryReductionStatsEffect : StatsEffectBase
{
    protected override void ApplyImpact(AbilityContext context, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                context.Services.Vital.Recovery(statType, impactValue);
                break;
            case StatImpactType.Reduction:
                context.Services.Vital.Reduction(statType, impactValue);
                break;
        }
    }
}