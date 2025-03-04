using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability SO/Activate Skill/Dash Ability")]
public class DashAbility : ActivateSkill
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
        player.Core.Movement.SetVeclocity(player.StatsBehavior.DirectionMouseVector * dashingPower);
    }
    public override void Exit()
    {
        this.player.Anim.speed = 1;
        base.Exit();

    }
}