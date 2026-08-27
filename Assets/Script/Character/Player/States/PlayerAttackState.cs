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
        startAttackTime = startTime;
        weaponHolder.Attack();
    }

    public override void LogicUpdate()
    {
        switch (Status)
        {
            case StatusAnimation.Start:
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
                    Debug.Log("End Window Attack");
                    weaponHolder.Attack();
                    int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    player.Anim.Play(stateHash, 0, 0f);
                    Status = StatusAnimation.Start;
                    Debug.Log("Next Attack");
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
                Debug.Log("PlayerAttackState End");
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
