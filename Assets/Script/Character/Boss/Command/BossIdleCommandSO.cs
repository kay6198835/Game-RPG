using UnityEngine;

/// <summary>
/// Boss stands still and reads the player. This is the fight's breathing room — the window the
/// player uses to close distance or heal, so it is a designed beat, not dead time.
/// </summary>
[CreateAssetMenu(fileName = "BossIdle", menuName = "Boss/Command/Idle")]
public class BossIdleCommandSO : BossCommandSO
{
    [Header("Idle")]
    [SerializeField][Range(0.1f, 6f)] private float holdTime = 1f;
    [SerializeField] private bool faceTarget = true;

    public float HoldTime => holdTime;
    public bool FaceTarget => faceTarget;

    public override IBossCommand CreateRuntime() => new BossIdleCommand(this);
}

public class BossIdleCommand : BossCommandRuntime<BossIdleCommandSO>
{
    public BossIdleCommand(BossIdleCommandSO data) : base(data) { }

    protected override void OnEnter()
    {
        if (ctx.Movement != null) ctx.Movement.StopMove();
    }

    protected override void OnTick(float deltaTime)
    {
        if (data.FaceTarget) FaceTarget();
        if (Elapsed >= data.HoldTime) TryResolveEffect();
    }
}
