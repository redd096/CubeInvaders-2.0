using UnityEngine;
using System.Collections.Generic;

public class PlayerMove : PlayerState
{
    protected Coordinates coordinates;

    public PlayerMove(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        //get previous coordinates
        this.coordinates = coordinates;
    }

#if UNITY_ANDROID

    protected bool isMoving;
    protected Cell selectedCell;

    public override void Execution()
    {
        base.Execution();

        //do only if there are touch
        if (Input.touchCount <= 0)
            return;

        switch (Input.GetTouch(0).phase)
        {
            case TouchPhase.Began:
                //on touch, check if select world or nope
                isMoving = true;
                selectedCell = TrySelectCell();
                break;
            case TouchPhase.Moved:
                //on move, check if rotate world or move camera
                MoveTouch();
                break;
            //case TouchPhase.Stationary:
            //    break;
            //case TouchPhase.Ended:
            //    break;
            //case TouchPhase.Canceled:
            //    break;
            //default:
            //    break;
        }
    }

    #region private API

    protected Cell TrySelectCell()
    {
        //check if hit world
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;
        float distance = 100;
        int layer = redd096.CreateLayer.LayerOnly("World");

        //if hit world, select cell
        if (Physics.Raycast(ray, out hit, distance, layer, QueryTriggerInteraction.Collide))
        {
            return hit.transform.GetComponentInParent<Cell>();
        }
        //else remove cell selected
        else
        {
            return null;
        }
    }

    void MoveTouch()
    {
        //if stopped moving, can't do anything until retouch
        if (isMoving == false)
            return;

        //try rotate if hitted the world
        if(selectedCell)
        {
            TryRotate();
        }
        //else move camera
        else
        {
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
            CinemachineMovement(deltaPosition.x, deltaPosition.y);
        }
    }

    void TryRotate()
    {
        Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

        if(deltaPosition.x > 0)
        {
            DoRotation(ERotateDirection.right);
        }
        else if(deltaPosition.x < 0)
        {
            DoRotation(ERotateDirection.left);
        }
        else if(deltaPosition.y > 0)
        {
            DoRotation(ERotateDirection.up);
        }
        else if(deltaPosition.y < 0)
        {
            DoRotation(ERotateDirection.down);
        }
        else
        {
            return;
        }

        //stop moving if do rotation
        isMoving = false;
    }

    #endregion

#else

    public override void Execution()
    {
        base.Execution();

        //movement
        CinemachineMovement("Mouse X", "Mouse Y");

        SelectCell();

        Rotate();
    }

    #region private API

    void SelectCell()
    {
        EFace face = WorldUtility.SelectFace(transform);

        //if change face, reselect center cell
        if (face != coordinates.face)
            coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);

        //select cell
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
        }

        //save coordinates and  show selector
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

    void Rotate()
    {
        //rotate
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                DoRotation(ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                DoRotation(ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                DoRotation(ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                DoRotation(ERotateDirection.left);
        }
    }

    #endregion

#endif

    void DoRotation(ERotateDirection rotateDirection)
    {
        //if selector is greater, rotate more cells
        if (GameManager.instance.levelManager.levelConfig.SelectorSize > 1)
        {
            List<Coordinates> coordinatesToRotate = RotateMoreCells(rotateDirection);
            coordinatesToRotate.Add(coordinates);
            GameManager.instance.world.Rotate(coordinatesToRotate.ToArray(), WorldUtility.LateralFace(transform), rotateDirection);
        }
        //else rotate only this cell
        else
        {
            GameManager.instance.world.Rotate(coordinates, WorldUtility.LateralFace(transform), rotateDirection);
        }

        //change state
        player.SetState(new PlayerWaitRotation(player, coordinates, WorldUtility.LateralFace(transform), rotateDirection));
    }

    List<Coordinates> RotateMoreCells(ERotateDirection rotateDirection)
    {
        int selectorSize = GameManager.instance.levelManager.levelConfig.SelectorSize;

        //check if get cells on x or y
        bool useX = rotateDirection == ERotateDirection.up || rotateDirection == ERotateDirection.down;
        int value = useX ? coordinates.x : coordinates.y;

        //check if there are enough cells to the right (useX) or up (!useX)
        bool increase = value + selectorSize - 1 < GameManager.instance.world.worldConfig.NumberCells;

        //min (next after our cell) and max (until selector size)
        //or min (from selector cell) and max (next after our cell)
        int min = increase ? value + 1 : value - (selectorSize - 1);
        int max = increase ? value + selectorSize : value;

        //increase or decrease
        List<Coordinates> coordinatesToRotate = new List<Coordinates>();
        for (int i = min; i < max; i++)
        {
            //get coordinates using x or y
            Coordinates coords = useX ? new Coordinates(coordinates.face, i, coordinates.y) : new Coordinates(coordinates.face, coordinates.x, i);

            //if there is a cell, add it
            if (GameManager.instance.world.Cells.ContainsKey(coords))
                coordinatesToRotate.Add(coords);
        }

        return coordinatesToRotate;
    }
}
