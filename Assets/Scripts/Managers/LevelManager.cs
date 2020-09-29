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

    public System.Action onStartStrategicPhase;
    public System.Action onEndStrategicPhase;
    public System.Action onStartAssaultPhase;
    public System.Action onEndAssaultPhase;
    public System.Action onEndGame;

    [Header("Debug")]
    public EPhase CurrentPhase;

    void Start()
    {
        //hide UI and all
        onEndStrategicPhase?.Invoke();
        onEndAssaultPhase?.Invoke();

        //try randomize world
        if (RandomizeWorld() == false)
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

    #region start

    bool RandomizeWorld()
    {
        //check if there is WorldRandomRotate and start it
        WorldRandomRotate WorldRandomRotate = GameManager.instance.world.GetComponent<WorldRandomRotate>();
        if (WorldRandomRotate)
        {
            return WorldRandomRotate.StartRandomize();
        }

        return false;
    }

    #endregion

    #region public API

    public void StartGame()
    {
        StartStrategicPhase();
    }

    public void EndGame(bool win)
    {
        onEndGame?.Invoke();
    }

    #endregion
}
