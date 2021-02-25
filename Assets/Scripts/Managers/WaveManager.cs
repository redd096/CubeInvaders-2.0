using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    [Header("Important")]
    public WaveConfig waveConfig;
    public int currentWave = 0;

    List<EnemyStruct> enemies = new List<EnemyStruct>();
    Coroutine wave_coroutine;

    void Start()
    {
        //add events
        AddEvents();
    }

    void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    void StartNewLevel()
    {
        //current wave
        WaveStruct wave = waveConfig.Waves[this.currentWave];

        //update level config (change level) and update resources for player
        GameManager.instance.levelManager.UpdateLevel(wave.LevelConfig);
        GameManager.instance.player.CurrentResources = wave.resourcesMax;
    }

    void StartWave()
    {
        //start coroutine
        if (gameObject.activeInHierarchy)
        {
            if (wave_coroutine != null)
                StopCoroutine(wave_coroutine);

            wave_coroutine = StartCoroutine(Wave_Coroutine());
        }
    }

    void EndWave()
    {
        //wave +1
        currentWave++;

        //end assault phase
        GameManager.instance.levelManager.EndAssaultPhase();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;
    }

    void OnStartStrategicPhase()
    {
        //remove all enemies
        ClearEnemies();

        //if there aren't other waves
        if(waveConfig.Waves == null || currentWave >= waveConfig.Waves.Length || currentWave < 0)
        {
            //win
            GameManager.instance.levelManager.EndGame(true);
        }

        StartNewLevel();
    }

    void OnStartAssaultPhase()
    {
        //start wave
        StartWave();
    }

    void OnEndAssaultPhase()
    {
        //stop coroutine if still running
        if (wave_coroutine != null)
            StopCoroutine(wave_coroutine);
    }

    void OnEnemyDeath(Enemy enemy)
    {
        //remove from the list
        foreach(EnemyStruct enemyStruct in enemies)
        {
            if(enemyStruct.Enemy == enemy)
            {
                enemies.Remove(enemyStruct);
                break;
            }
        }

        //if there are no other enemies, end wave
        if(enemies.Count <= 0)
        {
            EndWave();
        }
    }

    #endregion

    #region private API

    void ClearEnemies()
    {
        //remove every child
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //clear list
        enemies.Clear();
    }

    IEnumerator Wave_Coroutine()
    {
        //current wave
        WaveStruct wave = waveConfig.Waves[currentWave];

        //foreach enemy in this wave, instantiate but deactivate
        foreach (EnemyStruct enemyStruct in wave.EnemiesStructs)
        {
            InstantiateNewEnemy(enemyStruct.Enemy, enemyStruct.TimeToAddBeforeSpawn);
            yield return null;
        }

        //enemies copy(copy because when enemy is killed, it's removed from list)
        List<EnemyStruct> enemiesCopy = enemies.CreateCopy();

        //queue to not spawn on same face
        Queue<EFace> facesQueue = new Queue<EFace>();

        //for every enemy
        foreach (EnemyStruct enemyStruct in enemiesCopy)
        {
            //wait for this enemy
            yield return new WaitForSeconds(enemyStruct.TimeToAddBeforeSpawn);

            //randomize coordinates to attack
            EFace face = WorldUtility.GetRandomFace(facesQueue, waveConfig.Waves[currentWave].IgnorePreviousFacesAtSpawn);
            int x = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            int y = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            Coordinates coordinatesToAttack = new Coordinates(face, x, y);

            //get position and rotation
            Vector3 position;
            Quaternion rotation;
            GameManager.instance.world.GetPositionAndRotation(coordinatesToAttack, waveConfig.Waves[currentWave].DistanceFromWorld, out position, out rotation);

            //set enemy position and rotation, then activate
            enemyStruct.Enemy.transform.position = position;
            enemyStruct.Enemy.transform.rotation = rotation;

            //instantiate portal at position and rotation
            if (GameManager.instance.levelManager.generalConfig.PortalPrefab)
            {
                Instantiate(GameManager.instance.levelManager.generalConfig.PortalPrefab, position, rotation);
            }

            //set enemy destination and activate
            enemyStruct.Enemy.Init(coordinatesToAttack);

            //wait for next enemy
            yield return new WaitForSeconds(wave.TimeBetweenSpawns);
        }
    }

    #endregion

    #region public API

    public Enemy InstantiateNewEnemy(Enemy enemyPrefab, float timeToAddBeforeSpawn)
    {
        //instantiate and set parent but deactivate
        Enemy enemy = Instantiate(enemyPrefab, transform);
        enemy.gameObject.SetActive(false);

        //save in the list and add to the event
        enemies.Add(new EnemyStruct(enemy, timeToAddBeforeSpawn));
        enemy.onEnemyDeath += OnEnemyDeath;

        return enemy;
    }

    #endregion
}
