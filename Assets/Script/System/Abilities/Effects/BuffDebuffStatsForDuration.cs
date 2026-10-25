using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : AbilityEffectDefinition
{
    public StatModifierGroup statModifierGroup;
    public float duration;
    IEffectReceiver effectReceiver;
    public EffectRecipient recipient = EffectRecipient.Caster;
    public override void Apply(AbilityContext context)
    {
        if (!context.TryGetRecipientComponent(recipient, out IVitalComponent vital)) return;
        effectReceiver.BuffDebuffForDuration(statModifierGroup, duration);
    }
}
