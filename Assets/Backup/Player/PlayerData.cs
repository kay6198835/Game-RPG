using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName ="newPLayerData",menuName ="Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask weaponLayerMask;
    [SerializeField] private float maxHealth;
    [SerializeField] private float rangeFindWeapon;
    [SerializeField] public float currentHealth;
    public float MaxHealth { get => maxHealth; }
    public LayerMask EnemyLayerMask { get => enemyLayerMask; }
    public LayerMask WeaponLayerMask { get => weaponLayerMask;}

    [Header("Move State")]
    public float movementVelocities = 10f;
    private void Awake()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
    }
    public void Reborn()
    {
        currentHealth = maxHealth;
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
