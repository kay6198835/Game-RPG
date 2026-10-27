using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : AbilityEffectDefinition
{
    public StatModifierGroup statModifierGroup;
    public float duration;
    IEffectReceiver effectReceiver;
    public override void Apply(AbilityContext context)
    {
        if (!context.TryGetRecipientComponent(Recipient, out effectReceiver)) return;

        effectReceiver.AdjustCurrentForDuration(statModifierGroup, duration);
    }
}
