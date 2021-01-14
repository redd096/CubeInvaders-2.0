using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Game Manager")]
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

    public void UpdateLevel(BiomesConfig biomesConfig)
    { 
        //update biomes config and regen world
        if (instance.world.biomesConfig != biomesConfig)
        {
            instance.world.biomesConfig = biomesConfig;
            instance.world.RegenWorld();
        }
    }

    public void UpdateLevel(LevelConfig levelConfig)
    {
        //update level config
        instance.levelManager.levelConfig = levelConfig;

        //reset turret
        if (levelConfig.ResetTurrets)
        {
            foreach(Turret turret in FindObjectsOfType<Turret>())
            {
                turret.RemoveTurret();
            }
        }
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