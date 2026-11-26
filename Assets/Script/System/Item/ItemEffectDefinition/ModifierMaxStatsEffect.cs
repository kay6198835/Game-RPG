using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Effect/StatModifier/Modifier Max Stats")]
public class ModifierMaxStatsEffect : StatModifierEffect
{
    public override void Apply(ICharacter target)
    {
        base.Apply(target);
        if (effectReceiver != null)
        {
            effectReceiver.AdjustMaxStats(statModifierGroup);
        }
    }
}