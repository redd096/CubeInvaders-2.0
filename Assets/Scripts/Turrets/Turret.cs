using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Turret : MonoBehaviour
{
    public Cell CellOwner { get; private set; }
    public bool IsActive { get; private set; }

    #region on world rotate

    protected virtual void OnWorldRotate(Coordinates coordinates)
    {
        //use cellOwner.onWorldRotate to know when start to rotate
        GameManager.instance.world.onEndRotation += OnEndRotation;
    }

    protected virtual void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;
    }

    #endregion

    #region public API

    public virtual void ActivateTurret(Cell cellOwner)
    {
        IsActive = true;

        //get owner and set event
        this.CellOwner = cellOwner;
        cellOwner.onWorldRotate += OnWorldRotate;
    }

    public virtual void DeactivateTurret()
    {
        IsActive = false;

        //deactive and remove event
        gameObject.SetActive(false);
        CellOwner.onWorldRotate -= OnWorldRotate;
    }

    #endregion
}
