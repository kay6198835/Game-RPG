using UnityEngine;

/// <summary>
/// The one supported path from a hit collider to the character that owns it.
/// </summary>
public static class CharacterLookup
{
    /// <summary>
    /// Resolves the character that owns a HURTBOX. A collider counts as a hurtbox only when its
    /// GameObject carries the character's damage receiver; any other collider under a character
    /// (weapon hitbox, pickup trigger) resolves to nothing, so one hit never lands twice on the same
    /// character. Non-allocating; no FindObjectOfType, no singleton.
    /// </summary>
    public static bool TryGetCharacter(this Component hit, out ICharacter character)
    {
        character = null;
        if (hit == null || !hit.TryGetComponent(out INegativeReceiver _)) return false;
        character = hit.GetComponentInParent<ICharacter>();
        return character != null;
    }
}
