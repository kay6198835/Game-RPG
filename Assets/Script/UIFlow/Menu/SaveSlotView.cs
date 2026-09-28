using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>Một dòng trong màn chọn save: tên, class, level, thời gian chơi, vị trí.</summary>
    public class SaveSlotView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button deleteButton;

        public void Bind(SaveSlotData data, Action<SaveSlotData> onLoad, Action<SaveSlotData> onDelete)
        {
            titleText.text = $"{data.characterName}  <size=70%>{data.className} · Lv.{data.level}</size>";
            detailText.text = $"Thời gian chơi {data.PlayTimeText}   ·   {data.locationName}   ·   Lưu lúc {data.lastSavedText}";

            loadButton.onClick.RemoveAllListeners();
            deleteButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => onLoad(data));
            deleteButton.onClick.AddListener(() => onDelete(data));
        }
    }
}
