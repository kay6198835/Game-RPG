using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBody : MonoBehaviour
{
    private Rigidbody2D _rb;
    private IObjecPoolService _pool;
    private ProjectileConfig _config;
    private IProjectilePayload _payload;
    private float _power;
    private float _despawnTime;
    private int _pierceLeft;
    private bool _active;

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
                       IProjectilePayload payload, float power)
    {
        _config = config;
        _payload = payload;
        _power = power;
        _pierceLeft = config.pierceCount;
        _despawnTime = Time.time + config.lifetime;
        _rb.velocity = direction.normalized * config.speed;
        _active = true;
    }

    private void Update()
    {
        if (_active && Time.time >= _despawnTime) Release();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active) return;

        int layer = other.gameObject.layer;
        if (IsInMask(_config.blockMask, layer))
        {
            Release();
            return;
        }
        if (!IsInMask(_config.targetMask, layer)) return;

        // `?.` skips Unity's fake-null check, so a destroyed MonoBehaviour payload must be caught explicitly.
        if (_payload is Object unityPayload && unityPayload == null)
        {
            Release();
            return;
        }
        _payload?.OnHit(other, transform.Position2D(), _power);

        if (_pierceLeft-- <= 0) Release();
    }

    // A pooled instance must never carry the previous shot's payload into its next life.
    private void OnDisable()
    {
        _active = false;
        _payload = null;
        if (_rb != null) _rb.velocity = Vector2.zero;
    }

    private void Release()
    {
        if (!_active) return;
        _active = false;
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