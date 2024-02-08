using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class WeaponsController : MonoBehaviour
{
    private GameObject currWp;

    [SerializeField]
    private Transform WeaponHolder;

    [SerializeField]
    private WeaponDataSO EquipWp;

    [SerializeField] PlayerCombat meleeCombat;

    public int slot, maxSlot;

    public void equipped(WeaponDataSO WPdata)
    {
        EquipWp = WPdata;
        currWp = Instantiate(WPdata.weaponPrefab);
        currWp.transform.SetParent(WeaponHolder);
        currWp.transform.localPosition = Vector3.zero;
        currWp.transform.localRotation = Quaternion.identity;
        slot++;

        if (EquipWp.Type == WeaponType.RangeWP)
        {
            Shooting shootScript = currWp.GetComponent<Shooting>();
            shootScript.enabled = true;

        }
        else if (EquipWp.Type == WeaponType.MeleeWP)
        {
            gameObject.GetComponent<Player>().Weapon = currWp.GetComponent<WeaponMelee>();
            meleeCombat = currWp.GetComponent<PlayerCombat>();
            meleeCombat.Animator = gameObject.GetComponent<Animator>();
            meleeCombat.Player = gameObject.GetComponent<Player>();
            meleeCombat.enabled = true;
        }
    }

    public void Drop()
    {
        if (EquipWp.Type == WeaponType.RangeWP)
        {
            Shooting shootScript = currWp.GetComponent<Shooting>();
            shootScript.enabled = false;
        }
        else if (EquipWp.Type == WeaponType.MeleeWP)
        {
            PlayerCombat meleeCombat = currWp.GetComponent<PlayerCombat>();
            meleeCombat.Animator = gameObject.GetComponent<Animator>();
            meleeCombat.enabled = false;
        }

        currWp.transform.SetParent(null);
        currWp.transform.localPosition = new Vector3(transform.position.x, transform.position.y - 2);
        slot--;
    }
}
