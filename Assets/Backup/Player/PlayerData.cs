using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newPLayerData",menuName ="Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxHealth;
    [SerializeField] public float currentHealth;
    public float MaxHealth { get => maxHealth; }

    [Header("Move State")]
    public float movementVelocities = 10f;


    private void Awake()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }
    public void Reborn()
    {
        currentHealth = maxHealth;
    }
}
