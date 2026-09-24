using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Effect/StatModifier")]
public class StatModifierEffectDefinition: ItemEffectDefinition
{
    [SerializeField] StatModifierGroup statModifierGroup;
    public override void Apply(ICharacter target)
    {
        target.Vital.ApplyBuffDebuff(statModifierGroup);
    }
}