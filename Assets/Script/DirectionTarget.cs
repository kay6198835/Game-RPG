using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTarget : MonoBehaviour
{
    [SerializeField] protected Transform target;
    // Start is called before the first frame update
    public virtual void DirectionToTarget()
    {
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
