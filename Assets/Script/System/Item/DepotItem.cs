using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Item SO/Item Depot")]
public class DepotItem : ScriptableObject
{
    [SerializeField] public List<DepotItemSlot> listItems = new List<DepotItemSlot>();

    [Tooltip("Mỗi TierRate là % tuyệt đối cho một lần roll. Tổng các dòng = tỷ lệ rơi đồ của kho này. " +
             "VD Common 50 + Epic 10 + Rare 5 → 65% rơi đồ, 35% không rơi gì.")]
    [SerializeField] private List<DropRateItemSlot> listTierValidItems = new List<DropRateItemSlot>();

    [Tooltip("Chỉ đọc. Tổng TierRate SAU KHI đã loại các tier không có item nào trong listItems.")]
    [SerializeField] private float rateDepotDropItem;

    private readonly List<DropRateItemSlot> _rollableTiers = new List<DropRateItemSlot>();
    private float _totalDropRate;
    private bool _cacheBuilt;

    private void OnEnable() => _cacheBuilt = false;

    private void OnValidate()
    {
        _cacheBuilt = false;
        FitRates();
    }

    /// <summary>Buộc dựng lại cache tier ở lần roll kế tiếp. Gọi sau khi sửa bảng từ code hoặc editor tool.</summary>
    public void InvalidateCache() => _cacheBuilt = false;

    /// <summary>Tỷ lệ rơi đồ thực tế (%), đã loại tier rate 0 và tier không có item.</summary>
    public float TotalDropRate
    {
        get
        {
            if (!_cacheBuilt) BuildCache();
            return _totalDropRate;
        }
    }

    /// <summary>Các tier thực sự tham gia roll. Dùng cho editor tool và chẩn đoán.</summary>
    public IReadOnlyList<DropRateItemSlot> RollableTiers
    {
        get
        {
            if (!_cacheBuilt) BuildCache();
            return _rollableTiers;
        }
    }

    /// <summary>Bảng tier designer khai, chưa lọc. Dùng cho editor tool để so với RollableTiers.</summary>
    public IReadOnlyList<DropRateItemSlot> DeclaredTiers => listTierValidItems;

    private void BuildCache()
    {
        _rollableTiers.Clear();
        _totalDropRate = 0f;

        foreach (var tier in listTierValidItems)
        {
            if (tier == null || tier.TierRate <= 0f) continue;
            if (!HasItemOfTier(tier.RarityTier)) continue;

            _rollableTiers.Add(tier);
            _totalDropRate += tier.TierRate;
        }

        _cacheBuilt = true;
    }

    public bool HasItemOfTier(RarityTierItem rarityTier)
    {
        foreach (var slot in listItems)
        {
            if (slot != null && slot.RarityTier == rarityTier && slot.DepotItem != null) return true;
        }
        return false;
    }

    /// <summary>
    /// Một lần roll cộng dồn quyết định cả "có rơi không" lẫn "rơi tier nào".
    /// Dải [0, TotalDropRate) chia cho các tier theo đúng TierRate; dải [TotalDropRate, 100) là không rơi.
    /// </summary>
    public bool TryRollItem(out ItemSO item, out RarityTierItem tier)
    {
        item = null;
        tier = RarityTierItem.Common;

        if (!_cacheBuilt) BuildCache();
        if (_rollableTiers.Count == 0) return false;

        float roll = Random.Range(0f, 100f);
        float acc = 0f;

        for (int i = 0; i < _rollableTiers.Count; i++)
        {
            acc += _rollableTiers[i].TierRate;
            if (roll >= acc) continue;

            tier = _rollableTiers[i].RarityTier;
            item = GetRandomItemByRarity(tier);
            return item != null;
        }

        return false;
    }

    public bool TryRollItem(out ItemSO item) => TryRollItem(out item, out _);

    public ItemSO GetRandomItem() => TryRollItem(out ItemSO item) ? item : null;

    // Reservoir sampling: chọn đều trong tier bằng một lượt duyệt, không cấp phát list tạm.
    // Đường này chạy mỗi lần quái chết nên phải zero-alloc (.claude/rules/gameplay-code.md).
    public ItemSO GetRandomItemByRarity(RarityTierItem rarityTier)
    {
        ItemSO chosen = null;
        int seen = 0;

        foreach (var slot in listItems)
        {
            if (slot == null || slot.RarityTier != rarityTier || slot.DepotItem == null) continue;

            seen++;
            if (Random.Range(0, seen) == 0) chosen = slot.DepotItem;
        }

        return chosen;
    }

    public ItemSO GetItemByRarity(RarityTierItem rarityTier)
    {
        foreach (var itemSlot in listItems)
        {
            if (itemSlot != null && itemSlot.RarityTier == rarityTier) return itemSlot.DepotItem;
        }
        return null;
    }

    private void FitRates()
    {
        BuildCache();
        rateDepotDropItem = _totalDropRate;

#if UNITY_EDITOR
        float declared = 0f;
        foreach (var tier in listTierValidItems)
        {
            if (tier == null) continue;
            declared += tier.TierRate;

            if (tier.TierRate > 0f && !HasItemOfTier(tier.RarityTier))
            {
                Debug.LogWarning(
                    $"[{nameof(DepotItem)}] {name}: tier {tier.RarityTier} khai TierRate {tier.TierRate} " +
                    $"nhưng listItems không có item nào thuộc tier này — tier bị loại khỏi tổng và khỏi vòng roll.",
                    this);
            }
        }

        if (declared > 100f)
        {
            Debug.LogWarning(
                $"[{nameof(DepotItem)}] {name}: tổng TierRate khai là {declared} > 100. " +
                $"Phần vượt quá 100 không bao giờ trúng — các tier ở cuối danh sách sẽ bị cắt cụt.",
                this);
        }
#endif
    }
}

[System.Serializable]
public class DepotItemSlot
{
    [field: SerializeField, FormerlySerializedAs("<depotItem>k__BackingField")]
    public ItemSO DepotItem { get; private set; }

    [field: SerializeField, FormerlySerializedAs("<rarityTier>k__BackingField")]
    public RarityTierItem RarityTier { get; private set; }
}

[System.Serializable]
public class DropRateItemSlot
{
    [field: SerializeField, Range(0f, 100f)] public float TierRate { get; private set; }
    [field: SerializeField] public RarityTierItem RarityTier { get; private set; }

    // Unity Inspector cần constructor không tham số để bấm "+" thêm dòng vào list.
    public DropRateItemSlot() { }

    public DropRateItemSlot(RarityTierItem rarityTier)
    {
        RarityTier = rarityTier;
    }
}

// Các giá trị số dưới đây chỉ còn là nhãn — giữ nguyên vì asset đã serialize theo chúng.
// Trước đây chúng là ngưỡng cộng dồn của GetRandomRarity(); cơ chế đó đã bị thay bằng
// TierRate khai tường minh trong DepotItem.listTierValidItems.
[System.Serializable]
public enum RarityTierItem
{
    Common = 0,
    Rare = 50,
    Epic = 80,
    Legendary = 95
}
