using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Recovery - Reduction Per Time For Duration")]
public class RecoveryReductionPerTimeForDuration : StatsEffectBase
{
    [SerializeField] private float perTime;
    [Range(1, 10)]
    [SerializeField] private int timeCount;
    protected override void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context)
    {
        // B6: one tick now, then every perTime seconds, timeCount ticks in all (duration = perTime x timeCount).
        // A refresh by the same skill extends the end and keeps the tick phase. perTime <= 0 is ignored.
        effectReceiver.ApplyStatus(new StatusRequest
        {
            Effect = this,
            StatusType = statusType,
            TickModifiers = statModifierGroup,
            TickInterval = perTime,
            Duration = perTime * timeCount,
            Context = context,
        });
    }
}
