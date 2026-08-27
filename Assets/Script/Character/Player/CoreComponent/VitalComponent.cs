using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using VContainer;

//Multiple inheritance interface
public class VitalStatsComponent : CoreComponent<Core>
{
    Dictionary<StatType, float> currentStats = new();
    IPlayerStatService playerStatService;
    [Inject]
    public void Construct(IPlayerStatService playerStatService, ObjectPoolManager objectPoolManager)
    {
        this.playerStatService = playerStatService;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        currentStats = playerStatServiceGetFullStat();
    }


    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statModifierGroup.Apply(playerStatService.AddModifiersFromSource, this);
    }

    public void ReceiverRecovery(StatType statType, float amount)
    {
        if (playerStatService[statType] + amount >= playerStatService.GetStatValue(statType))
        {
            playerStatService[statType] = playerStatService.GetStatValue(statType);
        }
        else
        {
            playerStatService[statType] += amount;
        }
    }

    public void ReceiveReduction(StatType statType, float amount)
    {
        if (playerStatService[statType] - amount <= 0)
        {
            playerStatService[statType] = 0;
        }
        else
        {
            playerStatService[statType] -= amount;
        }
    }
}