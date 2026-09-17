using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/SubEffects/Charge Damage")]
public class ChargeDamageSubEffect : AbilitySubEffectDefinition
{
    [SerializeField] private ChargeScaling charge = new();

    public ChargeScaling Charge => charge;

    public override void ModifyPower(AbilityContext context, ref AbilityPower power)
    {
        if (context == null) return;
        if (!charge.CanRelease(context.HoldRatio)) return;

        charge.Apply(context, ref power);
    }

    public override bool AllowCostTick(AbilityContext context) => charge.PayCostWhileCharging;
}
