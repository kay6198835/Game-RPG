using System;
using UnityEngine;
public class DamagesInManyPos : SpawnSummonBase
{
    [System.Serializable]
    public class ImpactWave { public Vector2[] points; }   // các điểm tên rơi trong 1 frame

    public ImpactWave[] waves;          // mỗi phần tử ứng với 1 frame impact
    public float arrowRadius = 0.3f;
    public int damagePerArrow = 10;
    public LayerMask enemyMask;

    // Gọi từ Animation Event, truyền index của wave
    public void OnArrowImpact(int waveIndex)
    {
        var hitThisWave = new HashSet<Collider2D>();
        foreach (var p in waves[waveIndex].points)
        {
            Vector2 pos = (Vector2)transform.position + p;
            foreach (var col in Physics2D.OverlapCircleAll(pos, arrowRadius, enemyMask))
            {
                if (hitThisWave.Add(col))   // 1 enemy chỉ ăn 1 hit mỗi wave
                    col.GetComponent<Enemy>()?.TakeDamage(damagePerArrow);
            }
        }
    }

    // Kết thúc animation thì hủy skill
    public void OnSkillEnd() => Destroy(gameObject);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var w in waves)
            foreach (var p in w.points)
                Gizmos.DrawWireSphere(transform.position + (Vector3)p, arrowRadius);
    }
}