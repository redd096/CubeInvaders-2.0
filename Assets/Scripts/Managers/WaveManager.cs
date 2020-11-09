using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    public WaveConfig waveConfig;
    public int currentWave = 0;

    List<Enemy> enemies = new List<Enemy>();
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
        enemies.Remove(enemy);

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

        //update level config and biomes config
        GameManager.instance.UpdateLevel(wave.LevelConfig);
        GameManager.instance.UpdateLevel(wave.BiomesConfig);

        //foreach enemy in this wave
        foreach (Enemy enemyPrefab in wave.EnemiesPrefabs)
        {
            //instantiate and set parent but deactivate
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);

            //save in the list and add to the event
            enemies.Add(enemy);
            enemy.onEnemyDeath += OnEnemyDeath;
        }
    }

    IEnumerator Wave_Coroutine()
    {
        //create a copy, so we can touch the original without affect the wave coroutine
        Enemy[] enemiesThisWave = enemies.CreateCopy().ToArray();

        //for every enemy
        foreach(Enemy enemy in enemiesThisWave)
        {
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
