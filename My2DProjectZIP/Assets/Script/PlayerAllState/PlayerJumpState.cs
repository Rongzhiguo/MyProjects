using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : playerState
{
    public PlayerJumpState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, player.jumpSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (xInput != 0)
            player.SetVelocity(xInput * player.runSpeed, rb.velocity.y);

        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);//如果玩家Y轴速度小于0则说明是下降，则切换为空气状态
    }
}
