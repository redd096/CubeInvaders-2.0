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
        //movement.Normalize();

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
        Debug.Log(this.ToString());
    }

    protected virtual void RemoveInputs()
    {
        //remove events
    }

    #endregion
}