using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Conditions/Has Enough Mana")]
public class HasEnoughManaCondition : AbilityConditionDefinition
{
    public override bool IsMet(AbilityContext context)
    {
        if (context?.Caster?.Stats == null) return false;
        return context.Caster.Stats.CurrentMana >= context.AbilityDefinition.ManaCost;
    }
}
