using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityHolder : CoreComponent<Core>, IAbilityOwner
{
    [field: SerializeField] private AbilitySystem abilitySystem { get; private set; }

    private IAbilityServices services;
    IPlayerStatService statsHandler;
    IResourceReceiver resourceReceiver;
    IVitalComponent vital;
    [field: SerializeField] private List<AbilityBinding> abilityBindings = new();

    private readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();

    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        statsHandler = Core.GetcomponentInChild<IPlayerStatService>();
        resourceReceiver = Core.GetcomponentInChild<IResourceReceiver>();
        vital = Core.GetcomponentInChild<IVitalComponent>();
        services = new AbilityServices(pool, statsHandler, resourceReceiver, vital);
    }
    // private void Awake()
    // {

    //     for (int i = 0; i < abilityBindings.Count; i++)
    //     {
    //         var binding = abilityBindings[i];
    //         if (binding.Ability == null) continue;

    //         Equip(binding.Slot, binding.Ability);
    //     }
    // }

    // private void Update()
    // {
    //     float dt = Time.deltaTime;

    //     foreach (var pair in _equipped)
    //     {
    //         pair.Value.Tick(dt);
    //     }

    //     HandleInput();
    // }

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
        foreach (var pair in _equipped)
        {
            var instance = pair.Value;
            var def = instance.Definition;

            if (def == null || def.DefaultKey == KeyCode.None)
                continue;

            if (def.ActivationType == AbilityActivationType.Hold)
            {
                if (Input.GetKeyDown(def.DefaultKey))
                {
                    if (instance.CanStart())
                    {
                        instance.StartHold();
                    }
                }

                if (Input.GetKeyUp(def.DefaultKey))
                {
                    instance.TryRelease();
                }
            }
            else
            {
                if (Input.GetKeyDown(def.DefaultKey))
                {
                    if (instance.CanStart())
                    {
                        instance.TryActivateInstant();
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class AbilityBinding
{
    public AbilitySlot Slot;
    public AbilityDefinition Ability;
}
