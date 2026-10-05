using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Hotbar kỹ năng giữa-dưới màn hình. Mỗi ô: màu/biểu tượng, phím, lớp phủ cooldown quay tròn.
    /// Hotbar không đọc input gameplay; ai dùng kỹ năng thì gọi UIEvents.UseSkill(index).
    /// </summary>
    public class SkillHotbar : MonoBehaviour
    {
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotTemplate;   // Con: "Icon" (Image), "Cooldown" (Image Filled Radial), "Key" (TMP), "Timer" (TMP)

        private class Slot
        {
            public Image cooldownOverlay;
            public TMP_Text timerText;
            public float remaining;
            public float duration;
        }

        private readonly List<Slot> _slots = new();

        private void Awake()
        {
            slotTemplate.SetActive(false);
        }

        public void Build(IReadOnlyList<SkillSlotData> skills)
        {
            foreach (Transform child in slotContainer)
            {
                if (child.gameObject != slotTemplate) Destroy(child.gameObject);
            }
            _slots.Clear();

            foreach (SkillSlotData skill in skills)
            {
                GameObject view = Instantiate(slotTemplate, slotContainer);
                view.SetActive(true);
                Image icon = view.transform.Find("Icon").GetComponent<Image>();
                if (skill.icon != null)
                {
                    icon.sprite = skill.icon;
                    icon.color = Color.white;
                    icon.preserveAspect = true;
                }
                else
                {
                    icon.color = skill.iconColor;
                }
                view.transform.Find("Key").GetComponent<TMP_Text>().text = skill.keyLabel;

                Slot slot = new()
                {
                    cooldownOverlay = view.transform.Find("Cooldown").GetComponent<Image>(),
                    timerText = view.transform.Find("Timer").GetComponent<TMP_Text>(),
                    duration = skill.cooldownSeconds,
                };
                slot.cooldownOverlay.fillAmount = 0f;
                slot.timerText.text = string.Empty;
                _slots.Add(slot);
            }
        }

        public void TriggerCooldown(int index)
        {
            if (index < 0 || index >= _slots.Count) return;
            Slot slot = _slots[index];
            if (slot.remaining > 0f) return;   // Đang hồi chiêu
            slot.remaining = slot.duration;
        }

        private void Update()
        {
            foreach (Slot slot in _slots)
            {
                if (slot.remaining <= 0f) continue;
                // deltaTime (không phải unscaled): cooldown đứng yên khi game tạm dừng.
                slot.remaining = Mathf.Max(0f, slot.remaining - Time.deltaTime);
                slot.cooldownOverlay.fillAmount = slot.duration > 0f ? slot.remaining / slot.duration : 0f;
                slot.timerText.text = slot.remaining > 0f ? slot.remaining.ToString(slot.remaining < 1f ? "0.0" : "0") : string.Empty;
            }
        }
    }
}
