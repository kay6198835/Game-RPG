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
        if (entityInput.TargetTransform.gameObject.TryGetComponen(out INegativeReceiver negativeReceiver))
        {
            negativeReceiver.TakeDamage(10, Core.Entity.transform.position);
        }
    }

    public bool IsInRangeAttack()
    {
        return entityFindTarget.DistanceToPlayer() <= maxRange && DistanceToPlayer() >= minRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (currrentSA != null)
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