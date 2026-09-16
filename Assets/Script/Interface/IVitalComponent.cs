public interface IVitalComponent
{
    float GetCurrentStatValue(StatType statType);
    void ApplyBuffDebuff(StatModifierGroup statModifierGroup);
    void Recovery(StatType statType, float amount);
    void Reduction(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
}