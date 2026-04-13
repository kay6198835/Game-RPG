using UnityEngine;

[RequireComponent(typeof(Health))]
public class SimpleDamageReceiver : MonoBehaviour, Damageable
{
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public void ReceiveDamage(float damage)
    {
        _health.TakeDamage(damage);
    }
}
