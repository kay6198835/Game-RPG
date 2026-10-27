public interface IWallet
{
    int GetCurrentCoin();
    void AddCoins(int amount);
    bool TryPayCoin(int amountPay);
}