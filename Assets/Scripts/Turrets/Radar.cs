using UnityEngine;
using System.Linq;
using redd096;

[AddComponentMenu("Cube Invaders/Turret/Radar")]
public class Radar : BuildableObject
{
    public Enemy EnemyToAttack { get; private set; }

    void Start()
    {
        //set builded at start (to set cell owner and set is not a preview)
        BuildTurret(GetComponentInParent<Cell>());
    }

    void Update()
    {
        EnemyToAttack = FindEnemy();
    }

    Enemy FindEnemy()
    {
        //find enemies attacking this face and get the nearest
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(x => x.coordinatesToAttack.face == CellOwner.coordinates.face).ToArray();
        return enemies.FindNearest(transform.position);
    }
}
