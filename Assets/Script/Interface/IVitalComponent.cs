using System;

/// <summary>
/// CURRENT stat values of ONE character — player or enemy. Implemented by VitalStatsComponent and
/// EntityVitalStats (both via VitalStatsBase). From outside a character, get it with the GetComponent
/// family (e.g. from the <see cref="ICharacter"/> root with GetComponentInChildren).
/// </summary>
public interface IVitalComponent
{
    /// <summary>Raised after any current value changes (damage, heal, cost, buff, Reborn).</summary>
    event Action CurrentStatsChanged;
    float GetCurrentStatValue(VitalStatType vitalStatType);
    void Recovery(VitalStatType vitalStatType, float amount);
    void Reduction(VitalStatType vitalStatType, float amount);
}

public interface IVitalComponent<T> : IVitalComponent where T : CoreBase
{

}
