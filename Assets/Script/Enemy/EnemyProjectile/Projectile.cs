using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private LayerMask collisionMask;
    public float Speed { get => speed; set => speed = value; }
    private void Start()
    {
        Invoke("DestroyProjectile", projectileLifeTime);
    }
    private void Update()
    {
        CheckCollisions(speed * Time.deltaTime);
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    void CheckCollisions(float moveDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, transform.right, moveDistance, collisionMask);
        if (hit.collider != null)
        {
            OnHitObject(hit);
            //Debug.Log("On");
        }
    }
    void OnHitObject(RaycastHit2D hit)
    {
            hit.transform.gameObject.GetComponent<Player>().TakeDamage(damage, gameObject);
            DestroyProjectile();
    }
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
