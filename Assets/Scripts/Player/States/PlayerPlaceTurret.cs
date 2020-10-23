using UnityEngine;

public class PlayerPlaceTurret : PlayerState
{
    Coordinates coordinates;

    public PlayerPlaceTurret(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //stop camera movement and show preview
        StopCinemachine();
        GameManager.instance.world.Cells[coordinates].ShowPreview();
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to remove preview
        GameManager.instance.world.Cells[coordinates].HidePreview();
    }

#if UNITY_ANDROID

    public override void Execution()
    {
        base.Execution();

        //on touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //try select a cell
            Cell selectedCell = TrySelectCell();

            //if not selected anything, exit from this state
            if(selectedCell == null)
            {
                StopPlaceTurret();
                return;
            }
            else
            {
                //if selected same cell, place turret
                if(selectedCell.coordinates == coordinates)
                {
                    PlaceTurret();
                }
                //if selected another cell, change preview
                else
                {
                    //hide old preview and show new one
                    GameManager.instance.world.Cells[coordinates].HidePreview();
                    GameManager.instance.world.Cells[selectedCell.coordinates].ShowPreview();

                    //save new coordinates
                    coordinates = selectedCell.coordinates;
                }
            }
        }

        //exit from preview
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlaceTurret();
        }
    }

    Cell TrySelectCell()
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

#else

    public override void Execution()
    {
        base.Execution();

        //save previous coordinates
        Coordinates previousCoordinates = coordinates;

        SelectCell();

        //if differente coordinates
        if(previousCoordinates != coordinates)
        {
            //hide old preview and show new one
            GameManager.instance.world.Cells[previousCoordinates].HidePreview();
            GameManager.instance.world.Cells[coordinates].ShowPreview();
        }

        //place turret
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlaceTurret();
        }

        //exit from preview
        if(Input.GetKeyDown(KeyCode.Return))
        {
            StopPlaceTurret();
        }
    }

    void SelectCell()
    {
        //select cell
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
        }

        //save coordinates and  show selector
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

#endif

    void PlaceTurret()
    {
        //place turret
        GameManager.instance.world.Cells[coordinates].Interact();

        //exit from place turret
        StopPlaceTurret();
    }

    void StopPlaceTurret()
    {
        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }
}
