using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Soulbind Graphics")]
public class EnemySoulbindGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem particlesFirstSoulbind = default;
    [SerializeField] AudioClip soundFirstSoulbind = default;
    [SerializeField] ParticleSystem particlesSecondSoulbind = default;
    [SerializeField] AudioClip soundSecondSoulbind = default;

    Pooling<ParticleSystem> poolParticlesFirstSoulbind = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundFirstSoulbind = new Pooling<AudioSource>();
    Pooling<ParticleSystem> poolParticlesSecondSoulbind = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundSecondSoulbind = new Pooling<AudioSource>();

    EnemySoulbind enemy;

    void OnEnable()
    {
        enemy = GetComponent<EnemySoulbind>();
        enemy.onSpawnSoulbind += OnSpawnSoulbind;
    }

    void OnDisable()
    {
        if (enemy)
        {
            enemy.onSpawnSoulbind -= OnSpawnSoulbind;
        }
    }

    void OnSpawnSoulbind(Vector3 firstPosition, Quaternion firstRotation, Vector3 secondPosition, Quaternion secondRotation)
    {
        //previous
        ParticlesManager.instance.Play(poolParticlesFirstSoulbind, particlesFirstSoulbind, firstPosition, firstRotation);
        SoundManager.instance.Play(poolSoundFirstSoulbind, soundFirstSoulbind, firstPosition);

        //new
        ParticlesManager.instance.Play(poolParticlesSecondSoulbind, particlesSecondSoulbind, secondPosition, secondRotation);
        SoundManager.instance.Play(poolSoundSecondSoulbind, soundSecondSoulbind, secondPosition);
    }
}
