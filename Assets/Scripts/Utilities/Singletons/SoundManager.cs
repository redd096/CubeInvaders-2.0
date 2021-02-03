namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Important")]
        [SerializeField] AudioSource audioPrefab = default;

        private AudioSource backgroundAudioSource;
        AudioSource BackgroundAudioSource
        {
            get
            {
                //create audio source if null
                if (backgroundAudioSource == null)
                    backgroundAudioSource = gameObject.AddComponent<AudioSource>();

                //return audio source
                return backgroundAudioSource;
            }
        }

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public void PlayBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //start music from this audio source
            Play(BackgroundAudioSource, clip, false, volume, loop);
        }

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public static void Play(AudioSource audioSource, AudioClip clip, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different clip (so we can have same music in different scenes without stop)
            if (forceReplay || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public void Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return;

            AudioSource audioSource = pool.Instantiate(audioPrefab, position, Quaternion.identity);
            Play(audioSource, clip, true, volume);
        }
    }
}