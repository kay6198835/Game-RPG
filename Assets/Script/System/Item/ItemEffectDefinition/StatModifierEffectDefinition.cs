using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Effect/StatModifier")]
public class StatModifierEffectDefinition: ItemEffectDefinitionSO
{
    [SerializeField] StatModifierGroup statModifierGroup;
    [SerializeField] StatImpactType impactType;
    public override void Apply(ICharacter<ICore> target)
    {
        var vital = target.Transform.GetComponentInChildren<IVitalComponent>();
        if (vital != null) vital.ApplyBuffDebuff(statModifierGroup);
    }
}