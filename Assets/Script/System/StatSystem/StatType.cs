using UnityEngine;

/// <summary>
/// Toàn bộ loại chỉ số trong game.
/// Primary (0–99): player cộng điểm trực tiếp.
/// Derived (100+): tính từ primary qua DerivedStatFormula, không cộng điểm trực tiếp.
/// </summary>
[System.Serializable]
public enum StatType
{
    [Header("Primary Stats")]
    // ----- Primary (chỉ số gốc) -----
    STR = 0,   // Sức mạnh
    DEX = 1,   // Nhanh nhẹn
    INT = 2,   // Trí tuệ
    VIT = 3,   // Thể chất
    LUK = 4,   // May mắn
    [Header("Derived Stats")]
    // ----- Derived (chỉ số dẫn xuất) -----
    HP = 100,
    Mana = 101,
    PhysicalDamage = 102,
    MagicDamage = 103,
    Defense = 104,
    AttackSpeed = 105,  // số đòn / giây
    CritChance = 106,  // đơn vị %: 5 = 5%
    CritDamage = 107,  // đơn vị %: 150 = x1.5
    MoveSpeed = 108,
    HPRegen = 109,  // HP / giây
    ManaRegen = 110,  // Mana / giây
    Evasion = 111,  // đơn vị %
    // NEED CHECK: new 2026-11-04. Distances in world units. Enemy AI reads them in EntityFindTarget
    // (was EntityFindTarget.minRange / maxRange on the prefab). Every BaseStatsSO asset carries a formula for both.
    MaxRangeAttack = 112,  // attack when the target is at most this far
    MinRangeAttack = 113,  // back off when the target is closer than this
}

public static class StatTypeExtensions
{
    public static bool IsPrimary(this StatType type) => (int)type < 100;
}