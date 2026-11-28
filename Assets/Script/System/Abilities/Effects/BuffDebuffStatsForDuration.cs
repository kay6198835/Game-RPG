using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : StatsEffectBase
{
    public float duration;

    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context)
    {
        // B6: this asset is half of the status key, so a re-hit by the same skill only refreshes the time.
        effectReceiver.ApplyStatus(new StatusRequest
        {
            Effect = this,
            StatusType = statusType,
            MaxModifiers = statModifierGroup,
            Duration = duration,
            Context = context,
        });
    }
}
