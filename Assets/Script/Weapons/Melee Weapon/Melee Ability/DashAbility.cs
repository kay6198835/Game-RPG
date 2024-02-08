using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability SO/Dash Ability")]
public class DashAbility : AbilitySO
{
    [SerializeField] private float dashingPower;

    public override void Activate(GameObject player)
    {
        Rigidbody2D rigidbody = player.GetComponentInParent<Rigidbody2D>();
        rigidbody.AddForce(player.GetComponentInParent<PlayerMovement>().MousePosition
            * dashingPower, ForceMode2D.Impulse);  
    }
}