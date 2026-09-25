using UnityEngine;

/// <summary>
/// The intent source of one character — player input or enemy AI. Weapons and abilities read aim
/// through it; damage receivers notify it of a hit. Implemented via CharacterInputBase.
/// </summary>
public interface ICharacterInput : IAimProvider
{
    /// <summary>World point the character is aiming at (mouse for the player, target for an enemy).</summary>
    Vector2 AimPoint { get; }
    bool IsTakeDamage { get; }
    void OnTakeDamage(Vector2 attackPosition);
}
