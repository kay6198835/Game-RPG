using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [SerializeField] protected Vector2 movement;
    [SerializeField] protected Vector2 mousePosition;
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected Player player;

    public float OffsetY { get; private set; } = 0.3f;

    public Vector2 MovementInput { get => movement; }
    public Vector2 MousePosition { get => mousePosition; }
    public float MoveSpeed { get => speed; }
    public Camera MainCamera { get => mainCamera; }
    private void Awake()
    {
        LoadPlayerMovement();
        SetPosition(transform.position);
    }
    void LoadPlayerMovement()
    {
        player = GetComponent<Player>();
        rb = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponent<Animator>();
    }
    protected void Start()
    {
        mainCamera = Camera.main;
    }
    protected override void Update()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (!player.IsAbility)
        {
            AngleCalculate(mousePosition);
        }
        DirectionCharacter();
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        Move();
    }
    public override void Move()
    {
        if (player.IsFreeze)
        {
            movement = Vector2.zero;
        }
        else
        {
            rb.velocity = movement.normalized * speed * Time.deltaTime;
        }
        if (movement == Vector2.zero)
        {
            player.AnimationManager.Animation_1_Idle();
        }
        else
        {
            player.AnimationManager.Animation_2_Run();
        }
        player.AnimationManager.Animation_Direction(direction);
    }
    protected override void DirectionCharacter()
    {
        base.DirectionCharacter();
    }
    public override void AngleCalculate(Vector2 targetTowards)
    {
        if (player.IsFreeze)
        {
            return;
        }
        base.AngleCalculate(targetTowards);
    }

    public void SetPosition(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }
}
