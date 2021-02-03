using UnityEngine;
using System.Collections;

public class BuildableObject : MonoBehaviour
{
    public ParticleSystem buildTurret;
    public ParticleSystem fireTurret;

    public Cell CellOwner { get; private set; }
    public bool IsPreview { get; private set; } = true;     //is a preview turret

    bool isActive;                                          //is active (shot and spawn shield)
    public bool IsActive

    {
        get
        {
            //if not preview and is active, return true - else, return false
            //added Time.time > timer enemies effect
            return !IsPreview && isActive && Time.time > TimerObjectDeactivated;
        }
    }

    #region protected API

    protected virtual void ActivateTurret()
    {
        isActive = true;
    }

    protected virtual void DeactivateTurret()
    {
        isActive = false;
    }

    #endregion

    #region on world rotate

    protected virtual void OnWorldRotate(Coordinates coordinates)
    {
        //use cellOwner.onWorldRotate to know when start to rotate
        GameManager.instance.world.onEndRotation += OnEndRotation;

        //deactivate it
        DeactivateTurret();
    }

    protected virtual void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;

        //try activate it
        TryActivateTurret();
    }

    #endregion

    #region public API

    public virtual void BuildTurret(Cell cellOwner)
    {
        IsPreview = false;
       
        //get owner and set event
        this.CellOwner = cellOwner;
        cellOwner.onWorldRotate += OnWorldRotate;

        //try activate it
        TryActivateTurret();
    }

    public virtual void RemoveTurret()
    {
        IsPreview = true;
      
        //deactive and remove event
        gameObject.SetActive(false);
        CellOwner.onWorldRotate -= OnWorldRotate;

        //deactive it
        DeactivateTurret();
    }

    public virtual void TryActivateTurret()
    {
        ActivateTurret();
    }

    public virtual void TryDeactivateTurret()
    {
        DeactivateTurret();
    }

    #endregion

    #region deactivate effect

    public System.Action<float> onDeactivateStart;
    public float TimerObjectDeactivated { get; private set; }    //timer setted by enemies effect, to deactivate this object

    public void Deactivate(float durationEffect)
    {
        //called by enemies effect, deactive until this time
        TimerObjectDeactivated = Time.time + durationEffect;

        //call event
        onDeactivateStart?.Invoke(durationEffect);
    }

    #endregion
}
