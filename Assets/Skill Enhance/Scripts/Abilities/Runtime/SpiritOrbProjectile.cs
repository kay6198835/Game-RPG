using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SpiritOrbProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _damagePerTick;
    private float _duration;
    private GameObject _summonPrefab;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    public void Launch(Vector2 direction, float speed, float lifetime,
                       float damagePerTick, float duration, GameObject summonPrefab)
    {
        _damagePerTick = damagePerTick;
        _duration = duration;
        _summonPrefab = summonPrefab;

        _rb.velocity = direction * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponent<Health>()
                     ?? other.GetComponentInParent<Health>();

        if (health == null || health.IsDead)
            return;

        // tránh gán nhiều DoT lên cùng một mục tiêu
        if (health.GetComponent<SpiritDoTBehaviour>() != null)
            return;

        var dot = health.gameObject.AddComponent<SpiritDoTBehaviour>();
        dot.Initialize(_damagePerTick, _duration, _summonPrefab);

        Destroy(gameObject);
    }
}
