public interface IPlayerStatService
{
    Dictionary<StatType, StatsViewDTO> GetFullViewStats();
    StatsViewDTO GetViewStat(StatType statType);
    Stat GetStat(StatType statType);
    float GetStatValue(StatType statType);
    void AddPrimaryPoint(StatType statType);
    int GetLevelUpStatsBonus();
    void RemoveModifiersFromSource(object source);
    void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers);
}