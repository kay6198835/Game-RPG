using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObjects : MonoBehaviour,IInteractable
{
    protected string promt;
    public string InteractionPromt => promt;
    protected virtual void Awake()
    {
        //gameObject.layer = LayerMask.GetMask("Interactable");
    }

    public abstract bool Interact(Interact interactor);

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
