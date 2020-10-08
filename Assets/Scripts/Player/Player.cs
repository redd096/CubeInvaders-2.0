using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using Cinemachine;

[AddComponentMenu("Cube Invaders/Player")]
public class Player : StateMachine
{
    [Header("Debug")]
    [SerializeField] string currentState;

    public CinemachineFreeLook virtualCam { get; private set; }

    //used to come back from pause
    State previousState;

    void Start()
    {
        //get references
        virtualCam = FindObjectOfType<CinemachineFreeLook>();

        //set state
        SetState(new PlayerPause(this));

        AddEvents();
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    void Update()
    {
        state?.Execution();
    }

    public override void SetState(State stateToSet)
    {
        base.SetState(stateToSet);

        //for debug
        currentState = state?.ToString();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
    }

    void OnStartStrategicPhase()
    {
        //go to player move, starting from center cell
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
    }

    void OnStartAssaultPhase()
    {
        //go to player move, starting from center cell
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerAssault(this, new Coordinates(EFace.front, centerCell)));
    }

    #endregion

    #region public API

    public void PausePlayer(bool pause)
    {
        if(pause)
        {
            //pause
            previousState = state;
            SetState(new PlayerPause(this));
        }
        else
        {
            //resume
            SetState(previousState);
        }
    }

    #endregion
}