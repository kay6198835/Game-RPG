using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public List<AbilityConditionDefinition> SubConditions;
    public List<AbilityEffectDefinition> SubEffects;
    public abstract void Apply(AbilityContext context);
    public virtual bool Casting(AbilityContext context)
    {
        if (SubConditions == null || SubConditions.Count == 0)
            return false;
        return true;
    }
}
