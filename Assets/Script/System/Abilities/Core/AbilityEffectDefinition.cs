using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public string AbilityName = "";
    public List<AbilityConditionDefinition> SubConditions;
    public List<AbilityEffectDefinition> SubEffects;
    public List<StatCost> Costs;
    public abstract void Apply(AbilityContext context);
    public virtual bool TryCast(AbilityContext context)
    {
        if (SubConditions.Count > 0)
        {
            foreach (var condition in SubConditions)
            {
                if (!condition.IsMet(context)) return false;
            }
        }
        if (!CheckPayCostValid(context)) return false;
        return true;
    }

    protected virtual bool CheckPayCostValid(AbilityContext context)
    {
        if (Costs == null || Costs.Count == 0) return true;
        foreach (var cost in Costs)
        {
            if (cost.value > context.CasterCharacter.Vital.GetCurrentStatValue(cost.statType)) return false;
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
