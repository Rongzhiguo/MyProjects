using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerState
{
    protected playerStateMachine stateMachine;
    protected player player;
    protected float xInput { get; private set; }
    protected float yInput { get; private set; }

    protected Rigidbody2D rb;

    protected float stateTime;

    protected bool triggerCalled;

    private string animBoolName;
    public playerState(player _player, playerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.animator.SetBool(animBoolName, true);
        triggerCalled = false;
        rb = player.rb;
    }

    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        if (player.joystick.Horizontal != 0)
        {
            xInput = player.joystick.Horizontal;
        }
        yInput = Input.GetAxisRaw("Vertical");
        if (player.joystick.Vertical != 0)
        {
            yInput = player.joystick.Vertical;
        }
        player.animator.SetFloat("yVelocity", rb.velocity.y);

        stateTime -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        player.animator.SetBool(animBoolName, false);
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void AnimationFinshTrigger()
    {
        triggerCalled = true;
    }
}
