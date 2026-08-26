[CreateAssetMenu]
public class CurrencyEffectDefinition : ItemEffectDefinition
{
    [SerializeField] private int amount;

    public override void Apply(ResourceReceiver player)
    {
        // player.Wallet.Add(amount);
    }
}