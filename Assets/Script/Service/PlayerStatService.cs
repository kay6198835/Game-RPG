using System.Collections.Generic;
using UnityEngine;

public class PlayerStatService : MonoBehaviour, IPlayerStatService
{
    private StatsSO statsSO;
    Dictionary<StatType, StatsViewDTO> GetFullViewStats()
    {
        return statsSO.FullStatView();
    }
    StatsViewDTO GetViewStat(StatType statType)
    {
        return statsSO.GetViewStat(statType);
    }
    Stat GetStat(StatType statType)
    {
        return statsSO.GetStat(statType);
    }
    float GetStatValue(StatType statType)
    {
        return statsSO.GetStatValue(statType);
    }
    void AddPrimaryPoint(StatType statType)
    {
        statsSO.AddPrimaryPoint(statType);
    }
    int GetLevelUpStatsBonus()
    {
        return statsSO.CalculateStatUnusedBonus();
    }
    void RemoveModifiersFromSource(object source)
    {
        statsSO.RemoveModifiersFromSource(source);
    }
    void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
    {
        statsSO.AddModifiersFromSource(source, modifiers);
    }
}