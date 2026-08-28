using UnityEngine;

public class EntityNegativeReciver : EntityCoreComponent<EntityCore>, INegativeReceiver
{
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        Core.GetCoreComponent(out EntityVitalStats entityVitalStats);
        entityVitalStats.ReceiveReduction(StatType.HP, amoutDamage);
        Core.GetCoreComponent(out EntityInput input);
        input.OnTakeDamage(attackPosition);
    }
}