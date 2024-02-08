using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] int money;

    public event Action OnMoneyChanged;

    public static Wallet Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
    }

    public bool HasMoney(int amount)
    {
        return amount <= money;
    }

    public int Money => money;
}
