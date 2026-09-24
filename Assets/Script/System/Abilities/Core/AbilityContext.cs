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
    /// The hurtbox collider a spawned object just hit. Set-then-invoke: the object that detects the hit
    /// assigns it (null when the collider carries no INegativeReceiver) immediately before invoking the
    /// callback. Effects get the interface they need from it themselves.
    /// </summary>
    [NonSerialized] public Collider2D Target;

    /// <summary>
    /// Finds component <typeparamref name="T"/> on the character that is the recipient: from the caster
    /// or from the hit collider, up to its <see cref="ICharacter"/> root, then down to the component.
    /// </summary>
    public bool TryGetRecipientComponent<T>(EffectRecipient recipient, out T component) where T : class
    {
        component = null;
        Component source = recipient == EffectRecipient.Target ? (Component)Target : Caster?.Transform;
        if (source == null) return false;
        ICharacter character = source.GetComponentInParent<ICharacter>();
        if (character == null) return false;
        component = character.Transform.GetComponentInChildren<T>();
        return component != null;
    }
}

/// <summary>World-level services an ability needs. Character data is read from the characters themselves.</summary>
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
