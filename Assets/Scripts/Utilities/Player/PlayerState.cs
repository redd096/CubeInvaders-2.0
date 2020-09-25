namespace redd096
{
    using UnityEngine;

    public class PlayerState : State
    {
        protected Player player;
        protected Transform transform;
        protected Rigidbody rb;

        public PlayerState(StateMachine stateMachine) : base(stateMachine)
        {
            //get references
            player = stateMachine as Player;
            transform = player.transform;
            rb = transform.GetComponent<Rigidbody>();
        }

        //public override void AwakeState(StateMachine stateMachine)
        //{
        //    base.AwakeState(stateMachine);
        //
        //    //get references
        //    player = stateMachine as Player;
        //    transform = player.transform;
        //    rb = transform.GetComponent<Rigidbody>();
        //}
    }
}