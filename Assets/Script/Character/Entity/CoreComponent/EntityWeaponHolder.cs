using UnityEngine;

/// <summary>
/// Enemy weapon holder: the same attack lifecycle and the same <see cref="Weapon"/> types as the player.
/// With no weapon assigned in the Inspector, it equips EntityData.WeaponSO's prefab once — only when that
/// prefab carries a Weapon component; otherwise the enemy keeps attacking through EntityAttack.
/// </summary>
public class EntityWeaponHolder : WeaponHolderBase<EntityCore>
{
    protected override void Start()
    {
        base.Start();
        if (weapon == null) EquipFromData();
    }

    private void EquipFromData()
    {
        WeaponSO weaponSO = Core.Entity.Data.WeaponSO;
        GameObject prefab = weaponSO != null ? weaponSO.Weapon : null;
        if (prefab == null || !prefab.TryGetComponent(out Weapon _)) return;

        // Once per instance: a pooled enemy keeps its weapon across re-spawns.
        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
        instance.GetComponent<Weapon>().Equid(this);
    }
}
