using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrezeeState : PlayerState
{
    public bool IsFrezee { get; private set; }
    public float timeDuration { get; protected set; }
    public float frezeeTime { get; protected set; }

    public FrezeeState(NewPlayer player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        frezeeTime = startTime + timeDuration;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timeDuration <= Time.time)
        {
            IsFrezee = true;
        }
        else
        {
            IsFrezee = false;
        }
    }
}
