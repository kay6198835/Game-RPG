using System.Collections.Generic;
using UnityEngine;

public abstract class StatsEffectBase : AbilityEffectDefinition
{
    [field: SerializeField] protected StatModifierGroup statModifier;
    [field: SerializeField] protected StatImpactType impactType;
    

    public override void Apply(AbilityContext context)
    {
        // Same path for a player or an enemy: only IVitalComponent is touched.
        if (!context.TryGetRecipientComponent(Recipient, out IEffectReceiver effectReceiver)) return;
        object source = Recipient == EffectRecipient.Target ? (object)context.Target : context.Caster;
        ApplyImpact(effectReceiver, statModifier, context);
    }

    protected abstract void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context);
}
