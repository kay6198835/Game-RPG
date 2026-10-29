using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Shared Abilities v2 owner for Player and Entity: equips AbilityDefinitions per slot and drives the
/// Start → Cast → Do → Exit instance. Bindings come from CharacterData (PlayerData / EntityData); aim comes from
/// ICharacterInput and costs from IVitalComponent, so the same AbilityDefinition runs on either side.
/// </summary>
public abstract class AbilityHolderBase<TCore> : CoreComponentBase<TCore>, IAbilityOwner where TCore : CoreBase
{
    protected static readonly AbilitySlot[] Slots = (AbilitySlot[])Enum.GetValues(typeof(AbilitySlot));

    [SerializeField] protected List<AbilityBinding> abilityBindings = new();
    [field: SerializeField] public bool IsHolding { get; protected set; }
    [SerializeField] protected AbilityInstance currentAbility;

    protected readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();
    protected IObjectPoolService objectPoolService;
    protected IAbilityServices services;
    protected IVitalComponent<TCore> vital;
    protected IStatService statService;
    protected ICharacterInput input;
    protected Animator animator;

    public Transform Transform => this.transform;
    public AbilityState CurrentAbilityState => currentAbility?.State ?? AbilityState.Start;
    public AbilityActivationType CurrentActivationType => currentAbility.Definition.ActivationType;
    public AbilityDefinition CurrentDefinition => currentAbility?.Definition;

    /// <summary>An equipped ability started its cooldown: slot and full duration in seconds. Read by the HUD.</summary>
    public event Action<AbilitySlot, float> AbilityCooldownStarted;

    // Lazy: a pooled enemy runs Awake before Pool.Spawn() injects it, so the pool is not known yet in Setup().
    protected IAbilityServices Services => services ??= new AbilityServices(objectPoolService);
    protected IVitalComponent<TCore> Vital
    {
        get
        {
            if (vital == null) Core.TryGetCapability(out vital);
            return vital;
        }
    }
    protected IStatService StatService
    {
        get
        {
            if (statService == null) Core.TryGetCapability(out statService);
            return statService;
        }
    }
    protected ICharacterInput CharacterInput
    {
        get
        {
            if (input == null) Core.TryGetCapability(out input);
            return input;
        }
    }
    protected Animator Animator => animator != null ? animator : (animator = GetComponentInParent<Animator>());

    [Inject]
    public void Construct(IObjectPoolService pool)
    {
        objectPoolService = pool;
    }

    /// <summary>Where this character's slot → ability bindings are authored: CharacterData, never the weapon.</summary>
    protected virtual List<AbilityBinding> ResolveBindings() => Core.Data.AbilityBindings;

    protected override void Start()
    {
        base.Start();
    }

    public void SetAbility()
    {
        List<AbilityBinding> bindings = ResolveBindings();
        if (bindings != null) abilityBindings = bindings;
        for (int i = 0; i < abilityBindings.Count; i++)
        {
            var binding = abilityBindings[i];
            if (binding.Ability == null) continue;

            Equip(binding);
        }
    }

    /// <summary>Ticks cooldowns and hold timers. Call every frame from the character's states.</summary>
    public void Processing()
    {
        float dt = Time.deltaTime;

        foreach (var pair in _equipped)
        {
            pair.Value.Tick(dt);
        }
    }

    public float GetCurrentStatValue(VitalStatType statType)
    {
        return Vital.GetCurrentStatValue(statType);
    }

    public float GetCurrentStatValue(StatType statType)
    {
        return StatService.GetStatValue(statType);
    }
    public void PayCost(VitalStatType statType, float amount)
    {
        Vital.Reduction(statType, amount);
    }
    public Vector2 DirectorForward()
    {
        return CharacterInput.AimDirection;
    }
    public int DirectorForwardIndex()
    {
        return CharacterInput.AimDirectionIndex;
    }
    public Vector2 TargetPosition()
    {
        return CharacterInput.AimPoint;
    }

    public void Equip(AbilityBinding binding)
    {
        if (binding.Ability == null) return;

        var instance = new AbilityInstance(binding.Ability, this, Services);
        instance.CooldownStarted += duration => AbilityCooldownStarted?.Invoke(binding.Slot, duration);
        _equipped[binding.Slot] = instance;
    }

    public void Unequip(AbilitySlot slot)
    {
        if (_equipped.ContainsKey(slot))
        {
            _equipped.Remove(slot);
        }
    }

    public AbilityInstance GetAbility(AbilitySlot slot)
    {
        _equipped.TryGetValue(slot, out var instance);
        currentAbility = instance;
        if (currentAbility != null) ApplyAnimatorOverride(currentAbility.Definition);
        return instance;
    }

    public bool TryDoAbility(AbilitySlot slot)
    {
        if (!_equipped.TryGetValue(slot, out var instance))
        {
            return false;
        }
        currentAbility = instance;
        if (!currentAbility.CanStart()) return false;
        ApplyAnimatorOverride(currentAbility.Definition);

        return true;
    }

    /// <summary>Selects the first equipped ability, in AbilitySlot order, that can start now.</summary>
    public bool TryDoAnyAbility()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (TryDoAbility(Slots[i])) return true;
        }
        return false;
    }

    public void HandleInput()
    {
        if (currentAbility == null) return;
        var instance = currentAbility;
        var def = instance.Definition;
        if (def == null)
            return;
        switch (instance.State)
        {
            case AbilityState.Start:
                instance.ActivateInstant();
                break;
            case AbilityState.Cast:
                instance.CastInstant();
                break;
            case AbilityState.Do:
                instance.DoInstant();
                break;
            case AbilityState.Exit:
                instance.Exit();
                break;
        }
    }

    public void StartHold()
    {
        IsHolding = true;
        currentAbility.StartHold();
        currentAbility.ChangeState(AbilityState.Start);
    }

    public void CancelHold()
    {
        IsHolding = false;
        currentAbility.CancelHold();
    }

    protected void ApplyAnimatorOverride(AbilityDefinition definition)
    {
        if (definition.AnimatorOverride == null) return;
        Animator.runtimeAnimatorController = definition.AnimatorOverride;
    }
}
