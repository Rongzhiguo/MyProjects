using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{

    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName, Enemy_Skeleton _enemy) : base(_enemyBase, _enemyState, _animatName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.moveSpeed * enemy.facinDir, rb.velocity.y);
        if (enemy.Iswalled() || !enemy.IsGrounded())
        { 
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
