using UnityEngine;
using System.Linq;

[AddComponentMenu("Cube Invaders/Turret Component/Turret Shot")]
[SelectionBase]
public class TurretShot : MonoBehaviour
{
    [Header("Shot")]
    [SerializeField] float shotSpeed = 1;
    [Tooltip("When shot target die, start autodestruction timer")] [SerializeField] float timerAutodestructionWithoutEnemy = 5;
    [Tooltip("On autodestruction, do area damage or area slow anyway")] [SerializeField] bool areaEffectAlsoOnAutodestruction = false;

    [Header("Effect")]
    [Min(0)]
    [SerializeField] float damage = 10;
    [Range(0, 100)]
    [SerializeField] float slowPercentage = 0;
    [Min(0)]
    [SerializeField] float slowDuration = 0;
    [Min(0)]
    [SerializeField] float area = 0;

    Coordinates coordinatesToDefend;
    Enemy enemyToAttack;

    float timerAutodestruction;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //look at enemy
        if (enemyToAttack)
        {
            transform.LookAt(enemyToAttack.transform);
        }

        //and check if is time to auto destruction
        TryAutoDestruction();
    }

    void FixedUpdate()
    {
        //direction to enemy or forward
        Vector3 direction = Vector3.zero;

        if (enemyToAttack != null)
        {
            direction = enemyToAttack.transform.position - transform.position;
        }
        else
        {
            direction = transform.forward;
        }

        //move
        rb.velocity = direction.normalized * shotSpeed;
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
            DestroyShot(enemy);
        }
    }

    #region private API

    void TryAutoDestruction()
    {
        //update timer
        if (enemyToAttack == null)
        {
            timerAutodestruction += Time.deltaTime;
        }

        //check timer
        if (timerAutodestruction >= timerAutodestructionWithoutEnemy)
        {
            //autodestruction
            DestroyShot(null);
        }
    }

    void DestroyShot(Enemy hitEnemy)
    {
        //if hit enemy, or can do area effect also on autodestruction
        if(hitEnemy || areaEffectAlsoOnAutodestruction)
        {
            //do area effect
            AreaEffect(hitEnemy);
        }

        //destroy this
        redd096.Pooling.Destroy(gameObject);
    }

    void AreaEffect(Enemy hitEnemy)
    {
        //find enemies on the same face, inside the area effect
        FindObjectsOfType<Enemy>().Where(
            x => x != hitEnemy 
            && x.coordinatesToAttack.face == coordinatesToDefend.face 
            && Vector3.Distance(x.transform.position, transform.position) < area).ToList()
            
            //apply effect on every enemy
            .ForEach(x => ApplyEffect(x));
    }

    void ApplyEffect(Enemy enemy)
    {
        //do damage and slow
        enemy.GetDamage(damage);
        enemy.GetSlow(slowPercentage, slowDuration);
    }

    #endregion

    #region public API

    public void Init(TurretShooter owner, Enemy enemyToAttack)
    {
        coordinatesToDefend = owner.CellOwner.coordinates;
        this.enemyToAttack = enemyToAttack;

        //reset timer
        timerAutodestruction = 0;
    }

    #endregion
}
