using System.Collections.Generic;
using UnityEngine;

public interface IAbilityOwner
{
    Transform Transform { get; }
    float GetCurrentStatValue(VitalStatType statType);
    float GetCurrentStatValue(StatType statType);
    void PayCost(VitalStatType statType, float amount);
    Vector2 DirectorForward();
    int DirectorForwardIndex();
    Vector2 TargetPosition();
}
