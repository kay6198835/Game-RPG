using UnityEngine;

/// <summary>
/// The contract every character — player or enemy — exposes to systems outside it. Anything that
/// changes stats (abilities, items, weapons) talks to this, never to Player or Entity.
/// The non-generic form is for coordination (like IGrid); <see cref="ICharacter{TCore}"/> is typed access.
/// </summary>
public interface ICharacter
{
    Transform Transform { get; }
    ICore Core { get; }
    /// <summary>Max values (stat profile).</summary>
    IStatService Stats { get; }
    /// <summary>Current values (HP, Mana…).</summary>
    IVitalComponent Vital { get; }
    /// <summary>The character's single damage receiver.</summary>
    INegativeReceiver DamageReceiver { get; }
}

public interface ICharacter<out TCore> : ICharacter where TCore : ICore
{
    new TCore Core { get; }
}
