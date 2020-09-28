using System.Collections;
using System.Collections.Generic;
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

        //stop camera movement
        StopCinemachine();
    }

    public override void Execution()
    {
        base.Execution();

        SelectCell();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlaceTurret();
        }

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

    void PlaceTurret()
    {
        //place turret
        GameManager.instance.world.Cells[coordinates].ActivateCell();
    }

    void StopPlaceTurret()
    {
        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }
}
