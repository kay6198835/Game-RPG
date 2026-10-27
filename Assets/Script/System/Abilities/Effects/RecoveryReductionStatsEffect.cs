using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Stats Effect")]
public class RecoveryReductionStatsEffect : StatsEffectBase
{
    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context)
    {
        effectReceiver.AdjustMaxToCurrentStats(statModifierGroup, true, context.Caster);
    }
}
