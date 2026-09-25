using UnityEngine;

/// <summary>
/// Enemy counterpart of PlayerSkillWeaponState. The player steps the ability from animation events;
/// an enemy's animator carries none, so this state steps Start → Cast → Do → Exit from LogicUpdate.
/// The Cast phase dispatches once, then holds for the ability's MaxHoldTime (0 for Active) before
/// releasing — the same shape as a player pressing and letting go.
/// Requires an "Ability" bool on the enemy's animator controller.
/// </summary>
public class EntityAbilityState : EntityState
{
    private EntityAbilityHolder abilityHolder;
    private EntityMovement entityMovement;
    private float holdStartTime;
    private bool castDispatched;

    public EntityAbilityState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out abilityHolder);
        entity.Core.GetCoreComponent(out entityMovement);
        entityMovement.Stop();
        abilityHolder.StartHold();
        holdStartTime = Time.time;
        castDispatched = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        abilityHolder.Processing();

        switch (abilityHolder.CurrentAbilityState)
        {
            case AbilityState.Start:
                abilityHolder.HandleInput();
                break;
            case AbilityState.Cast:
                if (abilityHolder.IsHolding)
                {
                    if (!castDispatched)
                    {
                        abilityHolder.HandleInput();
                        castDispatched = true;
                    }
                    float holdTime = abilityHolder.CurrentDefinition != null ? abilityHolder.CurrentDefinition.MaxHoldTime : 0f;
                    if (Time.time - holdStartTime >= holdTime) abilityHolder.CancelHold();
                }
                else
                {
                    abilityHolder.HandleInput();
                }
                break;
            case AbilityState.Do:
                abilityHolder.HandleInput();
                break;
            case AbilityState.Exit:
                stateMachine.ChangeState(entity.IdleState);
                return;
        }
    }

    public override void Exit()
    {
        if (abilityHolder != null && abilityHolder.IsHolding) abilityHolder.CancelHold();
        // An ability's AnimatorOverride replaced the controller; restore the enemy's own.
        entity.Anim.runtimeAnimatorController = entityData.Aima;
        base.Exit();
        abilityHolder = null;
        entityMovement = null;
    }
}
