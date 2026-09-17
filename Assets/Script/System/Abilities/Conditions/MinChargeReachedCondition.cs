using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Conditions/Min Charge Reached")]
public class MinChargeReachedCondition : AbilityConditionDefinition
{
    [Range(0f, 1f)] public float MinHoldRatio = 0.5f;
    public float MinHoldTime = 0f;

    public override bool IsMet(AbilityContext context)
    {
        if (context == null) return false;
        if (context.HoldRatio < MinHoldRatio) return false;
        if (MinHoldTime > 0f && context.HoldTime < MinHoldTime) return false;
        return true;
    }
}
