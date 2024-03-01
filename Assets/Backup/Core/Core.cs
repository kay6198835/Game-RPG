using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Weapon weapon;

    public PlayerMovement Movement { get => movement;}
    public Weapon Weapon { get => weapon;}

    private void Awake()
    {
        movement = GetComponentInChildren<PlayerMovement>();
        weapon = GetComponentInChildren<Weapon>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LogicUpdate()
    {

    }
}
