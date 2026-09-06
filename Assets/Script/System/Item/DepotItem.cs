using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Item Depot")]
public class DepotItem : ScriptableObject
{
    [SerializeField] public List<DepotItemSlot> listItems;

    public ItemSO GetRandomItemByRarity(RarityTierItem rarityTier)
    {
        List<ItemSO> itemsOfRarity = new List<ItemSO>();

        foreach (var itemSlot in listItems)
        {
            if (itemSlot.rarityTier == rarityTier)
            {
                itemsOfRarity.Add(itemSlot.depotItem);
            }
        }

        if (itemsOfRarity.Count > 0)
        {
            int randomIndex = Random.Range(0, itemsOfRarity.Count);
            return itemsOfRarity[randomIndex];
        }

        return null; // Không tìm thấy item với rarityTier tương ứng
    }

    public ItemSO GetRandomItem()
    {
        if (listItems.Count > 0)
        {
            RarityTierItem randomRarity = GetRandomRarity();
            return GetRandomItemByRarity(randomRarity);
        }

        return null; // Không có item trong danh sách
    }

    public RarityTierItem GetRandomRarity()
    {
        int randomValue = Random.Range(0, 100); // Lấy một số ngẫu nhiên từ 0 đến 99

        RarityTierItem rarityTier = RarityTierItem.Common;
        // Mặc định là Common
        // 100-95: Legendary (5%)
        // 95-80: Epic (15%)
        // 80-50: Rare (30%)
        // 50-0: Common (50%)
        if (randomValue >= RarityTierItem.Legendary.ValueRate()) rarityTier = RarityTierItem.Legendary;      // 95-99, 5%
        else if (randomValue >= RarityTierItem.Epic.ValueRate()) rarityTier = RarityTierItem.Epic;      // 80-94, 15%
        else if (randomValue >= RarityTierItem.Rare.ValueRate()) rarityTier = RarityTierItem.Rare;
        return rarityTier;
    }
    public ItemSO GetItemByRarity(RarityTierItem rarityTier)
    {
        foreach (var itemSlot in listItems)
        {
            if (itemSlot.rarityTier == rarityTier)
            {
                return itemSlot.depotItem;
            }
        }
        return null; // Không tìm thấy item với rarityTier tương ứng
    }
}
[System.Serializable]
public class DepotItemSlot
{
    [field: SerializeField] public ItemSO depotItem { get; private set; }
    [field: SerializeField] public RarityTierItem rarityTier { get; private set; }

}
[System.Serializable]
public enum RarityTierItem
{
    Common = 0,
    Rare = 50,
    Epic = 80,
    Legendary = 95
}
public static class RarityTierValue
{
    public static int ValueRate(this RarityTierItem tier) => (int)tier;
}
