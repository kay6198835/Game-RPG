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

    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statModifierGroup.Apply(playerStatService.AddModifiersFromSource, this);
    }

    public void ReceverRecovery(StatType statType, float amount)
    {
        
    }

    public void ReceverCurrency(float amount)
    {

    }
}