using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public abstract class StatModifierEffect : ItemEffectDefinitionSO
{
    [SerializeField] protected StatModifierGroup statModifierGroup;
    [SerializeField] protected FilterStatsType filterType;
    protected IEffectReceiver effectReceiver;

    public override void Apply(ICharacter target)
    {
        target.Core.TryGetCapability(out effectReceiver);
    }

    void OnValidate()
    {
        switch (filterType)
        {
            case FilterStatsType.Primary:
                PrimaryFilter();
                break;
            case FilterStatsType.Derived:
                DerivedFilter();
                break;
            default:
                break;
        }
    }

    public void PrimaryFilter()
    {
        statModifierGroup.PrimaryFilter();
    }

    public void DerivedFilter()
    {
        statModifierGroup.DerivedFilter();
    }
}

public enum FilterStatsType
{
    None,
    Primary,
    Derived
}