using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : playerState
{
    public PlayerAirState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput != 0)
            player.SetVelocity(xInput * player.runSpeed * .8f, rb.velocity.y);

        if (player.Iswalled())
        {
            stateMachine.ChangeState(player.wallSlideState);
            //player.SetVelocity(0, 0);
        }
        else if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
