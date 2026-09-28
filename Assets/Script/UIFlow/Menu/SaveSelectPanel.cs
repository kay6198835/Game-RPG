using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Chọn file save. Xóa cần bấm 2 lần (lần đầu chỉ hỏi lại) để tránh xóa nhầm.
    /// </summary>
    public class SaveSelectPanel : UIPanel
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private SaveSlotView slotTemplate;
        [SerializeField] private TMP_Text emptyText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button backButton;

        private readonly List<SaveSlotView> _views = new();
        private int _pendingDeleteSlot = -1;

        public event Action<SaveSlotData> SaveChosen;

        protected override void Awake()
        {
            base.Awake();
            slotTemplate.gameObject.SetActive(false);
            backButton.onClick.AddListener(CloseSelf);
        }

        protected override void OnShown()
        {
            _pendingDeleteSlot = -1;
            messageText.text = string.Empty;
            Rebuild();
        }

        private void Rebuild()
        {
            foreach (SaveSlotView view in _views) Destroy(view.gameObject);
            _views.Clear();

            IReadOnlyList<SaveSlotData> slots = UIServices.Save.GetSlots();
            emptyText.gameObject.SetActive(slots.Count == 0);

            foreach (SaveSlotData slot in slots)
            {
                SaveSlotView view = Instantiate(slotTemplate, listContainer);
                view.gameObject.SetActive(true);
                view.Bind(slot, OnLoad, OnDelete);
                _views.Add(view);
            }
        }

        private void OnLoad(SaveSlotData slot)
        {
            UIServices.Save.SelectedSlot = slot.slotIndex;
            UIServices.OnSaveSelected();
            SaveChosen?.Invoke(slot);
        }

        private void OnDelete(SaveSlotData slot)
        {
            if (_pendingDeleteSlot != slot.slotIndex)
            {
                _pendingDeleteSlot = slot.slotIndex;
                messageText.text = $"Bấm Xóa lần nữa để xóa vĩnh viễn save của {slot.characterName}.";
                return;
            }

            UIServices.Save.Delete(slot.slotIndex);
            _pendingDeleteSlot = -1;
            messageText.text = $"Đã xóa save của {slot.characterName}.";
            Rebuild();
        }
    }
}
