public struct AbilityPower
{
    public float Damage;
    public float SizeMultiplier;
    public float SpeedMultiplier;
    public int TierIndex;

    public static AbilityPower From(float baseDamage)
    {
        return new AbilityPower
        {
            Damage = baseDamage,
            SizeMultiplier = 1f,
            SpeedMultiplier = 1f,
            TierIndex = -1
        };
    }
}
