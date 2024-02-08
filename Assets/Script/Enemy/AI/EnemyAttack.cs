using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float attackRate;
    [SerializeField] protected Vector2 attackPointVector;
    [SerializeField] protected LayerMask playerMask;
    //[SerializeField] protected AttackState attackState;
    [SerializeField] protected StateManager stateManager;

    protected virtual void Awake()
    {
        stateManager= GetComponent<StateManager>();
    }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }
    public float AttackRate { get => attackRate; set => attackRate = value; }
    public LayerMask PlayerMask { get => playerMask; set => playerMask = value; }
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)this.transform.position, attackRange);
    }
    public virtual void Attack()
    {
        attackPointVector = (stateManager.Target.position - transform.position).normalized;
    }
}
