using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponMelee : Weapon
{
    [Header("Stats")]
    [SerializeField] private WeaponMeleeStats stats;
    [SerializeField] private int currentState;

    public WeaponMeleeStats Stats { get => stats; }

    public override void Attack()
    {
        //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackState.attackRange / 2, attackState.enemyLayers);
        //foreach (Collider2D enemy in hitEnemies)
        //{
        //    if (enemy.GetComponent<Enemy>() != null)
        //    {
        //        enemy.GetComponent<Enemy>().TakeDamage(attackState.attackDamege, gameObject);
        //    }
        //}
    }
    
    public override bool CheckCanAttack(NewPlayer player)
    {
        lastClickTime = player.AttackState.StartAttackTime;
        
        if (lastClickTime + deplayTime > Time.time)
        {
            canAttack = false;
        }
        else
        {
            if (currentState == stats.attackState.Count || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentState = 0;
            }
            player.Anim.runtimeAnimatorController = stats.attackState[currentState].directionAttackAnimatorOV;
            currentState++;
            canAttack = true;
        }
        return canAttack;
    }
}