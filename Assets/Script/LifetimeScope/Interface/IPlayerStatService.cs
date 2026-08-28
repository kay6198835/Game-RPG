using System.Collections.Generic;

public interface IPlayerStatService
{
    int GetLevel();
    Dictionary<StatType, StatsViewDTO> GetFullViewStats();
    StatsViewDTO GetViewStat(StatType statType);
    Stat GetStat(StatType statType);
    Dictionary<StatType, float> GetFullStat();
    float GetStatValue(StatType statType);
    void AddPrimaryPoint(StatType statType, int amount);
    int GetLevelUpStatsBonus();
    void RemoveModifiersFromSource(object source);
    void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers);
}
