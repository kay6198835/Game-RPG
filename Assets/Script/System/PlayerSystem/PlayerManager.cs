using UnityEngine;
public class PlayerManager : MonoBehaviour, IPlayerService
{
    [SerializeField] private Player playerPrefab;

    public Transform GetPlayerTransform()
    {
        return playerPrefab.transform;
    }

    public void SetPlayerPosition(Vector2 position)
    {
        playerPrefab.transform.position = position;
    }

}