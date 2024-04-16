using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Core : MonoBehaviour,IDamageable
{
    [SerializeField] private NewPlayer player;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private AbilityHolder abilityHolder;

    public PlayerMovement Movement { get => movement;}
    public WeaponHolder WeaponHolder { get => weaponHolder; }
    public AbilityHolder AbilityHolder { get => abilityHolder; }
    public NewPlayer Player { get => player; }

    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (player.Data.currentHealth <= 0) return;
        player.Data.currentHealth -= amoutDamage;
        Debug.Log("Player TakeDamage");
        player.InputHandler.OnTakeDamage(attackPosition);

    }
    private void Awake()
    {
        player = GetComponentInParent<NewPlayer>();
        movement = GetComponentInChildren<PlayerMovement>();
        weaponHolder = GetComponentInChildren<WeaponHolder>();
        abilityHolder = GetComponentInChildren<AbilityHolder>();
    }
}
