using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SimpleCharacterMotor))]
public class PlayerAbilityOwner : MonoBehaviour, IAbilityOwner
{
    public CharacterStats Stats { get; private set; }
    public Health Health { get; private set; }
    public SimpleCharacterMotor Motor { get; private set; }

    Transform IAbilityOwner.Transform => transform;

    private void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Health = GetComponent<Health>();
        Motor = GetComponent<SimpleCharacterMotor>();
    }
}
