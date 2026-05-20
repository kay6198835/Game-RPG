using UnityEngine;

public interface IAbilityOwner
{
    Transform Transform { get; }
    CharacterStats Stats { get; }
    Health Health { get; }
    SimpleCharacterMotor Motor { get; }
}
