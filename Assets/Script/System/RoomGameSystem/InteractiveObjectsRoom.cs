using UnityEngine;
using UIFlow;
[RequireComponent(typeof(CircleCollider2D))]
public class InteractiveObjectsRoom : InteractiveObjects
{
    [SerializeField] protected CircleCollider2D circleCollider;
    protected ICharacter character;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected InteractData interactData;

    public override bool Interact(Interact interactor)
    {
        character = interactor.GetComponentInParent<ICharacter>();
        return character != null;
    }

    protected override void Awake()
    {
        base.Awake();
        circleCollider = GetComponent<CircleCollider2D>();
        layerMask = LayerMask.GetMask("Player");
    }

    protected virtual void TriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == layerMask.value)
        {
            UIEvents.SwitchInteractiveNotification(interactData);
        }
    }

    protected virtual void TriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.layer == layerMask.value)
        {
            UIEvents.SwitchInteractiveNotification();
        }
    }
}