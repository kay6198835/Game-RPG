using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public string AbilityName = "";
    public List<AbilityConditionDefinition> SubConditions;
    public abstract void Apply(AbilityContext context);
    public virtual bool Casting(AbilityContext context)
    {
        if (SubConditions.Count > 0)
        {
            foreach (var condition in SubConditions)
            {
                if (!condition.IsMet(context)) return false;
            }
        }
        return true;
    }

    public void OnValidate()
    {
        if (string.IsNullOrEmpty(AbilityName)) AbilityName = name;
    }
}
