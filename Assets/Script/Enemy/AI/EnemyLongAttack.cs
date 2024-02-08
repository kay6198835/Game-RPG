using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLongAttack : EnemyAttack
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Vector2 shootPosition;
    [SerializeField] private float powerShoot;
    protected override void Awake()
    {
        base.Awake();
        bulletPrefab = stateManager.EnemyCtrl.EnemySO.projectile;
        powerShoot = stateManager.EnemyCtrl.EnemySO.powerShoot;
    }
    private void Start()
    {
        bulletPrefab = stateManager.EnemyCtrl.EnemySO.projectile;
    }
    public override void Attack()
    {
        base.Attack();
        shootPosition =(Vector2) transform.position + attackPointVector;
        var angle = Mathf.Atan2(attackPointVector.y, attackPointVector.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        GameObject gameObject = Instantiate(bulletPrefab, shootPosition, rotation);
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(gameObject.transform.right*powerShoot, ForceMode2D.Impulse);
    }
}
