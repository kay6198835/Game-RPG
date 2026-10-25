using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CoinModifierEffect : ItemEffectDefinitionSO
{
    [SerializeField] private int coinAmount;
    IWallet wallet;
    public override void Apply(ICharacter target)
    {
        target.Core.TryGetCapability(out wallet);
        if (wallet != null)
        {
            wallet.AddCoins(coinAmount);
        }
    }
}
