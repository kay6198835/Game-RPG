using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static PlayerInputHandler;

[CreateAssetMenu(fileName ="newPLayerData",menuName ="Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Serializable]
    public class PlayerStatsBehavior
    {
        [SerializeField] private Vector2 moveVector;
        [SerializeField] private Vector2 mouseVector;

        [Header("Direction by Keyboard")]
        [SerializeField] private Vector2 directionKeyboardVector;
        [SerializeField] private float angleKeyboardDirection;
        [SerializeField] private int directionKeyboard;

        [Header("Direction by Externality")]
        [SerializeField] private Vector2 directionExternalityVector;
        [SerializeField] private float angleExternalityDirection;
        [SerializeField] private int directionExternality;

        [Header("Direction by Mouse")]
        [SerializeField] private Vector2 directionMouseVector;
        [SerializeField] private int directionMouse;
        [SerializeField] private float angleMouseDirection;
        [SerializeField] private float angleRotationPlayer;

        [Header("Enum Value")]
        [SerializeField] private SkillState state;
        [SerializeField] private SkillType skill;
        [SerializeField] private DisadvantageState disadvantage;

        public Vector2 MoveVector { get => moveVector; set => moveVector = value; }
        public Vector2 MouseVector { get => mouseVector; set => mouseVector = value; }
        public Vector2 DirectionKeyboardVector { get => directionKeyboardVector; set => directionKeyboardVector = value; }
        public float AngleKeyboardDirection { get => angleKeyboardDirection; set => angleKeyboardDirection = value; }
        public int DirectionKeyboard { get => directionKeyboard; set => directionKeyboard = value; }
        public Vector2 DirectionExternalityVector { get => directionExternalityVector; set => directionExternalityVector = value; }
        public float AngleExternalityDirection { get => angleExternalityDirection; set => angleExternalityDirection = value; }
        public int DirectionExternality { get => directionExternality; set => directionExternality = value; }
        public Vector2 DirectionMouseVector { get => directionMouseVector; set => directionMouseVector = value; }
        public int DirectionMouse { get => directionMouse; set => directionMouse = value; }
        public float AngleMouseDirection { get => angleMouseDirection; set => angleMouseDirection = value; }
        public float AngleRotationPlayer { get => angleRotationPlayer; set => angleRotationPlayer = value; }
        public SkillState State { get => state; set => state = value; }
        public SkillType Skill { get => skill; set => skill = value; }
        public DisadvantageState Disadvantage { get => disadvantage; set => disadvantage = value; }
    }
    [Serializable]
    public class PlayerStatsState
    {
        [Header("Bool Value")]
        [SerializeField] private bool isAttack;
        [SerializeField] private bool isSkill;
        [SerializeField] private bool isDisadvantage;
        [SerializeField] private bool isTakeDamage;
        [SerializeField] private bool isEquip_Unequip = false;
        [SerializeField] private bool isInteractor = false;

        public bool IsAttack { get => isAttack; set => isAttack = value; }
        public bool IsSkill { get => isSkill; set => isSkill = value; }
        public bool IsDisadvantage { get => isDisadvantage; set => isDisadvantage = value; }
        public bool IsTakeDamage { get => isTakeDamage; set => isTakeDamage = value; }
        public bool IsEquip_Unequip { get => isEquip_Unequip; set => isEquip_Unequip = value; }
        public bool IsInteractor { get => isInteractor; set => isInteractor = value; }
    }

    [SerializeField] private float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] public PlayerStatsState StatsState;
    [SerializeField] public PlayerStatsBehavior StatsBehavior;

    public float MaxHealth { get => maxHealth; }

    [Header("Move State")]
    public float movementVelocities;

    private void Reset()
    {
        Reborn();
    }
    public void Reborn()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        movementVelocities = 10f;
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
