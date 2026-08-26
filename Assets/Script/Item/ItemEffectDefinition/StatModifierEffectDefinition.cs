using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu]
public class StatModifierEffectDefinition: ItemEffectDefinition
{
    [SerializeField] StatModifierGroup statModifierGroup;
    public override void Apply(ResourceReceiver resourceReceiver)
    {
        resourceReceiver.ReceverModifierGroup(statModifierGroup);
    }
}