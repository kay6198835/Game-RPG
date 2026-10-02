using System;
using System.Collections;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBody : MonoBehaviour
{
    private Rigidbody2D _rb;
    private IObjecPoolService _pool;
    private ProjectileConfig _config;
    private IProjectilePayload _payload;
    private float _lifetime;

    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        if (TryGetComponent(out Collider2D col)) col.isTrigger = true;
    }

    /// <summary>
    /// Starts flight. The payload decides what a hit does; power is handed back to it unchanged.
    /// </summary>
    public void Launch(Vector2 direction, ProjectileConfig config,
                       IProjectilePayload payload)
    {
        _config = config;
        _payload = payload;
        _lifetime = config.lifetime;
        _rb.velocity = direction.normalized * config.speed;
        if (_lifetime > 0)
        {
            StartCoroutine(DespawnOneselfAffterDuration());
        }
    }

    protected virtual IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_lifetime);
        _pool.Release(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        int layer = other.gameObject.layer;
        if (IsInMask(_config.blockMask, layer))
        {
            Release();
            return;
        }
        if (!IsInMask(_config.targetMask, layer)) return;


        _payload?.OnHit(other, transform.Position2D());

        Release();
    }

    // A pooled instance must never carry the previous shot's payload into its next life.
    private void OnDisable()
    {
        _payload = null;
        if (_rb != null) _rb.velocity = Vector2.zero;
    }

    private void Release()
    {
        _rb.velocity = Vector2.zero;
        _payload = null;

        // Only Pool.Spawn injects; a projectile placed in a scene by hand has no pool to return to.
        if (_pool == null)
        {
            Destroy(gameObject);
            return;
        }
        _pool.Release(gameObject);
    }

    private static bool IsInMask(LayerMask mask, int layer) => (mask.value & (1 << layer)) != 0;
}



[Serializable]
public struct ProjectileConfig
{
    [Range(1f, 60f)] public float speed;
    [Range(0.1f, 30f)] public float lifetime;
    public LayerMask targetMask;
    public LayerMask blockMask;
}