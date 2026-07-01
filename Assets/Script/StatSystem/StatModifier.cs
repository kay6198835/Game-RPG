/// <summary>
/// Thứ tự áp dụng: Flat → PercentAdd (cộng dồn rồi nhân 1 lần) → PercentMult (nhân riêng từng cái).
/// Giá trị enum cũng là Order mặc định.
/// </summary>
public enum ModifierType
{
    Flat = 100,  // +20 Damage
    PercentAdd = 200,  // +10% (0.10f) — các PercentAdd cộng dồn với nhau trước khi nhân
    PercentMult = 300,  // x(1+15%) — nhân riêng, dùng cho buff đặc biệt
}

/// <summary>
/// Một bonus đơn lẻ gắn vào Stat. Bất biến (immutable) — tạo mới thay vì sửa.
/// Source là object tạo ra modifier (item, buff, passive...) để gỡ hàng loạt theo nguồn.
/// </summary>
public sealed class StatModifier
{
    public readonly float Value;
    public readonly ModifierType Type;
    public readonly int Order;
    public readonly object Source;

    public StatModifier(float value, ModifierType type, object source = null, int? order = null)
    {
        Value = value;
        Type = type;
        Source = source;
        Order = order ?? (int)type;
    }
}