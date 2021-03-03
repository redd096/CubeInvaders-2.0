using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPhase
{
    strategic, assault, endStrategic, endAssault
}

[AddComponentMenu("Cube Invaders/Manager/Level Manager")]
public class LevelManager : MonoBehaviour
{
    [Header("Important")]
    public LevelConfig levelConfig;
    public GeneralConfig generalConfig;

    public System.Action onStartGame;
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
        if (levelConfig && levelConfig.RandomizeWorldAtStart)
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
            CurrentPhase = EPhase.endStrategic;

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
            CurrentPhase = EPhase.endAssault;

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

        //call event
        onStartGame?.Invoke();
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

    public void UpdateLevel(LevelConfig levelConfig)
    {
        //update level config
        this.levelConfig = levelConfig;
    }

    #endregion
}
