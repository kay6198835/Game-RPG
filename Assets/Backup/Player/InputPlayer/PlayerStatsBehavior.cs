using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;
[CreateAssetMenu(fileName = "PLayerStatsBehavior", menuName = "Data/PLayer Data/Behavior Statss Data")]
public class PlayerStatsBehavior : ScriptableObject
{
    [SerializeField] public Vector2 MoveVector;
    [SerializeField] public Vector2 MouseVector;
    [Header("Direction by Keyboard")]
    [SerializeField] public Vector2 DirectionKeyboardVector;
    [SerializeField] public float AngleKeyboardDirection;
    [SerializeField] public int DirectionKeyboard;
    [Header("Direction by Externality")]
    [SerializeField] public Vector2 DirectionExternalityVector;
    [SerializeField] public float AngleExternalityDirection;
    [SerializeField] public int DirectionExternality;
    [Header("Direction by Mouse")]
    [SerializeField] public Vector2 DirectionMouseVector;
    [SerializeField] public int DirectionMouse;
    [SerializeField] public float AngleMouseDirection;
    [SerializeField] public float AngleRotationPlayer;
    [Header("Bool Value")]
    [SerializeField] public bool IsAttack;
    [SerializeField] public bool IsSkill;
    [SerializeField] public bool IsDisadvantage;
    [SerializeField] public bool IsTakeDamage;
    [SerializeField] public bool IsEquip_Unequip = false;
    [SerializeField] public bool IsInteractor = false;
    [Header("Enum Value")]
    [SerializeField] public SkillState State;
    [SerializeField] public SkillType Skill;
    [SerializeField] public DisadvantageState Disadvantage;
}
