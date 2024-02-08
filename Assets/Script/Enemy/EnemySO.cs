using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy SO/Enemy")]
public class EnemySO : StatsCharacter
{
    [Header("Stats")]
    [SerializeField] public new string name;
    [SerializeField] public int level;
    [SerializeField] public float speedMove;
    [SerializeField] public float fieldOfViewRange;
    [Header("Stats Attack")]
    [SerializeField] public float rateAttack;
    [SerializeField] public float attackRange;
    [Header("Stats Melee Attack")]
    [SerializeField] public int damage;
    [Header("Stats Long Attack")]
    [SerializeField] public float powerShoot;
    [SerializeField] public GameObject projectile;
    [Header("Atributte")]
    [SerializeField] public LayerMask layerMask;
    [SerializeField] public DepotItemPrefab depotItem;
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player");
    }
}
