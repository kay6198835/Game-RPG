using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    [SerializeField] protected float lastClickedTime = 0;
    [SerializeField] protected float delayAttack;
    //[SerializeField] protected Player player;
    [SerializeField] protected Animator animator;
    //[SerializeField] protected WeaponMelee weaponMelee;

    public void Attack(Weapon currentweaponMelee)
    {

        if (lastClickedTime + delayAttack <= Time.time )
        {
            DoAttack(currentweaponMelee);
            lastClickedTime = Time.time ;
        }
    }
    protected abstract void DoAttack(Weapon currentweaponMelee);
}
