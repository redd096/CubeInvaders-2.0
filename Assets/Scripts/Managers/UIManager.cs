using UnityEngine;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = default;

    GameObject selector;

    #region public API

    public void PauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
    }

    public void EnableSelector(Coordinates coordinates)
    {
        //set size, position and active it
        float size = GameManager.instance.world.worldConfig.CellsSize;
        selector.transform.localScale = new Vector3(size, size, size);

        selector.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinates);

        selector.SetActive(true);
    }

    public void DisableSelector()
    {
        selector.SetActive(false);
    }

    #endregion
}