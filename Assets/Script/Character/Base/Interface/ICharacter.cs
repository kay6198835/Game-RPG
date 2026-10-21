using UnityEngine;

/// <summary>
/// Identity of a character — player or enemy. It marks the character root and carries NO component
/// interfaces: a system that needs one (INegativeReceiver, IVitalComponent, IStatService…) gets it with
/// TryGetComponent / GetComponent / GetComponentInParent / GetComponentInChildren, whichever fits its
/// context — e.g. <c>hit.GetComponentInParent&lt;ICharacter&gt;().Transform.GetComponentInChildren&lt;IVitalComponent&gt;()</c>.
/// <c>out</c> (covariant) is required: Player is an ICharacter&lt;Core&gt;, and every caller asks for
/// ICharacter&lt;ICore&gt; (GetComponentInParent, the ON_PLAYER_READY payload). Without it those lookups return null.
/// </summary>
public interface ICharacter<out Tcore> where Tcore : ICore
{
    Tcore Core { get; }
    Transform Transform { get; }
    void SetCharacterData(CharacterData data);
    void Reborn();
}
