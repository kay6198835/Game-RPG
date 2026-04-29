using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        CurrentHealth -= damage;
        if (CurrentHealth < 0f) CurrentHealth = 0f;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {CurrentHealth}");
    }
}
