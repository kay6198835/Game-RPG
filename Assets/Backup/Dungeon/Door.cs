using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]

public class Door : InteractiveObjects
{
    [SerializeField] public BoxCollider2D m_Collider;
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
        Debug.Log("Open Door");
    }

    public override bool Interact(Interact interactor)
    {
        OpenDoor();
        return true;
    }
}
