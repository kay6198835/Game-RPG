using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Stats Effect")]
public class RecoveryReductionStatsEffect : StatsEffectBase
{
    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                effectReceiver.Recovery(statType, impactValue);
                break;
            case StatImpactType.Reduction:
                effectReceiver.Reduction(statType, impactValue);
                break;
        }
    }
}
