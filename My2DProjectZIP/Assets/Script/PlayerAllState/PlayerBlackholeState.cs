using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : playerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;
    private float defGravity;

    public PlayerBlackholeState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        defGravity = rb.gravityScale;
        skillUsed = false;
        stateTime = flyTime;
        rb.gravityScale = 0;
        player.SetUpAberration(.85f);
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = defGravity;
        player.fx.MakeTransprent(false);
        player.DefauleAberration();
    }

    public override void Update()
    {
        base.Update();

        if (stateTime > 0)
            rb.velocity = new Vector2(0, 7);

        if (stateTime < 0)
        {
            rb.velocity = new Vector2(0, -.1f);
            if (!skillUsed)
            {
                if (player.skill.blackhole.CanUseSkill())
                {
                    skillUsed = true;
                }
            }
        }

        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }

    public override void AnimationFinshTrigger()
    {
        base.AnimationFinshTrigger();
    }
}
