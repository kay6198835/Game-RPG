public interface IVitalComponent
{
    float GetCurrentStatValue(StatType statType);
    void ApplyBuffDebuff(StatModifierGroup statModifierGroup);
    void Recovery(StatType statType, float amount);
    void Reduction(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
    void RecoveryPerTimeForDuration(StatType statType, float amount, float perTime, float duration);
    void ReductionPerTimeForDuration(StatType statType, float amount, float perTime, float duration);
}