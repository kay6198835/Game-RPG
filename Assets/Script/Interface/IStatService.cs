using System.Collections.Generic;

/// <summary>
/// Max (profile) stat values of ONE character — player or enemy. Current values live in
/// <see cref="IVitalComponent"/>. Implemented by StatHandler and EntityStatsHandler. From outside a
/// character, get it with the GetComponent family; never through the DI container (it is per-instance).
/// </summary>
public interface IStatService
{
    int GetLevel();
    Dictionary<StatType, StatsViewDTO> GetFullViewStats();
    StatsViewDTO GetViewStat(StatType statType);
    Stat GetStat(StatType statType);
    Dictionary<StatType, float> GetFullStat();
    float GetStatValue(StatType statType);
    void RemoveModifiersFromSource(object source);
    void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers);
}
