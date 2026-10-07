using UnityEngine;

/// <summary>
/// Identity of a character — player or enemy. It marks the character root and carries NO component
/// interfaces: a system that needs one (INegativeReceiver, IVitalComponent, IStatService…) gets it with
/// TryGetComponent / GetComponent / GetComponentInParent / GetComponentInChildren, whichever fits its
/// context — e.g. <c>hit.GetComponentInParent&lt;ICharacter&gt;().Transform.GetComponentInChildren&lt;IVitalComponent&gt;()</c>.
/// </summary>
public interface ICharacter
{
    Transform Transform { get; }
}
