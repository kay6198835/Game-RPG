using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerManager : MonoBehaviour, IPlayerService
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private IObjectResolver resolver;
    private IObjectPoolService objectPoolService;
    private Player player;

    [Inject]
    public void Construct(IObjectResolver resolver, IObjectPoolService objectPoolService)
    {
        this.resolver = resolver;
        this.objectPoolService = objectPoolService;
    }

    // Lazy: the first consumer (a service factory during container build, or Awake) spawns the player,
    // so the order in which the container injects scene components does not matter.
    public Player Player => player != null ? player : (player = SpawnPlayer());
    public IPlayerStatService StatService => Player.GetComponentInChildren<StatHandler>(true);

    private void Awake()
    {
        if (resolver != null) _ = Player;
    }
    public void Enable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_DEATH, DespawnPlayer);
    }

    public void Disable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_DEATH, DespawnPlayer);
    }

    // resolver.Instantiate injects the whole hierarchy before Awake, so no player component needs registering.
    private Player SpawnPlayer()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject instance = objectPoolService.Spawn(playerPrefab.gameObject, position, Quaternion.identity, transform);
        Player spawned = instance.GetComponent<Player>();
        Camera.main.transform.SetParent(spawned.transform);
        return spawned;
    }

    public Transform GetPlayerTransform() => Player.transform;

    public void SetPlayerPosition(Vector2 position) => Player.transform.position = position;

    private void DespawnPlayer(object _)
    {
        if (player != null) objectPoolService.Despawn(player.gameObject);
    }
}
