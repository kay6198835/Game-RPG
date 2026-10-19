using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class DamagesInManyPos : SpawnSummonBase
{
    [SerializeField]
    private List<DirImpactWave> dirsImpactWave;
    [SerializeField, Tooltip("Runtime: set bởi SetDir(). Editor: chọn dir để xem gizmo")]
    private int dirIndex;
    private List<ImpactWave> waves;          // mỗi phần tử ứng với 1 frame impact
    public float arrowRadius = 0.3f;
    public int damagePerArrow = 10;
    public LayerMask enemyMask;
    [SerializeField] private int maxTargetsPerPoint = 16;

    private Collider2D[] _hitBuffer;
    private readonly HashSet<Collider2D> _hitThisWave = new();

    protected override void Awake()
    {
        base.Awake();
        _hitBuffer = new Collider2D[Mathf.Max(1, maxTargetsPerPoint)];
    }

    private void OnEnable()
    {
        SetDir(_context.DirectionIndex);
    }

    // Gọi khi object được enable, trước khi animation chạy tới wave đầu tiên
    private void SetDir(int index)
    {
        if (index < 0 || index >= dirsImpactWave.Count)
        {
            Debug.LogWarning($"[DamagesInManyPos] dir {index} out of range (0..{dirsImpactWave.Count - 1})", this);
            waves = null;
            return;
        }
        dirIndex = index;
        waves = dirsImpactWave[index].waves;
    }

    // Gọi từ Animation Event (clip RainArrows_dir N), Int = index của wave
    public void OnArrowImpact(int waveIndex)
    {
        if (waves == null) return;
        if (waveIndex < 0 || waveIndex >= waves.Count) return;
        if (waves[waveIndex].points == null || waves[waveIndex].points.Length == 0) return;

        //Debug.Log($"OnArrowImpact {waveIndex} {waves[waveIndex].points.Length}");
        DamageEnemiesInWave(waveIndex);
    }
    private void DamageEnemiesInWave(int waveIndex)
    {
        var wave = waves[waveIndex];
        float radius = wave.radius > 0f ? wave.radius : arrowRadius;
        _hitThisWave.Clear();
        foreach (var p in wave.points)
        {
            Vector2 pos = (Vector2)transform.position + p;
            int count = Physics2D.OverlapCircleNonAlloc(pos, radius, _hitBuffer, enemyMask);
            for (int i = 0; i < count; i++)
            {
                var col = _hitBuffer[i];
                if (_hitThisWave.Add(col) && col.TryGetComponent(out INegativeReceiver receiver))   // 1 enemy chỉ ăn 1 hit mỗi wave
                    receiver.TakeDamage(damagePerArrow, pos);
            }
        }
    }

    // Kết thúc animation thì hủy skill
    public void OnSkillEnd() => Destroy(gameObject);

    void OnDrawGizmosSelected()
    {
        if (dirsImpactWave == null || dirIndex < 0 || dirIndex >= dirsImpactWave.Count) return;
        var dir = dirsImpactWave[dirIndex];
        if (dir.waves == null) return;
        foreach (var w in dir.waves)
        {
            if (w.points == null) continue;
            // Phần tử thêm bằng nút "+" trong Inspector bị Unity zero-init (bỏ qua initializer): alpha = 0, radius = 0
            var color = w.gizmoColor;
            if (color.a <= 0f) color.a = 1f;
            Gizmos.color = color;
            float radius = w.radius > 0f ? w.radius : arrowRadius;
            foreach (var p in w.points)
                Gizmos.DrawWireSphere(transform.position + (Vector3)p, radius);
        }
    }
}

[System.Serializable]
public class ImpactWave
{
    public Vector2[] points;
    public float radius = 0.3f;
    public Color gizmoColor = Color.black;
}   // các điểm tên rơi trong 1 frame
[System.Serializable]
public class DirImpactWave
{
    public List<ImpactWave> waves;
}   // các frame của 1 hướng bắn