using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee")]
    [SerializeField] private int maxTargetsPerSwing = 8;

    private Collider2D[] hits;
    private Vector2 centerAttackPosition;

    protected override void Awake()
    {
        base.Awake();
        hits = new Collider2D[maxTargetsPerSwing];
    }

    public override void OnAttackEnter(Player player)
    {
        base.OnAttackEnter(player);
        centerAttackPosition = (Vector2)player.transform.position
            + aim.AimDirection.normalized * currentStage.attackRange;
    }

    public override void OnActivate(float finalDamage)
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            centerAttackPosition, currentStage.attackRange, hits, stats.LayerMask);

        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out INegativeReceiver receiver))
            {
                receiver.TakeDamage(finalDamage, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (currentStage != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currentStage.attackRange);
        }
    }
}
