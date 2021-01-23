using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class SoundLibrary : MonoBehaviour
    {
        public AudioClip rotationAudio;
        public AudioClip turretBuildingAudio;
        public AudioClip enemyDestructionAudio;
        public AudioClip enemySlimestateAudio;

        private AudioSource music;

        private void Start()
        {
            music = GetComponent<AudioSource>();
        }

        public  void Rotation()
        {
                music.PlayOneShot(rotationAudio, 0.1f);      
        }

        public void TurretBuilding()
        {
                music.PlayOneShot(turretBuildingAudio, 1);
        }

        public void EnemyDestruction()
        {
                music.PlayOneShot(enemyDestructionAudio, 1);
        }

        public void EnemySlime()
        {
               music.PlayOneShot(enemySlimestateAudio, 1);
        }
    }

