using UnityEngine;

public class ResourceReceiver : Interact
{
    VitalStatsComponent vitalStatsComponent;
    protected override void Awake()
    {
        base.Awake();
    }

    public void ReceverModifierGroup(StatModifierGroup statModifierGroup)
    {
        Core.GetCoreComponent<VitalStatsComponent>(out vitalStatsComponent);
        vitalStatsComponent.ApplyBuffDebuff(statModifierGroup);
    }

    public void ReceverRecovery(StatType statType, float amount)
    {

    }
}