using System.Collections.Generic;
using UnityEngine;

public enum AbilityActivationType
{
    Active,
    Hold,
    Passive
}

[CreateAssetMenu(menuName = "Game/Abilities/Ability Definition")]
public class AbilityDefinition : ScriptableObject
{
    [Header("Info")]
    public string Id;
    public string DisplayName;
    public Sprite Icon;

    [Header("Activation")]
    public AbilityActivationType ActivationType = AbilityActivationType.Active;
    public KeyCode DefaultKey = KeyCode.None;

    [Header("Cost & Cooldown")]
    public float Cooldown = 1f;
    public float ManaCost = 0f;

    [Header("Hold")]
    public float MaxHoldTime = 0f;

    [Header("Conditions")]
    public List<AbilityConditionDefinition> Conditions = new();

    [Header("Effects")]
    public List<AbilityEffectDefinition> Effects = new();
}
