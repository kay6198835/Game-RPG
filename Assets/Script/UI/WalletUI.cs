using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] Text MoneyTxt;

    private void Start()
    {
        Wallet.Instance.OnMoneyChanged += SetMoneyTxt;
    }

    public void ShowWallet()
    {
        gameObject.SetActive(true);
        SetMoneyTxt();
    }

    public void CloseWallet()
    {
        gameObject.SetActive(false);
    }

    void SetMoneyTxt()
    {
        MoneyTxt.text = "" + Wallet.Instance.Money;
    }
}
