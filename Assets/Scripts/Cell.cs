using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/World/Cell")]
[RequireComponent(typeof(CellGraphics))]
public class Cell : MonoBehaviour
{
    [Header("Modifier")]
    [SerializeField] bool isInvincible = false;
    [SerializeField] bool onlyOneLife = false;

    [Header("Important")]
    [SerializeField] GameObject toRemoveOnDead = default;
    [SerializeField] BuildableObject turretToCreate = default;
    [SerializeField] bool buildTurretAtStart = false;
    [SerializeField] bool canRemoveTurret = true;

    [Header("Resources")]
    [SerializeField] float resourcesToRecreateCell = 100;
    [SerializeField] float resourcesToCreateTurret = 10;
    [SerializeField] float resourcesOnSellTurret = 10;

    [Header("Cell Models")]
    [SerializeField] GameObject center = default;
    [SerializeField] GameObject side = default;
    [SerializeField] GameObject angle = default;

    [Header("Debug")]
    public Coordinates coordinates;

    //used from turret to know when is rotating
    public System.Action<Coordinates> onWorldRotate;
    public System.Action onDestroyCell;

    public BuildableObject turret { get; private set; }

    public bool IsAlive { get; private set; } = true;

    public BuildableObject TurretToCreate => turretToCreate;

    void Awake()
    {
        //if build at start, build turret 
        BuildAtStart();
    }

    void OnDestroy()
    {
        //be sure to reset event
        onWorldRotate = null;
    }
    
    void OnTriggerEnter(Collider other)
    {
        //if hitted by enemy
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            //only if not enemy poison
            if (enemy.GetType() != typeof(EnemyPoison))
            {
                //kill cell 
                KillCell();
            }

            //destroy enemy
            enemy.Die(this);
        }
    }

    #region private API

    void BuildAtStart()
    {
        //if build at start, build turret 
        if (buildTurretAtStart)
        {
            ShowPreview();
            BuildOnCell();
        }
    }

    void BuildOnCell()
    {
        //do only if there is a turret and is only a preview
        if (turret && turret.IsPreview)
        {
            //build it
            turret.BuildTurret(this);
        }
    }

    void RemoveBuildOnCell()
    {
        //do only if there is a turret and is not a preview
        if (turret && turret.IsPreview == false)
        {
            //remove it
            turret.RemoveTurret();
        }
    }

    void DestroyCell()
    {
        IsAlive = false;

        //remove turret
        RemoveBuildOnCell();

        //remove biome
        ActiveRemoveOnDead(false);

        onDestroyCell?.Invoke();
    }

    void RecreateCell()
    {
        IsAlive = true;

        //recreate biome
        ActiveRemoveOnDead(true);

        //if was builded at start, rebuild turret 
        BuildAtStart();
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

        //instantiate (with parent) or build it
        if (turret == null)
            turret = Instantiate(turretToCreate, transform);
        else
            turret.gameObject.SetActive(true);

        //set position, rotation and size
        turret.transform.localPosition = Vector3.zero;
        turret.transform.localRotation = Quaternion.identity;
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
    public bool Interact()
    {
        //if dead, try recreate cell
        if(IsAlive == false)
        {
            if (GameManager.instance.levelManager.levelConfig.CanRecreateCell && GameManager.instance.player.CurrentResources >= resourcesToRecreateCell)
            {
                RecreateCell();
                GameManager.instance.player.CurrentResources -= resourcesToRecreateCell;    //remove resources to recreate cell
                return true;
            }

            return false;
        }

        //else check if there is a turret to create
        if (turretToCreate == null)
            return false;

        //if there is already a turret, try remove it
        if (turret != null && turret.IsPreview == false)
        {
            if (canRemoveTurret)
            {
                RemoveBuildOnCell();
                GameManager.instance.player.CurrentResources += resourcesOnSellTurret;      //give resources from sell turret
                return true;
            }

            return false;
        }

        //else build
        if (GameManager.instance.player.CurrentResources >= resourcesToCreateTurret)
        {
            BuildOnCell();
            GameManager.instance.player.CurrentResources -= resourcesToCreateTurret;        //remove resources to create turret
            return true;
        }

        return false;
    }

    /// <summary>
    /// Kill the cell or lose the game
    /// </summary>
    public void KillCell(bool canEndGame = true)
    {
        //do nothing if invincible
        if (isInvincible)
            return;

        //destroy cell or lose game (lose game only if canEndGame is true)
        if (IsAlive)
        {
            DestroyCell();

            //if only one life, call function again to kill definitively
            if (onlyOneLife)
                KillCell(canEndGame);
        }
        else if(canEndGame)
        {
            GameManager.instance.levelManager.EndGame(false);
        }
    }

    #endregion
}
