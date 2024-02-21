using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;

    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");
        mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}