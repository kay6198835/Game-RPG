using System;
using System.Collections.Generic;
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

    public void ActivateInstant()
    {
        PayCost(Definition.Costs);
        ChangeState(AbilityState.Cast);
    }
    public void CastInstant()
    {
        Casting();
        if (Definition.ActivationType == AbilityActivationType.Hold)
        {
            if (IsHolding) return;
        }
        ChangeState(AbilityState.Do);
    }
    public void DoInstant()
    {
        Execute(AbilityContext);
        StartCooldown();
        ChangeState(AbilityState.Exit);
    }

    public void Exit()
    {
        //ChangeState(SkillState.Start);
    }

    private void Casting()
    {
        foreach (var effect in Definition.Effects)
        {
            if (effect == null) continue;
            if (effect.TryCast(AbilityContext))
            {
                PayCost(effect.Costs);
            }
            else
            {
                CancelHold();
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
        BuildContext();
        if (!Definition.TryStart(AbilityContext)) return false;
        return true;
    }

    public void StartHold()
    {
        IsHolding = true;
        CurrentHoldTime = 0f;
    }

    public void CancelHold()
    {
        IsHolding = false;
        CurrentHoldTime = 0f;
    }
    private void BuildContext()
    {
        float holdRatio = 0f;
        if (Definition.MaxHoldTime > 0f)
        {
            holdRatio = Mathf.Clamp01(CurrentHoldTime / Definition.MaxHoldTime);
        }

        AbilityContext = new AbilityContext
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

    private void PayCost(List<StatCost> statCosts)
    {
        foreach (var statCost in statCosts)
        {
            Owner.PayCost(statCost.statType, statCost.value);
        }
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
