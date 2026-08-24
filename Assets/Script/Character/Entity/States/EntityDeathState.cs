public class EntityDeathState : EntityBasicState
{
    public EntityDeathState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entityMovement.StopMove();
    }

    public override void LogicUpdate()
    {
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            EventManager.Emit(EventID.ON_ENEMY_DEATH, entity.transform.position);
        }
    }
}
