/// <summary>
/// CURRENT stat values of ONE character — player or enemy. Implemented by VitalStatsComponent and
/// EntityVitalStats (both via VitalStatsBase). From outside a character, get it with the GetComponent
/// family (e.g. from the <see cref="ICharacter"/> root with GetComponentInChildren).
/// </summary>
public interface IVitalComponent
{
    float GetCurrentStatValue(StatType statType);
    void ApplyBuffDebuff(StatModifierGroup statModifierGroup);
    void Recovery(StatType statType, float amount);
    void Reduction(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
    void RecoveryPerTimeForDuration(StatType statType, float amount, float perTime, float duration);
    void ReductionPerTimeForDuration(StatType statType, float amount, float perTime, float duration);
    /// <summary>Refills every current value to its max.</summary>
    void Reborn();
}
