using UnityEngine;
using redd096;
using Cinemachine;
using UnityEngine.InputSystem;

[AddComponentMenu("Cube Invaders/Player")]
public class Player : StateMachine
{
    #region lock 60 fps

    [Header("Lock 60 FPS")]
    [SerializeField] bool lock60FPS;

    private void OnValidate()
    {
        if (lock60FPS)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;
    }

    #endregion

    [Header("Camera")]
    public float mouseSpeedX = 300;
    public float mouseSpeedY = 2;
    public float gamepadSpeedX = 150;
    public float gamepadSpeedY = 1;
    public bool invertY = false;

    [Header("Player")]
    [Range(0.1f, 0.9f)] public float deadZoneAnalogs = 0.6f;

    [Header("Debug")]
    [SerializeField] string currentState;

    public CinemachineFreeLook VirtualCam { get; private set; }
    public NewControls Controls { get; private set; }
    public PlayerInput playerInput { get; private set; }

    float currentResources;
    public float CurrentResources
    {
        get
        {
            return currentResources;
        }
        set
        {
            currentResources = value;
            GameManager.instance.uiManager.SetResourcesText(currentResources);  //update UI
        }
    }

    //used to come back from pause
    State previousState;

    void Start()
    {
        //get virtual cam and player controls
        VirtualCam = FindObjectOfType<CinemachineFreeLook>();
        Controls = new NewControls();
        playerInput = GetComponent<PlayerInput>();

        //by default deactive cinemachine
        VirtualCam.enabled = false;

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
        if (state.GetType() == typeof(PlayerPlaceTurret) && Controls.Gameplay.PauseButton.activeControl.name == Controls.Gameplay.DenyTurret.activeControl.name)
            return;

        //if not ended game && game is running && is not end assault phase (showing panel to end level)
        if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale > 0 && GameManager.instance.levelManager.CurrentPhase != EPhase.endAssault)
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

        //if not ended game && game is paused && is not end assault phase (showing panel to end level)
        if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale <= 0 && GameManager.instance.levelManager.CurrentPhase != EPhase.endAssault)
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
        GameManager.instance.levelManager.onStartGame += OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartGame -= OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame -= OnEndGame;
    }

    void OnStartGame()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //start in strategic phase
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
    }

    void OnStartStrategicPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //if in pause, set as previous state strategic phase
        if (state is PlayerPause)
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            previousState = new PlayerStrategic(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to player move, starting from center cell
        else
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnStartAssaultPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //if in pause, set as previous state assault phase
        if (state is PlayerPause)
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            previousState = new PlayerAssault(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to player move, starting from center cell
        else
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            SetState(new PlayerAssault(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnEndGame(bool win)
    {
        //stop control pause menu
        Controls.Disable();

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