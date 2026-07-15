
public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private readonly Queue<PoolMember> inactiveObjects = new Queue<PoolMember>();
    public void Spawn(Vector2 position)
    {
        if (inactiveObjects.Count == 0)
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
            PoolMember member = obj.GetComponent<PoolMember>();
            member.Initialize(this);
        }
        else
        {
            Reload(position);
        }
    }

    public void Reload(Vector2 position)
    {
        var reload = inactiveObjects.Dequeue();
        reload.gameObject.SetActive(true);
        reload.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);
        reload.SwitchIsInPool(false);
    }

    public void Release(GameObject gameObject)
    {
        gameObject.SetActive(false);
        if (!gameObject.TryGetComponent<PoolMember>(out PoolMember member))
        {
            member = gameObject.AddComponent<PoolMember>();
        }

        if (member.isInPool) return;
        inactiveObjects.Enqueue(member);
        member.SwitchIsInPool(true);
    }

    public void Register(GameObject gameObject)
    {
        this.prefab = gameObject;
    }
}