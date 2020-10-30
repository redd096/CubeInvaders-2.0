﻿using UnityEngine;

[SelectionBase]
public class EnemyBase : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;

    [Header("Debug")]
    public Coordinates coordinatesToAttack;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //move to the cell
        Vector3 direction = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack) - transform.position;

        rb.velocity = direction.normalized * speed;
    }

    protected virtual void OnTriggerEnter(Collider other)
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
            //kill cell
            other.GetComponentInParent<Cell>().KillCell();

            //destroy this enemy
            Die(true);
        }
    }

    #region private API

    protected bool CheckHit<T>(Collider other) where T : Component
    {
        //check hit
        T obj = other.GetComponentInParent<T>();
        if(obj)
        {
            return true;
        }

        return false;
    }

    protected virtual void Die(bool hitPlayer)
    {
        //destroy this enemy
        Destroy(gameObject);
    }

    #endregion

    #region public API

    public void GetDamage(float damage)
    {
        //get damage
        health -= damage;

        //check death
        if (health <= 0)
        {
            Die(false);
            return;
        }
    }

    #endregion
}
