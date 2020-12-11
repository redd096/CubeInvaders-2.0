using UnityEngine;
using redd096;

public class PlayerState : State
{
    protected Player player;
    protected NewControls controls;
    protected Transform transform;

    Cinemachine.CinemachineFreeLook virtualCam;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
        //get references
        player = stateMachine as Player;
        controls = player.Controls;
        transform = player.transform;
        virtualCam = player.VirtualCam;
    }

    protected void CinemachineMovement(Vector2 movement)
    {
        //normalize the vector (we need only direction)
        movement.Normalize();
        Debug.Log("input: " + player.GetComponent<UnityEngine.InputSystem.PlayerInput>().currentControlScheme + " and normalized: " + movement);

        //set invert y for y axis
        movement.y = player.invertY ? -movement.y : movement.y;

        //set axis value
        virtualCam.m_XAxis.Value += movement.x * player.speedX * Time.deltaTime;
        virtualCam.m_YAxis.Value += movement.y * player.speedY * Time.deltaTime;
    }

    protected virtual void StopCinemachine()
    {
        //be sure to not move cinemachine (y axis is a fixed value)
        virtualCam.m_XAxis.Value = 0;
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