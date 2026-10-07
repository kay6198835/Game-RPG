using UnityEngine;

/// <summary>
/// Shared base of Player and Entity (the BaseGrid&lt;T&gt; of the character layer). Owns the typed core
/// hub reference and marks the root as an <see cref="ICharacter"/>. It does not hand out components.
/// </summary>
public abstract class CharacterBase<TCore> : BaseEntity, ICharacter where TCore : CoreBase
{
    [SerializeField] protected TCore core;

    public TCore Core => core;
    public Transform Transform => transform;
}
