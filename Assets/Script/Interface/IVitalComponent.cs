/// <summary>
/// CURRENT stat values of ONE character — player or enemy. Implemented by VitalStatsComponent and
/// EntityVitalStats (both via VitalStatsBase). From outside a character, get it with the GetComponent
/// family (e.g. from the <see cref="ICharacter"/> root with GetComponentInChildren).
/// </summary>
public interface IVitalComponent
{
    /// <summary>Raised after any current value changes (damage, heal, cost, buff, Reborn).</summary>
    event System.Action CurrentStatsChanged;
    float GetCurrentStatValue(StatType statType);
    void Recovery(StatType statType, float amount);
    void Reduction(StatType statType, float amount);
}
