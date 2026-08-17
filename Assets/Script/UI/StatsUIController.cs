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
    public ObjectPoolManager objectPoolManager;
    void Awake()
    {
        objectPoolManager = GetComponent<ObjectPoolManager>();
        GetStatsUI(statsSO);
    }

    private void GetStatsUI(StatsSO stats)
    {
        foreach (var stat in stats.statViewDTOs)
        {
            GameObject statSlotObject = objectPoolManager.Spawn(Vector2.zero, Quaternion.identity,
            StatTypeExtensions.IsPrimary(stat.Key) ? PrimaryStatSlotPrefab.gameObject : DerivedStatSlotPrefab.gameObject,
            StatTypeExtensions.IsPrimary(stat.Key) ? primaryStatSlotContainer.transform : derivedStatSlotContainer.transform);
            StatSlot statSlot = statSlotObject.GetComponent<StatSlot>();
            statSlot.statViewDTO = stat.Value;
            (StatTypeExtensions.IsPrimary(stat.Key) ? ListPrimaryStat : ListDerivedStat)[stat.Key] = statSlot;
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
    }


}