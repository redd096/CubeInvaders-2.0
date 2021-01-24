using UnityEngine;
using redd096;

public class PlayerState : State
{
    protected Player player;
    protected NewControls controls;
    protected Transform transform;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
        //get references
        player = stateMachine as Player;
        controls = player.Controls;
        transform = player.transform;
    }

    protected bool CheckClick(ref bool pressedInput)
    {
        //if pressed input, set to released and return true
        if (pressedInput)
        {
            pressedInput = false;
            return true;
        }

        //else return false
        return false;
    }

    #region inputs

    public override void Enter()
    {
        base.Enter();

        AddInputs();
    }

    public override void Exit()
    {
        base.Exit();

        RemoveInputs();
    }

    protected virtual void AddInputs()
    {
        //add events
    }

    protected virtual void RemoveInputs()
    {
        //remove events
    }

    #endregion
}