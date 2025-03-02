using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName ="newPLayerData",menuName ="Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] public float currentHealth;
    public float MaxHealth { get => maxHealth; }

    [Header("Move State")]
    public float movementVelocities;

    private void Reset()
    {
        Reborn();
    }
    public void Reborn()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        movementVelocities = 10f;
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
