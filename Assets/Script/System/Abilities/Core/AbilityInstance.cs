using UnityEngine;

[System.Serializable]
public class AbilityInstance
{
    public AbilityDefinition Definition { get; }
    [NonSerialized] public IAbilityOwner Owner { get; }

    public float CooldownRemaining { get; private set; }
    public bool IsHolding { get; private set; }
    public float CurrentHoldTime { get; private set; }
    public SkillState State { get; private set; } = SkillState.None;
    private AbilityContext abilityContext;
    public AbilityInstance(AbilityDefinition definition, IAbilityOwner owner)
    {
        Definition = definition;
        Owner = owner;
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

    public void TryActivateInstant()
    {
        abilityContext = BuildContext();
        if (!ValidateConditions(abilityContext))
            return false;

        if (!TryPayCost())
            return false;

        ChangeState(SkillState.Cast);
        return true;
    }
    public void TryCastInstant()
    {
        if (Definition.ActivationType == AbilityActivationType.Hold)
        {
            if (!TryPayCost())
                return false;
            Casting();
            if (IsHolding) return;
        }
        ChangeState(SkillState.Do);
    }
    public void TryDoInstant()
    {
        Execute(abilityContext);
        StartCooldown();
        ChangeState(SkillState.Exit);
    }

    public void Exit()
    {

    }

    public void Casting()
    {

    }

    public void ChangeState(SkillState updateState)
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
            Forward = Owner.Transform.forward,
            TargetPoint = Owner.Transform.position + Owner.Transform.forward * 2f,
            HoldTime = CurrentHoldTime,
            HoldRatio = holdRatio,
            AbilityDefinition = Definition,
            AbilityInstance = this
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
        foreach (var statCost in Definition.Costs)
        {
            if (Owner.GetCurrentStatValue(statCost.statType) < statCost.value)
                return false;
        }

        foreach (var statCost in Definition.Costs)
        {
            Owner.PayCost(StatType.Mana, manaCost);
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
