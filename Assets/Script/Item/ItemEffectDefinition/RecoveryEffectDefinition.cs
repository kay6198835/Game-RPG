[CreateAssetMenu]
public class RecoveryEffectDefinition : ItemEffectDefinition
{
    [SerializeField] private int amount;

    public override void Apply(ResourceReceiver player)
    {
        // player.Health.Heal(amount);
    }
}