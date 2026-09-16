using UnityEngine;

public class ResourceReceiver : Interact, INegativeReceiver, IResourceReceiver
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
        vitalStatsComponent.Reduction(statType, amount);
    }
    public void Reduction(StatType statType, float amount)
    {
        vitalStatsComponent.Recovery(statType, amount);
    }
    public void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration)
    {
        vitalStatsComponent.BuffDebuffForDuration(statModifierGroup, duration);
    }
    public void TakeDamage(float amoutDamage, Vector2 attackPosition)
    {
        Core.GetCoreComponent(out VitalStatsComponent vitalStatsComponent);
        vitalStatsComponent.Reduction(StatType.HP, amoutDamage);
        Core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
    }
}