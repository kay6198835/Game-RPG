using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Stats")]
public class EntityStatsSO : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseVelocities;
    [SerializeField] private float baseAmor;
    [Header("Modifiers  Stats")]
    [SerializeField] private float modifiersHealth;
    [SerializeField] private float modifiersVelocities;
    [SerializeField] private float modifiersAmor;
    [Header("Amount Stats")]
    [SerializeField] private float health;
    [SerializeField] private float velocities;
    [SerializeField] private float amor;
    public float Health { get => health; }
    public float Velocities { get => velocities; }
    public float Amor { get => amor; }
    public float ModifiersHealth { get => modifiersHealth; 
        set 
        {
            if (modifiersHealth!=value)
            {
                modifiersHealth = value;
                UpdateStats(ref health, baseHealth, modifiersHealth);
            }
        }
    }
    public float ModifiersVelocities
    {
        get => modifiersVelocities;
        set
        {
            if (modifiersVelocities != value)
            {
                modifiersVelocities = value;
                UpdateStats(ref velocities, baseVelocities, modifiersVelocities);
            }
        }
    }
    public float ModifiersAmor
    {
        get => modifiersAmor;
        set
        {
            if (modifiersAmor != value)
            {
                modifiersAmor = value;
                UpdateStats(ref amor, baseAmor, modifiersAmor);
            }
        }
    }

    public void ResetStats()
    {
        modifiersHealth = 0f;
        modifiersVelocities = 0f;
        modifiersAmor = 0f;
        UpdateStats(ref health, baseHealth, modifiersHealth);
        UpdateStats(ref velocities, baseVelocities, modifiersVelocities);
        UpdateStats(ref amor, baseAmor, modifiersAmor);
    }

    private void UpdateStats(ref float amountStat,float baseStat , float modifiersStat)
    {
        amountStat = baseStat + modifiersStat;
    }
}
