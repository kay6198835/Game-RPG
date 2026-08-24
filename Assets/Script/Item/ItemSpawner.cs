public class ItemSpawner()
{
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private IObjecPoolService objecPoolService;
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, DropItem);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, DropItem);
    }

    bool CheckRate()
    {
        return true;
    }

    void DropItem(object obj = null)
    {
        if (!CheckRate()) return;
        objecPoolService.Spawn((Vector2)obj, Quaternion.identity, droppedItemPrefab);
    }
}