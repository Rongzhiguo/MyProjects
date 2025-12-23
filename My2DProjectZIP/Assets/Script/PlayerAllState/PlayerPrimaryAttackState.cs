using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : playerState
{
    public int comboCounter { get; private set; }
    private float lastTimeAttacked;
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || Time.time - lastTimeAttacked >= comboWindow) comboCounter = 0;
        player.animator.SetInteger("comboCounter", comboCounter);
        stateTime = .1f;

        float attackDir = player.facinDir;
        if (Input.GetAxisRaw("Horizontal") != 0 || player.joystick.Horizontal != 0)
        {
            attackDir = Input.GetAxisRaw("Horizontal") != 0 ? Input.GetAxisRaw("Horizontal") : player.joystick.Horizontal;
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        //player.animator.speed = 5;
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .15f);
        comboCounter++;
        lastTimeAttacked = Time.time;
        //player.animator.speed = 1;
    }

    public override void Update()
    {
        base.Update();
        if (stateTime < 0)
            player.SetZeroVelocity();
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
