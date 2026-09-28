using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Một ô túi đồ. Dùng các interface của EventSystem để kéo thả — không cần tự viết raycast:
    /// - IBeginDrag / IDrag / IEndDrag: ô bị kéo đi.
    /// - IDrop: ô nhận món được thả vào.
    /// - IPointerEnter / Exit: hiện / ẩn tooltip.  - IPointerClick: chuột phải để mặc đồ.
    /// Ô chỉ báo cho InventoryPanel; panel mới là nơi đổi dữ liệu.
    /// </summary>
    public class InventorySlot : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image rarityFrame;
        [SerializeField] private TMP_Text countText;

        private InventoryPanel _owner;

        public int Index { get; private set; }
        public ItemData Item { get; private set; }

        public void Init(InventoryPanel owner, int index)
        {
            _owner = owner;
            Index = index;
        }

        public void SetItem(ItemData item)
        {
            Item = item;
            bool hasItem = item != null;
            icon.enabled = hasItem;
            if (hasItem)
            {
                icon.sprite = item.icon;
                icon.color = item.icon != null ? Color.white : item.iconColor;   // Chưa có sprite → ô màu
            }
            rarityFrame.color = hasItem ? ItemTooltip.RarityColor(item.rarity) : new Color(1f, 1f, 1f, 0.1f);
            countText.text = hasItem && item.stackCount > 1 ? item.stackCount.ToString() : string.Empty;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item == null || eventData.button != PointerEventData.InputButton.Left) return;
            _owner.BeginDrag(this);
        }

        // Bắt buộc phải có OnDrag thì Unity mới gửi OnBeginDrag / OnEndDrag.
        public void OnDrag(PointerEventData eventData)
        {
            _owner.UpdateDrag(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _owner.EndDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            _owner.DropOn(this);
        }

        public void OnPointerEnter(PointerEventData eventData) => _owner.ShowTooltip(this);

        public void OnPointerExit(PointerEventData eventData) => _owner.HideTooltip();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right) _owner.EquipFrom(this);
        }
    }
}
