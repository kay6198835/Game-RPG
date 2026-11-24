using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trap : SpawnEffectMono
{
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _damage;

    protected void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_targetMask != other.gameObject.layer) return;
        _context.Target = other;
        _callback.Invoke(_context);
    }
}