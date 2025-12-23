using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : EnemyState
{
    Enemy_Slime enemy;
    public SlimeAttackState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName , Enemy_Slime _enemy) : base(_enemyBase, _enemyState, _animatName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();
        enemy.SetZeroVelocity();
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
