using UnityEngine;

public abstract class AbilityConditionDefinition : ScriptableObject
{
    public abstract bool IsMet(AbilityContext context);
}
