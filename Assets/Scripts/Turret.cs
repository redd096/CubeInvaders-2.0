using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret")]
public class Turret : MonoBehaviour
{
    Cell cellOwner;

    void OnWorldRotate()
    {
        //use cellOwner.onWorldRotate to know when start to rotate

        GameManager.instance.world.onEndRotation += OnEndRotation;
    }

    void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;
    }

    #region public API

    public void ActivateTurret(Cell cellOwner)
    {
        //get owner and set event
        this.cellOwner = cellOwner;
        cellOwner.onWorldRotate += OnWorldRotate;
    }

    public void DeactivateTurret()
    {
        //deactive and remove event
        gameObject.SetActive(false);
        cellOwner.onWorldRotate -= OnWorldRotate;
    }

    #endregion
}
