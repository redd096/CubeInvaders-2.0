using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject pauseMenu = default;
    [SerializeField] GameObject endMenu = default;
    [SerializeField] Text endText = default;

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

        //hide menus and strategic canvas
        PauseMenu(false);
        EndMenu(false);
        strategicCanvas.SetActive(false);

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
        string text = win ? GameManager.instance.levelManager.WinText : GameManager.instance.levelManager.LoseText;
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
        endMenu.SetActive(active);

        //set text
        endText.text = text;
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
        selector.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinates);

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
        Vector3 position = GameManager.instance.world.CoordinatesToPosition(coordinates);
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