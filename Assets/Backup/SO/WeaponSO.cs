using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Enemy/Weapon")]
public class WeaponSO : ItemOS
{
    [SerializeField] private WeaponStats stats;
    public WeaponStats Stats { get => stats; }
}
