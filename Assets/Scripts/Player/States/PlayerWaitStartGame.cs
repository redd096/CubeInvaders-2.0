using UnityEngine;

public class PlayerWaitStartGame : PlayerState
{
    public PlayerWaitStartGame(redd096.StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //wait start game
        GameManager.instance.world.onStartGame += OnStartGame;

        StopCinemachine();
    }

    public override void Exit()
    {
        base.Exit();

        //remove event
        GameManager.instance.world.onStartGame -= OnStartGame;
    }

    void OnStartGame()
    {
        //go to player move, starting from center cell
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        player.SetState(new PlayerMove(player, new Coordinates(EFace.front, centerCell.x, centerCell.y)));
    }
}
