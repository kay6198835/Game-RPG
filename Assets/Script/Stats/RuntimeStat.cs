using System.Collections.Generic;

public class RuntimeStat
{
    public float BaseValue;
    private readonly List<StatModifier> _modifiers = new();

    public RuntimeStat(float baseValue)
    {
        BaseValue = baseValue;
    }

    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Add(modifier);
    }

    public void RemoveModifiersBySource(object source)
    {
        _modifiers.RemoveAll(x => x.Source == source);
    }

    public float Value
    {
        get
        {
            float flat = 0f;
            float percent = 0f;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                var mod = _modifiers[i];
                if (mod.Type == StatModifierType.Flat)
                    flat += mod.Value;
                else if (mod.Type == StatModifierType.Percent)
                    percent += mod.Value;
            }

            return (BaseValue + flat) * (1f + percent);
        }
    }
}
