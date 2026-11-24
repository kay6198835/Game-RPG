using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Damage Effect")]
public class DamageEffect : AbilityEffectDefinition
{
    [SerializeField] protected float baseDamage = 30;
    public override void Apply(AbilityContext context)
    {
        if (context.Target != null
            && context.Target.TryGetComponent(out INegativeReceiver negativeReceiver))
        {
            //need fix to flexible stat type
            var finalDamage = baseDamage;
            negativeReceiver.TakeDamage(finalDamage, context.Origin);
        }
    }
}