using UnityEngine;

public class CheckIsInRange : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] float range ;
    [SerializeField] Transform target;
    [SerializeField] bool isInRange;
    [SerializeField] private LayerMask targetMask;

    public float Range { get => range; set => range = value; }
    public Transform Target { get => target; set => target = value; }
    public bool IsInRange { get => isInRange; set => isInRange = value; }
    public LayerMask TargetMask { get => targetMask; set => targetMask = value; }
    public void Check()
    {
        Collider2D targetsInViewRadius = Physics2D.OverlapCircle(transform.position, range, targetMask);
        if (targetsInViewRadius != null )
        {
            isInRange = true;
            target = targetsInViewRadius.transform;
        }
        else
        {
            isInRange = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)this.transform.position, range);
    }
}