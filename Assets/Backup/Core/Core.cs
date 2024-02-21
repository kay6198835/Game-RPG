using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private PlayerMovement movement;

    public PlayerMovement Movement { get => movement;}

    private void Awake()
    {
        movement = GetComponentInChildren<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LogicUpdate()
    {

    }
}
