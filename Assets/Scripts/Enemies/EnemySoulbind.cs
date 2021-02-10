using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using System.Linq;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Soulbind")]
[RequireComponent(typeof(EnemySoulbindGraphics))]
public class EnemySoulbind : Enemy
{
    [Header("Soulbind")]
    [Tooltip("Share health, or different health but die together?")] [SerializeField] bool shareHealth = true;
    [CanShow("shareHealth", NOT = true)]
    [Tooltip("On death, kill also soulbind or keep alive?")] [SerializeField] bool killSoulbindOnDeath = true;
    [Tooltip("Use opposite face or adjacent face?")] [SerializeField] bool useOppositeFace = true;
    [Tooltip("Instantiate at same coordinates or random?")] [SerializeField] bool sameCell = false;
    [CanShow("sameCell", NOT = true)]
    [Tooltip("Check there are no enemies where spawn")] [SerializeField] bool checkNoHitEnemies = true;

    public System.Action<Vector3, Quaternion, Vector3, Quaternion> onSpawnSoulbind;

    public EnemySoulbind soulBind { get; set; }
    public TurretShooter[] turretsAiming { get; private set; }
    public bool damageSoulBind { get; set; } = true;

    public override void Init(Coordinates coordinatesToAttack)
    {
        base.Init(coordinatesToAttack);

        //get opposite face
        EFace oppositeFace = WorldUtility.GetOppositeFace(coordinatesToAttack.face);

        //use opposite face, or every adjacent face (no this one and no opposite)
        Queue<EFace> facesQueue = new Queue<EFace>();
        facesQueue.Enqueue(coordinatesToAttack.face);
        facesQueue.Enqueue(oppositeFace);
        EFace faceSoulbind = useOppositeFace ? oppositeFace : WorldUtility.GetRandomFace(facesQueue, 2);

        //use same coordinates on new face, or get random coordinates
        Coordinates newCoordinates = sameCell ? new Coordinates(faceSoulbind, coordinatesToAttack.x, coordinatesToAttack.y) : GetNewCoordinates(faceSoulbind);
        if (newCoordinates != null)
        {
            //save distance
            float distance = Vector3.Distance(transform.position, coordinatesToAttack.position);

            //get new position and rotation
            Vector3 position;
            Quaternion rotation;
            GameManager.instance.world.GetPositionAndRotation(newCoordinates, distance, out position, out rotation);

            //instantiate soulbind
            soulBind = GameManager.instance.waveManager.InstantiateNewEnemy(this, 0) as EnemySoulbind;
            soulBind.transform.position = position;             //set position
            soulBind.transform.rotation = rotation;             //set rotation
            soulBind.coordinatesToAttack = newCoordinates;      //set new coordinates to attack
            soulBind.soulBind = this;                           //set this one as its soulbind

            //active soulbind and call event
            soulBind.gameObject.SetActive(true);
            onSpawnSoulbind?.Invoke(transform.position, transform.rotation, position, rotation);

        }
        //kill if there are no coordinates
        else
        {
            Die(this);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //find every turret shooter aiming at this enemy
        turretsAiming = FindObjectsOfType<TurretShooter>().Where(x => x.EnemyToAttack == this).ToArray();
    }

    public override void GetDamage(float damage, TurretShot whoHit)
    {
        //if aiming at this one and its soulbind, get damage
        if (turretsAiming.Length > 0 && (soulBind == null || soulBind.turretsAiming.Length > 0))
        {
            base.GetDamage(damage, whoHit);

            //if share health, damage also soulbind (only if damageSoulBind is true)
            if(soulBind && shareHealth && damageSoulBind)
            {
                soulBind.damageSoulBind = false;
                soulBind.GetDamage(damage, whoHit);
                soulBind.damageSoulBind = true;
            }
        }
    }

    public override void Die<T>(T hittedBy)
    {
        base.Die(hittedBy);

        //if kill also soulbind (only if damageSoulBind is true)
        if (soulBind && killSoulbindOnDeath && damageSoulBind)
        {
            //be sure shareHealth is false, otherwise is already killed by get damage
            if (shareHealth == false)
            {
                soulBind.damageSoulBind = false;
                soulBind.Die(hittedBy);
                soulBind.damageSoulBind = true;
            }
        }
    }

    #region private API

    Coordinates GetNewCoordinates(EFace newFace)
    {
        //get cells in new face
        List<Cell> possibleCells = GameManager.instance.world.GetEveryCellInFace(newFace);

        //removes coordinates where there are already enemies
        if (checkNoHitEnemies)
        {
            WorldUtility.CheckOverlap(transform.position, coordinatesToAttack.position, possibleCells);
        }

        //return random
        return possibleCells[Random.Range(0, possibleCells.Count)].coordinates;
    }

    #endregion
}
