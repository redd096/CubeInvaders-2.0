﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/World/Cell")]
[SelectionBase]
public class Cell : MonoBehaviour
{
    [Header("Modifier")]
    [SerializeField] bool isInvincible = false;
    [SerializeField] bool onlyOneLife = false;

    [Header("Important")]
    [SerializeField] GameObject toRemoveOnDead = default;
    [SerializeField] BuildableObject turretToCreate = default;
    [SerializeField] bool canRemoveTurret = true;

    [Header("Cell Models")]
    [SerializeField] GameObject center = default;
    [SerializeField] GameObject side = default;
    [SerializeField] GameObject angle = default;

    [Header("Debug")]
    public Coordinates coordinates;

    //used from turret to know when is rotating
    public System.Action<Coordinates> onWorldRotate;

    public BuildableObject turret { get; private set; }

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
        if (turret && turret.IsPreview)
        {
            //build it
            turret.BuildTurret(this);
        }
    }

    void RemoveBuildOnCell(bool sell)
    {
        //do only if there is a turret and is not a preview
        if (turret && turret.IsPreview == false)
        {
            //remove it
            turret.RemoveTurret();

            if (sell)
            {
                //get resources
            }
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
        //do only if there is a turret to create, and there isn't already a turret builded on it
        if (turretToCreate == null || (turret != null && turret.IsPreview == false))
            return;

        //instantiate or build it
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
        if (turret != null && turret.IsPreview)
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
        if (turret != null && turret.IsPreview == false)
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
        //do nothing if invincible
        if (isInvincible)
            return;

        //destroy cell or lose game
        if(alive)
        {
            DestroyCell();

            //if only one life, call function again to kill definitively
            if (onlyOneLife)
                KillCell();
        }
        else
        {
            GameManager.instance.levelManager.EndGame(false);
        }
    }

    #endregion
}
