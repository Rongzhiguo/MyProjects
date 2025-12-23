using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : playerState
{
    public PlayerAimSwordState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.sword_Skill.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (player.transform.position.x > mousePosition.x && player.facinDir == 1)
        {
            player.Flip();
        }
        else if (player.transform.position.x < mousePosition.x && player.facinDir == -1)
        {
            player.Flip();
        }
    }
}
