﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy")]
[SelectionBase]
public class Enemy : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] float health = 100;
    [SerializeField] float speed = 1;

    [Header("Debug")]
    public Coordinates coordinatesToAttack;

    float currentSpeed;

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
            Destroy(gameObject);
        }
    }

    #region private API

    IEnumerator SlowTimer(float slowEffect, float slowDuration)
    {
        //wait duration
        yield return new WaitForSeconds(slowDuration);

        //remove effect
        currentSpeed += slowEffect;
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
            Destroy(gameObject);
        }
    }

    public void GetSlow(float slowPercentage, float slowDuration)
    {
        //do only if there is slow effect
        if (slowPercentage <= 0 || slowDuration <= 0)
            return;

        //slow
        float speedToDecrease = currentSpeed / 100 * slowPercentage;
        currentSpeed -= speedToDecrease;

        //start slow timer
        StartCoroutine(SlowTimer(speedToDecrease, slowDuration));
    }

    public void GetDamageFromShield()
    {
        //instant death
        Destroy(gameObject);
    }

    #endregion
}
