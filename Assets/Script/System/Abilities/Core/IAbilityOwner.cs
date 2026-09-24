using UnityEngine;

public interface IAbilityOwner
{
    Transform Transform { get; }
    /// <summary>The character casting — its Stats/Vital are what the ability reads and pays from.</summary>
    ICharacter Character { get; }
    float GetCurrentStatValue(StatType statType);
    void PayCost(StatType statType, float amount);
    Vector2 DirectorForward();
    Vector2 TargetPosition();
}
