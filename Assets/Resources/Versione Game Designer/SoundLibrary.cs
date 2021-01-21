using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class SoundLibrary : MonoBehaviour
    {
        public AudioClip rotationAudio;
        public AudioClip cellDestructionAudio;
        public AudioClip destructionTurretAudio;
        public AudioClip enemyDestructionAudio;
        public AudioClip enemySlimestateAudio;

        private AudioSource music;

        private void Start()
        {
            music = GetComponent<AudioSource>();
        }

        public  void Rotation()
        {
                music.PlayOneShot(rotationAudio, 1);      
        }

        public void CellDestruction()
        {

        }

        public void TurretDestruction()
        {

        }

        public void EnemyDestruction()
        {

        }

        public void EnemySlime()
        {

        }
    }

