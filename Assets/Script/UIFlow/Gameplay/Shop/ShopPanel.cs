using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Cửa hàng: cột trái = hàng của NPC (Mua), cột phải = túi đồ người chơi (Bán).
    /// Mọi thay đổi vàng / túi đồ đi qua IInventoryProvider; panel chỉ vẽ lại khi provider báo Changed.
    /// </summary>
    public class ShopPanel : UIPanel
    {
        [SerializeField] private Transform buyContainer;
        [SerializeField] private Transform sellContainer;
        [SerializeField] private ShopItemRow rowTemplate;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private ItemTooltip tooltip;
        [SerializeField] private Button closeButton;

        private readonly List<ShopItemRow> _rows = new();

        protected override void Awake()
        {
            base.Awake();
            rowTemplate.gameObject.SetActive(false);
            closeButton.onClick.AddListener(CloseSelf);
        }

        protected override void OnShown()
        {
            messageText.text = "Rê chuột lên món đồ để so sánh.";
            UIServices.Inventory.Changed += Rebuild;
            Rebuild();
        }

        protected override void OnHidden()
        {
            UIServices.Inventory.Changed -= Rebuild;
            tooltip.Hide();
        }

        private void Rebuild()
        {
            foreach (ShopItemRow row in _rows) Destroy(row.gameObject);
            _rows.Clear();

            IInventoryProvider inventory = UIServices.Inventory;
            goldText.text = $"<color=#FFD54A>{inventory.Gold}</color> vàng";

            foreach (ItemData item in inventory.GetShopStock())
            {
                ItemData captured = item;
                AddRow(buyContainer, item, item.buyPrice, "Mua", () =>
                {
                    inventory.Buy(captured, out string message);
                    messageText.text = message;
                });
            }

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                ItemData item = inventory.Items[i];
                if (item == null) continue;
                int index = i;
                AddRow(sellContainer, item, item.sellPrice, "Bán", () =>
                {
                    tooltip.Hide();   // Món vừa bán biến mất → ẩn tooltip của nó
                    inventory.Sell(index, out string message);
                    messageText.text = message;
                });
            }
        }

        private void AddRow(Transform container, ItemData item, int price, string label, System.Action onAction)
        {
            ShopItemRow row = Instantiate(rowTemplate, container);
            row.gameObject.SetActive(true);
            row.Bind(item, price, label, onAction,
                hovered => tooltip.Show(hovered, UIServices.Inventory.GetEquipped(hovered.slotType), showSellPrice: container == sellContainer),
                tooltip.Hide);
            _rows.Add(row);
        }
    }
}
