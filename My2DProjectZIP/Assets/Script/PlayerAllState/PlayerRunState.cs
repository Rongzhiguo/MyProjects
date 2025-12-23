using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(6);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(6);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(xInput * player.runSpeed, rb.velocity.y);

        if (xInput == 0 || player.Iswalled())
            stateMachine.ChangeState(player.idleState);
    }
}
