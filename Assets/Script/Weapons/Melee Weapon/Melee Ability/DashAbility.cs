using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability SO/Dash Ability")]
public class DashAbility : AbilitySO
{
    [SerializeField] private float dashingPower;


    public override void Enter(NewPlayer player)
    {
        base.Enter(player);
        this.player.Anim.speed = 2f;
    }
    public override void Activate()
    {
        base.Activate();

    }
    public override void Cast()
    {
        base.Cast();
    }

    public override void Do()
    {
        base.Do();
        player.Core.Movement.SetVeclocity(player.InputHandler.DirectionVector * dashingPower);
    }
    public override void Exit()
    {
        this.player.Anim.speed = 1;
        base.Exit();

    }
}