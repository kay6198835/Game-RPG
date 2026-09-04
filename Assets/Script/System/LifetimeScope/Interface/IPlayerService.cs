using UnityEngine;

public interface IPlayerService
{
    Transform GetPlayerTransform();
    void SetPlayerPosition(Vector2 position);
}