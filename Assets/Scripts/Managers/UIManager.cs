using UnityEngine;
using UnityEngine.UI;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject pauseMenu = default;
    [SerializeField] GameObject endMenu = default;
    [SerializeField] Text endText = default;
    [SerializeField] string winString = "YOU WON!!";
    [SerializeField] string loseString = "YOU LOST...";

    [Header("Resources")]
    [SerializeField] Text resourcesText = default;
    [SerializeField] string stringBeforeResources = "Resources: ";
    [SerializeField] [Min(0)] int decimalsResourcesText = 0;
    [SerializeField] Text costText = default;
    [SerializeField] string stringBeforeCost = "Cost: ";
    [SerializeField] string stringBeforeSell = "Sell: ";
    [SerializeField] [Min(0)] int decimalsCostText = 0;

    [Header("Current Level")]
    [SerializeField] Text currentLevelText = default;
    [SerializeField] string currentLevelString = "Level: ";

    [Header("Strategic")]
    [SerializeField] GameObject strategicCanvas = default;
    [SerializeField] Slider readySlider = default;

    GameObject selector;
    GameObject multipleSelector;

    void Start()
    {
        //instantiate and disable selector
        selector = Instantiate(GameManager.instance.levelManager.generalConfig.Selector);
        multipleSelector = Instantiate(GameManager.instance.levelManager.generalConfig.MultipleSelector);
        HideSelector();

        //hide all
        PauseMenu(false);
        EndMenu(false);
        SetCostText(false);
        strategicCanvas.SetActive(false);

        //show default wave
        UpdateCurrentLevelText(GameManager.instance.waveManager.CurrentWave);

        //add events
        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase += OnEndStrategicPhase;
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase -= OnEndStrategicPhase;
        GameManager.instance.levelManager.onEndGame -= OnEndGame;
    }

    void OnStartStrategicPhase()
    {
        //show strategic canvas
        strategicCanvas.SetActive(true);
    }

    void OnEndStrategicPhase()
    {
        //hide strategic canvas
        strategicCanvas.SetActive(false);
    }

    void OnEndGame(bool win)
    {
        //show end menu
        string text = win ? winString : loseString;
        EndMenu(true, text);
    }

    #endregion

    #region public API

    #region general

    public void PauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
    }

    public void EndMenu(bool active, string text = "")
    {
        //if active, be sure pause menu is deactivated
        if (active)
            PauseMenu(false);

        endMenu.SetActive(active);

        //set text
        endText.text = text;
    }

    #endregion

    #region resources

    public void SetResourcesText(float resources)
    {
        //set resources text
        if (resourcesText)
        {
            resourcesText.text = stringBeforeResources + resources.ToString($"F{decimalsResourcesText}");
        }
    }

    public void SetCostText(bool active, bool isBuying = false, float cost = 0)
    {
        //set cost text + active or deactive
        if(costText)
        {
            string stringBefore = isBuying ? stringBeforeCost : stringBeforeSell;

            costText.text = stringBefore + cost.ToString($"F{decimalsCostText}");
            costText.gameObject.SetActive(active);
        }
    }

    #endregion

    #region current level

    public void UpdateCurrentLevelText(int currentWave)
    {
        if (currentLevelText)
        {
            //set text (current wave +1, so player doesn't see wave 0)
            currentLevelText.text = currentLevelString + (currentWave + 1);
        }
    }

    #endregion

    #region strategic

    public void UpdateReadySlider(float value)
    {
        readySlider.value = value;
    }

    #endregion

    #region selector

    public void ShowSelector(Coordinates coordinates)
    {
        //set size
        float size = GameManager.instance.world.worldConfig.CellsSize;
        selector.transform.localScale = new Vector3(size, size, size);

        //position of our cell
        selector.transform.position = coordinates.position;

        //active selector
        selector.SetActive(true);

        //if select more cells, show multiple selector
        if (GameManager.instance.levelManager.levelConfig.SelectorSize > 1)
            ShowMultipleSelector(coordinates);
    }

    void ShowMultipleSelector(Coordinates coordinates)
    {
        //cell size * selector size
        float size = GameManager.instance.world.worldConfig.CellsSize * GameManager.instance.levelManager.levelConfig.SelectorSize;
        multipleSelector.transform.localScale = new Vector3(size, size, size);

        //position of our cell + move to select other cells
        Vector3 position = coordinates.position;
        position += MoveSelector(true, coordinates);
        position += MoveSelector(false, coordinates);

        multipleSelector.transform.position = position;

        //active selector
        multipleSelector.SetActive(true);
    }

    Vector3 MoveSelector(bool useX, Coordinates coordinates)
    {
        //movement for our selector
        Vector3 movement = Vector3.zero;
        int selectorSize = GameManager.instance.levelManager.levelConfig.SelectorSize;

        int value = useX ? coordinates.x : coordinates.y;

        //check if there are enough cells to the right (useX) or up (!useX)
        bool increase = value + selectorSize - 1 < GameManager.instance.world.worldConfig.NumberCells;

        //min (next after our cell) and max (until selector size)
        //or min (from selector cell) and max (next after our cell)
        int min = increase ? value + 1 : value - (selectorSize -1);
        int max = increase ? value + selectorSize : value;

        //increase or decrease
        for (int i = min; i < max; i++)
        {
            //get coordinates using x or y
            Coordinates coordinatesToRotate = useX ? new Coordinates(coordinates.face, i, coordinates.y) : new Coordinates(coordinates.face, coordinates.x, i);

            //if there is a cell
            if (GameManager.instance.world.Cells.ContainsKey(coordinatesToRotate))
            {
                int b = increase ? +1 : -1;
                movement += useX ? Vector3.right * (GameManager.instance.world.worldConfig.CellsSize /2) * b : Vector3.up * (GameManager.instance.world.worldConfig.CellsSize /2) * b;
            }
        }

        //rotate movement to the face
        movement = WorldUtility.RotateTowardsFace(movement, coordinates.face);

        //return movement
        return movement;
    }

    public void HideSelector()
    {
        //hide selector
        selector.SetActive(false);
        multipleSelector.SetActive(false);
    }

    #endregion

    #endregion
}