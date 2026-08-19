using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(ObjectPoolManager))]
public class StatsUIController : MonoBehaviour
{
    public StatSlot PrimaryStatSlotPrefab;
    public StatSlot DerivedStatSlotPrefab;
    public Dictionary<StatType, StatSlot> ListDerivedStat = new();
    public Dictionary<StatType, StatSlot> ListPrimaryStat = new();
    public GameObject primaryStatSlotContainer;
    public GameObject derivedStatSlotContainer;
    public GameObject UIPannel;
    public StatsSO statsSO;
    ObjectPoolManager objectPoolManager;
    Dictionary<StatType, int> gainKeyValues;
    public int totalLevelUpBonusValue;
    void Awake()
    {
        objectPoolManager = GetComponent<ObjectPoolManager>();
        GetStatsUI(statsSO);
    }
    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_INCREASE_STATS_BY_UI, IncreaseStat);
        EventManager.Resgister(EventID.ON_DECREASE_STATS_BY_UI, DecreaseStat);
        EventManager.Resgister(EventID.ON_UPDATE_STATS_BY_UI, GetStatsViewModel);
    }
    void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_INCREASE_STATS_BY_UI, IncreaseStat);
        EventManager.UnResgister(EventID.ON_DECREASE_STATS_BY_UI, DecreaseStat);
        EventManager.UnResgister(EventID.ON_UPDATE_STATS_BY_UI, GetStatsViewModel);
    }
    private void GetStatsUI(StatsSO stats)
    {
        foreach (var stat in stats.statViewDTOs)
        {
            GameObject statSlotObject = objectPoolManager.Spawn(Vector2.zero, Quaternion.identity,
            stat.Key.IsPrimary() ? PrimaryStatSlotPrefab.gameObject : DerivedStatSlotPrefab.gameObject,
            stat.Key.IsPrimary() ? primaryStatSlotContainer.transform : derivedStatSlotContainer.transform);
            StatSlot statSlot = statSlotObject.GetComponent<StatSlot>();
            statSlot.statViewDTO = stat.Value;
            (stat.Key.IsPrimary() ? ListPrimaryStat : ListDerivedStat)[stat.Key] = statSlot;
            statSlot.UpdateStatSlot(stat.Value);
        }
        CloseStatsUI();
    }

    public void CloseStatsUI()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            objectPoolManager.Get(PrimaryStatSlotPrefab.gameObject).Release(statSlot.gameObject);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            objectPoolManager.Get(DerivedStatSlotPrefab.gameObject).Release(statSlot.gameObject);
        }
        foreach (var (type, amount) in gainKeyValues)
        {
            statsSO.AddPrimaryPoint(type, -amount);
        }
        gainKeyValues.Clear();
        totalLevelUpBonusValue = 0;
    }

    public void OpenStatsUI()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            StatsViewDTO statsViewDTO = statsSO.statViewDTOs[statSlot.statType];
            statSlot.UpdateStatSlot(statsViewDTO);
            objectPoolManager.Get(PrimaryStatSlotPrefab.gameObject).Reload(Vector2.one, primaryStatSlotContainer.transform);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            StatsViewDTO statsViewDTO = statsSO.statViewDTOs[statSlot.statType];
            statSlot.UpdateStatSlot(statsViewDTO);
            objectPoolManager.Get(DerivedStatSlotPrefab.gameObject).Reload(Vector2.one, derivedStatSlotContainer.transform);
        }


        totalLevelUpBonusValue = statsSO.StatUnusedBonus;
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, totalLevelUpBonusValue);
    }
    public void UpdateViewRunTime()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            StatsViewDTO statsViewDTO = statsSO.statViewDTOs[statSlot.statType];
            statSlot.UpdateStatSlot(statsViewDTO);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            StatsViewDTO statsViewDTO = statsSO.statViewDTOs[statSlot.statType];
            statSlot.UpdateStatSlot(statsViewDTO);
        }
    }

    private void GetStatsViewModel(object obj = null)
    {
        foreach (var (type, amount) in gainKeyValues)
        {
            statsSO.AddPrimaryPoint(type, amount);
        }
    }

    public void IncreaseStat(object obj = null)
    {
        StatType statType = (StatType)obj;
        if (!gainKeyValues.TryGetValue(statType, out int value))
        {
            value = 1;
            gainKeyValues.Add(statType, value);
        }
        else
        {
            value++;
            gainKeyValues[statType] = value;
        }
        statsSO.AddPrimaryPoint(statType, 1);
        totalLevelUpBonusValue--;
        UpdateViewRunTime();
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, totalLevelUpBonusValue);
    }

    public void DecreaseStat(object obj = null)
    {
        StatType statType = (StatType)obj;
        if (gainKeyValues.TryGetValue(statType, out int value) && value > 0)
        {
            value--;
            gainKeyValues[statType] = value;
        }
        statsSO.AddPrimaryPoint(statType, -1);
        totalLevelUpBonusValue++;
        UpdateViewRunTime();
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, totalLevelUpBonusValue);
    }
}