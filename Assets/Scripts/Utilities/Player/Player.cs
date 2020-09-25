namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Player")]
    public class Player : StateMachine
    {
        void Start()
        {
            //set state
            SetState(new PlayerState(this));  // create new one, with constructor
            //SetState(playerState);          // use serialized state (use AwakeState instead of constructor)
        }

        void Update()
        {
            state?.Execution();
        }
    }
}