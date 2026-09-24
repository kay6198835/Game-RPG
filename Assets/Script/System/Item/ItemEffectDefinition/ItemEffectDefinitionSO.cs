using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ItemEffectDefinition : ScriptableObject
{
    /// <summary>Applies the item to any character through its contract — never a concrete type.</summary>
    public abstract void Apply(ICharacter target);
}
