using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class SpawnMono : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected float _damage;
    protected float _duration;
    protected IObjecPoolService _pool;
    protected Action<INegativeReceiver, Vector2> _callbackMethodTakeDamage;
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }
    public virtual void Launch(Vector2 direction, float speed, float lifetime,
                       float damage, IObjecPoolService pool, Action<INegativeReceiver, Vector2> callbackMethod)
    {
        _damage = damage;
        _duration = lifetime;
        _pool = pool;
        _callbackMethodTakeDamage = callbackMethod;

        _rb.velocity = direction * speed;
        DespawnOneself();
    }
    protected virtual void DespawnOneself()
    {
        StartCoroutine(DespawnOneselfAffterDuration());
    }

    protected virtual IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_duration);
        _pool.Release(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out INegativeReceiver receiver))
        {
            _pool.Release(gameObject);
            _callbackMethodTakeDamage?.Invoke(receiver, transform.position);
        }
    }
}