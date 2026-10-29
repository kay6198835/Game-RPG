using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public abstract class StatModifierEffect : ItemEffectDefinitionSO
{
    [SerializeField] protected StatModifierGroup statModifierGroup;
    protected IEffectReceiver effectReceiver;

    public override void Apply(ICharacter target)
    {
        target.Core.TryGetCapability(out effectReceiver);
    }

    void OnValidate()
    {

    }
}