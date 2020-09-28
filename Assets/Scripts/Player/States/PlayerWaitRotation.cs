using redd096;

public class PlayerWaitRotation : PlayerState
{
    Coordinates coordinates;

    public PlayerWaitRotation(StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //wait end rotation
        GameManager.instance.world.onEndRotation += OnEndRotation;

        GameManager.instance.uiManager.HideSelector();
        StopCinemachine();
    }

    public override void Exit()
    {
        base.Exit();

        //remove event
        GameManager.instance.world.onEndRotation -= OnEndRotation;
    }

    void OnEndRotation()
    {
        //wait end rotation and come back to movement
        player.SetState(new PlayerMove(player, coordinates));
    }
}
