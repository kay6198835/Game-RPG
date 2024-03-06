using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityHolder : MonoBehaviour
{
    [SerializeField] private AbilitySO ability;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float activeTime;
    [SerializeField] private KeyCode keyCode;
    public void SetAblityWeapon(WeaponMelee weapon)
    {
        ability = weapon.CurrentAbilitySO;
    }
}