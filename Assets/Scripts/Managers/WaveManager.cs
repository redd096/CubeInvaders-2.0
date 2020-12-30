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
        if(waveConfig.Waves == null || currentWave >= waveConfig.Waves.Length)
        {
            //win
            GameManager.instance.levelManager.EndGame(true);
            return;
        }

        CreateWave();
    }

    void OnStartAssaultPhase()
    {
        //start wave
        if (wave_coroutine != null)
            StopCoroutine(wave_coroutine);

        wave_coroutine = StartCoroutine(Wave_Coroutine());
    }

    void OnEndAssaultPhase()
    {
        //wave +1
        currentWave++;

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

        //if there are no other enemies, end assault phase
        if(enemies.Count <= 0)
        {
            GameManager.instance.levelManager.EndAssaultPhase();
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

    void CreateWave()
    {
        //do only if there are waves
        if (this.currentWave < 0 || this.currentWave >= waveConfig.Waves.Length)
            return;

        //current wave
        WaveStruct wave = waveConfig.Waves[this.currentWave];

        //update level config (change level)
        GameManager.instance.UpdateLevel(wave.LevelConfig);

        //foreach enemy in this wave, instantiate but deactivate
        foreach (EnemyStruct enemyStruct in wave.EnemiesStructs)
        {
            InstantiateNewEnemy(enemyStruct.Enemy, enemyStruct.TimeToAddBeforeSpawn);
        }
    }

    IEnumerator Wave_Coroutine()
    {
        //current wave + enemies copy (copy because when enemy is killed, it's removed from list)
        WaveStruct wave = waveConfig.Waves[currentWave];
        List<EnemyStruct> enemiesCopy = enemies.CreateCopy();

        //queue to not spawn on same face
        Queue<EFace> facesQueue = new Queue<EFace>();

        //for every enemy
        foreach (EnemyStruct enemyStruct in enemiesCopy)
        {
            //wait for this enemy
            yield return new WaitForSeconds(enemyStruct.TimeToAddBeforeSpawn);

            //randomize coordinates to attack
            EFace face = GetRandomFace(facesQueue);
            int x = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            int y = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            Coordinates coordinatesToAttack = new Coordinates(face, x, y);

            //set enemy position
            enemyStruct.Enemy.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack, wave.DistanceFromWorld);

            //set enemy destination and activate
            enemyStruct.Enemy.coordinatesToAttack = coordinatesToAttack;
            enemyStruct.Enemy.gameObject.SetActive(true);

            //wait for next enemy
            yield return new WaitForSeconds(wave.TimeBetweenSpawns);
        }
    }

    EFace GetRandomFace(Queue<EFace> facesQueue)
    {
        //check every possible face
        List<EFace> faces = new List<EFace>();
        for(int i = 0; i < System.Enum.GetNames(typeof(EFace)).Length; i++)
        {
            //if not inside facesQueue, add to list
            EFace tryingFace = (EFace)i;
            if(facesQueue.Contains(tryingFace) == false)
            {
                faces.Add(tryingFace);
            }
        }

        //select random face in list
        EFace selectedFace = faces[Random.Range(0, faces.Count)];

        //add to queue (clamp at max)
        facesQueue.Enqueue(selectedFace);
        if (facesQueue.Count > waveConfig.Waves[currentWave].IgnorePreviousFacesAtSpawn)
            facesQueue.Dequeue();

        return selectedFace;
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
