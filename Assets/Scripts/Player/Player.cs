using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using Cinemachine;

[AddComponentMenu("Cube Invaders/Player")]
public class Player : StateMachine
{
    [SerializeField] string currentState;

    [HideInInspector] public CinemachineFreeLook virtualCam;

    State previousState;

    void Start()
    {
        //get references
        virtualCam = FindObjectOfType<CinemachineFreeLook>();

        //set state
        SetState(new PlayerWaitStartGame(this));
    }

    void Update()
    {
        state?.Execution();
    }

    public override void SetState(State stateToSet)
    {
        base.SetState(stateToSet);

        currentState = state?.ToString();
    }

    #region public API

    public void PausePlayer(bool pause)
    {
        if(pause)
        {
            previousState = state;
            SetState(new PlayerPause(this));
        }
        else
        {
            SetState(previousState);
        }
    }

    #endregion
}