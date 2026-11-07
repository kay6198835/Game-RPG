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
        inputHandler.SetStatusAnimation(StatusAnimation.Start);
        weaponHolder.Attack();
        player.Anim.Play(animBoolName, 0, 0f);
        player.Anim.Update(0f);
    }
    public override void AnimationOnAction()
    {
        base.AnimationOnAction();
        weaponHolder.MakeDamage();
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        weaponHolder.EndDamage();
    }
    public override void AnimationEnd()
    {
        bool chain = (inputHandler.BufferIsAttack || inputHandler.IsAttack) && weaponHolder.CanChain();
        inputHandler.SetBufferAttack(false);
        if (chain)
        {
            int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
            weaponHolder.Attack();
            player.Anim.Play(stateHash, 0, 0f);
            player.Anim.Update(0f);
            player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionMouse);
            return;
        }
        base.AnimationEnd();
        base.LogicUpdate();
    }
    public override void LogicUpdate()
    {
        inputHandler.SetStatusAnimation(Status);
    }
    public override void Exit()
    {
        inputHandler.SetBufferAttack(false);
        inputHandler.SetStatusAnimation(StatusAnimation.None);
        base.Exit();
    }
}
