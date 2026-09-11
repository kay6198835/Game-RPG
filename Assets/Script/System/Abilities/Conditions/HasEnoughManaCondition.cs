using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Conditions/Has Enough Mana")]
public class HasEnoughManaCondition : AbilityConditionDefinition
{
    public override bool IsMet(AbilityContext context)
    {
        if (context?.Services?.Vital.GetCurrentStatValue(StatType.Mana)
         < context.AbilityDefinition.GetCostValues(StatType.Mana))
            return false;
        return true;
    }
}
