using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasTestEnemyDeath : MonoBehaviour
{
    public void OnDisable()
    {
        EventManager.Emit(EventID.ON_ENEMY_DEATH);
    }
}
