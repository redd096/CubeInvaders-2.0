using UnityEngine;
using redd096;
using Cinemachine;

public class PlayerState : State
{
    protected Player player;
    protected Transform transform;

    CinemachineFreeLook virtualCam;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
        //get references
        player = stateMachine as Player;
        transform = player.transform;
        virtualCam = player.VirtualCam;
    }

    //public override void AwakeState(StateMachine stateMachine)
    //{
    //    base.AwakeState(stateMachine);
    //
    //    //get references
    //    player = stateMachine as Player;
    //    transform = player.transform;
    //    rb = transform.GetComponent<Rigidbody>();
    //}

#if UNITY_ANDROID

    protected void CinemachineMovement(float axisX, float axisY)
    {
        //set axis
        virtualCam.m_XAxis.m_InputAxisValue = axisX;
        virtualCam.m_YAxis.m_InputAxisValue = axisY;

        //set speed
        float maxSpeedX = 200;
        float maxSpeedY = 2;
        bool invertY = false;
        float speedX = Time.timeScale > 1 ? maxSpeedX / Time.timeScale : maxSpeedX;
        float speedY = Time.timeScale > 1 ? maxSpeedY / Time.timeScale : maxSpeedY;

        virtualCam.m_XAxis.m_MaxSpeed = speedX;
        virtualCam.m_YAxis.m_MaxSpeed = speedY;

        //set invert y
        virtualCam.m_YAxis.m_InvertInput = invertY;
    }

#else

    protected void CinemachineMovement(string axisX, string axisY)
    {
        //set axis
        virtualCam.m_XAxis.m_InputAxisName = axisX;
        virtualCam.m_YAxis.m_InputAxisName = axisY;

        //set speed
        float maxSpeedX = 300;
        float maxSpeedY = 2;
        bool invertY = false;
        float speedX = Time.timeScale > 1 ? maxSpeedX / Time.timeScale : maxSpeedX;
        float speedY = Time.timeScale > 1 ? maxSpeedY / Time.timeScale : maxSpeedY;

        virtualCam.m_XAxis.m_MaxSpeed = speedX;
        virtualCam.m_YAxis.m_MaxSpeed = speedY;

        //set invert y
        virtualCam.m_YAxis.m_InvertInput = invertY;
    }

#endif

    protected virtual void StopCinemachine()
    {
        virtualCam.m_XAxis.m_InputAxisName = "";
        virtualCam.m_YAxis.m_InputAxisName = "";

        virtualCam.m_XAxis.m_MaxSpeed = 0;
        virtualCam.m_YAxis.m_MaxSpeed = 0;

        virtualCam.m_XAxis.m_InputAxisValue = 0;
        virtualCam.m_YAxis.m_InputAxisValue = 0;
    }
}