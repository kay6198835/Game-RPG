using UnityEngine;

public class PlayerMovement : MovementBase<Core>
{
    IStatService statsService;

    public override void Setup()
    {
        base.Setup();
        core.TryGetCapability(out statsService);
    }
    // Kept under its original (misspelt) name: five player states call it.
    public void SetVeclocity(Vector2 velocity)
    {
        SetVelocity(velocity);
    }

    // NEED CHECK: new 2026-11-04 — moves along a direction at the MoveSpeed stat (was computed in PlayerMoveState).
    /// <summary>Sets the velocity to direction × MoveSpeed stat; pass Vector2.zero to stop.</summary>
    public void Move(Vector2 direction)
    {
        SetVelocity(direction * MoveSpeed);
    }

    public void Dash(Vector2 direction)
    {
        rb.velocity = direction.normalized * statsService.GetStatValue(StatType.MoveSpeed);
    }

    public void ExitDash()
    {
        rb.velocity = Vector2.zero;
    }
}
