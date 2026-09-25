using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    public float duration;
    public float perTime;
    protected override void ApplyImpact(IVitalComponent vital, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                vital.RecoveryPerTimeForDuration(statType, impactValue, perTime, duration);
                break;
            case StatImpactType.Reduction:
                vital.ReductionPerTimeForDuration(statType, impactValue, perTime, duration);
                break;
        }
    }
}
