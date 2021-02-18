using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Teleport Graphics")]
public class EnemyTeleportGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem teleportPreviousPositionParticlePrefab = default;
    [SerializeField] AudioStruct teleportPreviousPositionSound = default;
    [SerializeField] ParticleSystem teleportNewPositionParticlePrefab = default;
    [SerializeField] AudioStruct teleportNewPositionSound = default;
    
    Pooling<ParticleSystem> poolTeleportPreviousPositionParticles = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolTeleportPreviousPositionSound = new Pooling<AudioSource>();
    Pooling<ParticleSystem> poolTeleportNewPositionParticles = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolTeleportNewPositionSound = new Pooling<AudioSource>();

    EnemyTeleport enemyTeleport;

    void OnEnable()
    {
        enemyTeleport = GetComponent<EnemyTeleport>();
        enemyTeleport.onTeleport += OnTeleport;
    }

    void OnDisable()
    {
        if (enemyTeleport)
        {
            enemyTeleport.onTeleport -= OnTeleport;
        }
    }

    void OnTeleport(Vector3 previousPosition, Quaternion previousRotation, Vector3 newPosition, Quaternion newRotation)
    {
        //previous
        ParticlesManager.instance.Play(poolTeleportPreviousPositionParticles, teleportPreviousPositionParticlePrefab, previousPosition, previousRotation);
        SoundManager.instance.Play(poolTeleportPreviousPositionSound, teleportPreviousPositionSound.audioClip, previousPosition, teleportPreviousPositionSound.volume);

        //new
        ParticlesManager.instance.Play(poolTeleportNewPositionParticles, teleportNewPositionParticlePrefab, newPosition, newRotation);
        SoundManager.instance.Play(poolTeleportNewPositionSound, teleportNewPositionSound.audioClip, newPosition, teleportNewPositionSound.volume);
    }
}
