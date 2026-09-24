using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : AbilityEffectDefinition
{
    public StatModifierGroup statModifierGroup;
    public float duration;
    public EffectRecipient recipient = EffectRecipient.Caster;
    public override void Apply(AbilityContext context)
    {
        ICharacter character = context.ResolveRecipient(recipient);
        if (character == null) return;
        character.Vital.BuffDebuffForDuration(statModifierGroup, duration);
    }
}
