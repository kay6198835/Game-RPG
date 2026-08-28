using System.Collections.Generic;
using UnityEngine;

public class StatHandler : CoreComponent<Core>, IPlayerStatService
{
    [SerializeField] private StatsSO statsSO;
    // Lazy-resolved on first real use, not Awake: core.Player is only set inside Core.Awake(),
    // and Unity does not guarantee Core.Awake() runs before this component's Awake().
    private StatsSO StatsSO => statsSO ??= core.Player.Stats;
    public int GetLevel()
    {
        return StatsSO.Level;
    }
    public Dictionary<StatType, StatsViewDTO> GetFullViewStats()
    {
        return StatsSO.FullStatView();
    }
    public StatsViewDTO GetViewStat(StatType statType)
    {
        return StatsSO.GetViewStat(statType);
    }
    public Stat GetStat(StatType statType)
    {
        return StatsSO.GetStat(statType);
    }
    public float GetStatValue(StatType statType)
    {
        return StatsSO.GetStatValue(statType);
    }
    public int GetLevelUpStatsBonus()
    {
        return StatsSO.GetStatUnusedBonus();
    }
    public void RemoveModifiersFromSource(object source)
    {
        StatsSO.RemoveModifiersFromSource(source);
    }
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
    {
        StatsSO.AddModifiersFromSource(source, modifiers);
    }

    public void AddPrimaryPoint(StatType statType, int amount)
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        StatsSO.AddPrimaryPoint(statType, amount);
    }
    public Dictionary<StatType, float> GetFullStat()
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        return StatsSO.FullStatsValue();
    }
}