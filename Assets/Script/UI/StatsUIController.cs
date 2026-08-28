using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
public class StatsUIController : MonoBehaviour
{
    [SerializeField] private StatSlot PrimaryStatSlotPrefab;
    [SerializeField] private StatSlot DerivedStatSlotPrefab;
    [SerializeField] private Dictionary<StatType, StatSlot> ListDerivedStat = new();
    [SerializeField] private Dictionary<StatType, StatSlot> ListPrimaryStat = new();
    [SerializeField] private GameObject primaryStatSlotContainer;
    [SerializeField] private GameObject derivedStatSlotContainer;
    [SerializeField] private GameObject UIPannel;
    [SerializeField] private Button updateStatsButton;
    [SerializeField] private Button revertStatsButton;
    [SerializeField] private Button restoreStatsButton;
    [SerializeField] private Button openUIButton;
    [SerializeField] private Button closeUIButton;
    Dictionary<StatType, int> gainKeyValues = new();
    public int totalLevelUpBonusValue;
    public int runtimeLevelUpBonusValue;
    IObjecPoolService objectPoolManager;
    IPlayerStatService _playerStatService;
    [Inject]
    public void Construct(IPlayerStatService playerStatService, IObjecPoolService objectPoolManager)
    {
        _playerStatService = playerStatService;
        this.objectPoolManager = objectPoolManager;
    }
    void Awake()
    {

    }
    void Start()
    {
        GetStatsUI();
    }
    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_INCREASE_STATS_BY_UI, IncreaseStat);
        EventManager.Resgister(EventID.ON_DECREASE_STATS_BY_UI, DecreaseStat);
    }
    void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_INCREASE_STATS_BY_UI, IncreaseStat);
        EventManager.UnResgister(EventID.ON_DECREASE_STATS_BY_UI, DecreaseStat);
    }
    private void GetStatsUI()
    {
        var initialize = _playerStatService.GetFullViewStats();
        foreach (var stat in initialize)
        {
            GameObject statSlotObject = objectPoolManager.Spawn(Vector2.zero, Quaternion.identity,
            stat.Key.IsPrimary() ? PrimaryStatSlotPrefab.gameObject : DerivedStatSlotPrefab.gameObject,
            stat.Key.IsPrimary() ? primaryStatSlotContainer.transform : derivedStatSlotContainer.transform);
            StatSlot statSlot = statSlotObject.GetComponent<StatSlot>();
            statSlot.statViewDTO = stat.Value;
            (stat.Key.IsPrimary() ? ListPrimaryStat : ListDerivedStat)[stat.Key] = statSlot;
            statSlot.UpdateStatSlot(stat.Value);
        }
        DisableStatsSlot();
    }
    private void DisableStatsSlot()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            objectPoolManager.Release(statSlot.gameObject);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            objectPoolManager.Release(statSlot.gameObject);
        }
    }
    private void EnableStatsSlot()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            StatsViewDTO statsViewDTO = _playerStatService.GetViewStat(statSlot.statType);
            statSlot.UpdateStatSlot(statsViewDTO);
            objectPoolManager.Spawn(Vector2.one, Quaternion.identity, PrimaryStatSlotPrefab.gameObject, primaryStatSlotContainer.transform);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            StatsViewDTO statsViewDTO = _playerStatService.GetViewStat(statSlot.statType);
            statSlot.UpdateStatSlot(statsViewDTO);
            objectPoolManager.Spawn(Vector2.one, Quaternion.identity, DerivedStatSlotPrefab.gameObject, derivedStatSlotContainer.transform);
        }
    }
    public void CloseStatsUI()
    {
        DisableStatsSlot();
        RevertStat();
        gainKeyValues.Clear();
        totalLevelUpBonusValue = 0;
        runtimeLevelUpBonusValue = 0;
        UIPannel.SetActive(false);
        openUIButton.gameObject.SetActive(true);
    }

    public void RevertStat()
    {
        if (gainKeyValues == null || gainKeyValues.Count == 0) return;
        foreach (var (type, amount) in gainKeyValues)
        {
            _playerStatService.AddPrimaryPoint(type, -amount);
        }
    }
    public void OpenStatsUI()
    {
        EnableStatsSlot();
        UpdateViewButtonChangeStat();
        totalLevelUpBonusValue = _playerStatService.GetLevelUpStatsBonus();
        runtimeLevelUpBonusValue = totalLevelUpBonusValue;
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, totalLevelUpBonusValue);
        UIPannel.SetActive(true);
        openUIButton.gameObject.SetActive(false);
    }
    private void UpdateViewButtonChangeStat()
    {
        restoreStatsButton.gameObject.SetActive(_playerStatService.GetLevel() > 1);
        revertStatsButton.gameObject.SetActive(totalLevelUpBonusValue > runtimeLevelUpBonusValue);
        updateStatsButton.gameObject.SetActive(totalLevelUpBonusValue != runtimeLevelUpBonusValue);
    }
    private void UpdateViewRunTime()
    {
        foreach (var statSlot in ListPrimaryStat.Values)
        {
            StatsViewDTO statsViewDTO = _playerStatService.GetViewStat(statSlot.statType);
            statSlot.UpdateStatSlot(statsViewDTO);
        }

        foreach (var statSlot in ListDerivedStat.Values)
        {
            StatsViewDTO statsViewDTO = _playerStatService.GetViewStat(statSlot.statType);
            statSlot.UpdateStatSlot(statsViewDTO);
        }
        UpdateViewButtonChangeStat();
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
        _playerStatService.AddPrimaryPoint(statType, 1);
        runtimeLevelUpBonusValue--;
        UpdateViewRunTime();
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, runtimeLevelUpBonusValue);
    }

    public void DecreaseStat(object obj = null)
    {
        StatType statType = (StatType)obj;
        if (gainKeyValues.TryGetValue(statType, out int value) && value > 0)
        {
            value--;
            gainKeyValues[statType] = value;
        }
        _playerStatService.AddPrimaryPoint(statType, -1);
        runtimeLevelUpBonusValue++;
        UpdateViewRunTime();
        EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, runtimeLevelUpBonusValue);
    }

    public void AcceptUpdate()
    {
        DisableStatsSlot();
        gainKeyValues.Clear();
        totalLevelUpBonusValue = 0;
        runtimeLevelUpBonusValue = 0;
        UIPannel.SetActive(false);
        openUIButton.gameObject.SetActive(true);
        //_playerStatService.GetLevelUpStatsBonus();
    }
}