using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Shared Abilities v2 owner for Player and Entity: equips AbilityDefinitions per slot and drives the
/// Start → Cast → Do → Exit instance. Subclasses only say where the bindings come from; aim comes from
/// ICharacterInput and costs from IVitalComponent, so the same AbilityDefinition runs on either side.
/// </summary>
public abstract class AbilityHolderBase<TCore> : CoreComponentBase<TCore>, IAbilityOwner where TCore : CoreBase
{
    private static readonly AbilitySlot[] Slots = (AbilitySlot[])Enum.GetValues(typeof(AbilitySlot));

    [SerializeField] protected List<AbilityBinding> abilityBindings = new();
    [field: SerializeField] public bool IsHolding { get; private set; }
    [SerializeField] protected AbilityInstance currentAbility;

    private readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();
    private IObjecPoolService objecPoolService;
    private IAbilityServices services;
    private IVitalComponent vital;
    private ICharacterInput input;
    private Animator animator;

    public Transform Transform => this.transform;
    public AbilityState CurrentAbilityState => currentAbility?.State ?? AbilityState.Start;
    public AbilityActivationType CurrentActivationType => currentAbility.Definition.ActivationType;
    public AbilityDefinition CurrentDefinition => currentAbility?.Definition;

    // Lazy: a pooled enemy runs Awake before Pool.Spawn() injects it, so the pool is not known yet in Setup().
    private IAbilityServices Services => services ??= new AbilityServices(objecPoolService);
    private IVitalComponent Vital
    {
        get
        {
            if (vital == null) Core.TryGetCapability(out vital);
            return vital;
        }
    }
    private ICharacterInput CharacterInput
    {
        get
        {
            if (input == null) Core.TryGetCapability(out input);
            return input;
        }
    }
    private Animator Animator => animator != null ? animator : (animator = GetComponentInParent<Animator>());

    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        objecPoolService = pool;
    }

    /// <summary>Where this character's slot → ability bindings are authored.</summary>
    protected abstract List<AbilityBinding> ResolveBindings();

    protected override void Start()
    {
        base.Start();
        List<AbilityBinding> bindings = ResolveBindings();
        if (bindings != null) abilityBindings = bindings;
        for (int i = 0; i < abilityBindings.Count; i++)
        {
            var binding = abilityBindings[i];
            if (binding.Ability == null) continue;

            Equip(binding.Slot, binding.Ability);
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

    public float GetCurrentStatValue(StatType statType)
    {
        return Vital.GetCurrentStatValue(statType);
    }
    public void PayCost(StatType statType, float amount)
    {
        Vital.Reduction(statType, amount);
    }
    public Vector2 DirectorForward()
    {
        return CharacterInput.AimDirection;
    }
    public Vector2 TargetPosition()
    {
        return CharacterInput.AimPoint;
    }

    public void Equip(AbilitySlot slot, AbilityDefinition definition)
    {
        if (definition == null) return;

        _equipped[slot] = new AbilityInstance(definition, this, Services);
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

    private void ApplyAnimatorOverride(AbilityDefinition definition)
    {
        if (definition.AnimatorOverride == null) return;
        Animator.runtimeAnimatorController = definition.AnimatorOverride;
    }
}
