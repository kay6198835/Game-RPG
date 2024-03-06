using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability SO/Dash Ability")]
public class DashAbility : AbilitySO
{
    [SerializeField] private float dashingPower;
}