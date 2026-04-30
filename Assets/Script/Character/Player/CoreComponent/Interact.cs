using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : CoreCompoment
{
    [SerializeField] protected float intertionPointRadius = 0.5f;
    [SerializeField] protected LayerMask interactableMask;
    [SerializeField] protected Collider2D[] colliders = new Collider2D[3];
    [SerializeField] protected IInteractable interactable;
    [SerializeField] protected Collider2D nearestObject = new Collider2D();
    [SerializeField] protected int numFound;

    public float IntertionPointRadius { get => intertionPointRadius;}
    public LayerMask InteractableMask { get => interactableMask;}
    public Collider2D[] Colliders { get => colliders; }
    public Collider2D NearestObject { get => nearestObject; }
    public IInteractable Interactable { get => interactable; }
    public int NumFound { get => numFound; }

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Interactable");
    }
    public virtual bool FindInteraction()
    {
        numFound = Physics2D.OverlapCircleNonAlloc(transform.position, intertionPointRadius, colliders, interactableMask);
        if (numFound > 0)
        {
            nearestObject = FindNearestObject();
            core.Player.InputHandler.AngleCalculateExternality(nearestObject.transform.position - transform.position);
            return true;
        }
        else
        {
            return false;
        }
    }
    public virtual void Intertion()
    {
        interactable = nearestObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }
    protected virtual Collider2D FindNearestObject()
    {
        float minDistance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            if (collider == null)
            {
                break;
            }
            float distance = Vector2.Distance(collider.transform.position, core.Player.InputHandler.MouseVector);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = collider;
            }
        }
        return nearestObject;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, intertionPointRadius);
    }
}
