using System;
using DG.Tweening;
using UnityEngine;
using VContainer;
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemController droppedItemPrefab;
    [SerializeField] private IObjecPoolService objecPoolService;
    [SerializeField] private DepotItem depotItem;
    [Inject]
    public void Construct(IObjecPoolService objecPoolService)
    {
        this.objecPoolService = objecPoolService;
    }
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, DropItem);
        EventManager.Resgister(EventID.ON_COLLECT_ITEM, CollectItem);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, DropItem);
        EventManager.UnResgister(EventID.ON_COLLECT_ITEM, CollectItem);
    }

    bool CheckRate()
    {
        return true;
    }

    void DropItem(object obj = null)
    {
        if (!CheckRate()) return;
        if (obj == null || depotItem == null) return;
        // Roll trượt (dải "không rơi") thì không spawn gì cả — nếu không SetDataItem sẽ nhận null và NRE.
        if (!depotItem.TryRollItem(out ItemSO rolled)) return;
        var itemObject = objecPoolService.Spawn(((GameObject)obj).transform.position, Quaternion.identity, droppedItemPrefab.gameObject);
        var itemController = itemObject.GetComponent<ItemController>();
        itemController.SetDataItem(rolled);
        itemObject.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }


    void CollectItem(object obj = null)
    {
        objecPoolService.Release((GameObject)obj);
    }
}