using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : Movement
{
    [SerializeField] private Transform player;
    [SerializeField] private StateManager stateManager;
    public float Speed { get => speed; set => speed = value; }
    public Transform Player { get => player; set => player = value; }
    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        rb = stateManager.EnemyCtrl.RigidbodyCharacter;
    }
    private void Start()
    {
        speed = stateManager.EnemyCtrl.EnemySO.speedMove;
    }
    protected override void Update()
    {
        if (player != null)
        {
            AngleCalculate(player.position);
        }
        DirectionCharacter();
    }
    void Flip(Transform transform)
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
    protected override void DirectionCharacter()
    {
        base.DirectionCharacter();
        if (angle < 180 && facingRight)
        {
            Flip(rb.transform);
        }
        if (angle > 180 && !facingRight)
        {
            Flip(rb.transform);
        }
    }
    public override void AngleCalculate(Vector2 targetTowards)
    {
        base.AngleCalculate(targetTowards);
    }
}
