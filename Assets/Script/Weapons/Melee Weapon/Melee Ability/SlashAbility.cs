using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Slash Ability")]
public class SlashAbility : AbilitySO
{
    [Header("Stats")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int levelMax;
    [SerializeField] private float distance;
    [SerializeField] private float timeBetween;
    [SerializeField] private int damage;
    [SerializeField] private float stunTime;

    public override void Activate(GameObject player)
    {
        currentLevel = 0;

        maxCastTime = levelMax * timeBetween;

        base.Activate(player);
    }
    public override void CastSkill(GameObject player)
    {
        base.CastSkill(player);
    }
    public override void BeginCooldown(GameObject player)
    {
        base.BeginCooldown(player);
        currentLevel = (int)(periodCastTime / timeBetween);
        if (currentLevel == 0)
        {
            return;
        }
        if (currentLevel > levelMax)
        {
            currentLevel = levelMax;
        }
        for (int i = 0; i < 1; i++)
        {
            Attack(player,i);
        }
    }
    public void Attack(GameObject player, int attackIndex)
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(PositionAttack(player),
            new Vector2(distance, 0.5f), player.transform.localRotation.eulerAngles.z, layerMask);
        foreach (var enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, player);
            }
            if (currentLevel == levelMax)
            {
                if (enemy.GetComponent<Enemy>() != null)
                {
                    enemy.GetComponent<Enemy>().Stun(stunTime);
                }
            }
        }
    }
    private Vector2 PositionAttack(GameObject player)
    {
        Vector2 mousePosition = player.GetComponent<PlayerMovement>().MousePosition;
        Vector2 direction = mousePosition - (Vector2)player.transform.position;
        Vector2 targetPosition = (Vector2)player.transform.position + direction.normalized * distance / 2;
        return targetPosition;
    }
}
