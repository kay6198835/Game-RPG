using UnityEngine;
using VContainer;

public class PlayerMovement : MovementBase<Core>
{
    IObjectResolver resolver;
    [SerializeField] SpawnMono dashEffectPrefab;
    GameObject dashEffectInstance;
    IObjectPoolService objectPoolManager;
    [Inject]
    public void Construct(IObjectResolver resolver, IObjectPoolService objectPoolManager)
    {
        this.resolver = resolver;
        this.objectPoolManager = objectPoolManager;
    }
    public override void Setup()
    {
        base.Setup();
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
        rb.velocity = direction.normalized * MoveSpeed * 2f;
        dashEffectInstance = objectPoolManager.Spawn(dashEffectPrefab.gameObject, transform.position, Quaternion.identity);
        
    }

    public void ExitDash()
    {
        rb.velocity = Vector2.zero;
    }
}
