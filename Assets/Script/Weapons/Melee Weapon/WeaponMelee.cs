using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AbilityHolder))]
public class WeaponMelee : Weapon
{
    [Header("Stats")]
    [SerializeField] private WeaponMeleeStats stats;
    //[SerializeField] private AbilityHolder abilityHolder;
    [SerializeField] private int currentStateIndex = 0;
    [SerializeField] private Vector2 centerAttackPosition;
    [SerializeField] private AttackSO currrentSA;
    protected override void Awake()
    {
        base.Awake();
        //abilityHolder = GetComponent<AbilityHolder>();
    }
    private void Start()
    {
        //currrentSA = stats.attackState[currentStateIndex];
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
        Debug.Log("Attack");
    }
    public override AbilitySO SetAbility()
    {
        if (core.PlayerCtr.InputHandler.Skill)
        {
            currentAbilitySO = stats.Skill;
        }
        else
        {
            currentAbilitySO = stats.SpecialAbility;
        }
        return currentAbilitySO;
    }
    //public override AbilitySO SetAblitty()
    //{
    //    return currentAbilitySO;
    //}
    public override bool CheckCanAttack(NewPlayer player)
    {
        lastClickTime = player.AttackState.StartAttackTime;

        if (lastClickTime + deplayTime > Time.time)
        {
            canAttack = false;
        }
        else
        {
            if (currentStateIndex == stats.AttackState.Count || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentStateIndex = 0;
            }
            currrentSA = stats.AttackState[currentStateIndex];
            player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
            //Attack Position
            CenterAttackPosition(player);
            currentStateIndex++;
            canAttack = true;
        }
        return canAttack;
    }
    private void CenterAttackPosition(NewPlayer player)
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