using UnityEngine;

public class PlayerAttackState : PlayerUseWeaponState
{
    private float startAttackTime;

    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public float StartAttackTime { get => startAttackTime; }

    public override void Enter()
    {
        base.Enter();
        Status = StatusAnimation.None;
        startAttackTime = startTime;
        // The buffer gate in PlayerInputHandler.OnAttack reads statusAnimation, so it has to be
        // open from the first frame of the swing — waiting for the AnimationTrigger event puts it
        // 67-79% into the clip and silently drops every earlier press.
        inputHandler.SetStatusAnimation(StatusAnimation.StartRangeTrigger);
        // Driving the first stage off the AnimationStart event made the bootstrap circular:
        // Attack() installs the clip that carries the event that calls Attack().
        weaponHolder.Attack();
    }

    public override void LogicUpdate()
    {
        switch (Status)
        {
            case StatusAnimation.Start:
                Status = StatusAnimation.None;
                break;

            case StatusAnimation.OnActivate:
                weaponHolder.MakeDamage();
                Status = StatusAnimation.OffActivate;
                break;

            case StatusAnimation.OffActivate:
                break;

            case StatusAnimation.EndRangeTrigger:
                weaponHolder.EndDamage();
                if ((inputHandler.BufferIsAttack || inputHandler.IsAttack) && weaponHolder.CanChain())
                {
                    // Read the hash before Attack() swaps runtimeAnimatorController: the swap
                    // rebinds the Animator, and querying it afterwards can report the layer's
                    // default state instead of Attack, which would make Play() jump elsewhere.
                    // Both stage overrides share Player.controller, so the hash stays valid.
                    int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    weaponHolder.Attack();
                    player.Anim.Play(stateHash, 0, 0f);
                }
                Status = StatusAnimation.None;
                if (inputHandler.BufferIsAttack)
                {
                    inputHandler.SetBufferAttack(false);
                }
                break;

            case StatusAnimation.None:
                break;
            case StatusAnimation.End:
                base.LogicUpdate();
                break;

            default:
                inputHandler.SetStatusAnimation(Status);
                break;
        }
    }
    public override void Exit()
    {
        inputHandler.SetBufferAttack(false);
        // statusAnimation has no other writer, so without this it latches at StartRangeTrigger
        // for the rest of the session and the gate stops reflecting whether a swing is running.
        inputHandler.SetStatusAnimation(StatusAnimation.None);
        base.Exit();
    }
}
