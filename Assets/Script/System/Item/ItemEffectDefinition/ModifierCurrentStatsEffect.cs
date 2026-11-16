using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Effect/StatModifier/ Adjust Current Stats")]
public class ModifierCurrentStatsEffect : StatModifierEffect
{
    public override void Apply(ICharacter target)
    {
        base.Apply(target);
        if (effectReceiver != null)
        {
            effectReceiver.AdjustCurrentStats(statModifierGroup);
        }
    }
}

