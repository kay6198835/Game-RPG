using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Buff - Debuff Stats For Duration")]
public class BuffDebuffStatsForDuration : AbilityEffectDefinition
{
    public StatModifierGroup statModifierGroup;
    public float duration;
    public override void Apply(AbilityContext context)
    {
        context.Services.Vital.BuffDebuffForDuration(statModifierGroup, duration);
    }
}

