using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatsUI : MonoBehaviour
{
    public StatSlot PrimaryStatSlotPrefab;
    public StatSlot DerivedStatSlotPrefab;
    public Dictionary<StatType, StatSlot> ListDerivedStat = new();
    public Dictionary<StatType, StatSlot> ListPrimaryStat = new();
    public GameObject primaryStatSlotContainer;
    public GameObject derivedStatSlotContainer;
    public StatsSO statsSO;

    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_OPEN_STATS_PLAYER_UI, OnOffUI);
    }
    void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_OPEN_STATS_PLAYER_UI, OnOffUI);
    }
    void Awake()
    {
        GetStatsUI(statsSO);
    }
    public void OnOffUI(object obj = null)
    {
        if ((bool)obj == true)
        {
            GetStatsUI(statsSO);
        }
        else
        {

        }
        this.gameObject.SetActive((bool)obj);
    }
    private void GetStatsUI(StatsSO stats)
    {
        foreach (var stat in stats.statViewDTOs)
        {
            StatSlot statSlot = new StatSlot();
            if (StatTypeExtensions.IsPrimary(stat.Key))
            {
                statSlot = Instantiate(PrimaryStatSlotPrefab, primaryStatSlotContainer.transform);
                statSlot.statViewDTO = stat.Value;
                ListPrimaryStat[stat.Key] = statSlot;
            }
            else
            {
                statSlot = Instantiate(DerivedStatSlotPrefab, derivedStatSlotContainer.transform);
                statSlot.statViewDTO = stat.Value;
                ListDerivedStat[stat.Key] = statSlot;
            }
            statSlot.UpdateStatSlot(stat.Value);
        }
    }

}