using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AbilityHolder))]
public class WeaponMelee : Weapon
{
    [SerializeField]
    [Header("Melee Weapon")]
    private WeaponMeleeStats statsMelee;
    private int currentStateIndex = 0;
    private Vector2 centerAttackPosition;
    private AttackSO currrentSA;
    protected override void Awake()
    {
        base.Awake();
        if (stats.GetType() == typeof(WeaponMeleeStats))
        {
            statsMelee = (WeaponMeleeStats)stats;
            //stats = new WeaponMeleeStats();
            
        }
    }
    private void Start()
    {
        deplayTime = 0.5f;
        durationNextAttack = 0.9f;
    }

    public override void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(centerAttackPosition, currrentSA.attackRange, currrentSA.enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(currrentSA.attackDamege, gameObject);
            }
        }
    }
    public override AbilitySO SetAbility()
    {

        if (core.Player.InputHandler.Skill == PlayerInputHandler.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.Ability;
        }
        else if(core.Player.InputHandler.Skill == PlayerInputHandler.SkillType.Special)
        {
            currentAbilitySO = statsMelee.Special;
        }
        return currentAbilitySO;
    }
    public override bool CheckCanAttack(NewPlayer player)
    {
        if (base.CheckCanAttack(player))
        {
            if (currentStateIndex == statsMelee.AttackState.Count || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentStateIndex = 0;
            }
            currrentSA = statsMelee.AttackState[currentStateIndex];
            player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
            //Attack Position
            CenterAttackPosition(player);
            currentStateIndex++;
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }
    protected void CenterAttackPosition(NewPlayer player)
    {
        centerAttackPosition = (Vector2)player.transform.position + player.InputHandler.DirectionVector.normalized * currrentSA.attackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if(currrentSA != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currrentSA.attackRange);
        }
    }
}