using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Poison")]
public class EnemyPoison : Enemy
{
    [Header("Poison")]
    [SerializeField] float timerPoison = 5;
    [SerializeField] int limitSpread = 1;

    protected override void OnTriggerEnter(Collider other)
    {
        //do once
        if (alreadyHit)
            return;

        alreadyHit = true;

        //check hit shield
        if (CheckHit<Shield>(other))
        {
            //damage shield
            other.GetComponentInParent<Shield>().ShieldGetDamage();

            //destroy this enemy
            Die(false);

            return;
        }

        //else check hit cell
        if (CheckHit<Cell>(other))
        {
            Cell cell = other.GetComponentInParent<Cell>();

            //poison cell instead of kill it
            cell.gameObject.AddComponent<PoisonCell>().Init(timerPoison, limitSpread);

            //destroy this enemy
            Die(true);
        }
    }
}
