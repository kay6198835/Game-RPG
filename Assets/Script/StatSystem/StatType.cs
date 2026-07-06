using System;
using System.Collections.Generic;

/// <summary>
/// Toàn bộ loại chỉ số trong game.
/// Primary (0–99): player cộng điểm trực tiếp.
/// Derived (100+): tính từ primary qua DerivedStatFormula, không cộng điểm trực tiếp.
/// </summary>
public enum StatType
{
    // ----- Primary (chỉ số gốc) -----
    STR = 0,   // Sức mạnh
    DEX = 1,   // Nhanh nhẹn
    INT = 2,   // Trí tuệ
    VIT = 3,   // Thể chất
    LUK = 4,   // May mắn

    // ----- Derived (chỉ số dẫn xuất) -----
    MaxHP = 100,
    MaxMana = 101,
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
}

public static class StatTypeExtensions
{
    public static bool IsPrimary(this StatType type) => (int)type < 100;
}

/// <summary>
/// Danh sách StatType tách theo nhóm, suy ra trực tiếp từ enum để không lệch khi thêm chỉ số mới.
/// </summary>
public static class StatTypes
{
    public static readonly StatType[] Primary;
    public static readonly StatType[] Derived;

    static StatTypes()
    {
        var primary = new List<StatType>();
        var derived = new List<StatType>();
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            if (t.IsPrimary()) primary.Add(t);
            else derived.Add(t);
        }
        Primary = primary.ToArray();
        Derived = derived.ToArray();
    }
}