using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class AbilityHolder : CoreComponent<Core>, IAbilityOwner
{
    IAbilityServices services;
    IPlayerStatService statsHandler;
    IResourceReceiver resourceReceiver;
    IVitalComponent vital;
    IObjecPoolService objecPoolService;
    PlayerInputHandler playerInputHandler;
    [field: SerializeField] private List<AbilityBinding> abilityBindings = new();
    private readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();
    public Transform Transform => this.transform;
    [field: SerializeField] public bool IsHolding { get; private set; }
    [field: SerializeField] private AbilityInstance currentAbility;
    public SkillState State => currentAbility?.State ?? SkillState.Start;


    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        objecPoolService = pool;
    }
    protected override void Start()
    {
        base.Start();
        abilityBindings = core.Player.Data.AbilityBindings;
        for (int i = 0; i < abilityBindings.Count; i++)
        {
            var binding = abilityBindings[i];
            if (binding.Ability == null) continue;

            Equip(binding.Slot, binding.Ability);
        }
        Core.GetCoreComponent(out playerInputHandler);
    }
    public override void Setup()
    {
        base.Setup();
        statsHandler = Core.GetComponentInChildren<IPlayerStatService>();
        resourceReceiver = Core.GetComponentInChildren<IResourceReceiver>();
        vital = Core.GetComponentInChildren<IVitalComponent>();
        services = new AbilityServices(objecPoolService, statsHandler, resourceReceiver, vital);
    }

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
        return vital.GetCurrentStatValue(statType);
    }
    public void PayCost(StatType statType, float amount)
    {
        vital.ReceiveReduction(statType, amount);
    }
    public Vector2 DirectorForward()
    {
        return playerInputHandler.DirectionMouseVector;
    }

    public void Equip(AbilitySlot slot, AbilityDefinition definition)
    {
        if (definition == null) return;

        _equipped[slot] = new AbilityInstance(definition, this, services);
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
        core.Player.Anim.runtimeAnimatorController = currentAbility.Definition.AnimatorOverride;
        return instance;
    }
    public bool TryDoAbility(AbilitySlot slot)
    {
        if (!_equipped.TryGetValue(slot, out var instance))
        {
            return false;
        }
        currentAbility = instance;
        currentAbility.SetupContext();
        foreach (var condition in currentAbility.Definition.Conditions)
        {
            if (!condition.IsMet(currentAbility.AbilityContext)) return false;
        }
        core.Player.Anim.runtimeAnimatorController = currentAbility.Definition.AnimatorOverride;

        return true;
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
            case SkillState.Start:
                instance.TryActivateInstant();
                break;
            case SkillState.Cast:
                instance.TryCastInstant();
                break;
            case SkillState.Do:
                instance.TryDoInstant();
                break;
            case SkillState.Exit:
                instance.Exit();
                break;
        }
    }

    public void StartHold()
    {
        IsHolding = true;
        currentAbility.StartHold();
        currentAbility.ChangeState(SkillState.Start);
    }

    public void CancelHold()
    {
        IsHolding = false;
        currentAbility.CancelHold();
    }
}

[System.Serializable]
public class AbilityBinding
{
    public AbilitySlot Slot;
    public AbilityDefinition Ability;
}
