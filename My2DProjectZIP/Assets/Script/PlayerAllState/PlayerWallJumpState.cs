using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : playerState
{
    public PlayerWallJumpState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTime = .4f;

        player.SetVelocity(5 * -player.facinDir, player.jumpSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTime < 0)
            stateMachine.ChangeState(player.airState);

        if (player.IsGrounded())
            stateMachine.ChangeState(player.idleState);
    }
}
