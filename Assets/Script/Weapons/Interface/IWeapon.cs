/// <summary>
/// Typed view of a weapon over its own stat profile, so a weapon reads its stats without casting.
/// The holder never sees this — it drives the attack lifecycle through the non-generic <see cref="Weapon"/>.
/// </summary>
public interface IWeapon<TStat> where TStat : IWeaponStat
{
    TStat Stats { get; }
    void SetStat(TStat stat);
}
