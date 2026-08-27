using UnityEngine;

public class StatHandler : CoreComponent<Core>, IPlayerStatService
{
    [SerializeField] private StatsSO statsSO;
    public override void Awake()
    {
        base.Awake();
        statsSO = core.player.Stats;
    }
    public int GetLevel()
    {
        return statsSO.Level;
    }
    public Dictionary<StatType, StatsViewDTO> GetFullViewStats()
    {
        return statsSO.FullStatView();
    }
    public StatsViewDTO GetViewStat(StatType statType)
    {
        return statsSO.GetViewStat(statType);
    }
    public Stat GetStat(StatType statType)
    {
        return statsSO.GetStat(statType);
    }
    public float GetStatValue(StatType statType)
    {
        return statsSO.GetStatValue(statType);
    }
    public int GetLevelUpStatsBonus()
    {
        return statsSO.GetStatUnusedBonus();
    }
    public void RemoveModifiersFromSource(object source)
    {
        statsSO.RemoveModifiersFromSource(source);
    }
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
    {
        statsSO.AddModifiersFromSource(source, modifiers);
    }

    public void AddPrimaryPoint(StatType statType, int amount)
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        statsSO.AddPrimaryPoint(statType, amount);
    }
    public Dictionary<StatType, float> GetFullStat()
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        statsSO.FullStatsValue();
    }
}