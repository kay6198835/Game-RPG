using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    EnemyBullet, PlayerBullet
}

[CreateAssetMenu(menuName = "WeaponData/BulletData")]
public class BulletDataSO : ScriptableObject
{
    public BulletType bullettype;
    public float lifetime;
    public int dmg;
}
