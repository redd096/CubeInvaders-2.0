using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    public WaveConfig waveConfig;
    public int currentWave = 0;

    int enemiesToEndWave;
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
        enemiesToEndWave--;

        //if there are no other enemies, end assault phase
        if(enemiesToEndWave <= 0)
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
    }

    void CreateWave()
    {
        //do only if there are waves
        if (currentWave < 0 || currentWave >= waveConfig.Waves.Length)
            return;

        //current wave
        WaveStruct wave = waveConfig.Waves[currentWave];

        //update level config and biomes config
        GameManager.instance.UpdateLevel(wave.LevelConfig);
        GameManager.instance.UpdateLevel(wave.BiomesConfig);
    }

    IEnumerator Wave_Coroutine()
    {
        //set enemies number
        enemiesToEndWave = waveConfig.Waves[currentWave].EnemiesPrefabs.Length;

        //for every enemy
        foreach (Enemy enemyPrefab in waveConfig.Waves[currentWave].EnemiesPrefabs)
        {
            //instantiate and set parent and event
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.onEnemyDeath += OnEnemyDeath;

            //randomize coordinates to attack
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            int y = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            Coordinates coordinatesToAttack = new Coordinates(face, x, y);

            //set enemy position
            float distanceFromWorld = waveConfig.distanceFromWorld;
            enemy.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack, distanceFromWorld);

            //set enemy destination and activate
            enemy.coordinatesToAttack = coordinatesToAttack;
            enemy.gameObject.SetActive(true);

            //wait for next enemy
            yield return new WaitForSeconds(waveConfig.TimeBetweenSpawns);
        }
    }

    #endregion
}
