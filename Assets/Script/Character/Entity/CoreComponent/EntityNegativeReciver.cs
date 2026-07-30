using UnityEngine;

public class EntityNegativeReciver : EntityCoreComponent<EntityCore>, INegativeReceiver
{
    public void TakeDamage(int amountDamage, Vector2 attackPosition)
    {
        throw new System.NotImplementedException();
    }
}