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
    }

    public override void LogicUpdate()
    {
        switch (Status)
        {
            case StatusAnimation.Start:
                Status = StatusAnimation.None;
                weaponHolder.Attack();

                break;

            case StatusAnimation.OnActivate:
                weaponHolder.MakeDamage();
                Status = StatusAnimation.OffActivate;
                break;

            case StatusAnimation.OffActivate:
                break;

            case StatusAnimation.EndRangeTrigger:
                weaponHolder.EndDamage();
                if (inputHandler.BufferIsAttack && weaponHolder.CanChain())
                {
                    // weaponHolder.Attack();
                    // int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    // player.Anim.Play(stateHash, 0, 0f);
                    int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    player.Anim.Play(stateHash, 0, 0f);
                    Status = StatusAnimation.Start;
                }
                else
                {
                    Status = StatusAnimation.None;
                }
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
        base.Exit();
    }
}
