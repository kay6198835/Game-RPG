using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicState : PlayerState
{
    public BasicState(NewPlayer player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

}
