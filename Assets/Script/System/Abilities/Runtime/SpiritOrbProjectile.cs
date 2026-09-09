using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SpiritOrbProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _damagePerTick;
    private float _duration;
    private GameObject _summonPrefab;
    private IObjecPoolService _pool;
    private Action _callbackMethod;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    public void Launch(Vector2 direction, float speed, float lifetime,
                       float damagePerTick, float duration, GameObject summonPrefab, IObjecPoolService pool, Action callbackMethod)
    {
        _damagePerTick = damagePerTick;
        _duration = duration;
        _summonPrefab = summonPrefab;
        _pool = pool;
        _callbackMethod = callbackMethod;

        _rb.velocity = direction * speed;
        DespawnOneself();
    }

    void DespawnOneself()
    {
        StartCoroutine(DespawnOneselfAffterDuration());
    }

    IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_duration);
        _pool.Release(gameobject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other[i].TryGetComponent(out INegativeReceiver receiver))
        {
            _pool.Release(gameobject);
            _callbackMethod?.Invoke(receiver, transform.position);
        }
    }
}
