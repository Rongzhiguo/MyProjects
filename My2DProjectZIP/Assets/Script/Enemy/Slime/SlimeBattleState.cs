using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBattleState : EnemyState
{
    private int moveDir;
    private Transform player;
    Enemy_Slime enemy;
    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName, Enemy_Slime _enemy) : base(_enemyBase, _enemyState, _animatName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        bool playerIsDead = player.GetComponent<CharacterStats>().isDead;
        if (playerIsDead)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (enemy.isPlayerDetected())
        {
            startTimer = enemy.battleTime;
            if (enemy.isPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (startTimer < 0 || Vector2.Distance(enemy.transform.position, player.transform.position) > 12)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        if (player.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        //if (Vector2.Distance(enemy.transform.position, player.transform.position) > 0.5)
        if (enemy.isPlayerDetected() && enemy.isPlayerDetected().distance < enemy.attackDistance - .15f)
            return;
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);

    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttack + enemy.attackCD)
        {
            enemy.attackCD = Random.Range(enemy.minAttackCD, enemy.maxAttackCD);
            enemy.lastTimeAttack = Time.time;
            return true;
        }
        return false;
    }
}
