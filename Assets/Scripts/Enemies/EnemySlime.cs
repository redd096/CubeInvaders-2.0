using UnityEngine;
using System.Collections.Generic;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Slime")]
public class EnemySlime : Enemy
{
    [Header("Slime Prefab")]
    [Tooltip("First prefab instantiated is in same position of this enemy")] [SerializeField] bool firstSlimeSamePosition = true;
    [Tooltip("Check there are no enemies where spawn slime")] [SerializeField] bool checkNoHitEnemies = true;
    [Tooltip("List of enemies to instantiate on death")] [SerializeField] Enemy[] enemiesPrefabs = default;

    List<Coordinates> coordinatesAlreadyUsed = new List<Coordinates>();
    bool instantiatedSlimes;

    public override void Die<T>(T hittedBy)
    {
        //instantiate slimes (if hit by shot and not already istantiated slimes)
        if (enemiesPrefabs != null && hittedBy.GetType() == typeof(TurretShot) && instantiatedSlimes == false)
        {
            instantiatedSlimes = true;
            InstantiateSlimes();
        }

        base.Die(hittedBy);
    }

    void InstantiateSlimes()
    {
        //foreach prefab
        for(int i = 0; i < enemiesPrefabs.Length; i++)
        {
            Enemy slime = GameManager.instance.waveManager.InstantiateNewEnemy(enemiesPrefabs[i]);

            //if is first slime, replace this enemy
            if(i <= 0 && firstSlimeSamePosition)
            {
                slime.transform.position = transform.position;      //same position
                slime.transform.rotation = transform.rotation;      //same rotation
                slime.coordinatesToAttack = coordinatesToAttack;    //attacking same face

                //add coordinates to list
                coordinatesAlreadyUsed.Add(coordinatesToAttack);
            }
            //else go to adjacent coordinates
            else
            {
                Coordinates adjacentCoordinates = GetAdjacentCoordinates();
                if (adjacentCoordinates != null)
                {
                    //save distance
                    float distance = Vector3.Distance(transform.position, coordinatesToAttack.position);

                    //get new position and rotation
                    Vector3 position;
                    Quaternion rotation;
                    GameManager.instance.world.GetPositionAndRotation(adjacentCoordinates, distance, out position, out rotation);

                    slime.transform.position = position;                //adjacent coordinates, but same distance
                    slime.transform.rotation = rotation;                //new rotation looking at cube
                    slime.coordinatesToAttack = adjacentCoordinates;    //attack this new coordinates

                    //add coordinates to list
                    coordinatesAlreadyUsed.Add(coordinatesToAttack);
                }
                //kill slime if there are no coordinates
                else
                {
                    slime.Die(this);
                    continue;
                }
            }

            //activate slime
            slime.gameObject.SetActive(true);
        }
    }

    Coordinates GetAdjacentCoordinates()
    {
        //get cells around 
        List<Cell> cellsAround = GameManager.instance.world.GetCellsAround(coordinatesToAttack);

        //remove coordinates already used
        foreach(Coordinates coordinates in coordinatesAlreadyUsed)
        {
            foreach(Cell cell in cellsAround)
            {
                if (cell.coordinates == coordinates)
                    cellsAround.Remove(cell);
            }
        }

        //removes coordinates where there are already enemies
        if (checkNoHitEnemies)
        {
            WorldUtility.CheckOverlap(transform.position, coordinatesToAttack.position, cellsAround);
        }

        //return random
        return cellsAround[Random.Range(0, cellsAround.Count)].coordinates;
    }
}
