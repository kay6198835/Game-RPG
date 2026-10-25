using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    [SerializeField] private float perTime;
    [Range(1, 10)]
    [SerializeField] private int timeCount;
    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatType statType, float impactValue)
    {
        switch (impactType)
        {
            case StatImpactType.Recovery:
                effectReceiver.RecoveryPerTimeForDuration(statType, impactValue, perTime, timeCount);
                break;
            case StatImpactType.Reduction:
                effectReceiver.ReductionPerTimeForDuration(statType, impactValue, perTime, timeCount);
                break;
        }
    }
}
