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

    public void CancelHold()
    {
        IsHolding = false;
        CurrentHoldTime = 0f;
    }
    private AbilityContext BuildContext()
    {
        float holdRatio = 0f;
        if (Definition.MaxHoldTime > 0f)
        {
            holdRatio = Mathf.Clamp01(CurrentHoldTime / Definition.MaxHoldTime);
        }

        return new AbilityContext
        {
            Caster = Owner,
            Origin = Owner.Transform.position,
            Forward = Owner.DirectorForward(),
            TargetPoint = Owner.TargetPosition(),
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
