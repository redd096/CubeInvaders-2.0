using UnityEngine;
using UnityEngine.UI;
using redd096;

[System.Serializable]
public struct MenuStruct
{
    public Button button;
    public string necessaryKey;
}

[AddComponentMenu("Cube Invaders/Menu System")]
public class MenuSystem : MonoBehaviour
{
    [Header("Disabled Buttons")]
    [SerializeField] bool setNotInteractable = false;
    [SerializeField] bool changeColor = true;
    [CanShow("changeColor")] [SerializeField] Color colorOnDisable = Color.red;

    [Header("Menu")]
    [SerializeField] MenuStruct[] levelButtons = default;

    void Start()
    {
        foreach(MenuStruct levelButton in levelButtons)
        {
            //if no key, or load is succesfull
            bool isActive = string.IsNullOrWhiteSpace(levelButton.necessaryKey) || Load(levelButton.necessaryKey);

            if(isActive == false)
            {
                //set interactable
                if (setNotInteractable)
                    levelButton.button.interactable = false;

                //change color
                if (changeColor)
                    levelButton.button.GetComponent<Image>().color = colorOnDisable;

                //remove event
                levelButton.button.onClick = new Button.ButtonClickedEvent();
            }

            //if not active, remove event
            if (isActive == false)
                levelButton.button.onClick = new Button.ButtonClickedEvent();
        }
    }

    /// <summary>
    /// Save data
    /// </summary>
    public static void Save(string key, bool win)
    {
        PlayerPrefs.SetInt(key, win ? 1 : 0);
    }

    /// <summary>
    /// Load data
    /// </summary>
    public static bool Load(string key)
    {
        return PlayerPrefs.GetInt(key, 0) > 0 ? true : false;
    }

    /// <summary>
    /// Delete data
    /// </summary>
    public static void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    /// <summary>
    /// Delete every data
    /// </summary>
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
