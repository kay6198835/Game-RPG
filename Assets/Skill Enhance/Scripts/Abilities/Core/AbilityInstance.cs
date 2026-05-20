using UnityEngine;

public class AbilityInstance
{
    public AbilityDefinition Definition { get; }
    public IAbilityOwner Owner { get; }

    public float CooldownRemaining { get; private set; }
    public bool IsHolding { get; private set; }
    public float CurrentHoldTime { get; private set; }

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

    public bool CanStart()
    {
        if (CooldownRemaining > 0f) return false;
        if (Owner == null) return false;
        if (Owner.Health == null || Owner.Health.IsDead) return false;
        if (Owner.Stats == null) return false;
        if (Owner.Stats.CurrentMana < Definition.ManaCost) return false;
        return true;
    }

    public void StartHold()
    {
        if (Definition.ActivationType != AbilityActivationType.Hold)
            return;

        IsHolding = true;
        CurrentHoldTime = 0f;
    }

    public void CancelHold()
    {
        IsHolding = false;
        CurrentHoldTime = 0f;
    }

    public bool TryRelease()
    {
        if (!IsHolding)
            return false;

        var context = BuildContext();
        if (!ValidateConditions(context))
            return false;

        if (!TryPayCost())
            return false;

        Execute(context);
        StartCooldown();

        IsHolding = false;
        CurrentHoldTime = 0f;
        return true;
    }

    public bool TryActivateInstant()
    {
        if (Definition.ActivationType == AbilityActivationType.Hold)
            return false;

        var context = BuildContext();
        if (!ValidateConditions(context))
            return false;

        if (!TryPayCost())
            return false;

        Execute(context);
        StartCooldown();
        return true;
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
        if (Owner.Stats.CurrentMana < Definition.ManaCost)
            return false;

        Owner.Stats.SpendMana(Definition.ManaCost);
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
