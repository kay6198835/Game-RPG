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
    void Awake()
    {

    }
    void OnEnable()
    {
        EventManager.Resgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, UpdateTotalLevelUpBonusValue);
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
        finalValueText.text = statViewDTO.FinalValue.ToString(StatsViewDTO.DISPLAY_FORMAT);
        bonusValueText.text = "(+ " + statViewDTO.EquipmentValue.ToString(StatsViewDTO.DISPLAY_FORMAT) + ")";
    }
    void UpdateTotalLevelUpBonusValue(object obj = null)
    {
        if (!statType.IsPrimary()) return;
        if (bottonIncreaseStat == null || bottonDecreaseStat == null) return;
        if (!(obj is StatBonusViewDTO bonusViewDTO)) return;

        // Trần điểm của RIÊNG slot này = điểm nó đã cộng + điểm còn lại của cả phiên.
        // Nhờ vậy `levelUpBonusValue < totalLevelUpBonusValue` đúng bằng "cả phiên còn điểm",
        // kể cả khi người chơi rải điểm sang nhiều stat khác.
        this.totalLevelUpBonusValue = levelUpBonusValue + bonusViewDTO.RemainingBonus;

        // Hiện/ẩn theo tổng điểm bonus của phiên (TotalBonus), KHÔNG theo số điểm còn lại:
        // ẩn theo số còn lại sẽ nuốt luôn nút giảm ngay khi người chơi tiêu hết điểm,
        // và không còn cách nào hoàn tác trước khi bấm Update.
        bool hasBonusThisSession = bonusViewDTO.TotalBonus > 0;
        bottonIncreaseStat.gameObject.SetActive(hasBonusThisSession);
        bottonDecreaseStat.gameObject.SetActive(hasBonusThisSession);

        bottonIncreaseStat.interactable = levelUpBonusValue < totalLevelUpBonusValue;
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
