using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]

public class Door : InteractiveObjects
{
    [SerializeField] public BoxCollider2D m_Collider;

    public string InteractionPromt => throw new System.NotImplementedException();
    protected override void Awake()
    {
        base.Awake();
        m_Collider = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OpenDoor()
    {
        gameObject.SetActive(false);
        //m_Collider.enabled = false;
        Debug.Log("Open Door");
    }

    public override bool Interact(Interactor interactor)
    {
        OpenDoor();
        return true;
    }
}
