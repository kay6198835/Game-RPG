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
    IObjecPoolService objecPoolService;
    [field: SerializeField] private List<AbilityBinding> abilityBindings = new();

    private readonly Dictionary<AbilitySlot, AbilityInstance> _equipped = new();

    public Transform Transform => this.transform;
    [field: SerializeField] public bool IsHolding { get; private set; }
    [field: SerializeField] private AbilityInstance currentAbility;


    [Inject]
    public void Construct(IObjecPoolService pool)
    {
        objecPoolService = pool;
    }
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < abilityBindings.Count; i++)
        {
            var binding = abilityBindings[i];
            if (binding.Ability == null) continue;

            Equip(binding.Slot, binding.Ability);
        }
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
        currentAbility = instance;
        core.Player.Anim.runtimeAnimatorController = currentAbility.Definition.AnimatorOverride;
        return instance;
    }

    public SkillState State => currentAbility?.State ?? SkillState.None;
// Check logic gọi đúng vị trí, nhưng chưa xử lý logic trong các state.
//  Cần bổ sung logic cho từng state trong phương thức HandleInput() và các phương thức liên quan.
    private void HandleInput()
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
                currentAbility = null;
                break;
        }
    }

    public void CheckChangeState(SkillState newState)
    {

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
