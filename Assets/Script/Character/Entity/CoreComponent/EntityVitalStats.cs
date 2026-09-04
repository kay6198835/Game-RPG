using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

//Multiple inheritance interface
//Affect max stat use modifier(statHandler)
//Affect current stats use StatType(Dictionary)
public class EntityVitalStats : EntityCoreComponent<EntityCore>
{
    Dictionary<StatType, float> currentStats = new();
    EntityStatsHandler statHandler;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        Reborn();
    }

    void OnEnable()
    {
        Reborn();
    }

    public void Reborn()
    {
        Core.GetCoreComponent(out statHandler);
        currentStats = statHandler.GetFullStat();
    }

    public float GetCurrentStatValue(StatType statType)
    {
        return currentStats[statType];
    }

    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statModifierGroup.Apply(statHandler.AddModifiersFromSource, this);
    }

    public void ReceiverRecovery(StatType statType, float amount)
    {
        if (currentStats[statType] + amount >= statHandler.GetStatValue(statType))
        {
            currentStats[statType] = statHandler.GetStatValue(statType);
        }
        else
        {
            currentStats[statType] += amount;
        }
    }

    public void ReceiveReduction(StatType statType, float amount)
    {
        if (currentStats[statType] - amount <= 0)
        {
            currentStats[statType] = 0;
        }
        else
        {
            currentStats[statType] -= amount;
        }
    }

    public void DebuffForDuration(StatModifierGroup statModifierGroup, float duration)
    {
        StartCoroutine(ApplyDebuffForDuration(statModifierGroup, duration));
    }

    IEnumerator ApplyDebuffForDuration(StatModifierGroup statModifierGroup, float duration)
    {
        statModifierGroup.Apply(statHandler.AddModifiersFromSource, this);
        yield return new WaitForSeconds(duration);
        statModifierGroup.Remmove(statHandler.RemoveModifiersFromSource, this);
    }
}