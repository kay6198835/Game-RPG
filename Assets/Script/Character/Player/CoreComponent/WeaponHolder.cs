using UnityEngine;

/// <summary>
/// Player weapon holder: the shared attack lifecycle from WeaponHolderBase, plus picking weapons up
/// from the ground. It no longer derives from Interact (C# allows one base class), so the pickup
/// fields below keep Interact's serialized names to preserve the values on PlayerTest.prefab.
/// </summary>
public class WeaponHolder : WeaponHolderBase<Core>
{
    [SerializeField] protected float intertionPointRadius = 0.5f;
    [SerializeField] protected LayerMask interactableMask;
    [SerializeField] protected Collider2D[] colliders = new Collider2D[3];
    [SerializeField] protected Collider2D nearestObject;
    [SerializeField] protected int numFound;

    private PlayerInputHandler playerInputHandler;

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Weapon");
    }

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out playerInputHandler);
    }

    /// <summary>Looks for a weapon on the ground in range; faces it when found.</summary>
    public bool FindInteraction()
    {
        numFound = Physics2D.OverlapCircleNonAlloc(transform.position, intertionPointRadius, colliders, interactableMask);
        if (numFound <= 0) return false;
        nearestObject = FindNearestObject();
        playerInputHandler.AngleCalculateExternality(nearestObject.transform.position - transform.position);
        return true;
    }

    /// <summary>Drops the held weapon, or equips the nearest one found by FindInteraction().</summary>
    public void Intertion()
    {
        if (weapon != null)
        {
            weapon.UnEquid(this);
            return;
        }
        if (nearestObject != null && nearestObject.TryGetComponent(out Weapon groundWeapon))
        {
            groundWeapon.Equid(this);
        }
    }

    private Collider2D FindNearestObject()
    {
        Collider2D nearest = null;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < numFound; i++)
        {
            float distance = Vector2.Distance(colliders[i].transform.position, playerInputHandler.MouseVector);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = colliders[i];
            }
        }
        return nearest;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, intertionPointRadius);
    }
}
