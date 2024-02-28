using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;
    [SerializeField] private Vector2 directionVector;
    [SerializeField] private int direction;
    [SerializeField] private KeyCode abilityWeapon;
    [SerializeField] private bool attack;
    private void Awake()
    {
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
    }
    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}
    public Vector2 DirectionVector { get => directionVector;}
    public int Direction { get => direction;}
    public KeyCode AbilityWeapon { get => abilityWeapon; }
    public bool Attack { get => attack;}
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
            attack = true;
        }
        if (context.canceled)
        {
            attack = false;
            Debug.Log("End");
        }
    }
    private void OnAbilityWeapon()
    {

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
        directionVector = (Vector2)targetTowards - (Vector2)this.transform.position;
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle);
    }
}