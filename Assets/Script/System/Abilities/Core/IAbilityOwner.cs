using UnityEngine;

public interface IAbilityOwner
{
    Transform Transform { get; }
    float GetCurrentStatValue(StatType statType);
    void PayCost(StatType statType, float amount);
}
