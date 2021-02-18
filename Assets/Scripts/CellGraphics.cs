using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/World/Cell Graphics")]
public class CellGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem rotationParticlePrefab = default;
    [SerializeField] AudioStruct rotationSound = default;
    [SerializeField] ParticleSystem explosionCellPrefab = default;
    [SerializeField] AudioStruct explosionCellSound = default;

    Pooling<ParticleSystem> poolRotationParticles = new Pooling<ParticleSystem>(1);
    Pooling<AudioSource> poolRotationSound = new Pooling<AudioSource>(1);
    Pooling<ParticleSystem> poolExplosionCell = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolExplosionSound = new Pooling<AudioSource>();

    CameraShake camShake;
    Cell cell;

    private void Awake()
    {
        camShake = FindObjectOfType<CameraShake>();
        cell = GetComponent<Cell>();

        cell.onWorldRotate += OnWorldRotate;
        cell.onDestroyCell += OnDestroyCell;
    }

    private void OnDestroy()
    {
        if(cell)
        {
            cell.onWorldRotate -= OnWorldRotate;
            cell.onDestroyCell -= OnDestroyCell;
        }
    }

    void OnWorldRotate(Coordinates coordinates)
    {
        ParticlesManager.instance.Play(poolRotationParticles, rotationParticlePrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(poolRotationSound, rotationSound.audioClip, transform.position, rotationSound.volume);
    }

    void OnDestroyCell()
    {
        ParticlesManager.instance.Play(poolExplosionCell, explosionCellPrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(poolExplosionSound, explosionCellSound.audioClip, transform.position, explosionCellSound.volume);

        //do camera shake
        camShake.DoShake();
    }
}
