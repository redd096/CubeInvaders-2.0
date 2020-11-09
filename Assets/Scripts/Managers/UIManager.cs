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

    void Start()
    {
        //instantiate and disable selector
        selector = Instantiate(GameManager.instance.levelManager.generalConfig.Selector);
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
        //set size, position and active selector
        float size = GameManager.instance.world.worldConfig.CellsSize;
        selector.transform.localScale = new Vector3(size, size, size);

        selector.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinates);

        selector.SetActive(true);
    }

    public void HideSelector()
    {
        //hide selector
        selector.SetActive(false);
    }

    #endregion

    #endregion
}