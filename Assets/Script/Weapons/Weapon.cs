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
    protected IWeaponHolder holder;

    public int CurrentStageIndex { get; protected set; }
    public WeaponStats Stats => stats;
    public AttackSO CurrentStage => currentStage;

    protected override void Awake()
    {
        base.Awake();
        pickupCollider = GetComponent<Collider2D>();
        CurrentStageIndex = 0;
    }

    /// <summary>True when a brand new attack may be started from a non-attacking state.</summary>
    public virtual bool CanAttack() => stats != null && stats.StageCount > 0;

    /// <summary>
    /// True when the attack state may play another stage without leaving the state.
    /// The index wraps to 0 after the last stage, so a zero index means the chain just completed.
    /// </summary>
    public virtual bool CanChain() => CanAttack() && CurrentStageIndex != 0;

    /// <summary>Selects the stage to play and prepares the animator. Called on every stage, not just the first.</summary>
    public virtual void OnAttackEnter(IWeaponHolder user)
    {
        if (!CanAttack()) return;

        // StageCount can shrink while the SO is edited in play mode, so clamp as well as time out.
        if (CurrentStageIndex >= stats.StageCount || lastAttackTime + chainWindow < Time.time)
        {
            CurrentStageIndex = 0;
        }

        currentStage = stats.GetStage(CurrentStageIndex);
        Animator animator = user.Animator;
        animator.speed = 1f;
        chainWindow = Utility.DurationNextAttack(
            Utility.GetOverrideClips(currentStage.directionAttackAnimatorOV, "Attack")) / animator.speed;
        animator.runtimeAnimatorController = currentStage.directionAttackAnimatorOV;

        lastAttackTime = Time.time;
        // Wrapping is what makes a zero index mean "the chain just completed", which is the whole
        // of CanChain(). A bare ++ leaves the index non-zero forever, so the combo never ends.
        CurrentStageIndex = (CurrentStageIndex + 1) % stats.StageCount;
    }

    /// <summary>The hit frame. Melee resolves a hitbox here, ranged spawns projectiles.</summary>
    public abstract void OnActivate(float damage);

    /// <summary>End of the active frames. Optional per weapon type.</summary>
    public virtual void OnDeactivate() { }

    public virtual void SetAbility()
    {
        //fix later
        //abilityHolder.SetAblityWeapon(currentAbilitySO);
    }

    public override bool Interact(Interact interactor)
    {
        // Up to the character root, then down to whichever weapon holder it has.
        ICharacter owner = interactor.GetComponentInParent<ICharacter>();
        IWeaponHolder weaponHolder = owner?.Transform.GetComponentInChildren<IWeaponHolder>();
        if (weaponHolder == null) return false;
        Equid(weaponHolder);
        return true;
    }

    public virtual void Equid(IWeaponHolder weaponHolder)
    {
        pickupCollider.enabled = false;
        holder = weaponHolder;
        aim = weaponHolder.Aim;
        weaponHolder.Equid_UnEquid(this);
        IStatService ownerStats = OwnerStats(weaponHolder);
        if (ownerStats != null) stats.StatModifiers.Apply(ownerStats.AddModifiersFromSource, this);
        else Debug.LogWarning($"[{name}] equipped by a holder with no character stats.", this);
        transform.SetParent(weaponHolder.Transform);
        transform.position = transform.parent.position;
    }

    // Up from the holder to the character root, then down to its stat profile.
    private static IStatService OwnerStats(IWeaponHolder weaponHolder)
    {
        ICharacter owner = weaponHolder.Transform.GetComponentInParent<ICharacter>();
        return owner?.Transform.GetComponentInChildren<IStatService>();
    }

    public virtual void UnEquid(IWeaponHolder weaponHolder)
    {
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + (Vector3)Vector2.one;
        }
        transform.SetParent(null);
        pickupCollider.enabled = true;
        IStatService ownerStats = OwnerStats(weaponHolder);
        if (ownerStats != null) stats.StatModifiers.Remmove(ownerStats.RemoveModifiersFromSource, this);
        weaponHolder.Equid_UnEquid(this);
        aim = null;
        this.holder = null;
        CurrentStageIndex = 0;
    }
}
