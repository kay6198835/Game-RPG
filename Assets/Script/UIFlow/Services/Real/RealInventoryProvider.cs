using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Túi đồ thật: để trống vì gameplay chưa có hệ thống túi đồ.
    /// TODO: nối logic thật — bọc hệ thống túi đồ của gameplay khi có (ItemSO → ItemData).
    /// </summary>
    public class RealInventoryProvider : IInventoryProvider
    {
        private readonly ItemData[] _items = new ItemData[24];
        private readonly List<ItemData> _shopStock = new();

        public event Action Changed { add { } remove { } }   // Chưa có gì để báo

        public int Capacity => _items.Length;

        public IReadOnlyList<ItemData> Items => _items;

        public int Gold => 0;

        public ItemData GetEquipped(ItemSlotType slotType) => null;

        public void SwapSlots(int fromIndex, int toIndex) { }

        public bool Equip(int index) => false;

        public IReadOnlyList<ItemData> GetShopStock() => _shopStock;

        public bool Buy(ItemData shopItem, out string message)
        {
            message = "Cửa hàng chưa được nối với gameplay.";
            return false;
        }

        public bool Sell(int index, out string message)
        {
            message = "Cửa hàng chưa được nối với gameplay.";
            return false;
        }
    }
}
