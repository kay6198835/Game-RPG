using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopSate { Menu, Buying, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] WalletUI walletUI;

    public GameObject Pistol, Rifle, SwordAndShield;

    public WeaponDataSO pistolData, rifleData, SaSData;

    public Transform Merchant;

    public event Action OnStartShopping, OnFinishShopping;

    ShopSate state;

    Merchant merchant;

    public static ShopController i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;

        OnStartShopping?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopSate.Menu;

        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("Want to buy anything kid? I have everything you need",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Nah" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            state = ShopSate.Buying;
            OnStartShopping?.Invoke();
        }
        else if (selectedChoice == 1)
        {
            OnFinishShopping?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopSate.Buying)
        {
            StartCoroutine(Buying());
        }
    }

    IEnumerator Buying()
    {
        state = ShopSate.Busy;
        walletUI.ShowWallet();

        int selectedChoice = 0;

        var positionToSpawn = new Vector3(Merchant.localPosition.x +1, Merchant.localPosition.y - 3);

        yield return DialogManager.Instance.ShowDialogText("Ok, What you wanna buy?",
            waitForInput: false,
            choices: new List<string>() { "Pistol - 1000$", "Rifle - 3000$", "Sword and Shield - 2000$" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            if (Wallet.Instance.HasMoney(pistolData.value))
            {
                Wallet.Instance.TakeMoney(pistolData.value);
                Instantiate(Pistol, positionToSpawn, Quaternion.identity);
                OnBackFormBuying();
                yield break;
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText("You don't have enough money kid. Go find some more.");
                OnBackFormBuying();
            }
        }

        else if (selectedChoice == 1)
        {
            if (Wallet.Instance.HasMoney(rifleData.value))
            {
                Wallet.Instance.TakeMoney(rifleData.value);
                Instantiate(Rifle, positionToSpawn, Quaternion.identity);
                OnBackFormBuying();
                yield break;
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText("You don't have enough money kid. Go find some more.");
                OnBackFormBuying();
            }
        }

        else if (selectedChoice == 2)
        {
            if (Wallet.Instance.HasMoney(SaSData.value))
            {
                Wallet.Instance.TakeMoney(SaSData.value);
                Instantiate(SwordAndShield, positionToSpawn, Quaternion.identity);
                OnBackFormBuying();
                yield break;
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText("You don't have enough money kid. Go find some more.");
                OnBackFormBuying();
            }
        }
    }

    void OnBackFormBuying()
    {
        walletUI.CloseWallet();
        OnFinishShopping?.Invoke();
    }
}
