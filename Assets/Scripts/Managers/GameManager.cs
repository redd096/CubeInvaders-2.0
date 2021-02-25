using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    public UIManager uiManager { get; private set; }
    public Player player { get; private set; }
    public World world { get; private set; }
    public LevelManager levelManager { get; private set; }
    public WaveManager waveManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();
        world = FindObjectOfType<World>();
        levelManager = FindObjectOfType<LevelManager>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    #region public API

    public void UpdateLevel(LevelConfig levelConfig)
    {
        //update level config
        instance.levelManager.UpdateLevel(levelConfig);
    }

    public void SetWave(int wave)
    {
        //set wave
        instance.waveManager.CurrentWave = wave;

        //end wave and start strategic phase after few seconds
        if (instance.levelManager.CurrentPhase == EPhase.assault)
        {
            instance.levelManager.EndAssaultPhase();
            instance.levelManager.Invoke("StartStrategicPhase", 1);
        }
        //or start immediatly strategic phase
        else
        {
            instance.levelManager.StartStrategicPhase();
        }

        //resume game
        SceneLoader.instance.ResumeGame();
    }

    public void NextWave()
    {
        //increase wave +1, than set wave
        SetWave(instance.waveManager.CurrentWave + 1);
    }

    #endregion
}