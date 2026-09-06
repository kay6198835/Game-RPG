using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ItemEffectDefinition : ScriptableObject
{
    public abstract void Apply(ResourceReceiver player);
}