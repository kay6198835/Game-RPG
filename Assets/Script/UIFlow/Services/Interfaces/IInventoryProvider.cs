using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>Túi đồ, trang bị, vàng và cửa hàng.</summary>
    public interface IInventoryProvider
    {
        /// <summary>Số ô trong túi. Items có đúng Capacity phần tử, ô trống = null.</summary>
        int Capacity { get; }

        IReadOnlyList<ItemData> Items { get; }

        int Gold { get; }

        /// <summary>Món đang mặc ở vị trí slotType, null nếu chưa mặc. Dùng để so sánh trong tooltip.</summary>
        ItemData GetEquipped(ItemSlotType slotType);

        /// <summary>Kéo thả: đổi chỗ 2 ô trong túi.</summary>
        void SwapSlots(int fromIndex, int toIndex);

        /// <summary>Mặc món ở ô index; món đang mặc (nếu có) quay về ô đó.</summary>
        bool Equip(int index);

        IReadOnlyList<ItemData> GetShopStock();

        bool Buy(ItemData shopItem, out string message);

        bool Sell(int index, out string message);

        event Action Changed;
    }
}
