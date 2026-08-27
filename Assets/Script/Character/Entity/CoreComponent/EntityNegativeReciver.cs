using UnityEngine;

public class EntityNegativeReciver : EntityCoreComponent<EntityCore>, INegativeReceiver
{
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        Core.GetCoreComponent(out EntityStatsHandler entityStatsHandler);
        entityStatsHandler.ReceiveReduction(StatType.HP, amount);
        Core.GetCoreComponent(out EntityInput input);
        input.OnTakeDamage(attackPosition);
    }
}