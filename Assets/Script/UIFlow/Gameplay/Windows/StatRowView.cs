using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Một dòng chỉ số trong tab Nhân vật: tên, giá trị, phần cộng từ trang bị, và nút −/+ (chỉ số chính).
    /// Dòng chỉ báo "người chơi bấm +/−" cho CharacterPanel; panel mới là nơi gọi provider.
    /// </summary>
    public class StatRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private TMP_Text bonusText;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;

        private Action<StatType, int> _onChange;

        public StatType StatType { get; private set; }

        public void Init(StatType statType, Action<StatType, int> onChange)
        {
            StatType = statType;
            _onChange = onChange;
            if (decreaseButton != null) decreaseButton.onClick.AddListener(() => _onChange?.Invoke(StatType, -1));
            if (increaseButton != null) increaseButton.onClick.AddListener(() => _onChange?.Invoke(StatType, +1));
        }

        /// <param name="showSteps">Hiện cặp −/+ trong cả phiên khi phiên mở ra có điểm, kể cả lúc đã tiêu hết
        /// điểm — nếu ẩn ngay khi về 0 thì người chơi không còn cách trừ lại.</param>
        public void Bind(StatRowData row, bool showSteps, bool canIncrease)
        {
            nameText.text = row.statName;
            valueText.text = row.finalValue.ToString("0.#");
            if (bonusText != null)
            {
                bonusText.text = row.spentThisSession > 0 ? $"<color=#8F8>+{row.spentThisSession}</color>"
                    : row.equipmentValue != 0f ? $"(+ {row.equipmentValue:0.#})" : string.Empty;
            }

            bool steps = showSteps && row.isPrimary;
            if (decreaseButton != null)
            {
                decreaseButton.gameObject.SetActive(steps);
                decreaseButton.interactable = row.spentThisSession > 0;
            }
            if (increaseButton != null)
            {
                increaseButton.gameObject.SetActive(steps);
                increaseButton.interactable = canIncrease;
            }
        }
    }
}
