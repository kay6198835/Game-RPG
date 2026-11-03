using System;
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

/// <summary>
/// Inner view for core components of the SAME character, resolved with Core.TryGetCapability.
/// </summary>
public interface IStatService<TCore> : IStatService where TCore : CoreBase
{
    /// <summary>Subscribes to max-stat changes; survives the runtime clone being rebuilt.</summary>
    void ResignStatsChange(Action<StatType> action);
    void UnResignStatsChange(Action<StatType> action);
}
