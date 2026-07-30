using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityCoreComponent<EntityCore>
{
    private EntityInput entityInput;
    private EntityFindTarget entityFindTarget;
    public float minRange, maxRange;
    public float attackRate;
    public float nextAttackTime;
    public Vector2 centerAttackPosition;
    public LayerMask layerMask;
    public Collider2D[] hitBuffer;
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        Core.GetCoreComponent(out entityFindTarget);

        // minRange, maxRange set by entity data
        nextAttackTime = Time.time;
    }
    public void Attack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(centerAttackPosition, maxRange - minRange, hitBuffer, layerMask);
        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i].TryGetComponent(out INegativeReceiver receiver))
                receiver.TakeDamage(10, Core.Entity.transform.position);
        }
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