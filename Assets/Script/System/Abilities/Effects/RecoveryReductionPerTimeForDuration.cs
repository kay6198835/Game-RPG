using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    public float duration;
    public float perTime;
    protected override void ApplyImpact(AbilityContext context, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                context.Services.Vital.RecoveryPerTimerForDuration(statType, impactValue, perTime, duration);
                break;
            case StatImpactType.Reduction:
                context.Services.Vital.ReductionPerTimerForDuration(statType, impactValue, perTime, duration);
                break;
        }
    }
}

