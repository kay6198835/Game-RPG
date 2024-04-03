using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour,IDamageable
{
    [SerializeField] private NewPlayer player;

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Weapon weapon;

    public PlayerMovement Movement { get => movement;}
    public Weapon Weapon { get => weapon;}
    public NewPlayer Player { get => player; }

    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        //if(player.Data.MaxHealth<=0) return;
        //player.Data.currentHealth -=amoutDamage;
    }
    private void Awake()
    {
        player = GetComponentInParent<NewPlayer>();
        movement = GetComponentInChildren<PlayerMovement>();
        weapon = GetComponentInChildren<Weapon>();
    }
}
