using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    private float duration;
    [Range(1, 10)]
    private int timeCount;
    protected override void ApplyImpact(IVitalComponent vital, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                vital.RecoveryPerTimeForDuration(statType, impactValue, perTime, timeCount);
                break;
            case StatImpactType.Reduction:
                vital.ReductionPerTimeForDuration(statType, impactValue, perTime, timeCount);
                break;
        }
    }
}
