using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected int damage;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] protected Rigidbody2D rigidbodyProjectile;
    [SerializeField] protected Vector2 startposition;
    public float Speed { get => speed; set => speed = value; }
    protected virtual  void Awake()
    {
        rigidbodyProjectile = GetComponent<Rigidbody2D>();
        collisionMask = LayerMask.GetMask("Enemy");
    }
    protected virtual  void Start()
    {
        Invoke("DestroyProjectile", projectileLifeTime);
        startposition = this.transform.position;
    }
    protected virtual  void Update()
    {
        CheckCollisions(speed * Time.deltaTime);
    }
    public virtual  void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public virtual  void SetVelocity(Vector3 velocity)
    {
        rigidbodyProjectile.velocity = velocity * speed;
    }
    protected virtual  void CheckCollisions(float moveDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, transform.right, moveDistance, collisionMask);
        if (hit.collider != null)
        {
            OnHitObject(hit);
        }
    }
    protected virtual  void OnHitObject(RaycastHit2D hit)
    {
        IDamageable damageable = hit.collider.GetComponentInChildren<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, startposition);
        }
        DestroyProjectile();
    }
    protected virtual  void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
