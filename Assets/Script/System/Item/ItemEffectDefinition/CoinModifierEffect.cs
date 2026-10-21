using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CoinModifierEffect : ItemEffectDefinitionSO
{
    [SerializeField] private int coinAmount;
    public override void Apply(ICharacter target)
    {
        var wallet = target.Transform.GetComponentInChildren<IWallet>();
        if (wallet != null)
        {
            wallet.AddCoins(coinAmount);
        }
    }
}
