using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Stats Effect")]
public class RecoveryReductionStatsEffect : StatsEffectBase
{
    protected override void ApplyImpact(IVitalComponent vital, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                vital.Recovery(statType, impactValue);
                break;
            case StatImpactType.Reduction:
                vital.Reduction(statType, impactValue);
                break;
        }
    }
}
