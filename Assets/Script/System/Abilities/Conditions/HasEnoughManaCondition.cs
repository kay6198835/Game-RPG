using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Conditions/Has Enough Mana")]
public class HasEnoughManaCondition : AbilityConditionDefinition
{
    public float currentMana;
    public float costMana;

    public override bool IsMet(AbilityContext context)
    {
        currentMana = context.Services.Vital.GetCurrentStatValue(StatType.Mana);
        costMana = context.AbilityDefinition.GetCostValues(StatType.Mana);
        if (context.Services.Vital.GetCurrentStatValue(StatType.Mana)
         < context.AbilityDefinition.GetCostValues(StatType.Mana))
            return false;
        return true;
    }
}
