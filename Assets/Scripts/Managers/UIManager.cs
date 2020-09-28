using UnityEngine;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = default;

    GameObject selector;

    void Start()
    {
        //instantiate and disable selector
        selector = Instantiate(GameManager.instance.world.worldConfig.Selector);
        HideSelector();
    }

    #region public API

    public void PauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
    }

    public void ShowSelector(Coordinates coordinates)
    {
        //set size, position and active it
        float size = GameManager.instance.world.worldConfig.CellsSize;
        selector.transform.localScale = new Vector3(size, size, size);

        selector.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinates);

        selector.SetActive(true);
    }

    public void HideSelector()
    {
        selector.SetActive(false);
    }

    #endregion
}