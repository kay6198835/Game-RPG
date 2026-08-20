using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatSlot : MonoBehaviour
{
    [SerializeField] public StatsViewDTO statViewDTO;
    [SerializeField] public StatType statType;
    [SerializeField] public TMPro.TextMeshProUGUI statNameText;
    [SerializeField] public TMPro.TextMeshProUGUI finalValueText;
    [SerializeField] public TMPro.TextMeshProUGUI bonusValueText;
    [SerializeField] public Button bottonIncreaseStat;
    [SerializeField] public Button bottonDecreaseStat;
    [SerializeField] public int levelUpBonusValue;
    [SerializeField] public int totalLevelUpBonusValue;
    void Awake()
    {

    }
    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, UpdateTotalLevelUpBonusValue);
        if (bottonIncreaseStat) bottonIncreaseStat.onClick.AddListener(IncreaseStat);
        if (bottonDecreaseStat) bottonDecreaseStat.onClick.AddListener(DecreaseStat);
    }
    void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, UpdateTotalLevelUpBonusValue);
        levelUpBonusValue = 0;
    }
    public void UpdateStatSlot(StatsViewDTO statViewDTO)
    {
        this.statViewDTO = statViewDTO;
        statType = statViewDTO.StatType;
        statNameText.text = GameConstants.StatTypeName[statType];
        finalValueText.text = statViewDTO.FinalValue.ToString();
        bonusValueText.text = "(+ " + statViewDTO.BonusValue.ToString() + ")";
    }
    void UpdateTotalLevelUpBonusValue(object obj = null)
    {
        if (!statType.IsPrimary()) return;
        totalLevelUpBonusValue = (int)obj;
        bottonIncreaseStat.gameObject.SetActive(totalLevelUpBonusValue > 0);
        bottonDecreaseStat.gameObject.SetActive(levelUpBonusValue > 0);
    }
    public void DecreaseStat()
    {
        levelUpBonusValue--;
        EventManager.Emit(EventID.ON_DECREASE_STATS_BY_UI, statType);
    }

    public void IncreaseStat()
    {
        levelUpBonusValue++;
        EventManager.Emit(EventID.ON_INCREASE_STATS_BY_UI, statType);
    }
}
