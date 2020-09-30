using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using System.Linq;

[AddComponentMenu("Cube Invaders/Turret")]
[SelectionBase]
public class Turret : MonoBehaviour
{
    #region variables

    [Header("Important")]
    [SerializeField] TurretShot shotPrefab = default;
    [Tooltip("Delay between every shoot")] [SerializeField] float delayShoot = 0.5f;
    [Tooltip("Where the shot spawn. Cycle between them")] [SerializeField] Transform[] shotSpawns = default;

    [Header("Shot")]
    public float ShotSpeed = 1;
    [Tooltip("When shot target die, start autodestruction timer")] public float TimerAutodestructionWithoutEnemy = 5;
    [Tooltip("On autodestruction, do area damage or area slow anyway")] [SerializeField] public bool AreaEffectAlsoOnAutodestruction = true;

    [Header("Effect")]
    [Min(0)]
    public float Damage = 10;
    [Range(0, 100)]
    public float SlowPercentage = 0;
    [Min(0)]
    public float SlowDuration = 0;
    [Min(0)]
    public float Area = 0;

    //used for the graphics
    public System.Action onShoot;

    public Cell CellOwner { get; private set; }

    bool canShoot = true;
    float timeToShoot;
    int indexSpawn;
    Pooling<TurretShot> shots = new Pooling<TurretShot>();

    #endregion

    void Update()
    {
        //do only if can shoot
        if (canShoot == false)
            return;

        TryAttack();
    }

    #region private API

    #region try attack

    void TryAttack()
    {
        Enemy enemyToAttack = FindEnemy();

        //if aiming an enemy and is time to shoot
        if(enemyToAttack != null && Time.time > timeToShoot)
        {
            timeToShoot = Time.time + delayShoot;

            Attack(enemyToAttack);
        }
    }

    Enemy FindEnemy()
    {
        //find enemies attacking this face and get the nearest
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(x => x.coordinatesToAttack.face == CellOwner.coordinates.face).ToArray();
        return enemies.FindNearest(transform.position);
    }

    void Attack(Enemy enemyToAttack)
    {
        //create shot (pool, position, rotation, scale, init)
        TurretShot shot = shots.Instantiate(shotPrefab, shotSpawns[indexSpawn].position, shotSpawns[indexSpawn].rotation);
        float size = GameManager.instance.world.worldConfig.CellsSize;
        shot.transform.localScale = new Vector3(size, size, size);
        shot.Init(this, enemyToAttack);

        //cycle between spawns
        indexSpawn = indexSpawn < shotSpawns.Length - 1 ? indexSpawn + 1 : 0;

        //call event
        onShoot?.Invoke();
    }

    #endregion

    #region on world rotate

    void OnWorldRotate()
    {
        //use cellOwner.onWorldRotate to know when start to rotate
        GameManager.instance.world.onEndRotation += OnEndRotation;

        canShoot = false;
    }

    void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;

        canShoot = true;
    }

    #endregion

    #endregion

    #region public API

    public void ActivateTurret(Cell cellOwner)
    {
        //get owner and set event
        this.CellOwner = cellOwner;
        cellOwner.onWorldRotate += OnWorldRotate;
    }

    public void DeactivateTurret()
    {
        //deactive and remove event
        gameObject.SetActive(false);
        CellOwner.onWorldRotate -= OnWorldRotate;
    }

    #endregion
}
