using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Enemy/Weapon")]
public class WeaponSO : ItemOS
{
    [SerializeField] private GameObject weapon;
    public GameObject Weapon { get => weapon; }
}
