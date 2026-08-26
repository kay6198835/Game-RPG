using UnityEngine;

public class EntityNegativeReciver : EntityCoreComponent<EntityCore>, INegativeReceiver
{
    public int currentHealth;
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amoutDamage;
        Core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
        if (currentHealth <= 0)
            EventManager.Emit(EventID.ON_ENEMY_DEATH);
    }
}