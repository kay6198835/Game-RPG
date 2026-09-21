using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public string AbilityName = "";
    public List<AbilityConditionDefinition> SubConditions;
    public List<AbilityEffectDefinition> SubEffects;
    public List<StatCost> Costs;
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

    protected virtual bool CheckPayCostValid(AbilityContext context)
    {
        if (Costs.Count == 0 || Costs == null) return true;
        foreach (var cost in Costs)
        {
            if (cost.value > context.Services.Vital.GetCurrentStatValue(cost.statType)) return false;
        }
        return true;
    }

    public void OnValidate()
    {
        if (string.IsNullOrEmpty(AbilityName)) AbilityName = name;
    }
}

public enum StatImpactType
{
    Recovery,
    Reduction
}
