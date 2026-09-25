using UnityEngine;

/// <summary>
/// Shared part of PlayerInputHandler and EntityInput: action flags, the timed take-damage flag and
/// the direction of the last hit. Subclasses only decide where intent comes from (keys/mouse or AI).
/// </summary>
public abstract class CharacterInputBase<TCore> : CoreComponentBase<TCore>, ICharacterInput where TCore : CoreBase
{
    [Header("Shared State")]
    [SerializeField] protected bool isTakeDamage;
    [SerializeField] protected bool isAttack;
    [SerializeField] protected bool isSkill;
    [SerializeField] protected float takeDamageFlagDuration = 0.1f;

    [Header("Hit Direction")]
    [SerializeField] protected Vector2 hitDirectionVector;
    [SerializeField] protected float hitDirectionAngle;
    [SerializeField] protected int hitDirection;

    public bool IsTakeDamage => isTakeDamage;
    public bool IsAttack => isAttack;
    public bool IsSkill => isSkill;

    public abstract Vector2 AimDirection { get; }
    public abstract Vector2 AimPoint { get; }

    public virtual void OnTakeDamage(Vector2 attackPosition)
    {
        CancelInvoke(nameof(ResetTakeDamage));
        Invoke(nameof(ResetTakeDamage), takeDamageFlagDuration);
        hitDirectionVector = (attackPosition - (Vector2)transform.position).normalized;
        AngleCalculate(hitDirectionVector, ref hitDirectionAngle, ref hitDirection);
        isTakeDamage = true;
    }

    // Protected, not private: Invoke() resolves the name on the runtime type and cannot see a
    // private method declared on a base class.
    protected void ResetTakeDamage()
    {
        isTakeDamage = false;
    }

    protected static void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        DirectionResolver.Calculate(directionVector, ref angle, ref direction);
    }
}
