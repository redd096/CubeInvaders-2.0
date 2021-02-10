using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy")]
[RequireComponent(typeof(EnemyGraphics))]
public class Enemy : EnemyBase
{
    struct SlowEffect
    {
        public float slowPercentage;

        public SlowEffect(float slowPercentage)
        {
            this.slowPercentage = slowPercentage;
        }
    }

    //used by wave manager
    public System.Action<Enemy> onEnemyDeath;

    List<SlowEffect> effectsOnEnemy = new List<SlowEffect>();
    float maxSpeed;

    protected override void Awake()
    {
        base.Awake();

        //set max speed
        maxSpeed = speed;
    }

    protected override void FixedUpdate()
    {
        //apply slow effects
        speed = ApplyEffects();

        base.FixedUpdate();
    }

    protected virtual void OnDestroy()
    {
        //be sure to remove event
        onEnemyDeath = null;
    }

    public override void Die<T>(T hittedBy)
    {
        base.Die(hittedBy);

        //call event
        onEnemyDeath?.Invoke(this);
    }

    #region slow

    public void GetSlow(float slowPercentage, float slowDuration)
    {
        //do only if there is slow effect
        if (slowPercentage <= 0 || slowDuration <= 0)
            return;

        //add slow effect
        SlowEffect slowEffect = new SlowEffect(slowPercentage);
        effectsOnEnemy.Add(slowEffect);

        //remove effect timer
        if(gameObject.activeInHierarchy)
            StartCoroutine(RemoveEffectTimer(new SlowEffect(slowPercentage), slowDuration));
    }

    IEnumerator RemoveEffectTimer(SlowEffect slowEffect, float slowDuration)
    {
        //wait duration
        yield return new WaitForSeconds(slowDuration);

        //remove effect
        effectsOnEnemy.Remove(slowEffect);
    }

    float ApplyEffects()
    {
        float newSpeed = maxSpeed;

        foreach (SlowEffect slowEffect in effectsOnEnemy)
        {
            //slow based on max speed
            float speedToDecrease = maxSpeed / 100 * slowEffect.slowPercentage;
            newSpeed -= speedToDecrease;

            //if reached 0, stop
            if (newSpeed <= 0)
            {
                newSpeed = 0;
                break;
            }
        }

        return newSpeed;
    }

    #endregion
}
