/// <summary>
/// Marker for any weapon stat profile. <see cref="WeaponStats"/> is the base implementation;
/// each weapon kind closes <see cref="IWeapon{TStat}"/> over its own concrete profile.
/// </summary>
public interface IWeaponStat
{
    WeaponType Type { get; }
}
