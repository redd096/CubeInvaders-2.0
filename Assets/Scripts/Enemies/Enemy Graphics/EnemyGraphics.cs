using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Graphics")]
public class EnemyGraphics : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkTime = 0.1f;

    [Header("VFX")]
    [SerializeField] ParticleSystem explosionParticlePrefab = default;
    [SerializeField] AudioStruct explosionSound = default;

    Pooling<ParticleSystem> poolExplosionParticles = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolExplosionSound = new Pooling<AudioSource>();

    Enemy enemy;

    //for blink
    Material originalMat;
    Coroutine blink_Coroutine;

    void OnEnable()
    {
        enemy = GetComponent<Enemy>();
        enemy.onGetDamage += OnGetDamage;
        enemy.onEnemyDeath += OnEnemyDeath;
    }

    void OnDisable()
    {
        if (enemy)
        {
            enemy.onGetDamage -= OnGetDamage;
        }
    }

    void OnGetDamage()
    {
        //blink on get damage
        if (blink_Coroutine == null && gameObject.activeInHierarchy)
            blink_Coroutine = StartCoroutine(Blink_Coroutine());
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

    void OnEnemyDeath(Enemy enemy)
    {
        //vfx and sound
        ParticlesManager.instance.Play(poolExplosionParticles, explosionParticlePrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(poolExplosionSound, explosionSound.audioClip, transform.position, explosionSound.volume);
    }
}
