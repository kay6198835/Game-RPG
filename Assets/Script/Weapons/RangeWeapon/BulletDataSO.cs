using UnityEngine;

public enum BulletType
{
    EnemyBullet, PlayerBullet
}

[CreateAssetMenu(menuName = "WeaponData/BulletData")]
public class BulletDataSO : ScriptableObject
{
    public BulletType bullettype;

    [Range(0.1f, 30f)] public float lifetime = 3f;
    [Range(0, 999)] public int dmg;
    [Range(1f, 60f)] public float speed = 12f;

    public LayerMask targetMask;
}
