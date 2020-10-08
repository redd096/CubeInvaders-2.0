using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy")]
[SelectionBase]
public class Enemy : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] float health = 100;
    [SerializeField] float speed = 1;

    [Header("Blink")]
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkTime = 0.1f;

    [Header("Debug")]
    public Coordinates coordinatesToAttack;

    public System.Action<Enemy> onEnemyDeath;

    //for blink
    Material originalMat;
    Coroutine blink_Coroutine;

    void OnDestroy()
    {
        //be sure to remove event
        onEnemyDeath = null;
    }

    void Update()
    {
        //move to the cell
        Vector3 direction = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack) - transform.position;

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        //check hit cell
        Cell cell = other.GetComponentInParent<Cell>();
        if(cell)
        {
            //kill cell
            cell.KillCell();

            //destroy this enemy
            Die();
        }
    }

    #region private API

    IEnumerator SlowTimer(float slowEffect, float slowDuration)
    {
        //wait duration
        yield return new WaitForSeconds(slowDuration);

        //remove effect
        speed += slowEffect;
    }

    IEnumerator Blink_Coroutine()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        //change material
        if (originalMat == null)
        {
            originalMat = renderer.material;
            renderer.material = blinkMaterial;
        }

        //wait
        yield return new WaitForSeconds(blinkTime);

        //back to original material
        renderer.material = originalMat;
        originalMat = null;

        blink_Coroutine = null;
    }

    void Die()
    {
        //destroy this enemy
        Destroy(gameObject);

        //call event
        onEnemyDeath?.Invoke(this);
    }

    #endregion

    #region public API

    public void GetDamage(float damage)
    {
        //get damage
        health -= damage;

        //check death
        if(health <= 0)
        {
            Die();
            return;
        }

        //blink on hit
        if(blink_Coroutine == null)
            blink_Coroutine = StartCoroutine(Blink_Coroutine());
    }

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

    public void GetDamageFromShield()
    {
        //instant death
        Die();
    }

    #endregion
}
