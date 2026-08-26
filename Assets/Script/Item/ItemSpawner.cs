using System;
using UnityEngine;
using VContainer;
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemController droppedItemPrefab;
    [SerializeField] private IObjecPoolService objecPoolService;
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
        var itemObject = objecPoolService.Spawn((Vector2)obj, Quaternion.identity, droppedItemPrefab);
        itemController = itemObject.GetComponent<ItemController>();
        itemController.SetDataItem();
        itemObject.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }

    void CollectItem(object obj = null)
    {
        objecPoolService.Release((GameObject)obj);
    }
}