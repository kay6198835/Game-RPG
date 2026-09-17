using UnityEngine;

public enum ChargeMode
{
    Continuous,
    Tiered
}

[System.Serializable]
public class ChargeTier
{
    public string Name = "Tier";
    [Range(0f, 1f)] public float HoldRatioThreshold = 0f;
    public float DamageMultiplier = 1f;
    public float SizeMultiplier = 1f;
}

[System.Serializable]
public class ChargeScaling
{
    [Header("Mode")]
    public ChargeMode Mode = ChargeMode.Continuous;

    [Header("Damage")]
    public float BaseDamage = 10f;
    public float BonusDamageAtFullCharge = 40f;
    public AnimationCurve ChargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Tiers (Mode = Tiered)")]
    public ChargeTier[] Tiers;

    [Header("Release Gate")]
    [Range(0f, 1f)] public float MinHoldRatioToRelease = 0f;

    [Header("Stat Scaling")]
    public StatType ScalingStat = StatType.PhysicalDamage;
    public float StatCoefficientAtNoCharge = 1f;
    public float StatCoefficientAtFullCharge = 1f;

    [Header("Cost")]
    public bool PayCostWhileCharging = false;

    public bool CanRelease(float holdRatio) => holdRatio >= MinHoldRatioToRelease;

    // AbilityInstance.Casting() pays the ability cost again for every effect that returns true,
    // so a charge attack must opt out unless it is deliberately a channel that drains per tick.
    public bool ShouldPayChargeTick(float holdRatio) => PayCostWhileCharging && CanRelease(holdRatio);

    public float GetChargePower(float holdRatio)
    {
        float ratio = Mathf.Clamp01(holdRatio);
        if (Mode == ChargeMode.Tiered)
        {
            int index = GetTierIndex(ratio);
            if (index < 0) return ratio;
            if (Tiers.Length == 1) return 1f;
            return (float)index / (Tiers.Length - 1);
        }
        return ChargeCurve == null ? ratio : ChargeCurve.Evaluate(ratio);
    }

    public int GetTierIndex(float holdRatio)
    {
        if (Tiers == null || Tiers.Length == 0) return -1;

        float ratio = Mathf.Clamp01(holdRatio);
        int index = -1;
        for (int i = 0; i < Tiers.Length; i++)
        {
            if (Tiers[i] == null) continue;
            if (ratio >= Tiers[i].HoldRatioThreshold) index = i;
        }
        return index;
    }

    public ChargeTier GetTier(float holdRatio)
    {
        int index = GetTierIndex(holdRatio);
        return index < 0 ? null : Tiers[index];
    }

    public float GetSizeMultiplier(float holdRatio)
    {
        if (Mode != ChargeMode.Tiered) return 1f;
        var tier = GetTier(holdRatio);
        return tier == null ? 1f : tier.SizeMultiplier;
    }

    public float EvaluateDamage(AbilityContext context)
    {
        float holdRatio = context == null ? 0f : Mathf.Clamp01(context.HoldRatio);
        float statValue = 0f;
        if (context?.Services?.Stats != null)
        {
            statValue = context.Services.Stats.GetStatValue(ScalingStat);
        }
        return EvaluateDamage(holdRatio, statValue);
    }

    public float EvaluateDamage(float holdRatio, float statValue)
    {
        float ratio = Mathf.Clamp01(holdRatio);

        if (Mode == ChargeMode.Tiered)
        {
            var tier = GetTier(ratio);
            float tierMultiplier = tier == null ? 1f : tier.DamageMultiplier;
            float tierCoefficient = Mathf.Lerp(StatCoefficientAtNoCharge, StatCoefficientAtFullCharge, ratio);
            return (BaseDamage + statValue * tierCoefficient) * tierMultiplier;
        }

        float power = ChargeCurve == null ? ratio : ChargeCurve.Evaluate(ratio);
        float coefficient = Mathf.Lerp(StatCoefficientAtNoCharge, StatCoefficientAtFullCharge, power);
        return BaseDamage + BonusDamageAtFullCharge * power + statValue * coefficient;
    }
}
