using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public abstract void Apply(AbilityContext context);
}
