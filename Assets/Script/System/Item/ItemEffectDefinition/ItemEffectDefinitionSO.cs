using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ItemEffectDefinition : ScriptableObject
{
    /// <summary>Applies the item to any character. Each effect gets the component interface it needs from
    /// <c>target.Transform</c> itself — never a concrete Player/Entity type.</summary>
    public abstract void Apply(ICharacter target);
}
