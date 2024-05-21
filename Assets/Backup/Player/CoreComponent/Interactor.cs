using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : CoreCompoment
{
    [SerializeField] public float intertionPointRadius = 0.5f;
    [SerializeField] public LayerMask interactableMask;
    [SerializeField] public Collider2D[] colliders = new Collider2D[3];
    [SerializeField] public Collider2D nearestObject;
    [SerializeField] public IInteractable interactable;
    [SerializeField] public int numFound;

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Interactable");
    }
    public bool FindInteraction()
    {
        numFound = Physics2D.OverlapCircleNonAlloc(transform.position, intertionPointRadius, colliders, interactableMask);
        
        if (numFound > 0)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }
    Collider2D FindNearestObject()
    {
        float minDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {

            if(collider == null)
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


    public void Intertion()
    {
        nearestObject = FindNearestObject();
        interactable = nearestObject.GetComponent<IInteractable>();


        if (interactable != null && core.Player.InputHandler.IsInteractor)
        {
            interactable.Interact(this);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, intertionPointRadius);
    }
}
