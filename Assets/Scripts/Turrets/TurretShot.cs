using UnityEngine;
using System.Linq;

[AddComponentMenu("Cube Invaders/Turret Shot")]
[SelectionBase]
public class TurretShot : MonoBehaviour
{
    Turret owner;
    Enemy enemyToAttack;

    float timerAutodestruction;

    public void Init(Turret owner, Enemy enemyToAttack)
    {
        this.owner = owner;
        this.enemyToAttack = enemyToAttack;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if(enemyToAttack != null)
        {
            direction = enemyToAttack.transform.position - transform.position;

            //look at enemy
            transform.LookAt(enemyToAttack.transform);
        }
        else
        {
            direction = transform.forward;

            //update timer
            timerAutodestruction += Time.deltaTime;
        }

        //move
        transform.position += direction.normalized * owner.ShotSpeed * Time.deltaTime;

        //and check if is time to auto destruction
        TryAutoDestruction();
    }

    void OnTriggerEnter(Collider other)
    {
        //check hit enemy
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            //apply effect
            ApplyEffect(enemy);

            //destroy shot after hit
            DestroyShot(true);
        }
    }

    #region private API

    void DestroyShot(bool hitEnemy)
    {
        //if hit enemy, or can do area effect also on autodestruction
        if(hitEnemy || owner.AreaEffectAlsoOnAutodestruction)
        {
            //do area effect
            AreaEffect();
        }

        //destroy this
        redd096.Pooling.Destroy(gameObject);
    }

    void TryAutoDestruction()
    {
        //check timer
        if(timerAutodestruction >= owner.TimerAutodestructionWithoutEnemy)
        {
            //autodestruction
            DestroyShot(false);
        }
    }

    void AreaEffect()
    {
        //find enemies on the same face, inside the area effect
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(x => x.coordinatesToAttack.face == owner.CellOwner.coordinates.face 
                                                            && Vector3.Distance(x.transform.position, transform.position) < owner.Area).ToArray();

        //apply effect on every enemy
        foreach (Enemy enemy in enemies)
            ApplyEffect(enemy);
    }

    void ApplyEffect(Enemy enemy)
    {
        //do damage and slow
        enemy.GetDamage(owner.Damage);
        enemy.GetSlow(owner.SlowPercentage, owner.SlowDuration);
    }

    #endregion
}
