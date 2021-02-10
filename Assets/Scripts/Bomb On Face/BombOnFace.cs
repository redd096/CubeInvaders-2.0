using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Bomb On Face/Bomb On Face")]
public class BombOnFace : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Cell cellOwner = default;
    [SerializeField] float timeBeforeCanRotate = 5;

    [Header("To Do On Explosion")]
    [SerializeField] bool endGame = false;
    [SerializeField] bool destroyEveryCellOnFace = true;

    bool canStartTimer;
    Coroutine timer_Coroutine;
    bool isExploded;

    public System.Action onStartTimer;
    public float TimeBeforeCanRotate => timeBeforeCanRotate;

    void Start()
    {
        //Set parent to follow cell rotation
        transform.SetParent(cellOwner.transform);

        AddEvents();
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        cellOwner.onWorldRotate += OnWorldRotate;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;
    }

    void RemoveEvents()
    {
        cellOwner.onWorldRotate -= OnWorldRotate;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;
    }

    void OnStartAssaultPhase()
    {
        //now can start timer
        canStartTimer = true;
    }

    void OnEndAssaultPhase()
    {
        //now timer is deactivate
        canStartTimer = false;
    }

    #endregion

    #region on world rotate

    void OnWorldRotate(Coordinates coordinates)
    {
        GameManager.instance.world.onEndRotation += OnEndRotation;

        //if timer was going, explode (a variable, to make explosion when finish rotation)
        if (timer_Coroutine != null && canStartTimer)
        {
            isExploded = true;
            canStartTimer = false;
        }
    }

    void OnEndRotation()
    {
        GameManager.instance.world.onEndRotation -= OnEndRotation;

        //start timer
        if (timer_Coroutine == null && canStartTimer)
        {
            if(gameObject.activeInHierarchy)
                timer_Coroutine = StartCoroutine(Timer_Coroutine());
        }
        //else if is exploded, do explosion
        else if(isExploded)
        {
            isExploded = false;
            Explosion();
        }
    }

    #endregion

    #region private API

    IEnumerator Timer_Coroutine()
    {
        //call event
        onStartTimer?.Invoke();

        //wait, then stop timer
        yield return new WaitForSeconds(timeBeforeCanRotate);

        timer_Coroutine = null;
    }

    void Explosion()
    {
        //end game
        if(endGame)
        {
            GameManager.instance.levelManager.EndGame(false);
        }

        //destroy every cell on this face
        if(destroyEveryCellOnFace)
        {
            EFace face = cellOwner.coordinates.face;

            //foreach cell on this face
            foreach(Cell cell in GameManager.instance.world.Cells.Values)
            {
                if(cell.coordinates.face == face)
                {
                    //kill cell (can't end game)
                    cell.KillCell(false);
                }
            }
        }

        //destroy self
        Destroy(gameObject);
    }

    #endregion
}
