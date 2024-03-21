using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFindTarget : EntityCoreComponent
{
    [SerializeField] private Transform target;
    [SerializeField] private float range;
    //[SerializeField] private bool isFindTarget;

    public Transform Target { get => target;}

    protected override void Awake()
    {
        base.Awake();
    }
    public Transform FindTargetMethod(float range)
    {
        this.range = range;
        Collider2D collider = Physics2D.OverlapCircle(
            this.transform.position,
            range,
            entityCore.Entity.Data.LayerMask);
        if(collider != null)
        {
            target = collider.transform;
        }
        else
        {
            target = null;
        }
        return target;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
