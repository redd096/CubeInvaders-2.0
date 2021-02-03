using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;
    public ParticleSystem expParticle;
    private SoundLibrary soundLibrary;

    [Header("Debug")]
    public Coordinates coordinatesToAttack;

    Rigidbody rb;
    MeshRenderer mR;

    public System.Action onGetDamage;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mR = GetComponentInChildren<MeshRenderer>();
        soundLibrary = GameObject.Find("AudioManager").GetComponent<SoundLibrary>();
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
        //destroy this enemy
        expParticle.Play();
        soundLibrary.EnemyDestruction();
        mR.enabled = false;
        StartCoroutine("Dead");
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    #endregion
}
