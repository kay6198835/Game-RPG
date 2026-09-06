using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class bullet : MonoBehaviour, IPoolable
{
    [field: SerializeField] public BulletDataSO BulletSO { get; private set; }
    [SerializeField] private LayerMask blockMask;

    private Rigidbody2D body;
    private PoolMember poolMember;
    private float despawnTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        OnSpawn();
    }

    public void OnSpawn()
    {
        despawnTime = Time.time + BulletSO.lifetime;
        body.velocity = transform.right * BulletSO.speed;
    }

    public void OnDespawn()
    {
        body.velocity = Vector2.zero;
    }

    private void Update()
    {
        if (Time.time >= despawnTime)
        {
            Despawn();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (IsInMask(blockMask, other.layer))
        {
            Despawn();
            return;
        }

        if (!IsInMask(BulletSO.targetMask, other.layer)) return;

        INegativeReceiver receiver = other.GetComponentInChildren<INegativeReceiver>();
        if (receiver != null)
        {
            receiver.TakeDamage(BulletSO.dmg, transform.position);
        }
        Despawn();
    }

    private static bool IsInMask(LayerMask mask, int layer) => (mask.value & (1 << layer)) != 0;

    private void Despawn()
    {
        OnDespawn();

        // Pool attaches PoolMember on first spawn; a bullet placed in a scene by hand has none.
        if (poolMember == null && !TryGetComponent(out poolMember))
        {
            Destroy(gameObject);
            return;
        }
        poolMember.GetPool().Release(gameObject);
    }
}
