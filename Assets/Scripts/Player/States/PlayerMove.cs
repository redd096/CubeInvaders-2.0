using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerState
{
    public PlayerMove(redd096.StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Execution()
    {
        base.Execution();

        CinemachineMovement("Mouse X", "Mouse Y");
    }
}
