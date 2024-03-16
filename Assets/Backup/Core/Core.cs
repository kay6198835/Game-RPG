using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private NewPlayer playerCtr;

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Weapon weapon;

    public PlayerMovement Movement { get => movement;}
    public Weapon Weapon { get => weapon;}
    public NewPlayer PlayerCtr { get => playerCtr; }

    private void Awake()
    {
        playerCtr = GetComponentInParent<NewPlayer>();
        movement = GetComponentInChildren<PlayerMovement>();
        weapon = GetComponentInChildren<Weapon>();
    }
}
