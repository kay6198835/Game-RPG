using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFindTarget : EntityCoreComponent
{
    [SerializeField] private Transform target;
    [SerializeField] private float range;
    [SerializeField] private LayerMask player, obstracles;
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
            player);
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

    public bool FindWall(Vector2 direction,float speed)
    {
        bool isFindWall;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed*0.5f, obstracles);
        if (hit.collider != null)
        {
            isFindWall=true;
        }
        else
        {
            isFindWall=false;
        }
        return isFindWall; 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
