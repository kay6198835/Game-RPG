using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class StatsCharacter : ScriptableObject
{
    [SerializeField] public int blockDMG = 30;
    [SerializeField] public int maxMana = 100;
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public RuntimeAnimatorController animator;
}
