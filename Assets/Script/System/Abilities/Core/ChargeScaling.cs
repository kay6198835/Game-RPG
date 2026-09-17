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

    [Header("Continuous")]
    public float BonusDamageAtFullCharge = 40f;
    public AnimationCurve ChargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Tiered")]
    public ChargeTier[] Tiers;

    [Header("Shaping")]
    public float SpeedMultiplierAtFullCharge = 1f;

    [Header("Release Gate")]
    [Range(0f, 1f)] public float MinHoldRatioToRelease = 0f;

    [Header("Stat Scaling")]
    public StatType ScalingStat = StatType.PhysicalDamage;
    public float StatCoefficientAtNoCharge = 0f;
    public float StatCoefficientAtFullCharge = 0f;

    [Header("Cost")]
    public bool PayCostWhileCharging = false;

    public bool CanRelease(float holdRatio) => holdRatio >= MinHoldRatioToRelease;

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

    public void Apply(AbilityContext context, ref AbilityPower power)
    {
        float ratio = context == null ? 0f : Mathf.Clamp01(context.HoldRatio);
        float chargePower = GetChargePower(ratio);

        float statValue = 0f;
        if (context?.Services?.Stats != null)
            statValue = context.Services.Stats.GetStatValue(ScalingStat);

        if (Mode == ChargeMode.Tiered)
        {
            var tier = GetTier(ratio);
            float statBonus = statValue * Mathf.Lerp(StatCoefficientAtNoCharge, StatCoefficientAtFullCharge, ratio);

            power.Damage = (power.Damage + statBonus) * (tier == null ? 1f : tier.DamageMultiplier);
            power.SizeMultiplier *= tier == null ? 1f : tier.SizeMultiplier;
            power.TierIndex = GetTierIndex(ratio);
        }
        else
        {
            float statBonus = statValue * Mathf.Lerp(StatCoefficientAtNoCharge, StatCoefficientAtFullCharge, chargePower);

            power.Damage += BonusDamageAtFullCharge * chargePower + statBonus;
        }

        power.SpeedMultiplier *= Mathf.Lerp(1f, SpeedMultiplierAtFullCharge, chargePower);
    }
}
