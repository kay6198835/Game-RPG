using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "WeaponData/WeaponSO")]
public class WeaponSO : ItemSO
{
    [SerializeField] private GameObject weapon;
    public GameObject Weapon { get => weapon; }
}
