using System.Collections;
using UnityEngine;
using redd096;
using System.Linq;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Turret/Turret Shooter")]
[RequireComponent(typeof(TurretShooterGraphics))]
public class TurretShooter : Turret
{
    #region variables

    [Header("Important")]
    [SerializeField] TurretShot shotPrefab = default;
    [Tooltip("Delay between every shoot")] [SerializeField] float delayShoot = 0.5f;
    [Tooltip("Where the shot spawn. Cycle between them")] [SerializeField] Transform[] shotSpawns = default;

    //used for the graphics
    public System.Action<Transform> onShoot;
    public Enemy EnemyToAttack { get; private set; }

    bool canShoot = true;
    float timeToShoot;
    int indexSpawn;
    Pooling<TurretShot> shots = new Pooling<TurretShot>();

    Coroutine canShootAgain_Coroutine;

    #endregion

    protected virtual void Update()
    {
        //do only if can shoot and is active
        if (canShoot == false || IsActive == false)
            return;

        TryAttack();
    }

    public override void RemoveTurret()
    {
        //if coroutine is running when we disable this turret, 
        //stop coroutine and set immediatly that can shoot, for when we will reactivate it
        if (canShootAgain_Coroutine != null)
            StopCoroutine(canShootAgain_Coroutine);

        canShoot = true;

        base.RemoveTurret();
    }

    #region try attack

    void TryAttack()
    {
        //find enemy
        EnemyToAttack = FindEnemy();

        //if aiming an enemy and is time to shoot
        if (EnemyToAttack != null && Time.time > timeToShoot)
        {
            timeToShoot = Time.time + delayShoot;

            Attack();
        }
    }

    Enemy FindEnemy()
    {
        //find enemies attacking this face and get the nearest
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(x => x.coordinatesToAttack.face == CellOwner.coordinates.face).ToArray();
        return enemies.FindNearest(transform.position);
    }

    protected virtual void Attack()
    {
        //create shot (pool, position, rotation, scale, init)
        TurretShot shot = shots.Instantiate(shotPrefab, shotSpawns[indexSpawn].position, shotSpawns[indexSpawn].rotation);
        float size = GameManager.instance.world.worldConfig.CellsSize;
        shot.transform.localScale = new Vector3(size, size, size);
        shot.Init(this, EnemyToAttack);

        //call event
        onShoot?.Invoke(shotSpawns[indexSpawn]);

        //cycle between spawns
        indexSpawn = indexSpawn < shotSpawns.Length - 1 ? indexSpawn + 1 : 0;
    }

    #endregion

    #region on world rotate

    protected override void OnWorldRotate(Coordinates coordinates)
    {
        base.OnWorldRotate(coordinates);

        //stop shooting on world rotate
        if (canShootAgain_Coroutine != null)
            StopCoroutine(canShootAgain_Coroutine);

        canShoot = false;
        EnemyToAttack = null;
    }

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        //start coroutine to shoot again
        if(gameObject.activeInHierarchy)
            canShootAgain_Coroutine = StartCoroutine(CanShootAgain());
    }

    IEnumerator CanShootAgain()
    {
        //wait, then can shoot again
        yield return new WaitForSeconds(0.2f);

        canShoot = true;
    }

    #endregion
}
