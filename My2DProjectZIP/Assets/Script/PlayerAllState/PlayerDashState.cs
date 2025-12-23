using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : playerState
{
    public PlayerDashState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.skill.dash_Skill.CloneOnDash();
        stateTime = player.dashDuration;
        player.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.skill.dash_Skill.CloneOnArrival();
        player.SetVelocity(0, rb.velocity.y);
        player.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        if (player.Iswalled() && !player.IsGrounded())
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTime < 0)
            stateMachine.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //ShadowPool.instance.GetFormShadow();
        ObjectPoolManager.instance.GetFormTypePool(PoolType.Dash);
    }
}
