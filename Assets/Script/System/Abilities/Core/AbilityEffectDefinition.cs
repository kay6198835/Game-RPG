using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityEffectDefinition : ScriptableObject
{
    public string AbilityName = "";
    public List<AbilityConditionDefinition> SubConditions;
    public List<AbilitySubEffectDefinition> SubEffects;

    public abstract void Apply(AbilityContext context);

    public virtual bool Casting(AbilityContext context)
    {
        if (SubConditions != null)
        {
            foreach (var condition in SubConditions)
            {
                if (condition == null) continue;
                if (!condition.IsMet(context)) return false;
            }
        }

        if (SubEffects != null)
        {
            foreach (var subEffect in SubEffects)
            {
                if (subEffect == null) continue;
                if (!subEffect.AllowCostTick(context)) return false;
            }
        }

        return true;
    }

    protected AbilityPower ResolvePower(AbilityContext context, float baseDamage)
    {
        var power = AbilityPower.From(baseDamage);

        if (SubEffects == null) return power;

        foreach (var subEffect in SubEffects)
        {
            if (subEffect == null) continue;
            subEffect.ModifyPower(context, ref power);
        }

        return power;
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
