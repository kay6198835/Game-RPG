using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerManager : MonoBehaviour, IPlayerService
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private IObjectResolver resolver;
    private Player player;

    [Inject]
    public void Construct(IObjectResolver resolver)
    {
        this.resolver = resolver;
    }

    // Lazy: the first consumer (a service factory during container build, or Awake) spawns the player,
    // so the order in which the container injects scene components does not matter.
    public Player Player => player != null ? player : (player = SpawnPlayer());
    public IPlayerStatService StatService => Player.GetComponentInChildren<StatHandler>(true);

    private void Awake()
    {
        if (resolver != null) _ = Player;
    }

    // resolver.Instantiate injects the whole hierarchy before Awake, so no player component needs registering.
    private Player SpawnPlayer()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject instance = resolver.Instantiate(playerPrefab.gameObject, position, Quaternion.identity, transform);
        Player spawned = instance.GetComponent<Player>();
        Camera.main.transform.SetParent(spawned.transform);
        return spawned;
    }

    public Transform GetPlayerTransform() => Player.transform;

    public void SetPlayerPosition(Vector2 position) => Player.transform.position = position;
}
