
using UIFlow;
using Unity.VisualScripting;
using UnityEngine;

public class BonfireController : InteractiveObjectsRoom
{
    protected override void Awake()
    {
        base.Awake();
        interactData.textMessage = "Press " + interactData.keyCode.ToString() + " to open stats";

    }
    public override bool Interact(Interact interactor)
    {
        if (base.Interact(interactor))
        {
            UIEvents.RequestStats();
            return true;
        }
        return false;
    }
}