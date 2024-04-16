using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public enum SkillState
    {
        Start,
        Cast,
        Do,
    }
    public enum SkillType
    {
        Special,
        Ability
    }
    public enum DisadvantageState
    {
        TakeDamaged,
    }
    [SerializeField] private NewPlayer player;
    [SerializeField] private PlayerInput playerInput;

    [Header("Direction Be Attacked")]
    [SerializeField] private Vector2 directionBeAttackedVector;
    [SerializeField] private float angleBeAttackedDirection;
    [SerializeField] private int directionBeAttacked;

    [Header("Direction Move/Attack")]
    [SerializeField] private Vector2 directionLookVector;
    [SerializeField] private int directionLook;
    [SerializeField] private float angleLookDirection;
    [SerializeField] private float angleRotationPlayer;
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;

    [Header("Bool Value")]
    [SerializeField] private bool isAttack;
    [SerializeField] private bool isSkill;
    [SerializeField] private bool isDisadvantage;
    [SerializeField] private bool isTakeDamage;
    [SerializeField] private bool isPick_Drop = false;

    [Header("Enum Value")]
    [SerializeField] private SkillState state;
    [SerializeField] private SkillType skill;
    [SerializeField] private DisadvantageState disadvantage;
    private void Awake()
    {
        player = GetComponentInParent<NewPlayer>();
        playerInput = new PlayerInput();
    }
    private void Start()
    {
        playerInput.Control.Movement.started += OnMove;
        playerInput.Control.Movement.performed += OnMove;
        playerInput.Control.Movement.canceled += OnMove;

        playerInput.Control.MousePosition.performed += OnDirection;

        playerInput.Control.Attack.started += OnAttack;
        playerInput.Control.Attack.canceled += OnAttack;

        playerInput.Control.SkillWeapon.started += OnSkillWeapon;
        playerInput.Control.SkillWeapon.performed += OnSkillWeapon;
        playerInput.Control.SkillWeapon.canceled += OnSkillWeapon;
        
        playerInput.Control.AbilityWeapon.started += OnAbilityWeapon;
        playerInput.Control.AbilityWeapon.performed += OnAbilityWeapon;
        playerInput.Control.AbilityWeapon.canceled += OnAbilityWeapon;

        playerInput.Control.PickDrop.started += OnPickDrop;
        playerInput.Control.PickDrop.canceled += OnPickDrop;
    }
    #region Get value 
    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}
    public Vector2 DirectionLookVector { get => directionLookVector;}
    public int DirectionLook { get => directionLook;}
    public bool IsAttack { get => isAttack;}
    public SkillState State { get => state;}
    public SkillType Skill { get => skill;}
    public bool IsSkill { get => isSkill;}
    public PlayerInput PlayerInput { get => playerInput;}
    public float AngleRotationPlayer { get => angleRotationPlayer;}
    public float AngleLookDirection { get => angleLookDirection; }
    public bool IsDisadvantage { get => isDisadvantage;}
    public bool IsTakeDamage { get => isTakeDamage; }
    public bool IsPick_Drop { get => isPick_Drop;}
    public Vector2 DirectionBeAttackedVector { get => directionBeAttackedVector;}
    public float AngleBeAttackedDirection { get => angleBeAttackedDirection;}
    public int DirectionBeAttacked { get => directionBeAttacked;}
    #endregion
    private void OnEnable()
    {
        playerInput.Control.Enable();
    }
    private void OnDisable()
    {
        playerInput.Control.Disable();
    }
    private void OnDirection(InputAction.CallbackContext context)
    {
        mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionLookVector = (mouseVector - (Vector2)this.transform.position).normalized;
        AngleCalculate(directionLookVector,ref angleLookDirection,ref directionLook);
        this.angleRotationPlayer = Vector2.SignedAngle(transform.right, directionLookVector);
        this.angleRotationPlayer = (this.angleRotationPlayer + 360) % 360;
    }
    private void OnPickDrop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isPick_Drop = true;
        }
        else if(context.canceled)
        {
            isPick_Drop = false;
        }
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttack = true;
        }
        if (context.canceled)
        {
            isAttack = false;
        }
    }
    private void OnSkillWeapon(InputAction.CallbackContext context)
    {
        skill = SkillType.Special;
        if (context.started)
        {
            state = SkillState.Start;
            isSkill = true;
            player.Core.WeaponHolder.Weapon.SetAbility();
            player.Core.AbilityHolder.SetCanUseAbility(true);
        }
        else if (context.performed)
        {
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            isSkill = false;
        }
    }
    private void OnAbilityWeapon(InputAction.CallbackContext context)
    {
        skill = SkillType.Ability;
        if (context.started)
        {
            state = SkillState.Start;
        }
        else if (context.performed)
        {
            isSkill = true;
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            isSkill = false;
        }
    }
    public void OnTakeDamage(Vector2 attackPosition)
    {
        Debug.Log("OnTakeDamage");
        ChangeIsTakeDamage();
        Invoke(nameof(ChangeIsTakeDamage), 0.1f);
        directionBeAttackedVector = ((attackPosition - (Vector2)this.transform.position)).normalized;
        AngleCalculate(directionBeAttackedVector, ref angleBeAttackedDirection, ref directionBeAttacked);
    }
    private void ChangeIsTakeDamage()
    {
        this.isTakeDamage = !this.isTakeDamage;
    }
    public void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle, ref direction);
    }
    protected void DirectionCaculate(float angle, ref int direction)
    {
        if ((angle > 22 && angle <= 67))
        {
            direction = 0;
        }
        else if (angle > 67 && angle <= 112)
        {
            direction = 1;
        }
        else if (angle > 112 && angle <= 157)
        {
            direction = 2;

        }
        else if (angle > 157 && angle <= 202)
        {
            direction = 3;
        }
        else if (angle > 202 && angle <= 247)
        {
            direction = 4;
        }
        else if (angle > 247 && angle <= 292)
        {
            direction = 5;
        }
        else if (angle > 292 && angle <= 337)
        {
            direction = 6;
        }
        else if (angle > 337 || angle < 22)
        {
            direction = 7;
        }
    }
}