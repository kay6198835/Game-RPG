using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : WeaponAttack
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletPref;

    [field: SerializeField] public WeaponRangeStats RWdata { get; private set; }

    void Update()
    {
        if(RWdata.timeBtwShots <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                shoot();
                RWdata.timeBtwShots = RWdata.StartTimeBtwShots;
            }
        }
        else
        {
            RWdata.timeBtwShots -= Time.deltaTime;
        }
    }

    void shoot()
    {
        GameObject bullet = Instantiate(bulletPref, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * RWdata.firerate, ForceMode2D.Impulse);
    }

    protected override void DoAttack(Weapon currentweaponMelee)
    {

    }
}
