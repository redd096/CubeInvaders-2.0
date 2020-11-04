using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPhase
{
    strategic, assault, waiting
}

[AddComponentMenu("Cube Invaders/Manager/Level Manager")]
public class LevelManager : MonoBehaviour
{
    [Header("Important")]
    public LevelConfig levelConfig;
    public GeneralConfig generalConfig;

    [Header("EndGame")]
    public string WinText = "YOU WON!!";
    public string LoseText = "YOU LOST...";

    public System.Action onStartStrategicPhase;
    public System.Action onEndStrategicPhase;
    public System.Action onStartAssaultPhase;
    public System.Action onEndAssaultPhase;
    public System.Action<bool> onEndGame;

    [Header("Debug")]
    public EPhase CurrentPhase;

    public bool GameEnded { get; private set; }

    void Start()
    {
        //check if randomize world
        if (levelConfig.RandomizeWorldAtStart)
        {
            GameManager.instance.world.RandomRotate();
        }
        else
        {
            //else start game after few seconds
            Invoke("StartGame", 1);
        }
    }

    #region events

    public void StartStrategicPhase()
    {
        onStartStrategicPhase?.Invoke();

        CurrentPhase = EPhase.strategic;
    }

    public void EndStrategicPhase()
    {
        if (CurrentPhase == EPhase.strategic)
        {
            CurrentPhase = EPhase.waiting;

            onEndStrategicPhase?.Invoke();

            Invoke("StartAssaultPhase", 1);
        }
    }

    public void StartAssaultPhase()
    {
        onStartAssaultPhase?.Invoke();

        CurrentPhase = EPhase.assault;
    }

    public void EndAssaultPhase()
    {
        if (CurrentPhase == EPhase.assault)
        {
            CurrentPhase = EPhase.waiting;

            onEndAssaultPhase?.Invoke();

            Invoke("StartStrategicPhase", 1);
        }
    }

    #endregion

    #region public API

    public void StartGame()
    {
        GameEnded = false;

        //start in strategic
        StartStrategicPhase();
    }

    public void EndGame(bool win)
    {
        //do only one time
        if (GameEnded)
            return;

        GameEnded = true;

        //call event
        onEndGame?.Invoke(win);
    }

    #endregion
}
