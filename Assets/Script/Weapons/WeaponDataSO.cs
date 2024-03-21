using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataSO : ScriptableObject
{
    [Header("WP data")]
    protected WeaponType type;
    //public GameObject weaponPrefab;
    //public int value;
    [SerializeField] protected LayerMask enemyLayers;
    [SerializeField] protected AbilitySO ability;
    [SerializeField] protected AbilitySO special;

    public WeaponType Type { get => type; }
    public LayerMask EnemyLayers { get => enemyLayers; }
    public AbilitySO Ability { get => ability; }
    public AbilitySO Special { get => special; }
}
