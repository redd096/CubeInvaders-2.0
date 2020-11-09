using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy/Enemy Poison")]
public class EnemyPoison : Enemy
{
    [Header("Poison")]
    [SerializeField] float timerPoison = 5;

    protected override void OnTriggerEnter(Collider other)
    {
        //check hit shield
        if (CheckHit<Shield>(other))
        {
            //destroy this enemy
            Die(false);

            return;
        }

        //else check hit cell
        if (CheckHit<Cell>(other))
        {
            Cell cell = other.GetComponentInParent<Cell>();

            //poison cell instead of kill it
            cell.gameObject.AddComponent<PoisonCell>().Init(timerPoison);

            //destroy this enemy
            Die(true);
        }
    }
}
