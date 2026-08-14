using UnityEngine;

public abstract class Weapon : InteractiveObjects
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected ActivateSkill currentAbilitySO;
    [SerializeField] protected Collider2D pickupCollider;

    protected AttackSO currentStage;
    protected float lastAttackTime;
    protected float chainWindow;

    protected IAimProvider aim;
    protected AbilityHolder abilityHolder;
    protected WeaponHolder holder;

    public int CurrentStageIndex { get; protected set; }
    public WeaponStats Stats => stats;
    public AttackSO CurrentStage => currentStage;

    protected override void Awake()
    {
        base.Awake();
        pickupCollider = GetComponent<Collider2D>();
    }

    /// <summary>True when a brand new attack may be started from a non-attacking state.</summary>
    public virtual bool CanAttack() => stats != null && stats.StageCount > 0;

    /// <summary>True when the attack state may play another stage without leaving the state.</summary>
    public virtual bool CanChain() => CanAttack() && CurrentStageIndex < stats.StageCount;

    /// <summary>Selects the stage to play and prepares the animator. Called on every stage, not just the first.</summary>
    public virtual void OnAttackEnter(Player player)
    {
        if (!CanAttack()) return;

        if (CurrentStageIndex >= stats.StageCount || lastAttackTime + chainWindow < Time.time)
        {
            CurrentStageIndex = 0;
        }

        currentStage = stats.GetStage(CurrentStageIndex);
        player.Anim.speed = 1f;
        chainWindow = Utility.DurationNextAttack(
            Utility.GetOverrideClips(currentStage.directionAttackAnimatorOV, "Attack")) / player.Anim.speed;
        player.Anim.runtimeAnimatorController = currentStage.directionAttackAnimatorOV;

        lastAttackTime = Time.time;
        CurrentStageIndex++;
    }

    /// <summary>The hit frame. Melee resolves a hitbox here, ranged spawns projectiles.</summary>
    public abstract void OnActivate();

    /// <summary>End of the active frames. Optional per weapon type.</summary>
    public virtual void OnDeactivate() { }

    public virtual void SetAbility()
    {
        abilityHolder.SetAblityWeapon(currentAbilitySO);
    }

    public override bool Interact(Interact interactor)
    {
        Equid((WeaponHolder)interactor);
        return true;
    }

    public virtual void Equid(WeaponHolder weaponHolder)
    {
        pickupCollider.enabled = false;
        holder = weaponHolder;
        weaponHolder.Core.GetCoreComponent(out abilityHolder);
        weaponHolder.Core.GetCoreComponent(out PlayerInputHandler inputHandler);
        aim = inputHandler;

        weaponHolder.Equid_UnEquid(this);
        transform.SetParent(weaponHolder.transform);
        transform.position = transform.parent.position;
    }

    public virtual void UnEquid(WeaponHolder weaponHolder)
    {
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + (Vector3)Vector2.one;
        }
        transform.SetParent(null);
        pickupCollider.enabled = true;

        weaponHolder.Equid_UnEquid(this);
        abilityHolder = null;
        aim = null;
        this.holder = null;
        CurrentStageIndex = 0;
    }
}
