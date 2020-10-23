using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/World/Cell")]
[SelectionBase]
public class Cell : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] GameObject toRemoveOnDead = default;
    [SerializeField] Turret turretToCreate = default;
    [SerializeField] bool canRemoveTurret = true;

    [Header("Cell Models")]
    [SerializeField] GameObject center = default;
    [SerializeField] GameObject side = default;
    [SerializeField] GameObject angle = default;

    [Header("Debug")]
    public Coordinates coordinates;

    //used from turret to know when is rotating
    public System.Action<Coordinates> onWorldRotate;

    Turret turret;

    bool alive = true;

    void OnDestroy()
    {
        //be sure to reset event
        onWorldRotate = null;
    }

    #region private API

    void BuildOnCell()
    {
        //do only if there is a turret and is only a preview
        if (turret == null || turret.IsActive)
            return;

        //activate it
        turret.ActivateTurret(this);
    }

    void RemoveBuildOnCell(bool sell)
    {
        //do only if there is a turret and is active (not simple preview)
        if (turret == null || turret.IsActive == false)
            return;

        //deactivate it
        turret.DeactivateTurret();

        if (sell)
        {
            //get resources
        }
    }

    void DestroyCell()
    {
        alive = false;

        //remove turret
        RemoveBuildOnCell(false);

        //remove biome
        ActiveRemoveOnDead(false);
    }

    void RecreateCell()
    {
        alive = true;

        //recreate biome
        ActiveRemoveOnDead(true);
    }

    void ActiveRemoveOnDead(bool active)
    {
        //enable or disable every renderer
        Renderer[] renderers = toRemoveOnDead.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = active;
    }

    #endregion

    #region public API

    public void SelectModel(int numberCells)
    {
        //left
        if(coordinates.x <= 0)
        {
            //down or up
            if(coordinates.y <= 0 || coordinates.y >= numberCells -1)
            {
                angle.SetActive(true);

                center.SetActive(false);
                side.SetActive(false);
            }
            //else is side
            else
            {
                side.SetActive(true);

                center.SetActive(false);
                angle.SetActive(false);
            }
        }
        //right
        else if(coordinates.x >= numberCells -1)
        {
            //down or up
            if (coordinates.y <= 0 || coordinates.y >= numberCells - 1)
            {
                angle.SetActive(true);

                center.SetActive(false);
                side.SetActive(false);
            }
            //else is side
            else
            {
                side.SetActive(true);

                center.SetActive(false);
                angle.SetActive(false);
            }
        }
        //center column
        else
        {
            //down or up is side
            if (coordinates.y <= 0 || coordinates.y >= numberCells - 1)
            {
                side.SetActive(true);

                center.SetActive(false);
                angle.SetActive(false);
            }
            //else is center
            else
            {
                center.SetActive(true);

                side.SetActive(false);
                angle.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Show turret without activate it
    /// </summary>
    public void ShowPreview()
    {
        //do only if there is a turret to create, and there isn't already a turret active on it
        if (turretToCreate == null || (turret != null && turret.IsActive))
            return;

        //instantiate or active it
        if (turret == null)
            turret = Instantiate(turretToCreate);
        else
            turret.gameObject.SetActive(true);

        //set position and rotation 
        turret.transform.position = transform.position;
        turret.transform.rotation = transform.rotation;

        //set child of cell and set size
        turret.transform.SetParent(transform);
        turret.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Remove preview
    /// </summary>
    public void HidePreview()
    {
        //check if there is a turret and is only a preview
        if (turret != null && turret.IsActive == false)
            turret.gameObject.SetActive(false);
    }

    /// <summary>
    /// Player interact with the cell
    /// </summary>
    public void Interact()
    {
        //if dead, try recreate cell
        if(alive == false)
        {
            if (GameManager.instance.levelManager.levelConfig.CanRecreateCell)
                RecreateCell();

            return;
        }

        //else check if there is a turret to create
        if (turretToCreate == null)
            return;

        //if there is a already a turret, try remove it
        if (turret != null && turret.IsActive)
        {
            if(canRemoveTurret)
                RemoveBuildOnCell(true);

            return;
        }

        //else build
        BuildOnCell();
    }

    /// <summary>
    /// Kill the cell or lose the game
    /// </summary>
    public void KillCell()
    {
        //destroy cell or lose game
        if(alive)
        {
            DestroyCell();
        }
        else
        {
            GameManager.instance.levelManager.EndGame(false);
        }
    }

    #endregion
}
