using UnityEngine;
using VContainer;
public class PlayerManager : MonoBehaviour, IPlayerService
{
    private Player player;
    [SerializeField] private Camera cameraMain;

    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
        this.player.transform.SetParent(this.transform);
        cameraMain = Camera.main;
        cameraMain.transform.SetParent(player.transform);
    }

    void Start()
    {
        cameraMain = Camera.main;
    }
    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public void SetPlayerPosition(Vector2 position)
    {
        player.transform.position = position;
    }

}