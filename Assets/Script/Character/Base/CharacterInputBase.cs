using UnityEngine;

/// <summary>
/// Shared part of PlayerInputHandler and EntityInput: action flags, the timed take-damage flag and
/// the direction of the last hit. Subclasses only decide where intent comes from (keys/mouse or AI).
/// </summary>
public abstract class CharacterInputBase<TCore> : CoreComponentBase<TCore>, ICharacterInput where TCore : CoreBase
{
    [Header("Shared State")]
    [SerializeField] protected bool isTakeDamage;
    [SerializeField] protected bool isDeath;
    [SerializeField] protected bool isAttack;
    [SerializeField] protected bool isSkill;

    [Header("Hit Direction")]
    [SerializeField] protected Vector2 hitDirectionVector;
    [SerializeField] protected float hitDirectionAngle;
    [SerializeField] protected int hitDirection;

    public bool IsTakeDamage => isTakeDamage;
    public bool IsDeath => isDeath;
    public bool IsAttack => isAttack;
    public bool IsSkill => isSkill;

    public abstract Vector2 AimDirection { get; }
    public abstract Vector2 AimPoint { get; }

    public abstract int AimDirectionIndex { get; }

    protected void ResetTakeDamage()
    {
        isTakeDamage = false;
    }
    public virtual void OnTakeDamage(Vector2 attackPosition)
    {
        CancelInvoke(nameof(ResetTakeDamage));
        Invoke(nameof(ResetTakeDamage), GameConstants.SettingStats.TAKE_DAMAGE_FLAG_DURATION);
        hitDirectionVector = (attackPosition - (Vector2)transform.position).normalized;
        AngleCalculate(hitDirectionVector, ref hitDirectionAngle, ref hitDirection);
        isTakeDamage = true;
    }

    // NEED CHECK: new 2026-11-05 — the lethal-hit signal lives on the input like every other state trigger;
    // the receiver no longer changes state itself. Sticky until ReloadComponent() (respawn / Reborn).
    public virtual void OnDeath(Vector2 attackPosition)
    {
        if(isDeath) return;
        hitDirectionVector = (attackPosition - (Vector2)transform.position).normalized;
        AngleCalculate(hitDirectionVector, ref hitDirectionAngle, ref hitDirection);
        isDeath = true;
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        CancelInvoke(nameof(ResetTakeDamage));
        isTakeDamage = false;
        isDeath = false;
        isAttack = false;
        isSkill = false;
    }

    protected static void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        DirectionResolver.Calculate(directionVector, ref angle, ref direction);
    }
}
