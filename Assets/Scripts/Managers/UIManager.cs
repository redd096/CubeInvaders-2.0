using UnityEngine;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = default;

    public void PauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
    }
}