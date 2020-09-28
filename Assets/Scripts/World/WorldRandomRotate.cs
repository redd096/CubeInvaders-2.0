﻿using System.Collections;
using UnityEngine;

[AddComponentMenu("Cube Invaders/World/World Random Rotate")]
public class WorldRandomRotate : MonoBehaviour
{
    [Tooltip("Enable randomize")]
    [SerializeField] bool active = true;
    [Tooltip("Loop or do only N times?")]
    [SerializeField] bool loop = false;
    [Tooltip("How many times you want to randomize")]
    [SerializeField] int randomizeTimes = 15;
    [Tooltip("Time to wait before start to randomize")]
    [SerializeField] float timeBeforeRandomize = 1;
    [Tooltip("Time for the animation")]
    [SerializeField] float rotationTime = 0.1f;
    [Tooltip("Time between every rotation")]
    [SerializeField] float timeBetweenRotation = 0f;

    World world;

    bool waitRotation;
    Coroutine randomizeWorld_Coroutine;

    public bool StartRandomize()
    {
        //do only if active
        if (active == false)
            return false;

        //set world
        world = GameManager.instance.world;

        //start randomize
        if (randomizeWorld_Coroutine != null)
            StopCoroutine(randomizeWorld_Coroutine);

        randomizeWorld_Coroutine = StartCoroutine(RandomizeWorld());

        return true;
    }

    IEnumerator RandomizeWorld()
    {
        //wait before randomize
        yield return new WaitForSeconds(timeBeforeRandomize);

        //for n times, rotate row or column
        for (int i = 0; i < randomizeTimes; i++)
        {
            //randomize rotation
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, world.worldConfig.NumberCells);
            int y = Random.Range(0, world.worldConfig.NumberCells);
            ERotateDirection randomRotation = (ERotateDirection)Random.Range(0, 5);

            //effective rotation
            world.RandomRotate(face, x, y, randomRotation, rotationTime);

            //wait until the end of the rotation
            OnStartRotation();
            yield return new WaitWhile(() => waitRotation);

            //if not last rotation, wait time between every rotation
            if (i < randomizeTimes - 1)
                yield return new WaitForSeconds(timeBetweenRotation);

            //repeat
            if (loop)
                i = 0;
        }

        //call start game
        GameManager.instance.levelManager.StartGame();
    }

    void OnStartRotation()
    {
        //start wait rotation
        waitRotation = true;
        world.onEndRotation += OnEndRotation;
    }

    void OnEndRotation()
    {
        //end wait rotation
        waitRotation = false;
        world.onEndRotation -= OnEndRotation;
    }
}