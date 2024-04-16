using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabRandomItem : MonoBehaviour
{
    [SerializeField] private GameObject droppedItemPrefab;
    //[SerializeField] DepotPrefab itemList;
    [SerializeField] public Enemy enemyCtrl;
    private void Start()
    {
        enemyCtrl=GetComponentInParent<Enemy>();
        //droppedItemPrefab = Resources.Load<GameObject>("Items/ItemPrefab");
    }
    DropRateItem GetDroppedItem()
    {
        int changceDropNumber = Random.Range(1,101);
        List<DropRateItem> itemsCanDropped = new List<DropRateItem>();
        foreach (DropRateItem itemSO in enemyCtrl.EnemySO.depotItem.dropRatesItem)
        {
            if(changceDropNumber <= itemSO.dropRate)
            {
                itemsCanDropped.Add(itemSO);
            }
        }
        if(itemsCanDropped.Count > 0)
        {
            DropRateItem itemDrop = itemsCanDropped[Random.Range(0,itemsCanDropped.Count)];
            return itemDrop;
        }
        return null;
    }
    public void InstantiatItemDrop(Vector2 spawnPosition)
    {
        DropRateItem item = GetDroppedItem();
        if (item != null)
        {
            GameObject itemGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
            itemGameObject.GetComponent<SpriteRenderer>().sprite = item.itemOS.ItemSprite;
            Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            itemGameObject.GetComponent<Rigidbody2D>().AddForce(dropDirection * 100f, ForceMode2D.Impulse);
        }
    }
}
