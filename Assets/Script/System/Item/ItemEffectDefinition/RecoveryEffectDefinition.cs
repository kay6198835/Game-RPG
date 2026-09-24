using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Item SO/Effect/Recovery")]
public class RecoveryEffectDefinition : ItemEffectDefinition
{
    [SerializeField] private int amount;

    public override void Apply(ICharacter target)
    {
        var vital = target.Transform.GetComponentInChildren<IVitalComponent>();
        if (vital != null) vital.Recovery(StatType.HP, amount);
    }
}