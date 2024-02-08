using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    //private PlayerInput playerInput;
    [SerializeField] private Camera cam;

    [SerializeField] private Vector2 rawMovementInput;
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private Vector2 direction;
    //public Vector2 RawDashDirectionInput { get; private set; }
    //public Vector2Int DashDirectionInput { get; private set; }
    public Vector2 RawMovementInput { get => rawMovementInput;}
    public Vector2 MousePosition { get => mousePosition;}
    public Vector2 Direction { get => direction;}

    //public bool JumpInput { get; private set; }
    //public bool JumpInputStop { get; private set; }
    //public bool GrabInput { get; private set; }
    //public bool DashInput { get; private set; }
    //public bool DashInputStop { get; private set; }

    //public bool[] AttackInputs { get; private set; }

    //[SerializeField]
    //private float inputHoldTime = 0.2f;

    //private float jumpInputStartTime;
    //private float dashInputStartTime;
    private void Awake()
    {

    }

    private void Start()
    {

        //int count = Enum.GetValues(typeof(CombatInputs)).Length;
        //AttackInputs = new bool[count];


    }
    public void OnUseWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Button down now");
        }
        if (context.performed)
        {
            Debug.Log("Button hold down");
        }
        if (context.canceled)
        {
            Debug.Log("Button release");
        }
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        rawMovementInput = context.ReadValue<Vector2>();
        //mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        //direction = (MousePosition - (Vector2)transform.position).normalized;
        Debug.Log("Move"+ context.ReadValue<Vector2>());
    }
    public void OnMouseInput(InputAction.CallbackContext context)
    {

        mousePosition = context.ReadValue<Vector2>();
        direction = (MousePosition - (Vector2)transform.position).normalized;

    }
}