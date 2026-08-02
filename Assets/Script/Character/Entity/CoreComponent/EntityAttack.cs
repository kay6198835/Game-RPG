using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class EntityAttack : EntityCoreComponent<EntityCore>
{
    private EntityInput entityInput;
    private EntityFindTarget entityFindTarget;
    public float minRange, maxRange;
    public float attackRate;
    public float nextAttackTime;
    public Vector2 centerAttackPosition;
    public LayerMask layerMask;
    private Collider2D[] hitColliders;
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        Core.GetCoreComponent(out entityFindTarget);

        // minRange, maxRange set by entity data
        nextAttackTime = Time.time;
        //collider = GetComponent<BoxCollider2D>();
        hitColliders = new Collider2D[10];
    }
    public void Attack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(Core.Entity.transform.position , maxRange, hitColliders, layerMask);
        for (int i = 0; i < count; i++)
        {
            if (hitColliders[i].TryGetComponent(out INegativeReceiver receiver))
            {
                receiver.TakeDamage(10, Core.Entity.transform.position);
                Debug.Log("Hit " + hitColliders[i].name);
            }
        }
    }

    public void Setting()
    {

    }

    public void Exit()
    {

    }

    public bool IsInRangeAttack()
    {
        return entityFindTarget.DistanceToPlayer() <= maxRange && entityFindTarget.DistanceToPlayer() >= minRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (Core != null)
        {
            Gizmos.DrawWireSphere(Core.Entity.transform.position, minRange);
            Gizmos.DrawWireSphere(Core.Entity.transform.position, maxRange);
        }
    }

    public bool CallAttack()
    {
        if (nextAttackTime <= Time.time && IsInRangeAttack())
        {
            return true;
        }
        return false;
    }
}