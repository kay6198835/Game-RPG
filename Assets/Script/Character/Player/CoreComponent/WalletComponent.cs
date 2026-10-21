public class WalletComponent : VitalStatsBase<Core>
{
    [SerializeField] private int currentCoin;

    public int GetCurrentCoin()
    {
        return currentCoin;
    }

    public void AddCoins(float amount)
    {
        currentCoin += amount;
    }
    public void PayCoin(float amount)
    {
        currentCoin -= amount;
    }
    public bool TryPayCoin(float amountPay)
    {
        if (amountPay > currentCoin) return false;
        PayCoin(amountPay);
        return true;
    }

    public override void ReLoadComponent()
    {
        base.ReLoadComponent();
        currentCoin = 0;
    }
}