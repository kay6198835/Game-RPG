using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Lunge Forward")]
public class LungeForwardEffect : AbilityEffectDefinition
{
    [Header("Distance")]
    public float BaseDistance = 1.5f;
    public float BonusDistanceAtMaxHold = 2f;

    [Header("Duration")]
    public float BaseDuration = 0.20f;
    public float MinDurationAtMaxHold = 0.08f;

    public override void Apply(AbilityContext context)
    {
        if (context?.Caster?.Motor == null) return;

        float distance = BaseDistance + BonusDistanceAtMaxHold * context.HoldRatio;
        float duration = Mathf.Lerp(BaseDuration, MinDurationAtMaxHold, context.HoldRatio);

        context.Caster.Motor.Lunge(context.Forward, distance, duration);

        Debug.Log(
            $"[{context.AbilityDefinition.DisplayName}] LungeForwardEffect => " +
            $"HoldRatio={context.HoldRatio:F2}, Distance={distance:F2}, Duration={duration:F2}");
    }
}
