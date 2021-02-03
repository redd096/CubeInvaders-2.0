using UnityEngine;
using redd096;

public class EnemyBase : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;

    [Header("VFX")]
    [SerializeField] ParticleSystem explosionParticlePrefab = default;
    [SerializeField] AudioClip explosionSound = default;

    [Header("Debug")]
    public Coordinates coordinatesToAttack;

    Rigidbody rb;

    public System.Action onGetDamage;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        //move to the cell
        Vector3 direction = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack) - transform.position;

        rb.velocity = direction.normalized * speed;
    }

    #region public API

    public virtual void GetDamage(float damage, TurretShot whoHit)
    {
        //invoke event
        onGetDamage?.Invoke();

        //get damage
        health -= damage;

        //check death
        if (health <= 0)
        {
            Die(whoHit);
            return;
        }
    }

    public virtual void Die<T>(T hittedBy) where T : Component
    {
        //vfx and sound
        Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
        SoundManager.StartMusic(explosionSound, transform.position);

        //destroy this enemy
        Destroy(gameObject);
    }

    #endregion
}
