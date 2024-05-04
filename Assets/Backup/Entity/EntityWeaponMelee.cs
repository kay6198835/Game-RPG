using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.WSA;

public class EntityWeaponMelee : EntityWeapon
{
    [Header("Melee Weapon")]
    [SerializeField] private WeaponMeleeStats statsMelee;
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
        else
        {
            statsMelee = null;
        }
    }
    private void Start()
    {
        deplayTime = 0.5f;
        durationNextAttack = 0.9f;
    }
    public override void Attack()
    {
        Collider2D hitEPlayer = Physics2D.OverlapCircle(centerAttackPosition, currrentSA.attackRange, statsMelee.LayerMask);
        if (hitEPlayer != null)
        {
            IDamageable damageable = hitEPlayer.GetComponentInChildren<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(currrentSA.attackDamege, transform.position);
            }
        }

    }
    public override ActivateSkill SetAbility()
    {
        if (holder.EntityCore.Entity.Input.Skill == EntityInput.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.Ability;
        }
        else if (holder.EntityCore.Entity.Input.Skill == EntityInput.SkillType.Special)
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
