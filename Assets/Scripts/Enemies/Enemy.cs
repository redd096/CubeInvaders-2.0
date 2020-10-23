using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy/Enemy")]
public class Enemy : EnemyBase
{
    public System.Action<Enemy> onEnemyDeath;

    void OnDestroy()
    {
        //be sure to remove event
        onEnemyDeath = null;
    }

    protected override void Die(bool hitPlayer)
    {
        base.Die(hitPlayer);

        //call event
        onEnemyDeath?.Invoke(this);
    }

    IEnumerator SlowTimer(float slowEffect, float slowDuration)
    {
        //wait duration
        yield return new WaitForSeconds(slowDuration);

        //remove effect
        speed += slowEffect;
    }

    #region public API

    public void GetSlow(float slowPercentage, float slowDuration)
    {
        //do only if there is slow effect
        if (slowPercentage <= 0 || slowDuration <= 0)
            return;

        //slow
        float speedToDecrease = speed / 100 * slowPercentage;
        speed -= speedToDecrease;

        //start slow timer
        StartCoroutine(SlowTimer(speedToDecrease, slowDuration));
    }

    #endregion
}
