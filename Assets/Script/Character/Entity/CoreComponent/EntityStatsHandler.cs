using System.Collections.Generic;
using UnityEngine;

public class EntityStatsHandler : EntityCoreComponent<EntityCore>, IPlayerStatService
{
    [SerializeField] private BaseStatsSO statsSO;
    public override void Setup()
    {
        base.Setup();
        statsSO = core.Entity.Data.StatsSO;
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
        return statsSO.FullStatsValue();
    }
}