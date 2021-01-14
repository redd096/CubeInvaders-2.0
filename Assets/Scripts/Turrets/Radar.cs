using UnityEngine;
using System.Linq;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Turret/Radar")]
[RequireComponent(typeof(RadarGraphics))]
public class Radar : BuildableObject
{
    public Enemy EnemyToAttack { get; private set; }

    void Update()
    {
        //if active, find enemy
        if (IsActive)
        {
            EnemyToAttack = FindEnemy();
        }
    }

    Enemy FindEnemy()
    {
        //find enemies attacking this face and get the nearest
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(x => x.coordinatesToAttack.face == CellOwner.coordinates.face).ToArray();
        return enemies.FindNearest(transform.position);
    }
}
