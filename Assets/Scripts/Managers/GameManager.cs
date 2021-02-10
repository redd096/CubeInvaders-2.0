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
        //end wave
        instance.levelManager.EndAssaultPhase(true);

        //set wave
        instance.waveManager.currentWave = wave;
    }

    #endregion
}