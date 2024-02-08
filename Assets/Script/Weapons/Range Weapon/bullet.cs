using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [field: SerializeField] public BulletDataSO BulletSO { get; private set; }

    private void Start()
    {
        Invoke("DestroyBullet", BulletSO.lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BlockObject"))
            DestroyBullet();

        if (BulletSO.bullettype == BulletType.PlayerBullet)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(BulletSO.dmg, gameObject);
                DestroyBullet();
            }
        }
        else if (BulletSO.bullettype == BulletType.EnemyBullet)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(BulletSO.dmg, gameObject);
                DestroyBullet();
            }
        }
        //DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }

}
