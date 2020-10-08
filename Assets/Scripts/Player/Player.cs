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

    public CinemachineFreeLook VirtualCam { get; private set; }

    //used to come back from pause
    State previousState;

    void Start()
    {
        //get references
        VirtualCam = FindObjectOfType<CinemachineFreeLook>();

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
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame -= OnEndGame;
    }

    void OnStartStrategicPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //go to player move, starting from center cell
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
    }

    void OnStartAssaultPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //go to player move, starting from center cell
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerAssault(this, new Coordinates(EFace.front, centerCell)));
    }

    void OnEndGame(bool win)
    {
        //set pause state and show mouse
        SetState(new PlayerPause(this));
        Utility.LockMouse(CursorLockMode.None);
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