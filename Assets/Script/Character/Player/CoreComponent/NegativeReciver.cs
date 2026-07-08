using UnityEngine;

public class NegativeReciver : CoreComponent, INegativeReceiver
{
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        var data = core.Player.Data;
        if (data.currentHealth <= 0) return;
        data.currentHealth = Mathf.Max(0f, data.currentHealth - amoutDamage);
    }
}
