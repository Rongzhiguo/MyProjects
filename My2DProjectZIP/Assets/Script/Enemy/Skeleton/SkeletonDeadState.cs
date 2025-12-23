using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;
    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName , Enemy_Skeleton _enemy) : base(_enemyBase, _enemyState, _animatName)
    {
        enemy = _enemy;
    }

    public override void AnimationFinshTrigger()
    {
        base.AnimationFinshTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool(enemy.lastAnimBoolName, true);
        enemy.animator.speed = 0;
        enemy.cd.enabled = false;
        startTimer = .15f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (startTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }
    }
}
