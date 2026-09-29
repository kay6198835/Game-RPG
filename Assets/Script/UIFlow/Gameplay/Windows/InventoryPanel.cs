using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Trang Túi đồ: lưới ô (GridLayoutGroup), kéo thả để đổi chỗ, chuột phải để mặc, tooltip so sánh chỉ số.
    /// Khi kéo, một "icon ma" (dragIcon) đi theo chuột; ô gốc mờ đi. Icon ma không chặn raycast,
    /// nếu không ô đích sẽ không nhận được OnDrop.
    /// </summary>
    public class InventoryPanel : TabPage
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private InventorySlot slotTemplate;
        [SerializeField] private Image dragIcon;
        [SerializeField] private ItemTooltip tooltip;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text equippedText;

        private static readonly ItemSlotType[] EquipSlots = { ItemSlotType.Weapon, ItemSlotType.Armor, ItemSlotType.Helmet, ItemSlotType.Accessory };

        private readonly List<InventorySlot> _slots = new();
        private readonly StringBuilder _builder = new();
        private InventorySlot _dragging;

        public override string Title => "Túi đồ";

        private void Awake()
        {
            slotTemplate.gameObject.SetActive(false);
            dragIcon.raycastTarget = false;
            dragIcon.gameObject.SetActive(false);
        }

        private void OnEnable() => UIServices.Inventory.Changed += Refresh;

        private void OnDisable()
        {
            UIServices.Inventory.Changed -= Refresh;
            EndDrag();
            if (tooltip != null) tooltip.Hide();
        }

        public override void OnTabShown()
        {
            if (_slots.Count != UIServices.Inventory.Capacity) BuildGrid();
            Refresh();
        }

        public override void OnTabHidden() => tooltip.Hide();

        private void BuildGrid()
        {
            foreach (InventorySlot slot in _slots) Destroy(slot.gameObject);
            _slots.Clear();
            for (int i = 0; i < UIServices.Inventory.Capacity; i++)
            {
                InventorySlot slot = Instantiate(slotTemplate, gridContainer);
                slot.gameObject.SetActive(true);
                slot.Init(this, i);
                _slots.Add(slot);
            }
        }

        private void Refresh()
        {
            IInventoryProvider inventory = UIServices.Inventory;
            for (int i = 0; i < _slots.Count; i++) _slots[i].SetItem(i < inventory.Items.Count ? inventory.Items[i] : null);
            goldText.text = $"<color=#FFD54A>{inventory.Gold}</color> vàng";

            _builder.Clear();
            _builder.Append("<b>Đang mặc</b>\n");
            foreach (ItemSlotType type in EquipSlots)
            {
                ItemData item = inventory.GetEquipped(type);
                _builder.Append(ItemTooltip.SlotName(type)).Append(": ");
                _builder.Append(item != null
                    ? $"<color=#{ColorUtility.ToHtmlStringRGB(ItemTooltip.RarityColor(item.rarity))}>{item.displayName}</color>"
                    : "<color=#777>—</color>");
                _builder.Append('\n');
            }
            equippedText.text = _builder.ToString();
        }

        public void BeginDrag(InventorySlot slot)
        {
            _dragging = slot;
            tooltip.Hide();
            dragIcon.gameObject.SetActive(true);
            dragIcon.transform.SetAsLastSibling();
            dragIcon.sprite = slot.Item.icon;
            dragIcon.color = slot.Item.icon != null ? Color.white : slot.Item.iconColor;
            slot.GetComponent<CanvasGroup>().alpha = 0.35f;
        }

        public void UpdateDrag(Vector2 screenPosition)
        {
            if (_dragging != null) dragIcon.transform.position = screenPosition;
        }

        public void DropOn(InventorySlot target)
        {
            if (_dragging == null || target == _dragging) return;
            UIServices.Inventory.SwapSlots(_dragging.Index, target.Index);   // Provider bắn Changed → Refresh
        }

        public void EndDrag()
        {
            if (_dragging != null) _dragging.GetComponent<CanvasGroup>().alpha = 1f;
            _dragging = null;
            if (dragIcon != null) dragIcon.gameObject.SetActive(false);
        }

        public void EquipFrom(InventorySlot slot)
        {
            if (slot.Item == null) return;
            string itemName = slot.Item.displayName;
            if (!UIServices.Inventory.Equip(slot.Index)) return;

            UIEvents.Notify("Đã mặc " + itemName);
            ShowTooltip(slot);   // Ô giờ chứa món cũ → cập nhật tooltip
        }

        public void ShowTooltip(InventorySlot slot)
        {
            if (_dragging != null || slot.Item == null) { tooltip.Hide(); return; }
            tooltip.Show(slot.Item, UIServices.Inventory.GetEquipped(slot.Item.slotType), showSellPrice: true);
        }

        public void HideTooltip() => tooltip.Hide();
    }
}
