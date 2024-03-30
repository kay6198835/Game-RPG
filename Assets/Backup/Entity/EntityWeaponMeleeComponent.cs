using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.WSA;

public class EntityWeaponMeleeComponent : EntityWeaponComponent
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
        if (entityCore.Entity.Input.Skill == EntityInput.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.Ability;
        }
        else if (entityCore.Entity.Input.Skill == EntityInput.SkillType.Special)
        {
            currentAbilitySO = statsMelee.Special;
        }
        return currentAbilitySO;
    }
    public override bool CheckCanAttack(Entity entity,float lastClickTime)
    {
        if (base.CheckCanAttack(entity,lastClickTime))
        {
            if (currentStateIndex == statsMelee.AttackState.Count || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentStateIndex = 0;
            }
            currrentSA = statsMelee.AttackState[currentStateIndex];
            entity.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
            //Attack Position
            CenterAttackPosition(entity);
            currentStateIndex++;
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }
    protected void CenterAttackPosition(Entity entity)
    {
        centerAttackPosition = (Vector2)entity.transform.position + entity.Input.DirectionLookVector.normalized * currrentSA.attackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if (currrentSA != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currrentSA.attackRange);
        }
    }
}
