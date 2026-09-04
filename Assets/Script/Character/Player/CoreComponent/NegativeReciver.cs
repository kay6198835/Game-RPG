using UnityEngine;

public class NegativeReciver : CoreComponent<Core>, INegativeReceiver
{
    public void TakeDamage(float amoutDamage, Vector2 attackPosition)
    {
        Core.GetCoreComponent(out VitalStatsComponent vitalStatsComponent);
        vitalStatsComponent.ReceiveReduction(StatType.HP, amoutDamage);
        Core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
    }
}
