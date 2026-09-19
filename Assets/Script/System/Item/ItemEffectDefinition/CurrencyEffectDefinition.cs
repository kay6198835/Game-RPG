using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Item SO/Effect/Currency")]
public class CurrencyEffectDefinition : ItemEffectDefinition
{
    [SerializeField] private int amount;

    public override void Apply(ResourceReceiver player)
    {
        // player.Wallet.Add(amount);
    }
}