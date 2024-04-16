using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateShoot : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] bool playerIsIdle;
    [SerializeField] Vector2 directionShoot;
    [SerializeField] Vector2 player0Position;
    [SerializeField] Vector2 player1Position;
    [SerializeField] float distance;
    [SerializeField] float durationMove;
    [SerializeField] float currentSpeed;
    [SerializeField] float lastSpeed;

    private void Start()
    {
        playerIsIdle = false;
        player0Position = player.position;
    }
    private void Update()
    {
        directionShoot = (player.position - this.transform.position).normalized;
        CalculateSpeed();
    }
    
    void CalculateSpeed()
    {
        player1Position = player.position;
        if (playerIsIdle)
        {
            player0Position = player1Position;
            playerIsIdle = false;
        }
        else
        {
            distance += Vector2.Distance(player0Position, player1Position);
            durationMove += Time.deltaTime;
            player0Position = player1Position;
            currentSpeed = distance / durationMove;
            if(lastSpeed != currentSpeed)
            {
                lastSpeed = currentSpeed;
                durationMove = 0f;
                distance = 0f;
            }
        }

    }
}
