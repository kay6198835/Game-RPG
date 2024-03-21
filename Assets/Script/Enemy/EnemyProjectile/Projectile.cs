using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Rigidbody2D rigidbodyProjectile;
    [SerializeField] private Vector2 startposition;
    public float Speed { get => speed; set => speed = value; }
    private void Awake()
    {
        rigidbodyProjectile = GetComponent<Rigidbody2D>();
        collisionMask = LayerMask.GetMask("Enemy");
    }
    private void Start()
    {
        Invoke("DestroyProjectile", projectileLifeTime);
        startposition = this.transform.position;
    }
    private void Update()
    {
        CheckCollisions(speed * Time.deltaTime);
        //transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetXVelocity(Vector3 velocity)
    {
        rigidbodyProjectile.velocity = velocity * speed;
       
        //Debug.Log("Run");
    }
    void CheckCollisions(float moveDistance)
    {
        //Debug.Log("Attack");
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, transform.right, moveDistance, collisionMask);
        if (hit.collider != null)
        {
            OnHitObject(hit);
            //Debug.Log("Attacked");
        }
    }
    void OnHitObject(RaycastHit2D hit)
    {
        //hit.transform.gameObject.GetComponent<Player>().TakeDamage(damage, gameObject);
        IDamageable damageable = hit.collider.GetComponentInChildren<IDamageable>();
        //Debug.Log("Attacked");
        if (damageable != null)
        {
            //Debug.Log("Damage");
            damageable.TakeDamage(damage, startposition);
        }
            DestroyProjectile();
    }
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
