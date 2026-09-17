using System;
using UnityEngine;

[System.Serializable]
public class AbilityInstance
{
    public AbilityDefinition Definition { get; }
    public IAbilityOwner Owner { get; }
    public IAbilityServices Services { get; }
    public float CooldownRemaining { get; private set; }
    public bool IsHolding { get; private set; }
    public float CurrentHoldTime { get; private set; }
    public AbilityState State { get; private set; } = AbilityState.Start;
    [field: SerializeField] public AbilityContext AbilityContext;
    public AbilityInstance(AbilityDefinition definition, IAbilityOwner owner, IAbilityServices abilityServices)
    {
        Definition = definition;
        Owner = owner;
        Services = abilityServices;
    }

    public void Tick(float deltaTime)
    {
        if (CooldownRemaining > 0f)
        {
            CooldownRemaining -= deltaTime;
            if (CooldownRemaining < 0f)
                CooldownRemaining = 0f;
        }

        if (IsHolding)
        {
            CurrentHoldTime += deltaTime;
            if (Definition.MaxHoldTime > 0f)
            {
                CurrentHoldTime = Mathf.Min(CurrentHoldTime, Definition.MaxHoldTime);
            }
        }

        RefreshHoldContext();
    }

    public float HoldRatio => AbilityRuntimeHelpers.SafeRatio(CurrentHoldTime, Definition.MaxHoldTime);

    // Context is built once per cast, so charge-scaling effects only see a live hold value
    // if it is pushed back into the context every tick.
    private void RefreshHoldContext()
    {
        if (AbilityContext == null) return;

        AbilityContext.HoldTime = CurrentHoldTime;
        AbilityContext.HoldRatio = HoldRatio;
        AbilityContext.Origin = Owner.Transform.position;
        AbilityContext.Forward = Owner.DirectorForward();
    }

    public bool TryActivateInstant()
    {
        if (!ValidateConditions(AbilityContext))
        {
            return false;
        }

        if (!TryPayCost())
        {
            return false;
        }

        ChangeState(AbilityState.Cast);
        return true;
    }
    public void TryCastInstant()
    {
        if (Definition.ActivationType == AbilityActivationType.Hold)
        {
            Casting();
            if (IsHolding) return;
        }
        ChangeState(AbilityState.Do);
    }
    public void TryDoInstant()
    {
        Execute(AbilityContext);
        StartCooldown();
        ChangeState(AbilityState.Exit);
    }

    public void Exit()
    {
        //ChangeState(SkillState.Start);
    }

    public void Casting()
    {
        foreach (var effect in Definition.Effects)
        {
            if (effect == null) continue;
            if (effect.Casting(AbilityContext))
            {
                if (!TryPayCost())
                    return;
            }
        }
    }

    public void ChangeState(AbilityState updateState)
    {
        State = updateState;
    }

    public bool CanStart()
    {
        if (CooldownRemaining > 0f) return false;
        if (Owner == null) return false;
        return true;
    }

    public void StartHold()
    {
        IsHolding = true;
        CurrentHoldTime = 0f;
    }
    public void SetupContext()
    {
        AbilityContext = BuildContext();
    }

    // Release, not abort: CurrentHoldTime must survive until TryDoInstant() runs, otherwise
    // charge-scaling effects always read HoldRatio == 0. StartHold() is what resets it.
    public void CancelHold()
    {
        IsHolding = false;
        RefreshHoldContext();
    }
    private AbilityContext BuildContext()
    {
        float holdRatio = HoldRatio;

        return new AbilityContext
        {
            Caster = Owner,
            Origin = Owner.Transform.position,
            Forward = Owner.DirectorForward(),
            TargetPoint = Owner.Transform.position + Owner.Transform.forward * 2f,
            HoldTime = CurrentHoldTime,
            HoldRatio = holdRatio,
            AbilityDefinition = Definition,
            AbilityInstance = this,
            Services = Services
        };
    }

    private bool ValidateConditions(AbilityContext context)
    {
        if (Definition.Conditions == null) return true;

        for (int i = 0; i < Definition.Conditions.Count; i++)
        {
            var condition = Definition.Conditions[i];
            if (condition == null) continue;

            if (!condition.IsMet(context))
                return false;
        }

        return true;
    }

    private bool TryPayCost()
    {
        foreach (var condition in Definition.Conditions)
        {
            if (condition == null) continue;
            if (!condition.IsMet(AbilityContext))
                return false;
        }

        foreach (var statCost in Definition.Costs)
        {
            Owner.PayCost(statCost.statType, statCost.value);
        }
        return true;
    }

    private void Execute(AbilityContext context)
    {
        if (Definition.Effects == null) return;

        for (int i = 0; i < Definition.Effects.Count; i++)
        {
            var effect = Definition.Effects[i];
            if (effect == null) continue;
            effect.Apply(context);
        }
    }

    private void StartCooldown()
    {
        CooldownRemaining = Definition.Cooldown;
    }
}
