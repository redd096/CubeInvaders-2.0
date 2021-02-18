using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Soulbind Graphics")]
public class EnemySoulbindGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem particlesSpawnFirstSoulbind = default;
    [SerializeField] AudioStruct soundSpawnFirstSoulbind = default;
    [SerializeField] ParticleSystem particlesSpawnSecondSoulbind = default;
    [SerializeField] AudioStruct soundSpawnSecondSoulbind = default;

    Pooling<ParticleSystem> poolParticlesSpawnFirstSoulbind = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundSpawnFirstSoulbind = new Pooling<AudioSource>();
    Pooling<ParticleSystem> poolParticlesSpawnSecondSoulbind = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundSpawnSecondSoulbind = new Pooling<AudioSource>();

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
        ParticlesManager.instance.Play(poolParticlesSpawnFirstSoulbind, particlesSpawnFirstSoulbind, firstPosition, firstRotation);
        SoundManager.instance.Play(poolSoundSpawnFirstSoulbind, soundSpawnFirstSoulbind.audioClip, firstPosition, soundSpawnFirstSoulbind.volume);

        //new
        ParticlesManager.instance.Play(poolParticlesSpawnSecondSoulbind, particlesSpawnSecondSoulbind, secondPosition, secondRotation);
        SoundManager.instance.Play(poolSoundSpawnSecondSoulbind, soundSpawnSecondSoulbind.audioClip, secondPosition, soundSpawnSecondSoulbind.volume);
    }
}
