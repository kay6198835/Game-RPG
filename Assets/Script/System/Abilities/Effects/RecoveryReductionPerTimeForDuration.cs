using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    [SerializeField] private float perTime;
    [Range(1, 10)]
    [SerializeField] private int timeCount;
    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context)
    {
        effectReceiver.AdjustCurrentPerTimeForDuration(statModifierGroup, perTime, timeCount, context.Caster);
    }
}
