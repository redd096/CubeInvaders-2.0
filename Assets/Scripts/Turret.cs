using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret")]
public class Turret : MonoBehaviour
{
    void OnWorldRotate()
    {
        //use Cell.onWorldRotate to know when start to rotate

        GameManager.instance.world.onEndRotation += OnEndRotation;
    }

    void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;
    }
}
