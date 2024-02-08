using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataSO : ScriptableObject
{
    [Header("WP data")]
    public WeaponType Type;
    public GameObject weaponPrefab;
    public int value;
}
