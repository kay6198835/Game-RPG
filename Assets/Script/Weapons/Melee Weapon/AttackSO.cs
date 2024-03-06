using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName ="WeaponData/MeeleeWeaponData/StateData/Normal Attack")]
public class AttackSO : ScriptableObject
{
    [SerializeField] public string nameState;
    [Header("Stats")]
    [SerializeField] public float attackRange = 1f;
    [SerializeField] public int attackDamege = 0;
    [SerializeField] public float attackRate = 1f;
    
    [Header("Atributte")]
    [SerializeField] public AnimatorOverrideController directionAttackAnimatorOV;
    [SerializeField] public LayerMask enemyLayers;
    [SerializeField] public AbilitySO ability;
    private void Awake()
    {
        enemyLayers = LayerMask.GetMask("Enemy");
    }
}
