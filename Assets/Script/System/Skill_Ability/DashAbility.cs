using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability SO/Activate Skill/Dash Ability")]
public class DashAbility : ActivateSkill
{
    PlayerMovement playerMovement;
    PlayerInputHandler inputHandler;
    [SerializeField] private float dashingPower;
    public override void Enter(Player player)
    {
        base.Enter(player);
        this.player.Anim.speed = 2f;
        player.Core.GetCoreComponent(out playerMovement);
        player.Core.GetCoreComponent(out inputHandler);
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
        playerMovement.SetVeclocity(inputHandler.DirectionMouseVector * dashingPower);
    }
    public override void Exit()
    {
        this.player.Anim.speed = 1;
        base.Exit();

    }
}