using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIdleState : SlimeGroundState
{
    public SlimeIdleState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName, Enemy_Slime _enemy) : base(_enemyBase, _enemyState, _animatName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetZeroVelocity();
        startTimer = enemy.idleTimer;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (startTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
