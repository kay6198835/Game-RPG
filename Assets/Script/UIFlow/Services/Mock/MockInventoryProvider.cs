using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Túi đồ giả 24 ô + 500 vàng + cửa hàng mẫu.
    /// TODO: nối logic thật — project hiện chưa có hệ thống túi đồ (chỉ có ItemSO / nhặt đồ).
    /// </summary>
    public class MockInventoryProvider : IInventoryProvider
    {
        private readonly ItemData[] _items;
        private readonly Dictionary<ItemSlotType, ItemData> _equipped = new();
        private readonly List<ItemData> _shopStock = MockCatalog.CreateShopStock();

        public event Action Changed;

        public int Capacity => _items.Length;

        public IReadOnlyList<ItemData> Items => _items;

        public int Gold { get; private set; } = 500;

        public MockInventoryProvider(int capacity = 24)
        {
            _items = new ItemData[capacity];
            List<ItemData> start = MockCatalog.CreateInventoryItems();
            for (int i = 0; i < start.Count && i < capacity; i++) _items[i] = start[i];

            foreach (ItemData item in MockCatalog.CreateEquipped()) _equipped[item.slotType] = item;
        }

        public ItemData GetEquipped(ItemSlotType slotType)
        {
            return _equipped.TryGetValue(slotType, out ItemData item) ? item : null;
        }

        public void SwapSlots(int fromIndex, int toIndex)
        {
            if (!IsValid(fromIndex) || !IsValid(toIndex) || fromIndex == toIndex) return;
            (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);
            Changed?.Invoke();
        }

        public bool Equip(int index)
        {
            if (!IsValid(index) || _items[index] == null || !_items[index].IsEquipment) return false;

            ItemData item = _items[index];
            _items[index] = GetEquipped(item.slotType);   // Món cũ (hoặc null) quay về ô vừa trống
            _equipped[item.slotType] = item;
            Changed?.Invoke();
            return true;
        }

        public IReadOnlyList<ItemData> GetShopStock() => _shopStock;

        public bool Buy(ItemData shopItem, out string message)
        {
            if (shopItem == null) { message = "Chưa chọn món nào."; return false; }
            if (Gold < shopItem.buyPrice) { message = "Không đủ vàng."; return false; }

            int empty = Array.IndexOf(_items, null);
            if (empty < 0) { message = "Túi đồ đã đầy."; return false; }

            Gold -= shopItem.buyPrice;
            _items[empty] = shopItem.Clone();
            message = "Đã mua " + shopItem.displayName + ".";
            Changed?.Invoke();
            return true;
        }

        public bool Sell(int index, out string message)
        {
            if (!IsValid(index) || _items[index] == null) { message = "Ô này trống."; return false; }

            ItemData item = _items[index];
            Gold += item.sellPrice;
            _items[index] = null;
            message = "Đã bán " + item.displayName + " được " + item.sellPrice + " vàng.";
            Changed?.Invoke();
            return true;
        }

        private bool IsValid(int index) => index >= 0 && index < _items.Length;
    }
}
