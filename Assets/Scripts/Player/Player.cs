using UnityEngine;
using redd096;
using Cinemachine;
using UnityEngine.InputSystem;

[AddComponentMenu("Cube Invaders/Player")]
public class Player : StateMachine
{
    [Header("Player")]
    public float speedX = 300;
    public float speedY = 2;
    public bool invertY = false;

    [Header("Debug")]
    [SerializeField] string currentState;

    public CinemachineFreeLook VirtualCam { get; private set; }
    public NewControls Controls { get; private set; }

    //used to come back from pause
    State previousState;

    void Start()
    {
        Application.targetFrameRate = 60;

        //get virtual cam and player controls
        VirtualCam = FindObjectOfType<CinemachineFreeLook>();
        Controls = new NewControls();

        //set state and lock mouse
        SetState(new PlayerPause(this));
        Utility.LockMouse(CursorLockMode.Locked);

        AddInputs();
        AddEvents();
    }

    void OnDestroy()
    {
        RemoveInputs();
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

    #region inputs (pause and resume)

    //used because pause and resume are called at same frame. Like this we wait when release button
    bool alreadyPressed;

    void AddInputs()
    {
        Controls.Enable();
        Controls.Gameplay.PauseButton.started += PauseGame;
        Controls.Gameplay.PauseButton.canceled += ResetAlreadyPaused;
        Controls.Gameplay.ResumeButton.started += ResumeGame;
        Controls.Gameplay.ResumeButton.canceled += ResetAlreadyPaused;
    }

    void RemoveInputs()
    {
        Controls.Disable();
        Controls.Gameplay.PauseButton.started -= PauseGame;
        Controls.Gameplay.PauseButton.canceled -= ResetAlreadyPaused;
        Controls.Gameplay.ResumeButton.started -= ResumeGame;
        Controls.Gameplay.ResumeButton.canceled -= ResetAlreadyPaused;
    }

    void PauseGame(InputAction.CallbackContext ctx)
    {
        //do only if not already pressed button
        if (alreadyPressed)
            return;

        //if state is place turret && press escape, doesn't pause (we use it to exit from this state)
        if (state.GetType() == typeof(PlayerPlaceTurret) && Controls.Gameplay.PauseButton.activeControl.name == "escape")
            return;

        //if not ended game && game is running
        if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale > 0)
        {
            SceneLoader.instance.PauseGame();
            alreadyPressed = true;
        }
    }

    void ResumeGame(InputAction.CallbackContext ctx)
    {
        //do only if not already pressed button
        if (alreadyPressed)
            return;

        //if not ended game && game is paused
        if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale <= 0)
        {
            SceneLoader.instance.ResumeGame();
            alreadyPressed = true;
        }
    }

    void ResetAlreadyPaused(InputAction.CallbackContext ctx)
    {
        alreadyPressed = false;
    }

    #endregion

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