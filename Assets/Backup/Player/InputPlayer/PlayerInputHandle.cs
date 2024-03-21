using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [Header("Direction")]
    [SerializeField] private int direction;
    [SerializeField] private float angleSin;
    [SerializeField] private float angleDirection;
    [Header("Move")]
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;
    [SerializeField] private Vector2 directionVector;
    [Header("Bool Value")]
    [SerializeField] private bool isAttack;
    [SerializeField] private bool isSkill;
    [SerializeField] private bool isDisadvantage;
    [Header("Test Dot Product")]
    [SerializeField] Transform enemy;
    [SerializeField] Vector2 eToP;
    [SerializeField] float dotProduct;
    
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
    [SerializeField] private SkillState state;
    [SerializeField] private SkillType skill;
    [SerializeField] private DisadvantageState disadvantage;
    private void Awake()
    {
        //playerCtr = GetComponentInParent<NewPlayer>();
        playerInput = new PlayerInput();
    }
    private void Start()
    {
        playerInput.Control.Movement.started += OnMove;
    
        playerInput.Control.Movement.performed += OnMove;
        playerInput.Control.Movement.performed += OnDirection;
    
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
    }
    private void Update()
    {
        if(enemy != null)
        {
            eToP = (enemy.position - transform.position).normalized;
            dotProduct = Vector2.Dot(eToP, directionVector);
        }

    }
    #region Get value 
    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}
    public Vector2 DirectionVector { get => directionVector;}
    public int Direction { get => direction;}
    public bool IsAttack { get => isAttack;}
    public SkillState State { get => state;}
    public SkillType Skill { get => skill;}
    public bool IsSkill { get => isSkill;}
    public PlayerInput PlayerInput { get => playerInput;}
    public float AngleSin { get => angleSin;}
    public float AngleDirection { get => angleDirection; }
    public bool IsDisadvantage { get => isDisadvantage;}
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
        AngleCalculate(mouseVector);
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
    protected void DirectionCaculate(float angle)
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
    public void AngleCalculate(Vector2 targetTowards)
    {
        float angle;
        directionVector = ((Vector2)targetTowards - (Vector2)this.transform.position).normalized;
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle);
        angleDirection = angle;
        //ChangeAngleCosToSin(angle);
        this.angleSin = Vector2.SignedAngle(transform.right, directionVector);
        this.angleSin = (this.angleSin + 360) % 360;
    }
}