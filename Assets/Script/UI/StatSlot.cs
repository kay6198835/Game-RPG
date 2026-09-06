using System;
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
    // Visibility latches to the pool the panel opened with, not to the live remaining
    // count: hiding the pair the moment the pool empties would strand the player with no
    // way to take a point back.
    private bool sessionPoolLatched;
    void Awake()
    {

    }
    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, UpdateTotalLevelUpBonusValue);
        sessionPoolLatched = false;
        // Held back until the first broadcast reports the pool - totalLevelUpBonusValue is
        // still last session's number at this point.
        if (!statType.IsPrimary()) return;
        SetStepButtonsVisible(false);
        if (bottonIncreaseStat) bottonIncreaseStat.onClick.AddListener(IncreaseStat);
        if (bottonDecreaseStat) bottonDecreaseStat.onClick.AddListener(DecreaseStat);
    }
    void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, UpdateTotalLevelUpBonusValue);
        levelUpBonusValue = 0;
        // Slots are pooled, so OnEnable runs again on reuse - without this the click
        // listeners stack and one press counts several times.
        if (!statType.IsPrimary()) return;
        if (bottonIncreaseStat) bottonIncreaseStat.onClick.RemoveListener(IncreaseStat);
        if (bottonDecreaseStat) bottonDecreaseStat.onClick.RemoveListener(DecreaseStat);
        SetStepButtonsVisible(false);
    }
    private void SetStepButtonsVisible(bool visible)
    {
        if (bottonIncreaseStat) bottonIncreaseStat.gameObject.SetActive(visible);
        if (bottonDecreaseStat) bottonDecreaseStat.gameObject.SetActive(visible);
    }
    public void UpdateStatSlot(StatsViewDTO statViewDTO)
    {
        this.statViewDTO = statViewDTO;
        statType = statViewDTO.StatType;
        statNameText.text = GameConstants.StatTypeName[statType];
        finalValueText.text = statViewDTO.FinalValue.ToString();
        bonusValueText.text = "(+ " + statViewDTO.EquipmentValue.ToString() + ")";
    }
    void UpdateTotalLevelUpBonusValue(object obj = null)
    {
        if (!statType.IsPrimary()) return;
        this.totalLevelUpBonusValue = (int)obj;

        if (!sessionPoolLatched)
        {
            sessionPoolLatched = true;
            SetStepButtonsVisible(totalLevelUpBonusValue > 0);
        }

        // Both stay on screen for the whole session; these conditions dim them instead.
        bottonIncreaseStat.interactable = totalLevelUpBonusValue > 0;
        bottonDecreaseStat.interactable = levelUpBonusValue > 0;
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
