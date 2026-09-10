using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class AbilityHolder : CoreComponent<Core>, IAbilityOwner
{
    private IAbilityServices services;
    IPlayerStatService statsHandler;
    IResourceReceiver resourceReceiver;
    IVitalComponent vital;
    [field: SerializeField] private List<AbilityBinding> abilityBindings = new();

    private readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();

    public Transform Transform => this.transform;
    public bool IsHolding { get; private set; }
    private AbilityInstance currentAbility;

    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        statsHandler = Core.GetComponentInChildren<IPlayerStatService>();
        resourceReceiver = Core.GetComponentInChildren<IResourceReceiver>();
        vital = Core.GetComponentInChildren<IVitalComponent>();
        services = new AbilityServices(pool, statsHandler, resourceReceiver, vital);
    }
    private void Awake()
    {

        for (int i = 0; i < abilityBindings.Count; i++)
        {
            var binding = abilityBindings[i];
            if (binding.Ability == null) continue;

            Equip(binding.Slot, binding.Ability);
        }
    }

    public void Processing()
    {
        float dt = Time.deltaTime;

        foreach (var pair in _equipped)
        {
            pair.Value.Tick(dt);
        }

        HandleInput();
    }

    public float GetCurrentStatValue(StatType statType)
    {
        return vital.GetCurrentStatValue(statType);
    }
    public void PayCost(StatType statType, float amount)
    {
        vital.ReceiveReduction(statType, amount);
    }

    public void Equip(AbilitySlot slot, AbilityDefinition definition)
    {
        if (definition == null) return;

        _equipped[slot] = new AbilityInstance(definition, this);
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
        return instance;
    }

    private void HandleInput()
    {
        if (currentAbility == null) return;
        var instance = currentAbility.Value;
        var def = instance.Definition;
        if (def == null || def.DefaultKey == KeyCode.None)
            continue;
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
                currentAbilitySlot = null;
                break;
        }
    }

    public void Tick(float deltaTime)
    {
        foreach (var pair in _equipped)
        {
            var instance = pair.Value;
            instance.Tick(deltaTime);
        }
    }

    public void GetSkill(AbilitySlot slot)
    {
        currentAbility = _equipped[slot];
    }

    public void StartHold()
    {
        IsHolding = true;
        currentAbility.StartHold();
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
