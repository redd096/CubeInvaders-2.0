using UnityEngine;
using redd096;

public class ParticlesManager : Singleton<ParticlesManager>
{
    /// <summary>
    /// Start particles at point and rotation
    /// </summary>
    public void Play(Pooling<ParticleSystem> pool, ParticleSystem prefab, Vector3 position, Quaternion rotation)
    {
        ParticleSystem particles = pool.Instantiate(prefab, position, rotation);
        particles.Play();
    }
}
