#if UNITY_EDITOR
using UnityEngine;

namespace QA.Automation
{
    // EntityMovement.Start reads EnemyManager.Instance.Grid. A QA scene has no tilemap to build one from, so
    // an empty grid is installed; it answers every query with null / Path.Failure, which the movement code
    // already handles (same approach as CombatTestEnemySpawner).
    public class QAEmptyPathGrid : MonoBehaviour
    {
        private void Start()
        {
            if (EnemyManager.Instance != null && EnemyManager.Instance.Grid == null)
                EnemyManager.Instance.SetPathfindingGrid(new PathfindingGrid());
        }
    }
}
#endif
