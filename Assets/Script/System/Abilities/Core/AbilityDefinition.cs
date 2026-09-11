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
    [SerializeField] private string id;
    public string Id => id;
    public string DisplayName;
    public Sprite Icon;

    [Header("Activation")]
    public AbilityActivationType ActivationType = AbilityActivationType.Active;

    [Header("Cost & Cooldown")]
    public float Cooldown = 1f;
    public List<StatCost> Costs;

    [Header("Hold")]
    public float MaxHoldTime = 0f;

    [Header("Conditions")]
    public List<AbilityConditionDefinition> Conditions = new();

    [Header("Effects")]
    public List<AbilityEffectDefinition> Effects = new();

    [field: SerializeField] public AnimatorOverrideController AnimatorOverride { get; private set; }
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Chỉ gán 1 lần, không ghi đè nếu đã có
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            DisplayName = name;
        }
    }
#endif
    public float GetCostValues(StatType statType)
    {
        GetCostValues(statType, out float cost);
        return cost;
    }
    public void GetCostValues(StatType statType, out float cost)
    {
        cost = 0f;
        foreach (var statCost in Costs)
        {
            if (statCost.statType == statType)
            {
                cost = statCost.value;
                return;
            }
        }
    }
}
[System.Serializable]
public class StatCost
{
    public float value;
    public StatType statType;
}

public enum SkillState
{
    None,
    Start,
    Cast,
    Do,
    Exit
}
