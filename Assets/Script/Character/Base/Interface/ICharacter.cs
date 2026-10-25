using UnityEngine;

/// <summary>
/// Identity of a character — player or enemy. It marks the character root and carries NO component
/// interfaces: a system that needs one (INegativeReceiver, IVitalComponent, IStatService…) gets it with
/// TryGetComponent / GetComponent / GetComponentInParent / GetComponentInChildren, whichever fits its
/// context — e.g. <c>hit.GetComponentInParent&lt;ICharacter&gt;().Transform.GetComponentInChildren&lt;IVitalComponent&gt;()</c>.
/// Unity looks components up by the exact interface a class declares and ignores C# generic variance, so
/// <c>GetComponentInParent&lt;ICharacter&lt;ICore&gt;&gt;()</c> finds nothing on a Player (an ICharacter&lt;Core&gt;).
/// Look up the non-generic <see cref="ICharacter"/> from outside; use ICharacter&lt;TCore&gt; only where the
/// concrete core type is known (e.g. WeaponHolderBase).
/// </summary>
public interface ICharacter
{
    ICore Core { get; }
    Transform Transform { get; }
    void SetCharacterData(CharacterData data);
    void Reborn();
}

public interface ICharacter<out TCore> : ICharacter where TCore : ICore
{
    new TCore Core { get; }
}
