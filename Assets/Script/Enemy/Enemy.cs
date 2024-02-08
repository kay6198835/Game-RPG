using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private EnemySO enemySO;
    [Header("Controller")]
    [SerializeField] private StateManager stateManager;

    public EnemySO EnemySO { get => enemySO;}
    public StateManager StateManager { get => stateManager; set => stateManager = value; }

    //public bool notInRoom = false;

    public void SetIsAttack()
    {
        stateManager.IsAttack = true;
    }
    public void SetIsChase()
    {
        stateManager.IsChase = true;
    }

    protected override void LoadCharacter()
    {
        base.LoadCharacter();
        stateManager = GetComponentInChildren<StateManager>();
    }
    protected override void SettingCharacter()
    {
        base.SettingCharacter();
        animator.runtimeAnimatorController = enemySO.animator;
    }
    private void Awake()
    {
        base.stats = enemySO;
        LoadCharacter();

    }
    private void Start()
    {
        SettingCharacter();
    }
    public override void TakeDamage(int damage, GameObject caller)
    {
        base.TakeDamage(damage,caller);
    }
    protected override void Die()
    {
        base.Die();
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        GetComponentInChildren<PrefabRandomItem>().InstantiatItemDrop(transform.position);
    }
}
