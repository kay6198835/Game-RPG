using UnityEngine;

public class NegativeReciver : CoreComponent, INegativeReceiver
{
    public int currentHealth;
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amoutDamage;
        core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
        if (currentHealth <= 0)
            EventManager.Emit(EventID.ON_PLAYER_DEATH);
    }
}
