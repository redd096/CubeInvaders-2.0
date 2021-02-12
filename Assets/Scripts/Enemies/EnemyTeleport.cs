using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Teleport")]
[RequireComponent(typeof(EnemyTeleportGraphics))]
public class EnemyTeleport : Enemy
{
    [Header("Teleport at percentage life")]
    [Tooltip("Check there are no enemies where teleport")] [SerializeField] bool checkNoHitEnemies = true;
    [Tooltip("Ignore previous faces when teleport")] [Min(1)] [SerializeField] int numberOfPreviousFacesToIgnore = 1;
    [Tooltip("When reach one of this percentages of life, do teleport")] [SerializeField] [Range(0, 100)] int[] percentagesLife = default;

    public System.Action<Vector3, Quaternion, Vector3, Quaternion> onTeleport;

    float maxHealth;
    int previousPercentage;

    //queue to not spawn on same face
    Queue<EFace> facesQueue = new Queue<EFace>();

    protected override void Awake()
    {
        base.Awake();

        //save max health and set max percentage
        maxHealth = health;
        previousPercentage = 100;
    }

    public override void Init(Coordinates coordinatesToAttack)
    {
        base.Init(coordinatesToAttack);

        //add start face at queue
        facesQueue.Enqueue(coordinatesToAttack.face);
    }

    public override void GetDamage(float damage, TurretShot whoHit)
    {
        base.GetDamage(damage, whoHit);

        //check if teleport
        if (CheckTeleport() == false)
            return;

        //get new random face to teleport and save previous position
        EFace randomFace = WorldUtility.GetRandomFace(facesQueue, numberOfPreviousFacesToIgnore);

        //find coordinates where teleport
        Coordinates newCoordinates = GetNewCoordinates(randomFace);
        if (newCoordinates != null)
        {
            //save distance
            float distance = Vector3.Distance(transform.position, coordinatesToAttack.position);

            //get new position and rotation
            Vector3 position;
            Quaternion rotation;
            GameManager.instance.world.GetPositionAndRotation(newCoordinates, distance, out position, out rotation);

            //call event
            onTeleport?.Invoke(transform.position, transform.rotation, position, rotation);

            transform.position = position;          //new coordinates, but same distance
            transform.rotation = rotation;          //new rotation looking at cube
            coordinatesToAttack = newCoordinates;   //attack this new coordinates

        }
        //kill if there are no coordinates
        else
        {
            Die(this);
        }
    }

    bool CheckTeleport()
    {
        //get current percentage
        int currentPercentage = Mathf.FloorToInt(health / maxHealth * 100);

        int percentageToCheck = 100;
        foreach (int percentage in percentagesLife)
        {
            //check if our life is lower then percentage
            if (currentPercentage <= percentage)
            {
                //get only lower one (nearest to our current percentage)
                if (percentage < percentageToCheck)
                    percentageToCheck = percentage;
            }
        }

        //if got nothing, or again previous percentage, do not teleport again
        if (percentageToCheck <= 0 || percentageToCheck >= previousPercentage)
            return false;

        //save previous
        previousPercentage = percentageToCheck;

        return true;
    }

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
}
