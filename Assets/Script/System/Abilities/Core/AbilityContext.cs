using System;
using UnityEngine;
[Serializable]
public class AbilityContext
{
    public IAbilityOwner Caster;
    public Vector3 Origin;
    public Vector3 Forward;
    public Vector3 TargetPoint;

    public float HoldTime;
    public float HoldRatio;

    [NonSerialized] public AbilityInstance AbilityInstance;
    public AbilityDefinition AbilityDefinition;

    [NonSerialized] public IAbilityServices Services;

    /// <summary>
    /// The character a spawned object just hit. Set-then-invoke: the object that detects the hit
    /// assigns it (null when the collider is not a hurtbox) immediately before invoking the callback.
    /// </summary>
    [NonSerialized] public ICharacter Target;

    public ICharacter CasterCharacter => Caster?.Character;

    public ICharacter ResolveRecipient(EffectRecipient recipient)
    {
        return recipient == EffectRecipient.Target ? Target : CasterCharacter;
    }
}

/// <summary>World-level services an ability needs. Character data is read from Caster / Target, not from here.</summary>
public interface IAbilityServices
{
    IObjecPoolService Pool { get; }
}

public sealed class AbilityServices : IAbilityServices
{
    public IObjecPoolService Pool { get; }

    public AbilityServices(IObjecPoolService pool)
    {
        Pool = pool;
    }
}

/// <summary>Which character an effect acts on. Caster = 0 so existing assets keep their behaviour.</summary>
public enum EffectRecipient
{
    Caster = 0,
    Target = 1
}
