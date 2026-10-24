using UnityEngine;

public class WalletComponent : CoreComponentBase<Core>, IWallet
{
    [SerializeField] private int currentCoin;

    public int GetCurrentCoin()
    {
        return currentCoin;
    }

    public void AddCoins(int amount)
    {
        currentCoin += amount;
    }
    public void PayCoin(int amount)
    {
        currentCoin -= amount;
    }
    public bool TryPayCoin(int amountPay)
    {
        if (amountPay > currentCoin) return false;
        PayCoin(amountPay);
        return true;
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        currentCoin = 0;
    }
}