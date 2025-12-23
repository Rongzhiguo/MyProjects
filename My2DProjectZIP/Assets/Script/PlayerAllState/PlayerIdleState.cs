using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .2f);
    }

    public override void Update()
    {
        base.Update();
        if (player.Iswalled() && xInput == player.facinDir) return;

        if (xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.runState);
    }
}
