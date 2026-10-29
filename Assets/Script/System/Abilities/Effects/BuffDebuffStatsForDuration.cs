using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : StatsEffectBase
{
    public float duration;

    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context)
    {
        effectReceiver.AdjustCurrentForDuration(statModifierGroup, duration, context);
    }
}
