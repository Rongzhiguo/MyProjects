using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : playerState
{
    public PlayerDeadState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinshTrigger()
    {
        base.AnimationFinshTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        GameObject.Find("Canvas").GetComponent<UI>().SwitchOnEndScreen();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.SetZeroVelocity();
    }
}
