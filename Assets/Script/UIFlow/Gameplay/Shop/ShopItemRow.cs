using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>Một dòng trong cửa hàng (mua hoặc bán). Rê chuột để xem tooltip so sánh.</summary>
    public class ShopItemRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button actionButton;

        private ItemData _item;
        private Action<ItemData> _onHover;
        private Action _onExit;

        public void Bind(ItemData item, int price, string actionLabel, Action onAction, Action<ItemData> onHover, Action onExit)
        {
            _item = item;
            _onHover = onHover;
            _onExit = onExit;
            icon.sprite = item.icon;
            icon.color = item.icon != null ? Color.white : item.iconColor;
            nameText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(ItemTooltip.RarityColor(item.rarity))}>{item.displayName}</color>";
            priceText.text = $"<color=#FFD54A>{price}</color>";
            actionButton.GetComponentInChildren<TMP_Text>().text = actionLabel;
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => onAction());
        }

        public void OnPointerEnter(PointerEventData eventData) => _onHover?.Invoke(_item);

        public void OnPointerExit(PointerEventData eventData) => _onExit?.Invoke();
    }
}
