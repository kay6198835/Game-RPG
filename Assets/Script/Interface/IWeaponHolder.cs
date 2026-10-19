using UnityEngine;

/// <summary>
/// Anything a <see cref="Weapon"/> can be equipped to — the player's WeaponHolder or an enemy's
/// EntityWeaponHolder. The weapon reads its user only through this, never a Player/Entity type.
/// </summary>
public interface IWeaponHolder
{
    /// <summary>Where the weapon is parented.</summary>
    Transform Transform { get; }
    /// <summary>The character root — attack origin.</summary>
    Transform OwnerTransform { get; }
    Animator Animator { get; }
    IAimProvider Aim { get; }
    Weapon Weapon { get; }
    void Equid_UnEquid(Weapon weapon);
}
