using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Item SO/Effect/Recovery")]
public class RecoveryEffectDefinition : ItemEffectDefinition
{
    [SerializeField] private int amount;

    public override void Apply(ResourceReceiver resourceReceiver)
    {
        resourceReceiver.ReceverRecovery(StatType.HP,amount);
    }
}