using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStunnedState : EnemyState
{
    Enemy_Slime enemy;
    public SlimeStunnedState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName, Enemy_Slime _enemy) : base(_enemyBase, _enemyState, _animatName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        startTimer = enemy.stunDuration;
        enemy.rb.velocity = new Vector2(-enemy.facinDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if (rb.velocity.y < .1f && enemy.IsGrounded())
        {
            enemy.fx.Invoke("CancelColorChange", 0);
            enemy.animator.SetTrigger("StunFold");
            enemy.stats.MakeInvincible(true);
        }

        if (startTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
