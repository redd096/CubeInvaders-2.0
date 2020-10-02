using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] int health = 1;
    [Tooltip("Distance from cell")] [SerializeField] float distanceShield = 1;

    [Header("Graphics")]
    [Tooltip("Time for spawn animation")] [SerializeField] float timeSpawn = 1;

    public System.Action onShieldDestroyed;

    int currentHealth;

    void OnTriggerEnter(Collider other)
    {
        //check hit enemy
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            //TODO
            //kill enemy
            ShieldGetDamage();
        }
    }

    #region private API

    void ShieldGetDamage()
    {
        currentHealth--;

        //if dead, shield destroyed
        if(currentHealth <= 0)
        {
            onShieldDestroyed?.Invoke();
        }
    }

    #endregion

    #region public API

    public void ResetHealth()
    {
        //reset health
        currentHealth = health;
    }

    public void ActivateShield(Coordinates coordinates)
    {
        //TODO
        //activate, same size of the face and turret rotation -> position of the cell + distance
    }

    #endregion
}
