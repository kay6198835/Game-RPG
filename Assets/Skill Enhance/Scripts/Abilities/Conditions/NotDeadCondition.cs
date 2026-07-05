using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Conditions/Not Dead")]
public class NotDeadCondition : AbilityConditionDefinition
{
    public override bool IsMet(AbilityContext context)
    {
        if (context?.Caster?.Health == null) return false;
        return !context.Caster.Health.IsDead;
    }
}
