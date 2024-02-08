using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item SO/Item Depot")]
public class DepotItemPrefab : ScriptableObject
{
    [SerializeField] public List<DropRateItem> dropRatesItem;
}
