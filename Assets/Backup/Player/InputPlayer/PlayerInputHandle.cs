using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private NewPlayer playerCtr;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;
    [SerializeField] private Vector2 directionVector;
    [SerializeField] private int direction;
    [SerializeField] private bool playerAttack=false;
    [SerializeField] private bool playerWeaponSkill=false;
    [SerializeField] private bool playerWeaponAbility=false;
    //[SerializeField] private AbilitySO skillSO;
    [SerializeField] private float angleSin;
    [SerializeField] private float angleDirection;
    
    public enum SkillState
    {
        Start,
        Cast,
        Do,
    }
    [SerializeField] private SkillState state;
    private void Awake()
    {
        playerCtr = GetComponentInParent<NewPlayer>();
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

        playerInput.Control.UseSkill.started += OnAbilityWeapon;
        playerInput.Control.UseSkill.performed += OnAbilityWeapon;
        playerInput.Control.UseSkill.canceled += OnAbilityWeapon;
    }
    #region Get value 
    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}
    public Vector2 DirectionVector { get => directionVector;}
    public int Direction { get => direction;}
    public bool Attack { get => playerAttack;}
    public SkillState State { get => state;}
    public bool Skill { get => playerWeaponSkill;}
    public PlayerInput PlayerInput { get => playerInput;}
    public bool Ability { get => playerWeaponAbility; }
    public float AngleSin { get => angleSin;}
    public float AngleDirection { get => angleDirection; }
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
            playerAttack = true;
        }
        if (context.canceled)
        {
            playerAttack = false;
        }
    }
    private void OnAbilityWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            state = SkillState.Start;
        }
        else if (context.performed)
        {
            playerWeaponSkill = true;
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            playerWeaponSkill = false;
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